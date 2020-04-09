using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image healthbar;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    healthbar.fillAmount = player.health / 100;
    //}


//******* FOR PAUSE MENU //*******

   public void ResumeButton()
    {

    }
    public void RestartButton()
    {

    }
    public void LoadButton()
    {

    }

   // **** FOR MAIN MENU // **** 

    public void ContinueButton()
    {

    }
    public void NewGameButton()
    {

    }
    public void SoundButton()
    {

    }
    public void QuitButton()
    {


    }
    



}
