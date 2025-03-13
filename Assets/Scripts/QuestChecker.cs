using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestChecker : MonoBehaviour
{
    TextMeshProUGUI[] texts;
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleQuests();
    }

    void HandleQuests()
    {
        foreach(TextMeshProUGUI text in texts)
        {
            if (text.color != Color.green) return;
        }

        levelManager.LoadGameFinished();
    }
}
