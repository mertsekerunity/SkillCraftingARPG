using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float angularSpeed = 5f;

    Camera mainCam;
    Rigidbody rb;
    List<Orb> activeOrbs = new List<Orb>();
    Dictionary<HashSet<Orb>, Skill> skillBook;

    Vector3 targetPoint = new Vector3();

    int maxOrbsCount = System.Enum.GetValues(typeof(Orb)).Length;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        skillBook = SkillBook.GetSkills();
    }

    // Update is called once per frame
    void Update()
    {
        HandleOrbSelection();
        HandleSkillCrafting();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = mainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if(Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                targetPoint = hit.point;
            }
            else
            {
                return;
            }

            targetPoint.y = transform.position.y;
            float delta = moveSpeed * Time.deltaTime;
            float angularDelta = angularSpeed * Time.deltaTime;
            Vector3 direction = (targetPoint - rb.position).normalized;
            Vector3 newPos = rb.position + direction * delta;
            Vector3 newOrientation = Vector3.RotateTowards(rb.position, targetPoint, angularDelta, 0f);
            newOrientation.x = 0;
            newOrientation.z = 0;
            Quaternion QuaternionNewOrientation = Quaternion.Euler(newOrientation);

            rb.MovePosition(newPos);
            rb.MoveRotation(QuaternionNewOrientation);
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

        Debug.Log($"Current orbs: {string.Join(", ", activeOrbs)}");
    }

    Skill HandleSkillCrafting()
    {   
        if (Input.GetKeyDown(KeyCode.R))
        {
            HashSet<Orb> orbSet = new HashSet<Orb>(activeOrbs);

            Debug.Log(string.Join(", ", orbSet));

            

            if (skillBook.TryGetValue(orbSet, out Skill skill))
            {
                Debug.Log($"Crafted Skill: {skill.skillName}");
                orbSet.Clear();
                return skill;
            }
        }
        return null;
    }
}
