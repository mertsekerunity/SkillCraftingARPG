using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] Canvas pauseCanvas;
    public bool isPaused;

    // Update is called once per frame
    void Update()
    {
        HandlePause();
    }

    void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            pauseCanvas.gameObject.SetActive(isPaused);
        }
    }

    public void ResetPause()
    {
        Time.timeScale = 1;
        pauseCanvas.gameObject.SetActive(false);
    }
}
