using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class PauseMenu : MonoBehaviour {

    public InputDevice Device { get; set; }

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject endGameUI;

    void Update () {

        var inputDevice = InputManager.ActiveDevice;

        if (PlayerController.inputPause)
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
	}

    public void Pause()
    {
        endGameUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }
}
