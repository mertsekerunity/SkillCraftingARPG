using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySelection : MonoBehaviour
{

    public void OnEasyPressed()
    {
        PlayerPrefs.SetString("Difficulty", "Easy");
        PlayerPrefs.Save(); // Save the preference
        LevelManager.Instance.LoadGame();
    }

    public void OnNormalPressed()
    {
        PlayerPrefs.SetString("Difficulty", "Normal");
        PlayerPrefs.Save();
        LevelManager.Instance.LoadGame();
    }
    public void OnHardPressed()
    {
        PlayerPrefs.SetString("Difficulty", "Hard");
        PlayerPrefs.Save();
        LevelManager.Instance.LoadGame();
    }
}
