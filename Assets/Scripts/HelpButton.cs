using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI helpText;

    private void Update()
    {
        HotkeysForHelp();
    }
    public void OnButtonPressed()
    {
        if (helpText.isActiveAndEnabled)
        {
            helpText.gameObject.SetActive(false);
        }
        else
        {
            helpText.gameObject.SetActive(true);
        }
    }

    void HotkeysForHelp()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && helpText.isActiveAndEnabled)
        {
            helpText.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!helpText.isActiveAndEnabled)
            {
                helpText.gameObject.SetActive(true);
            }
            else
            {
                helpText.gameObject.SetActive(false);
            }
        }
    }
}
