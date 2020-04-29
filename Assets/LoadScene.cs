using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadScene : MonoBehaviour
{
    public string sceneName;
    public float loadAfter;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TimerDelg.Instance.Add(() => { SceneManager.LoadScene(sceneName); }, loadAfter);
            SaveLoadManager.Instance.position = new Vector2(10, 13);
        }
    }
    
}
