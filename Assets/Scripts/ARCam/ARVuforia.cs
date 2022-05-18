using UnityEngine;
using System.Collections;
using Vuforia;
public class ARVuforia : MonoBehaviour
{
    private GameObject btn_1;
    void Start()
    {
        btn_1 = GameObject.Find("Canvas/Button");
        btn_1.gameObject.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonPressed(OnButtonPressed);
        btn_1.gameObject.GetComponent<VirtualButtonBehaviour>().RegisterOnButtonReleased(OnButtonReleased);
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        print("*** button Pressed " + vb.VirtualButtonName);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        print("*** button Released " + vb.VirtualButtonName);
    }
}