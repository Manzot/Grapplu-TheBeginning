using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerPersistence
{

    public static void SaveData(PlayerController playerController)
    {
        /*PlayerPrefs.SetFloat("x", playerData.Location.x);
        PlayerPrefs.SetFloat("y", playerData.Location.y);*/
        /* PlayerPrefs.SetFloat("health", playerController.playerData.Health);*/
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "Save.fun";
        FileStream file = new FileStream(path, FileMode.Create);
        PlayerData playerData = new PlayerData(playerController);
        binaryFormatter.Serialize(file, playerData);
        file.Close();
        


    }
    public static PlayerData LoadData()
    {
        /*float x = PlayerPrefs.GetFloat("x");
       float y = PlayerPrefs.GetFloat("y");
       float health = PlayerPrefs.GetFloat("health");

       PlayerData playerData = new PlayerData()
       {
           Location = new Vector2(x, y),
           *//*Health = health*//*
       };
       return playerData;*/

        string path = Application.persistentDataPath + "Save.fun";
        if (File.Exists(path))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = new FileStream(path, FileMode.Open);
            PlayerData playerData = binaryFormatter.Deserialize(file) as PlayerData;

            file.Close();
            return playerData;
        }
        else
        {
            Debug.LogError("Can't Load The Game, File Doesn't Exists.. "+ path);
            return null;
            
        }
       
    }
}


