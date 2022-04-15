using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;

public class Eventos : MonoBehaviour
{
    public GameObject cardItem;
    public GameObject contentAmbientes;
    public GameObject HomePanel;
    public GameObject AmbientesPanel;
    public GameObject AdminPanel;
    GameObject AceptarBtnH;
    private List<GameObject> horarios = new List<GameObject>();
    private int count = 0;
    private DatabaseReference reference;
    public string res;
    [Header("Agregar")]
    public InputField salon_id;
    public InputField facultad;
    public InputField nombre;
    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();

    void Start()
    {
        Debug.Log("1");
        buildCardsEvents();
    }

    private void iniciarDB()
    {
        FirebaseDatabase db = FirebaseDatabase.GetInstance("https://ar-position-u-default-rtdb.firebaseio.com/");
        reference = db.RootReference;
    }

    public GameObject generateGaObEvent(Evento _evento)
    {
        count++;
        GameObject g = (GameObject)Instantiate(cardItem, contentAmbientes.transform);
        g.SetActive(true);
        
        g.transform.Find("marginPanel/id").GetComponent<Text>().text = ""+_evento.evento_key;

        string _lugar = "lugares.Find(x => x.lugar_key == _evento.lugar_key).nombre_lugar";
        g.transform.Find("marginPanel/salon").GetComponent<Text>().text = _lugar;
        g.transform.Find("marginPanel/evento").GetComponent<Text>().text = _evento.nombre_evento;
        if(_evento.dia == 1){
            g.transform.Find("marginPanel/Lu").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }else if(_evento.dia == 2){
            g.transform.Find("marginPanel/Ma").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }else if(_evento.dia == 3){
            g.transform.Find("marginPanel/Mi").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }else if(_evento.dia == 4){
            g.transform.Find("marginPanel/Ju").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }else if(_evento.dia == 5){
            g.transform.Find("marginPanel/Vi").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }else if(_evento.dia == 6){
            g.transform.Find("marginPanel/Sa").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }else if(_evento.dia == 7){
            g.transform.Find("marginPanel/Do").GetComponent<Text>().color = new Color(0.2f,0.3f,0.7f,0.6f);
        }
        
        return g;                   
    }

    public void selectItemAmbientes(GameObject id){
        Debug.Log("Selected item: " + id.GetComponent<Text>().text);
    }

    public void OpenPanel(GameObject panel)
    {
        //panelAgregar.SetActive(false);
        //panelListar.SetActive(false);
        HomePanel.SetActive(false);
        AmbientesPanel.SetActive(false);
        AdminPanel.SetActive(false);
        panel.SetActive(true);
    }

    /* public void guardarAmbiente()
    {
        string ambSalonid = salon_id.text.Trim();
        string ambFacultad = facultad.text.Trim();
        string ambNombre = nombre.text.Trim();
        add(ambSalonid, ambFacultad, ambNombre);
        clearAmb();
    }

    private void clearAmb()
    {
        salon_id.text = "";
        facultad.text = "";
        nombre.text = "";
    }
    

    //Firebase Realtime
    private async void add(string salon_id, string facultad, string nombre)
    {
        Ambiente ambiente = new Ambiente(salon_id, facultad, nombre);
        string json = JsonUtility.ToJson(ambiente);
        iniciarDB();
        string ID = reference.Child(name_collection).Push().Key;

        string res = await reference.Child(name_collection).Child(ID).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                return "Cancelado";
            }
            if (task.IsFaulted)
            {
                return "Error: " + task.Exception;
            }
            return "Ambiente Creado";
        });
    }
 */

    private async void buildCardsEvents(){
        await getEventos();
        await getLugares();
        Debug.Log("xxxxxx Count:"+eventos.Count);
        for (int i = 0; i < eventos.Count; i++)
        {
            horarios.Add(generateGaObEvent(eventos[i]));
        }
    }

    private async Task getEventos()
    {
        iniciarDB();
        string NAME = "eventos";

        await reference.Child(NAME).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de evento"+NAME);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot temp = task.Result;
                var snapshot = temp.Child("0");
                foreach (var item in snapshot.Children)
                {
                    Debug.Log("xxxxx Evento: " + item.GetRawJsonValue());
                    Evento evento = JsonUtility.FromJson<Evento>(item.GetRawJsonValue());
                    
                    eventos.Add(evento);
                }
            }
        });
    }
    
    private async Task getLugares()
    {
        iniciarDB();
        string NAME = "lugares";
        await reference.Child(NAME).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de lugar"+NAME);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Debug.Log("Lugar: " + item.GetRawJsonValue());
                    Lugar lugar = JsonUtility.FromJson<Lugar>(item.GetRawJsonValue());
                    lugares.Add(lugar);
                }
            }
        });
    }

}

