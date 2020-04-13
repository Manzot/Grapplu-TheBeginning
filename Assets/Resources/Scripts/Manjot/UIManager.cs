using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UIManager : MonoBehaviour
{
    [HideInInspector]
    public Image healthbar;
    PlayerController player;
    Button continueButton;
    bool playing = true;
    public GameObject pauseMenuUI;
    public static bool isPaused = false;
    bool canContinue;
    ColorBlock newColors;
    string fileName = "Save.fun";
    private void Awake()
    {

        player = FindObjectOfType<PlayerController>();

        /*  */
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "MainMenu")
        {
            //continueButton = GameObject.Find("ContinueButton").GetComponent<Button>();

            //if (!continueButton)
            //{

            //    Debug.Log("Continue Button is not there in this Scene..");
            //}
            
            //    continueButton.interactable = canContinue;
            //    continueButton.gameObject.SetActive(false);
        }


    }
    void Start()
    {
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
        SaveLoadManager.Instance.ResetPosition();
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
    public void LoadButton()
    {
        SaveLoadManager.Instance.Load();
    }

    // **** FOR MAIN MENU // **** 

    public void ContinueButton()
    {


        string savedFile = Application.persistentDataPath + "Save.fun";
        Debug.Log(savedFile);

        if (File.Exists(savedFile))
        {
            canContinue = true;
            continueButton.interactable = canContinue;
            if (canContinue)
            {
                newColors.disabledColor = new Color(255, 255, 255);
                continueButton.colors = newColors;
                SaveLoadManager.Instance.Load();
            }
        }

        else
        {
            canContinue = false;
            newColors.disabledColor = new Color(0, 0, 0);
            continueButton.colors = newColors;
            continueButton.interactable = canContinue;
        }
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
        else if (!playing)
        {
            SoundManager.Instance.Play("Theme");
            playing = true;
        }
    }
    public void QuitButton()
    {
        Application.Quit();

    }

    public void LoadScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }



}
