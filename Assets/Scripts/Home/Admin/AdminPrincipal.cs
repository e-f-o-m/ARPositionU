using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminPrincipal : MonoBehaviour
{
    public GameObject AdminRootPanel;
    public GameObject AdminPrincipalPanel;
    public GameObject NuevoEventoPanel;
    public GameObject UsuariosPanel;

    public GameObject MiHorarioPanel;
    public GameObject EventosPanel;
    
    public void onBackPressed()
    {
        string backPress = PlayerPrefs.GetString("backPress", "");
        if (backPress == "Eventos AdminPrincipal"){
            EventosPanel.SetActive(true);
        } else if (backPress == "MiHorario AdminPrincipal"){
            MiHorarioPanel.SetActive(true);
        }
        AdminRootPanel.SetActive(false);
    }

    public void openNuevoEvento()
    {
        NuevoEventoPanel.SetActive(true);
        AdminPrincipalPanel.SetActive(false);
    }
    public void openUsuarios()
    {
        UsuariosPanel.SetActive(true);
        AdminPrincipalPanel.SetActive(false);
    }
}
