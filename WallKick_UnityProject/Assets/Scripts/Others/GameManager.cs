using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenuUI;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }
}
