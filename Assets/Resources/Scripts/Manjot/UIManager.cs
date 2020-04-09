using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [HideInInspector]
    public Image healthbar;
    PlayerController player;
     GameObject continueButton;
    bool playing = true;
    public GameObject pauseMenuUI;

    public static bool isPaused = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
       continueButton =   GameObject.Find("ContinueButton");
       
           

    }
    void Start()
    {
        if(continueButton)
        continueButton.SetActive(false);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    healthbar.fillAmount = player.health / 100;
    //}

        void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeButton();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    //******* FOR PAUSE MENU //*******

    public void ResumeButton()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    public void RestartButton()
    {
       string currentScene =  SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    public void LoadButton()
    {
        SaveLoadManager.Instance.Load();
    }

   // **** FOR MAIN MENU // **** 

    public void ContinueButton()
    {
        SaveLoadManager.Instance.Load();
    }
    public void NewGameButton()
    {
        SceneManager.LoadScene("DemoScene");
    }
    public void SoundButton()
    {
        if (playing)
        { 
            SoundManager.Instance.StopPlaying("Theme");
            playing = false;
           
        }
        else if(!playing)
        {
            SoundManager.Instance.Play("Theme");
            playing = true;
        }
    }
    public void QuitButton()
    {
        Application.Quit();

    }
    



}
