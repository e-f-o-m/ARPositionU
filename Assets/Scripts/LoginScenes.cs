using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoginScenes : MonoBehaviour
{
    [Header("Panels Login")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject recuperarPanel;
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }
}
