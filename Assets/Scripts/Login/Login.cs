using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Myscenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    [Header("Register")]
    public GameObject correoEtRe;

    public GameObject contrasenaEtRe;

    public GameObject nombreEtRe;

    public GameObject codigoEtRe;
    
    [Header("Login")]
    public GameObject correoEtLo;

    public GameObject contrasenaEtLo;
    public GameObject RetroalimentacionTxLo;
    public GameObject RetroalimentacionTxRe;
    public GameObject RetroalimentacionTxRc;

    [Header("Reset")]
    public GameObject correoEtRc;
    public GameObject EnviarBtnRc;
    public GameObject CancalarBtnRc;
    public GameObject VolverBtnRc;

    [Header("Other")]
    public GameObject LoginPanel;

    public GameObject RegisterPanel;

    public GameObject RecuperarPanel;

    [Header("Loading...")]
    public GameObject LoadingPanel;
    private Slider slider;

    private int T_LOGIN = 0;

    private int T_REGISTER = 1;

    private int T_RECUPERAR = 2;
    private FirebaseController fc;
    private Boolean isLogged = false;

    private Myscenes.Scenes newScene = new Myscenes.Scenes();

    void Start()
    {
        LoadingPanel.SetActive(true);
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        iniciarDB();
    }

    private async void iniciarDB()
    {
        slider.value = 40.0f;
        fc = new FirebaseController();
        isLogged = await fc.CheckUser();
        slider.value = 100.0f;
        if (isLogged)
        {
            newScene.LoadScene("Home");
        }
        LoadingPanel.SetActive(false);
    }

    public void AceptarAllLogin(int type)
    {
        optionLogin(type);
    }
    public async Task optionLogin(int type){
        LoadingPanel.SetActive(true);
        slider.value = 40.0f;
        if (type == T_REGISTER)
        {
            string stCorreoEtRe = correoEtRe.GetComponent<UnityEngine.UI.InputField>().text.Trim();
            string stContrasenaEtRe = contrasenaEtRe.GetComponent<UnityEngine.UI.InputField>().text;
            string stNombreEtRe = nombreEtRe.GetComponent<UnityEngine.UI.InputField>().text;
            string stCodigoEtRe = codigoEtRe.GetComponent<UnityEngine.UI.InputField>().text;

            if (stCorreoEtRe != "" && stContrasenaEtRe != "" && stNombreEtRe != "" && stCodigoEtRe != "")
            {
                correoEtRe.GetComponent<UnityEngine.UI.InputField>().interactable = false;
                contrasenaEtRe.GetComponent<UnityEngine.UI.InputField>().interactable = false;
                nombreEtRe.GetComponent<UnityEngine.UI.InputField>().interactable = false;
                codigoEtRe.GetComponent<UnityEngine.UI.InputField>().interactable = false;
                var res = await fc.registerNewUser(stCorreoEtRe, stContrasenaEtRe, stNombreEtRe, stCodigoEtRe);
                if(res != null){
                    RetroalimentacionTxRe.GetComponent<Text>().text = "Registro exitoso\nConfirme el correo que le hemos enviado para activar su cuenta";
                }else{
                    RetroalimentacionTxRe.GetComponent<Text>().text = "Error al registrar";
                }
            }
            else
            {
                RetroalimentacionTxRe.GetComponent<Text>().text = "Faltan datos";
            }
        }
        else if (type == T_LOGIN)
        {
            string stCorreoEtLo =
                correoEtLo.GetComponent<UnityEngine.UI.InputField>().text.Trim();
            string stContrasenaEtLo =
                contrasenaEtLo.GetComponent<UnityEngine.UI.InputField>().text;
            if (stCorreoEtLo != "" && stContrasenaEtLo != "")
            {
                Boolean bo = await fc.SingIn(stCorreoEtLo, stContrasenaEtLo);
                if (bo)
                {
                    newScene.LoadScene("Home");
                }else{
                    RetroalimentacionTxLo.GetComponent<Text>().text = "Usuario o contraseña incorrectos";
                }
            }
            else
            {
                RetroalimentacionTxLo.GetComponent<Text>().text = "Faltan datos";
            }
        }
        else if (type == T_RECUPERAR)
        {
            Boolean isError = true;
            EnviarBtnRc.SetActive(false);
            CancalarBtnRc.SetActive(false);
            VolverBtnRc.SetActive(true);

            string stCorreoEtRc = correoEtRc.GetComponent<UnityEngine.UI.InputField>().text.Trim();
            if (stCorreoEtRc != "")
            {
                string res = await fc.resetPassword(stCorreoEtRc);
                if (res != null)
                {
                    RetroalimentacionTxRc.GetComponent<Text>().text = "Se ha enviado un correo para restablecer su contraseña";
                    isError = false;
                }
                else
                {
                    RetroalimentacionTxRc.GetComponent<Text>().text = "Error al restablecer contraseña";
                }
            }
            else
            {
                RetroalimentacionTxRc.GetComponent<Text>().text = "Faltan datos";
            }
            if(isError){
                EnviarBtnRc.SetActive(true);
                CancalarBtnRc.SetActive(true);
                VolverBtnRc.SetActive(false);
            }
        }
        else
        {
            RetroalimentacionTxLo.GetComponent<Text>().text = "Error"; 
            RetroalimentacionTxRc.GetComponent<Text>().text = "Error"; 
            RetroalimentacionTxRe.GetComponent<Text>().text = "Error"; 
        }
        slider.value = 100.0f;
        LoadingPanel.SetActive(false);
    }

    public void CancelarReg()
    {
        newScene.LoadScene("ARCam");
    }

    public void OpenPanel(GameObject panel)
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(false);
        RecuperarPanel.SetActive(false);
        panel.SetActive(true);
        clearInpusts();
    }

    private void clearInpusts()
    {
        correoEtLo.GetComponent<UnityEngine.UI.InputField>().text = "";
        contrasenaEtLo.GetComponent<UnityEngine.UI.InputField>().text = "";
        correoEtRe.GetComponent<UnityEngine.UI.InputField>().text = "";
        contrasenaEtRe.GetComponent<UnityEngine.UI.InputField>().text = "";
        nombreEtRe.GetComponent<UnityEngine.UI.InputField>().text = "";
        codigoEtRe.GetComponent<UnityEngine.UI.InputField>().text = "";
        correoEtRc.GetComponent<UnityEngine.UI.InputField>().text = "";
    }

}
