using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ARCam : MonoBehaviour
{
    public GameObject clock_1;
    private Boolean isLogged = false;


    private FirebaseController fc;
    void Start()
    {
        iniciarDB();
    }

    void Update()
    {
        clock_1.GetComponent<Text>().text = DateTime.Now.ToString("HH:mm:ss");
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        isLogged = await fc.CheckUser();
    }

    public void openScene()
    {
        if (isLogged)
        {
            SceneManager.LoadScene("Home");
        }
        else
        {
            SceneManager.LoadScene("Login");
        }
    }

    
}
