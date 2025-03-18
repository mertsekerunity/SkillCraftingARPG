using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skillsText;
    [SerializeField] TextMeshProUGUI helpText;

    private void Update()
    {
        HotkeysForSkills();
    }
    public void OnButtonPressed()
    {
        if(helpText.isActiveAndEnabled)
        {
            helpText.gameObject.SetActive(false);
        }

        if (skillsText.isActiveAndEnabled)
        {
            skillsText.gameObject.SetActive(false);
        }
        else
        {
            skillsText.gameObject.SetActive(true);
        }
    }

    void HotkeysForSkills()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && skillsText.isActiveAndEnabled)
        {
            skillsText.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!skillsText.isActiveAndEnabled)
            {
                if (helpText.isActiveAndEnabled)
                {
                    helpText.gameObject.SetActive(false);
                }

                skillsText.gameObject.SetActive(true);
            }
            else
            {
                skillsText.gameObject.SetActive(false);
            }
        }
    }
}
