using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

/* TODO:
 * Pasar el loading
 * Dialogos 
 * Buscar que mas falta...
 */
public class Home : MonoBehaviour
{
    public GameObject HomePanel;
    public GameObject AmbientesPanel;
    public GameObject AdminPanel;
    public GameObject cardItem;
    public GameObject contentHorario;
    [SerializeField] GameObject AceptarBtnH;
    private List<GameObject> horarios = new List<GameObject>();

    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();

    string LUGARES_REF = "lugares";
    string EVENTOS_REF = "eventos";
    string HORARIO_REF = "horario";
    string USUARIOS_REF = "usuarios";
    string TOKEN_USER = "";
    private Firebase.Auth.FirebaseUser user = null;
    private Firebase.Auth.FirebaseAuth auth = null;
    private string eventKeySelected = null;
    private DatabaseReference reference;


    private int count = 0;
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            horarios.Add(generateHorario());
        }
    }

    public GameObject generateHorario()
    {
        count++;
        GameObject g = (GameObject)Instantiate(cardItem, contentHorario.transform);
        g.SetActive(true);
        //TODO: change id, firebase database
        g.transform.Find("marginPanel/id").GetComponent<Text>().text = ""+count;
        return g;                   
    }

    public void setData(){
        horarios.Add(generateHorario());
    }

    public void selectItemHorario(GameObject id){
        Debug.Log("Selected item: " + id.GetComponent<Text>().text);
    }

    public void OpenPanel(GameObject panel)
    {
        HomePanel.SetActive(false);
        AmbientesPanel.SetActive(false);
        AdminPanel.SetActive(false);
        panel.SetActive(true);
    }

}
