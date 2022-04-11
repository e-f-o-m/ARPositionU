using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCam : MonoBehaviour
{
    public GameObject ARCamCanvas;
    public GameObject HomeBtnAR;
    private Myscenes.Scenes newScene = new Myscenes.Scenes();

    public void openScene()
    {
        newScene.LoadScene("Login");
    }

    
}
