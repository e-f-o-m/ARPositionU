using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class HomeScenes : MonoBehaviour
{
    private static HomeScenes homeScenes;

    private HomeScenes(){}
 
    public static HomeScenes Instance
    {
        get
        {
            if (homeScenes == null)
            {
                homeScenes = new HomeScenes();
            }
            return homeScenes;
        }
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void openOnlyPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void closetPanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
