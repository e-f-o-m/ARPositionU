using System.Collections;

/* TODO: descargar
com.google.external-dependency-manager-1.2.170.tgz
com.google.firebase.app-8.9.0.tgz
com.google.firebase.auth-8.9.0.tgz
com.google.firebase.database-8.9.0.tgz

mover a la ruta "/GooglePackages/
*/

/* TODO:
 * Falta Pasar los datos del usuario a la database de firebase (La contraseña no, sino el token)
 * Problemas con codigo asincrono: async await: no entra en newScene en login
 * Buscar que mas falta...
 */
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using UnityEngine.SceneManagement;
using Myscenes;

public class Login : MonoBehaviour
{
    Firebase.Auth.FirebaseAuth auth = null;
    Firebase.Auth.FirebaseUser user = null;

    [Header("Register")]
    public GameObject correoEtRe;
    public GameObject contrasenaEtRe;
    public GameObject nombreEtRe;
    public GameObject codigoEtRe;
    public GameObject cancelarBtnRe;
    public GameObject aceptarBtnRe;
    public GameObject iniciarSesionBtnRe;

    [Header("Login")]
    public GameObject correoEtLo;
    public GameObject contrasenaEtLo;

    [Header("Reset")]
    public GameObject correoEtRc;

    [Header("Other")]
    public GameObject LoginPanel;
    public GameObject RegisterPanel;
    public GameObject RecuperarPanel;


    public GameObject LoadingPanel;
    private Slider slider;

    private int T_LOGIN = 0;
    private int T_REGISTER = 1;
    private int T_RECUPERAR = 2;

    private Myscenes.Scenes newScene = new Myscenes.Scenes();

    void Start()
    {
        LoadingPanel.SetActive(true);
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        StartCoroutine(LoadingScene(0.02f));

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                user = auth.CurrentUser;

                Debug.Log("user " + user.Email);
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "xxxxxxxxx Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    IEnumerator LoadingScene(float speed)
    {
        slider.value = 0.0f;
        float value = 0.0f;

        while (value <= 100.0f)
        {
            yield return new WaitForSeconds(speed);
            slider.value = value;
            value += 10.0f;
        }
        LoadingPanel.SetActive(false);
    }


    public void AceptarAllLogin(int type)
    {
        LoadingPanel.SetActive(true);
        StartCoroutine(LoadingScene(0.03f));
        if (type == T_REGISTER)
        {
            string stCorreoEtRe = correoEtRe.GetComponent<UnityEngine.UI.InputField>().text.Trim();
            string stContrasenaEtRe = contrasenaEtRe.GetComponent<UnityEngine.UI.InputField>().text;
            string stNombreEtRe = nombreEtRe.GetComponent<UnityEngine.UI.InputField>().text;
            string stCodigoEtRe = codigoEtRe.GetComponent<UnityEngine.UI.InputField>().text;
            if (stCorreoEtRe != "" && stContrasenaEtRe != "" && stNombreEtRe != "" && stCodigoEtRe != "")
            {
                RegisterNewUser(stCorreoEtRe, stContrasenaEtRe, stNombreEtRe, stCodigoEtRe);
            }
            else
            {
                Debug.Log("Error: campos vacios");
            }
        }
        else if (type == T_LOGIN)
        {
            string stCorreoEtLo = correoEtLo.GetComponent<UnityEngine.UI.InputField>().text.Trim();
            string stContrasenaEtLo = contrasenaEtLo.GetComponent<UnityEngine.UI.InputField>().text;
            if (stCorreoEtLo != "" && stContrasenaEtLo != "")
            {
                SingIn(stCorreoEtLo, stContrasenaEtLo);
            }
            else
            {
                Debug.Log("Error: campos vacios");
            }
        }
        else if (type == T_RECUPERAR)
        {
            string stCorreoEtRc = correoEtRc.GetComponent<UnityEngine.UI.InputField>().text.Trim();
            if (stCorreoEtRc != "")
            {
                resetPassword(stCorreoEtRc);
            }
            else
            {
                Debug.Log("Error: campos vacios");
            }
        }
        else
        {
            Debug.Log("Error: tipo de login no definido");
        }
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


    // ====================== FIREBASE ======================

    public void SingIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("xxxxxx SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("xxxxxx SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            user = task.Result;
            Debug.Log("xxxxx Acceso valido. user " + user.Email);
            Boolean isVerifiedEmail = auth.CurrentUser.IsEmailVerified;
            Debug.Log("verify email::::: " + isVerifiedEmail);
            if (isVerifiedEmail)
            {
                //FIXME: no ejecuta las funciones
                Debug.Log("========== Debe entrar aquí===========");
                LoginPanel.SetActive(false);
                RegisterPanel.SetActive(false);
                RecuperarPanel.SetActive(false);
                this.newScene.LoadScene("Home");
            }

        });
    }
    public Task RegisterNewUser(string email, string password, string stNombreEtRe, string stCodigoEtRe)
    {
        return auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            user = task.Result;
            user.SendEmailVerificationAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("xxxxxx Email sent successfully.");
                OpenPanel(LoginPanel);
            });
        });
    }

    public void SignOut()
    {
        auth.SignOut();
        user = null;
        CancelarReg();
    }

    public void resetPassword(string emailAddress)
    {
        if (emailAddress.Trim() != "")
        {
            auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("xxxxxxx Password reset email sent successfully.");
                OpenPanel(LoginPanel);

            });
        }
        else
        {
            Debug.Log("xxxxxxx Error: campos vacios");
        }
    }

}

