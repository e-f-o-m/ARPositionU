using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Ajustes : MonoBehaviour
{

    public GameObject CorreoTvAj;
    public GameObject CambiarNomobreTvAj;
    public GameObject CambiarCodigoTvAj;
    public GameObject AjustesPanel; 
    public GameObject EliminarCuentaBtnAj; 

    [Header("Dialog Select")]
    public GameObject DialogoValiHo;
    public GameObject DescripcionTvDiHo;
    private int optionBtnSelected = 0;
    private Usuario user;
    private FirebaseController fc;
    
    void Start()
    {
        iniciarDB();
    }
    
    private async void iniciarDB()
    {
        fc = new FirebaseController();
        await fc.CheckUser();
        user = await fc.getUserData();
        setDataUserToViews();
    }

    private void setDataUserToViews()
    {
        CorreoTvAj.GetComponent<Text>().text = user.correo;
        CambiarNomobreTvAj.GetComponent<Text>().text = user.nombre;
        CambiarCodigoTvAj.GetComponent<Text>().text = user.codigo;     
    }


    public void OpenBtnDelete(){
        EliminarCuentaBtnAj.SetActive(true);
    }

    public void OpenPanelDialog(int option){
        Boolean isOpen = DialogoValiHo.activeSelf;
        DialogoValiHo.SetActive(!isOpen);
        optionBtnSelected = option;
        if(option == 1){
            DescripcionTvDiHo.GetComponent<Text>().text = "¿Seguro desea cerrar sesión?";
        }else if(option == 2){
            DescripcionTvDiHo.GetComponent<Text>().text = "¿Seguro desea eliminar la cuenta?";
        }
    }

    public void OpenPanel(GameObject panel)
    {
        AjustesPanel.SetActive(false);
        panel.SetActive(true);
    }


    public void aceptarDialogLogout(){
        if(optionBtnSelected == 1){
            cerrarSesion();
        }else if(optionBtnSelected == 2){
            eliminarCuenta();
        }
    }

    public void cerrarSesion(){
        fc.logout();
        SceneManager.LoadScene("ARCam");
    }

    public async void eliminarCuenta(){
        Boolean res = await fc.deleteUser();
        if(res){
            SceneManager.LoadScene("ARCam");
        }
    }
}
