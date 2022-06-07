using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
//regex
using System.Text.RegularExpressions;
//normalize form
using System.Text;

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

    [Header("Dialog Select")]
    public GameObject DialogoValiEv;
    public GameObject DescripcionTvDiEv;
    public GameObject NewEventBtnEv;
    public GameObject EditarBtnDiEv;

    [Header("Private")]
    private int count = 0;
    private List<GameObject> eventosGmOb;
    private List<Evento> eventos;
    private List<Lugar> lugares;
    private string eventKeySelected = null;
    private FirebaseController fc;

    private Usuario user;

    //private HomeScenes scenes = new HomeScenes();

    void OnEnable()
    {
        eventosGmOb = new List<GameObject>();
        eventos = new List<Evento>();
        lugares = new List<Lugar>();

        iniciarDB();
    }


    void OnDisable()
    {
        for (int i = 0; i < eventosGmOb.Count; i++)
        {
            Destroy(eventosGmOb[i].gameObject);
        }
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        bool isLogged = await fc.CheckUser();
        eventos = await fc.getEventos();
        lugares = await fc.getLugares();

        Debug.Log("is login " + isLogged);
        if (isLogged)
        {
            user = await fc.getUserData();
            if (user != null)
            {
                Debug.Log("x xx x xx x x user: " + user.rol);
                if (user.rol > 0)
                {
                    NewEventBtnEv.SetActive(true);
                    EditarBtnDiEv.SetActive(true);
                }
            }
        }
        else
        {
            Debug.Log("No hay usuario");
        }

        generateGaObEventos();
    }

    public void generateGaObEventos()
    {
        for (int i = 0; i < eventos.Count; i++)
        {
            var _evento = eventos[i];
            GameObject g = (GameObject)Instantiate(cardItem, contentAmbientes.transform);
            g.SetActive(true);

            g.transform.Find("marginPanel/id").GetComponent<Text>().text = "" + _evento.evento_key;

            string _lugar = lugares.Find(x => x.lugar_key == _evento.lugar_key).nombre_lugar;
            g.transform.Find("marginPanel/Salon").GetComponent<Text>().text = _lugar;

            g.transform.Find("marginPanel/Evento").GetComponent<Text>().text = _evento.nombre_evento;

            Color selectedColor = new Color(79 / 255.0f, 140 / 255.0f, 238 / 255.0f);
            string[] days = { "Lu", "Ma", "Mi", "Ju", "Vi", "Sa", "Do" };
            string day = days[_evento.dia - 1];
            g.transform.Find("marginPanel/" + day).GetComponent<Text>().color = selectedColor;
            eventosGmOb.Add(g);
        }
    }

    public void selectItemAmbientes(GameObject id_event)
    {
        OpenPanelDialog(id_event.GetComponent<Text>().text);
    }

    public void OpenPanelDialog(string id_event)
    {
        eventKeySelected = id_event;
        Boolean isOpen = DialogoValiEv.activeSelf;
        DialogoValiEv.SetActive(!isOpen);
        PlayerPrefs.SetString("eventKeySelected", eventKeySelected);
        PlayerPrefs.SetString("backPress", "Eventos");
        PlayerPrefs.Save();
    }

    private void closetDialog()
    {
        if (DialogoValiEv.activeSelf)
        {
            DialogoValiEv.SetActive(false);
        }
    }

    public void aceptarDialog()
    {
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
        try
        {


            string _search = SearchEtEv.GetComponent<InputField>().text;
            if (_search.Length > 0)
            {
                for (int i = 0; i < eventosGmOb.Count; i++)
                {

                    string strSalon = eventosGmOb[i].transform.Find("marginPanel/Evento").GetComponent<Text>().text;
                    string strEvento = eventosGmOb[i].transform.Find("marginPanel/Salon").GetComponent<Text>().text;

                    //palabras sin tildes y en minusculas
                    strSalon = Regex.Replace(strSalon.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower();
                    strEvento = Regex.Replace(strEvento.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower();
                    _search = Regex.Replace(_search.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "").ToLower();

                    if (strSalon.Contains(_search) || strEvento.Contains(_search))
                    {
                        eventosGmOb[i].SetActive(true);
                    }
                    else
                    {
                        Debug.Log("strSalon: " + strSalon
                        + "\nstrEvento: " + strEvento
                        + "\n_search: " + _search);
                        eventosGmOb[i].SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < eventosGmOb.Count; i++)
                {
                    eventosGmOb[i].SetActive(true);
                }
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Error al filtrar eventos");
        }
    }

    public void BackPress()
    {
        MiHorarioPanel.SetActive(true);
        EventosPanel.SetActive(false);
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                closetDialog();
                BackPress();
            }
        }
    }

}

