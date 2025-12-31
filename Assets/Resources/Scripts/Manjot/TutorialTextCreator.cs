using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutorialTextCreator : MonoBehaviour
{
    public TextMeshProUGUI tutText;
    public string message;

    public void Start()
    {
        if(tutText.gameObject.activeSelf)
            tutText.gameObject.SetActive(false);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            tutText.gameObject.SetActive(true);
            tutText.text = message; 
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        tutText.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
