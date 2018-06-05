using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class PauseMenu : MonoBehaviour {

    public InputDevice Device { get; set; }

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject modeSelectionUI;

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
        modeSelectionUI.SetActive(false);
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

    public void ModeSelection()
    {
        pauseMenuUI.SetActive(false);
        modeSelectionUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
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