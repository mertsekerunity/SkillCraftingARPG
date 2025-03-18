using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI helpText;
    [SerializeField] TextMeshProUGUI skillsText;

    private void Update()
    {
        HotkeysForHelp();
    }
    public void OnButtonPressed()
    {
        if (skillsText.isActiveAndEnabled)
        {
            skillsText.gameObject.SetActive(false);
        }

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
                if (skillsText.isActiveAndEnabled)
                {
                    skillsText.gameObject.SetActive(false);
                }

                helpText.gameObject.SetActive(true);
            }
            else
            {
                helpText.gameObject.SetActive(false);
            }
        }
    }
}
