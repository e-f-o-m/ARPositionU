using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class Eventos : MonoBehaviour
{
    public GameObject contentAmbientes;
    public GameObject cardItem;
    public GameObject HomePanel;
    public GameObject AmbientesPanel;
    public GameObject AdminPanel;
    public GameObject BackBtnEv;
    
    [Header ("Dialog Select")]
    public GameObject DialogoValiAmb;
    public GameObject DescripcionTvDiEv;

    [Header("Private")]
    private List<GameObject> horarios = new List<GameObject>();
    private int count = 0;

    public InputField salon_id;
    public InputField facultad;
    public InputField nombre;
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

    void Start()
    {
        iniciarDB();
        
    }

    private async void iniciarDB()
    {
        FirebaseDatabase db = FirebaseDatabase.GetInstance("https://ar-position-u-default-rtdb.firebaseio.com/");
        //Unity persistence firebase
        db.SetPersistenceEnabled(false);
        reference = db.RootReference;
        reference.ValueChanged += HandleValueChanged;   

        await CheckUser();
        await buildCardsEvents();


    }

    private async Task CheckUser()
    {
        //slider.value = 40.0f;
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                try
                {
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                        if (auth.CurrentUser != null)
                        {
                            user = auth.CurrentUser;
                            TOKEN_USER = user.UserId;
                            Debug.Log("User logged in");
                        }
                        else
                        {
                            Debug.Log("User not logged in");
                        }
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
                return;

            });
        //slider.value = 100.0f;
        //LoadingPanel.SetActive(false);
        
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log(args.Snapshot);
    }

    public GameObject generateGaObEvent(Evento _evento)
    {
        count++;
        GameObject g = (GameObject)Instantiate(cardItem, contentAmbientes.transform);
        g.SetActive(true);
        
        g.transform.Find("marginPanel/id").GetComponent<Text>().text = ""+_evento.evento_key;

        string _lugar = lugares.Find(x => x.lugar_key == _evento.lugar_key).nombre_lugar;
        g.transform.Find("marginPanel/Salon").GetComponent<Text>().text = _lugar;
        
        g.transform.Find("marginPanel/Evento").GetComponent<Text>().text = _evento.nombre_evento;

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

    public void selectItemAmbientes(GameObject id_event){
        OpenPanelDialog(id_event.GetComponent<Text>().text);
    }

    public void OpenPanelDialog(string id_event){
        eventKeySelected = id_event;
        Boolean isOpen = DialogoValiAmb.activeSelf;
        DialogoValiAmb.SetActive(!isOpen);
    }

    public void OpenPanel(GameObject panel)
    {
        //TODO: gragar
        //panelAgregar.SetActive(false);
        //panelListar.SetActive(false);
        HomePanel.SetActive(false);
        AmbientesPanel.SetActive(false);
        AdminPanel.SetActive(false);
        panel.SetActive(true);
    }


    public void aceptarDialog(){
        DialogoValiAmb.SetActive(false);
        MiHorario miHorario = new MiHorario();
        miHorario.evento_key = eventKeySelected;
        addEvento(miHorario);
    }

    private async void addEvento(MiHorario miHorario)
    {
        var newRef = reference.Child(USUARIOS_REF).Child(TOKEN_USER).Child(HORARIO_REF);
        string ID = newRef.Push().Key;
        miHorario.horario_key = ID;
        string json = JsonUtility.ToJson(miHorario);
        await newRef.Child(ID).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCanceled)
            { 
                Debug.LogError("SetRawJsonValueAsync was canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SetRawJsonValueAsync encountered an error: " + task.Exception);
            }
            Debug.Log("SetRawJsonValueAsync succeeded.");
        });
    }

    private async Task buildCardsEvents(){
        await getEventos();
        await getLugares();
        for (int i = 0; i < eventos.Count; i++)
        {
            horarios.Add(generateGaObEvent(eventos[i]));
        }
    }

    private async Task getEventos()
    {
        await reference.Child(EVENTOS_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de evento eventos: "+EVENTOS_REF);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Evento evento = JsonUtility.FromJson<Evento>(item.GetRawJsonValue());
                    eventos.Add(evento);
                }
            }
        });
    }
    
    private async Task getLugares()
    {
        await reference.Child(LUGARES_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de lugar"+LUGARES_REF);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Lugar lugar = JsonUtility.FromJson<Lugar>(item.GetRawJsonValue());
                    lugares.Add(lugar);
                }
            }
        });
    }

}

