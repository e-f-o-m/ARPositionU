using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class NuevoEvento : MonoBehaviour
{
    [Header("Panels Admin")]
    public GameObject AdminPrincipalPanel;
    public GameObject AdminRootPanel;
    public GameObject EventosPanel;
    public GameObject NuevoEventoPanel;

    public GameObject TimePickerPanel;

    [Header("Form")]
    public Dropdown DropdownLugarNE;
    public GameObject NombreEventoNE;
    public GameObject ResponsableNE;
    public GameObject DescripcionNE;
    public GameObject contentDias;
    public GameObject InicioBtn;
    public GameObject FinBtn;
    public GameObject RetroalimentacionNE;
    // horas y minutos
    [Header("Scrollview")]
    public GameObject contentHoras;
    public GameObject cardItemHoras;
    public GameObject contentMinutos;
    public GameObject cardItemMinutos;

    [Header("Time Picker")]
    public GameObject amBtn;
    public GameObject pmBtn;
    // Aceptar y Cancelar


    // -- private --
    private List<int> hours = new List<int>();
    private List<int> minutes = new List<int>();

    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();
    private Boolean isInicio = true;


    private int eventHourSelected = 0;
    private int eventMinutSelected = 0;
    private FirebaseController fc;
    private Evento evento = new Evento();

    // get data firebase
    // set data vies
    void OnEnable()
    {
        var test = PlayerPrefs.GetString("eventKeySelected", "");
        generateTimePicker();
        iniciarDB();
    }

    void OnFirstDropdownValueChange(int value)
    {
        evento.lugar_key = lugares[value].lugar_key;
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        await fc.CheckUser();
        lugares = await fc.getLugares();

        DropdownLugarNE.ClearOptions();


        for (int i = 0; i < lugares.Count; i++)
        {
            DropdownLugarNE.AddOptions(new List<string>() { lugares[i].nombre_lugar });
        }

        DropdownLugarNE.onValueChanged.AddListener(OnFirstDropdownValueChange);


        //set data views
        string evento_key = PlayerPrefs.GetString("eventKeySelected", "");
        if (evento_key != "")
        {
            evento = await fc.getEvento(evento_key);

            //select dropdown
            DropdownLugarNE.value = lugares.FindIndex(x => x.lugar_key == evento.lugar_key);

            NombreEventoNE.GetComponent<InputField>().text = evento.nombre_evento;
            ResponsableNE.GetComponent<InputField>().text = evento.responsable;
            DescripcionNE.GetComponent<InputField>().text = evento.descripcion;

            InicioBtn.GetComponentInChildren<Text>().text = evento.hora_inicio;
            FinBtn.GetComponentInChildren<Text>().text = evento.hora_fin;
            SelectDay(evento.dia);
        }
    }

    public void generateTimePicker()
    {
        //TimePicker.SetActive(false);
        for (int i = 7; i < 24; i++)
        {
            hours.Add(i);
            minutes.Add(i-7);
        }
        hours.Add(0);hours.Add(1);hours.Add(2);hours.Add(3);hours.Add(4);hours.Add(5);hours.Add(6);
        for (int i = 24; i < 60; i++)
        {
            minutes.Add(i);
        }
        foreach (int hour in hours)
        {
            GameObject gh = (GameObject)Instantiate(cardItemHoras, contentHoras.transform);
            gh.SetActive(true);
            gh.transform.GetComponentInChildren<Text>().text = hour.ToString();

        }
        foreach (int minute in minutes)
        {
            GameObject gm = (GameObject)Instantiate(cardItemMinutos, contentMinutos.transform);
            gm.SetActive(true);
            gm.transform.GetComponentInChildren<Text>().text = minute.ToString();
        }
    }

    public void SelectDay(int day)
    {
        //restrt color
        for (int i = 0; i < contentDias.transform.childCount; i++)
        {
            contentDias.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
        //set color
        contentDias.transform.GetChild(day).GetComponent<Image>().color = Color.yellow;
        //set Day
        evento.dia = day + 1;
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void OpenTimePicker(Boolean _isInicio)
    {
        isInicio = _isInicio;
        TimePickerPanel.SetActive(!TimePickerPanel.activeSelf);
    }

    public void AceptarTimePicker()
    {
        TimePickerPanel.SetActive(false);

        TimeSpan span = TimeSpan.FromHours(16);

        if (isInicio)
        {
            evento.hora_inicio = eventHourSelected + ":" + eventMinutSelected;
            System.DateTime dateTime = System.DateTime.Parse(evento.hora_inicio);
            evento.hora_inicio = dateTime.ToString("hh:mm tt");

            InicioBtn.GetComponentInChildren<Text>().text = evento.hora_inicio;
        }
        else
        {
            evento.hora_fin = eventHourSelected + ":" + eventMinutSelected;
            System.DateTime dateTime = System.DateTime.Parse(evento.hora_fin);
            evento.hora_fin = dateTime.ToString("hh:mm tt");
            FinBtn.GetComponentInChildren<Text>().text = evento.hora_fin;

        }
    }

    public void selectHour(GameObject hour)
    {
        //restrt color
        for (int i = 0; i < contentHoras.transform.childCount; i++)
        {
            contentHoras.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
        }
        //set color
        hour.GetComponent<Text>().color = Color.blue;
        //set hour
        eventHourSelected = Int32.Parse(hour.transform.GetComponent<Text>().text);
    }

    public void selectMinute(GameObject minut)
    {
        for (int i = 0; i < contentMinutos.transform.childCount; i++)
        {
            contentMinutos.transform.GetChild(i).GetChild(0).GetComponent<Text>().color = Color.black;
        }
        //set color
        minut.GetComponent<Text>().color = Color.blue;
        //set minut
        eventMinutSelected = Int32.Parse(minut.transform.GetComponent<Text>().text);
    }

    public void AceptarBtn()
    {
        evento.nombre_evento = NombreEventoNE.GetComponent<UnityEngine.UI.InputField>().text;
        evento.descripcion = DescripcionNE.GetComponent<UnityEngine.UI.InputField>().text;
        evento.responsable = ResponsableNE.GetComponent<UnityEngine.UI.InputField>().text;

        if (evento.nombre_evento.Trim() != ""
            && evento.lugar_key.Trim() != ""
            && evento.hora_inicio.Trim() != ""
            && evento.hora_fin.Trim() != ""
            && evento.dia != 0
            && evento.hora_inicio != evento.hora_fin)
        {
            evento.version = 1; 
            
            setDataFirebase();
        }
        else
        {
            RetroalimentacionNE.GetComponent<Text>().text = "Faltan campos por llenar";
        }
    }

    private async void setDataFirebase()
    {
        string backPress = PlayerPrefs.GetString("backPress", "");
        if (backPress == "Eventos NuevoEvento" || backPress == "AdminPrincipal NuevoEvento"){    
            await fc.addEvento(evento);
            onBackPressed();
        } else {
            await fc.updateEvento(evento);
            onBackPressed();
        }
    }


    public void CancelarBtn()
    {
        onBackPressed();
    }

    public void onBackPressed()
    {
        string backPress = PlayerPrefs.GetString("backPress", "");
        if (backPress == "Eventos NuevoEvento" || backPress == "Eventos EditarEvento"){
            EventosPanel.SetActive(true);
            AdminRootPanel.SetActive(false);
        }else {
            AdminPrincipalPanel.SetActive(true);
        }
        NuevoEventoPanel.SetActive(false);
    }

}

