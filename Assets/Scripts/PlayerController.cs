using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;

    Camera myCamera;
    List<Orb> activeOrbs = new List<Orb>();

    int maxOrbsCount = 2; // any other way to get access to number of the elements inside enum??

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleOrbSelection();
    }

    void HandleMovement()
    {
        //float xValue = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        //float zValue = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        //transform.Translate(xValue, 0, zValue);

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 targetPoint = myCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, myCamera.nearClipPlane));
            //Vector3 targetPoint = myCamera.ScreenToWorldPoint(mousePos);
            //targetPoint.z = targetPoint.y;
            //targetPoint.y = transform.position.y;
            Debug.Log($"Target point: {targetPoint}");
            float delta = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, delta);
        }
    }

    void HandleOrbSelection()
    {
        if (Input.GetKeyDown(KeyCode.Q)) AddOrb(Orb.Quas);
        if (Input.GetKeyDown(KeyCode.W)) AddOrb(Orb.Wex);
        //if (Input.GetKeyDown(KeyCode.E)) AddOrb(Orb.Exort);
    }

    void AddOrb(Orb orb)
    {
        if(activeOrbs.Count >= maxOrbsCount)
        {
            activeOrbs.RemoveAt(0);
        }

        activeOrbs.Add(orb);

        Debug.Log($"Current orbs: {activeOrbs}");
    }
}
