using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using InControl;

public class ModeSelectionMenu : MonoBehaviour
{

    public InputDevice Device { get; set; }

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject modeSelectionUI;
    public GameObject coopModeReachIt;
    public GameObject coopModeHighScore;
    public GameObject vsModeHitOtherSide;
    public GameObject vsModeKeepItAway;

    void Update()
    {
        var inputDevice = InputManager.ActiveDevice;
    }

    public void CoopModeReachIt()
    {
        modeSelectionUI.SetActive(false);

        coopModeHighScore.SetActive(false);
        vsModeHitOtherSide.SetActive(false);
        vsModeKeepItAway.SetActive(false);

        coopModeReachIt.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void CoopModeHighScore()
    {
        modeSelectionUI.SetActive(false);

        coopModeReachIt.SetActive(false);
        vsModeHitOtherSide.SetActive(false);
        vsModeKeepItAway.SetActive(false);

        coopModeHighScore.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void VsModeHitOtherSide()
    {
        modeSelectionUI.SetActive(false);

        coopModeReachIt.SetActive(false);
        coopModeHighScore.SetActive(false);
        vsModeKeepItAway.SetActive(false);

        vsModeHitOtherSide.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void VsModeKeepItAway()
    {
        modeSelectionUI.SetActive(false);

        coopModeReachIt.SetActive(false);
        coopModeHighScore.SetActive(false);
        vsModeHitOtherSide.SetActive(false);

        vsModeKeepItAway.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Back()
    {
        modeSelectionUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
}
