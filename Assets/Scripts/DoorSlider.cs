using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlider : MonoBehaviour
{
    [SerializeField] float slideSpeed = 10f;
    float openDoorYPosition = -10f;
    float closedDoorYPosition = 0f;
    bool isTriggeredOn = false;
    Transform door;

    private void Start()
    {
        door = transform.GetChild(0);
    }
    private void Update()
    {
        HandleDoorOpening();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            isTriggeredOn = true;
            //Debug.Log("Door triggered.");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTriggeredOn = false;

        }
    }

    void HandleDoorOpening()
    {
        float desiredYPosition = isTriggeredOn ? openDoorYPosition : closedDoorYPosition;
        float delta = slideSpeed * Time.deltaTime;
        Vector3 desiredPos = new Vector3(door.localPosition.x, desiredYPosition, door.localPosition.z);
        door.localPosition = Vector3.MoveTowards(door.localPosition, desiredPos, delta);
    }
}
