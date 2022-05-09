using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class Usuarios : MonoBehaviour
{
    public GameObject UsuriosPanel;
    public GameObject AdminPrincipal;

    [Header("Cards")]
    public GameObject ContentUsers;
    public GameObject CardPanelUser;

    [Header("Dialog Select")]
    public GameObject DialogoUser;
    public GameObject TgEstudiante;
    public GameObject TgAdmin;
    public GameObject TgSAdmin;

    public GameObject SearchEtUs;

    private FirebaseController fc;
    private Usuario usuario = new Usuario();
    private List<Usuario> usuarios = new List<Usuario>();
    private List<GameObject> usuariosGaOb = new List<GameObject>();
    private Boolean isRepet = false;
    private int optionRol = 0;
    string usuarioKeySelected = "";

    // get data firebase
    // set data vies
    void OnEnable()
    {
        iniciarDB();
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        usuarios = await fc.getUsuarios();

        generateGaObUsuarios();
    }

    void OnDisable()
    {
        for(int i=0; i<usuariosGaOb.Count; i++) {
            Destroy(usuariosGaOb[i].gameObject);
        }
    }

    public void changeToggle(int _option)
    {
        if (isRepet == false){
            isRepet = true;
            if (_option == 0) {
                if (TgSAdmin.GetComponent<Toggle>().isOn)
                    TgSAdmin.GetComponent<Toggle>().isOn = false;
                if (TgAdmin.GetComponent<Toggle>().isOn)
                    TgAdmin.GetComponent<Toggle>().isOn = false;
            } else if (_option == 1) {
                if (TgSAdmin.GetComponent<Toggle>().isOn)
                    TgSAdmin.GetComponent<Toggle>().isOn = false;
                if (TgEstudiante.GetComponent<Toggle>().isOn)
                    TgEstudiante.GetComponent<Toggle>().isOn = false;
            } else {
                if (TgAdmin.GetComponent<Toggle>().isOn)
                    TgAdmin.GetComponent<Toggle>().isOn = false;
                if (TgEstudiante.GetComponent<Toggle>().isOn)
                    TgEstudiante.GetComponent<Toggle>().isOn = false;
            }
            optionRol = _option;
            isRepet = false;
        }
    }

    public void generateGaObUsuarios()
    {

        for (int i = 0; i < usuarios.Count; i++)
        {
            var _usuario = usuarios[i];
            GameObject g = (GameObject)Instantiate(CardPanelUser, ContentUsers.transform);
            g.SetActive(true);

            g.transform.Find("marginPanel/id").GetComponent<Text>().text = "" + _usuario.usuario_key;
            g.transform.Find("marginPanel/Nombre").GetComponent<Text>().text = _usuario.nombre;
            g.transform.Find("marginPanel/Correo").GetComponent<Text>().text = _usuario.correo;
            g.transform.Find("marginPanel/Codigo").GetComponent<Text>().text = _usuario.codigo;
            g.transform.Find("marginPanel/Rol").GetComponent<Text>().text = _usuario.rol.ToString();

            usuariosGaOb.Add(g);
        }
    }

    public void aceptarDialogRol ()
    {
        Usuario _usuario = usuarios.Find(x => x.usuario_key == usuarioKeySelected);
        _usuario.rol = optionRol;
        setDataFirebase(_usuario);
    }

    public async void setDataFirebase(Usuario usuario)
    {
        Boolean b = await fc.updateRolUser(usuario);
        closetDialogRole();
    }

    public void closetDialogRole()
    {
        DialogoUser.SetActive(false);
    }

    public void openDialogUser(GameObject usuario_key)
    {
        usuarioKeySelected = usuario_key.GetComponent<Text>().text;
        usuario = usuarios.Find(x => x.usuario_key == usuarioKeySelected);
        DialogoUser.SetActive(true);
        changeToggle(usuario.rol);
        optionRol = usuario.rol;
        if (usuario.rol == 0) {
            TgEstudiante.GetComponent<Toggle>().isOn = true;
        }else if(usuario.rol == 1) {
            TgAdmin.GetComponent<Toggle>().isOn = true;
        }else {   
            TgSAdmin.GetComponent<Toggle>().isOn = true;
        }
        
    }

    
    public void onBackPressed()
    {
        AdminPrincipal.SetActive(true);
        UsuriosPanel.SetActive(false);
    }
    

    public void filtrarUsuarios()
    {
        string _search = SearchEtUs.GetComponent<InputField>().text;
        if (_search.Length > 0)
        {
            for (int i = 0; i < usuariosGaOb.Count; i++)
            {
                if (usuariosGaOb[i].transform.Find("marginPanel/Nombre").GetComponent<Text>().text.Contains(_search)
                || usuariosGaOb[i].transform.Find("marginPanel/Correo").GetComponent<Text>().text.Contains(_search)
                || usuariosGaOb[i].transform.Find("marginPanel/Codigo").GetComponent<Text>().text.Contains(_search))
                {
                    usuariosGaOb[i].SetActive(true);
                }
                else
                {
                    usuariosGaOb[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < usuariosGaOb.Count; i++)
            {
                usuariosGaOb[i].SetActive(true);
            }
        }
    }
}
