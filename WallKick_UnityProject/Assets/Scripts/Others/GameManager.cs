using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenuUI;
    public int frameRate;
    public bool forcedFrameRate;
    public bool vSyncEnabled;

    //private void Awake()
    //{
    //    forcedFrameRate = false;
    //    vSyncEnabled = false;
    //}

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    //private void Update()
    //{
    //    if (forcedFrameRate)
    //    {
    //        vSyncEnabled = false;
    //        QualitySettings.vSyncCount = 0;  // VSync must be disabled to allow target framerate
    //        Application.targetFrameRate = frameRate;
    //    }
    //    else if (vSyncEnabled)
    //    {
    //        forcedFrameRate = false;
    //        QualitySettings.vSyncCount = 1;
    //    }
    //    else
    //    {
    //        QualitySettings.vSyncCount = 0;
    //    }
    //}
}
