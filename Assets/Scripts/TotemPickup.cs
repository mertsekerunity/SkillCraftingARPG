using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TotemPickup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI totemText;
    [SerializeField] Toggle totemTextToggle;
    float destroyDelay = 1.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            totemText.color = Color.green;
            totemTextToggle.isOn = true;
            Destroy(gameObject, destroyDelay);
            Debug.Log("Totem collected.");
        }
        else return;
    }
}
