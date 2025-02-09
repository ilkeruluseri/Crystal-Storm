using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    AudioManager audioManager;
    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        //DontDestroyOnLoad(transform.gameObject);
        //if (FindObjectsByType<SceneChanger>(FindObjectsSortMode.None).Length != 1)
        //{
        //    Destroy(gameObject);
        //}
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadWin()
    {
        SceneManager.LoadScene("WinScene");
        if (audioManager != null)
        {
            audioManager.Stop("stormMusic");
        }
    }

    public void LoadLose()
    {
        SceneManager.LoadScene("LoseScene");
        if (audioManager != null)
        {
            audioManager.Stop("stormMusic");
        }
    }

    public void LoadInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void LoadStart()
    {
        SceneManager.LoadScene("StartScene");
    }


}
