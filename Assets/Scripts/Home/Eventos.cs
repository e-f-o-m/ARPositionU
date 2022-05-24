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
    [Header("Panels Admin")]
    public GameObject MiHorarioPanel;
    public GameObject AdminPanelRoot;
    public GameObject EventosPanel;
    [Header("Cards")]
    public GameObject contentAmbientes;
    public GameObject cardItem;
    public GameObject BackBtnEv;

    [Header("search")]
    public GameObject SearchEtEv;
    
    [Header ("Dialog Select")]
    public GameObject DialogoValiEv;
    public GameObject DescripcionTvDiEv;

    [Header("Private")]
    private int count = 0;
    private List<GameObject> horarios = new List<GameObject>();
    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();
    private string eventKeySelected = null;
    private FirebaseController fc;

    //private HomeScenes scenes = new HomeScenes();

    void Start()
    {
        iniciarDB();
    }

    void OnDisable()
    {
        for(int i=0; i<horarios.Count; i++) {
            Destroy(horarios[i].gameObject);
        }
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        await fc.CheckUser();
        eventos = await fc.getEventos();
        lugares = await fc.getLugares();
        generateGaObEventos();
    }

    public void generateGaObEventos()
    {
        for (int i = 0; i < eventos.Count; i++)
        {
            var _evento = eventos[i];
            GameObject g = (GameObject)Instantiate(cardItem, contentAmbientes.transform);
            g.SetActive(true);
            
            g.transform.Find("marginPanel/id").GetComponent<Text>().text = ""+_evento.evento_key;

            string _lugar = lugares.Find(x => x.lugar_key == _evento.lugar_key).nombre_lugar;
            g.transform.Find("marginPanel/Salon").GetComponent<Text>().text = _lugar;
            
            g.transform.Find("marginPanel/Evento").GetComponent<Text>().text = _evento.nombre_evento;

            Color selectedColor = new Color(79/255.0f, 140/255.0f, 238/255.0f);
            string [] days  = {"Lu", "Ma", "Mi", "Ju", "Vi", "Sa", "Do"};
            string day = days[_evento.dia - 1];
            g.transform.Find("marginPanel/"+day).GetComponent<Text>().color = selectedColor;
            horarios.Add(g);
        }          
    }

    public void selectItemAmbientes(GameObject id_event){
        OpenPanelDialog(id_event.GetComponent<Text>().text);
    }

    public void OpenPanelDialog(string id_event){
        eventKeySelected = id_event;
        Boolean isOpen = DialogoValiEv.activeSelf;
        DialogoValiEv.SetActive(!isOpen);
        PlayerPrefs.SetString("eventKeySelected", eventKeySelected);
        PlayerPrefs.SetString("backPress", "Eventos");
        PlayerPrefs.Save();
    }

    public void aceptarDialog(){
        MiHorario miHorario = new MiHorario();
        miHorario.evento_key = eventKeySelected;
        fc.addHorario(miHorario);
        DialogoValiEv.SetActive(false);
    }

    public void OpenNewEvent()
    {
        PlayerPrefs.SetString("eventKeySelected", "");
        PlayerPrefs.SetString("backPress", "Eventos NuevoEvento");
        PlayerPrefs.Save();
        AdminPanelRoot.SetActive(true);
        EventosPanel.SetActive(false);
    }

    public void OpenEditEvent()
    {
        PlayerPrefs.SetString("eventKeySelected", eventKeySelected);
        PlayerPrefs.SetString("backPress", "Eventos EditarEvento");
        PlayerPrefs.Save();
        AdminPanelRoot.SetActive(true);
        EventosPanel.SetActive(false);
    }

    public void filtrarEventos()
    {
        string _search = SearchEtEv.GetComponent<InputField>().text;
        if (_search.Length > 0)
        {
            for (int i = 0; i < horarios.Count; i++)
            {
                if (horarios[i].transform.Find("marginPanel/Evento").GetComponent<Text>().text.Contains(_search)
                || horarios[i].transform.Find("marginPanel/Salon").GetComponent<Text>().text.Contains(_search))
                {
                    horarios[i].SetActive(true);
                }
                else
                {
                    horarios[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < horarios.Count; i++)
            {
                horarios[i].SetActive(true);
            }
        }
    }

    public void BackPress()
    {
        MiHorarioPanel.SetActive(true);
        EventosPanel.SetActive(false);
    }

}

