using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlider : MonoBehaviour
{
    [SerializeField] float slideSpeed = 10f;
    float openDoorYPosition = -3f;

    private void OnTriggerEnter(Collider other)
    {
        float desiredYPosition = this.transform.position.y;
        if (other.gameObject.CompareTag("Player"))
        {
            while (this.transform.position.y > openDoorYPosition)
                desiredYPosition -= slideSpeed * Time.deltaTime;
                this.transform.position = new Vector3(transform.position.x, desiredYPosition, transform.position.z);
        }
    }
}
