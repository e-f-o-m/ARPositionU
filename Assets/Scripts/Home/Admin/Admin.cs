using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Admin : MonoBehaviour
{
    public GameObject UsuriosPanel;
    public GameObject EventosPanel;
    public GameObject NuevoEventoPanel;
    public GameObject AdminPrincipalPanel;
    
    void OnEnable(){
        NuevoEventoPanel.SetActive(false);
        UsuriosPanel.SetActive(false);
        AdminPrincipalPanel.SetActive(false);
        string backPress = PlayerPrefs.GetString("backPress", "");
        if (backPress == "Eventos NuevoEvento" || backPress == "Eventos EditarEvento") {
            NuevoEventoPanel.SetActive(true);
        } else if (backPress == "MiHorario Usuarios"){
            UsuriosPanel.SetActive(true);
        } else if (backPress == "MiHorario AdminPrincipal"){
            AdminPrincipalPanel.SetActive(true);
        }
    }
}
