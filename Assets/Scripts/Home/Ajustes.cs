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
    public GameObject HomePanel; 
    public GameObject EliminarCuentaBtnAj; 

    [Header("Dialog Select")]
    public GameObject DialogoValiHo;
    public GameObject DescripcionTvDiHo;
    public GameObject DialogoDataUser;
    private int optionBtnSelected = 0;
    private int optionUpdate = 0;
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
        bool res = await fc.deleteUser();
        if(res){
            SceneManager.LoadScene("ARCam");
        }
    }


    public async void setDataFirebase(Usuario usuario)
    {
        bool b = await fc.updateUsuario(usuario);
        if(b){
            setDataUserToViews();
        }
        closetDialogUser();
    }

    public void updateDataUser(GameObject input){
        string strData = input.GetComponent<Text>().text;
        if(optionUpdate == 0){
            UpdateNameUser(strData);
        }else{
            UpdateCodeUser(strData);
        }
    }

    public void UpdateNameUser(string name){
        user.nombre = name;
        setDataFirebase(user);
    }

    public void UpdateCodeUser(string code){
        user.codigo = code;
        setDataFirebase(user);
    }


    public void closetDialogUser(){
        DialogoDataUser.SetActive(false);
    }

    public void openDialogUser(int option){
        if(option == 0){
            DialogoDataUser.SetActive(true);
            DialogoDataUser.transform.Find("TituloTvDiHo").GetComponent<Text>().text = "Cambiar Nombre";
            DialogoDataUser.transform.Find("UserInput").GetComponent<InputField>().text = user.nombre;
        }else if(option == 1){
            DialogoDataUser.SetActive(true);
            DialogoDataUser.transform.Find("TituloTvDiHo").GetComponent<Text>().text = "Cambiar Código";
            DialogoDataUser.transform.Find("UserInput").GetComponent<InputField>().text = user.codigo;
        }
        optionUpdate = option;
    }


        void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                AjustesPanel.SetActive(false);
                HomePanel.SetActive(true);
            }
        }
    }


}
