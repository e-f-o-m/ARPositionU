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

    [Header("Panel")]
    public GameObject AdminPanel;
    public GameObject NuevoEventosPanel;
    public GameObject TimePickerPanel;
    
    [Header ("Form")]
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
    private Boolean isAm = true;
    private Boolean isInicio = true;


    private int eventHourSelected = 0;
    private int eventMinutSelected = 0;
    private FirebaseController fc;
    private Evento evento = new Evento();

    void Start()
    {
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
        eventos = await fc.getEventos();
        lugares = await fc.getLugares();

        DropdownLugarNE.ClearOptions();


        for (int i = 0; i < lugares.Count; i++)
        {
            DropdownLugarNE.AddOptions(new List<string>() { lugares[i].nombre_lugar });
        }
        
        DropdownLugarNE.onValueChanged.AddListener(OnFirstDropdownValueChange);
    }

    public void generateTimePicker()
    {
        //TimePicker.SetActive(false);
        for (int i = 0; i < 24; i++)
        {
            hours.Add(i);
            minutes.Add(i);
        }
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
        evento.dia = day+1;
    }

    public void OpenPanel(GameObject panel)
    {
        NuevoEventosPanel.SetActive(false);
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
        string stAm = " a. m.";
        if(!isAm)
        {
            stAm = " p. m.";
        }
        
        TimeSpan span = TimeSpan.FromHours(16);

        if (isInicio)
        {
            evento.hora_inicio = eventHourSelected + ":" + eventMinutSelected + stAm;
            System.DateTime dateTime = System.DateTime.Parse(evento.hora_inicio);
            evento.hora_inicio = dateTime.ToString("hh:mm tt");
            
            InicioBtn.GetComponentInChildren<Text>().text = evento.hora_inicio;
        }
        else
        {
            evento.hora_fin = eventHourSelected + ":" + eventMinutSelected + stAm;
            System.DateTime dateTime = System.DateTime.Parse(evento.hora_fin);
            evento.hora_fin = dateTime.ToString("hh:mm tt");
            FinBtn.GetComponentInChildren<Text>().text = evento.hora_fin;

        }
    }

    public void SetIsAm(Boolean isAm)
    {
        this.isAm = isAm;
        if (isAm)
        {
            amBtn.GetComponent<Image>().color = Color.blue;
            pmBtn.GetComponent<Image>().color = Color.white;
        }
        else
        {
            amBtn.GetComponent<Image>().color = Color.white;
            pmBtn.GetComponent<Image>().color = Color.blue;
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

    public void AceptarBtn(){
        evento.nombre_evento = NombreEventoNE.GetComponent<UnityEngine.UI.InputField>().text;
        evento.descripcion = DescripcionNE.GetComponent<UnityEngine.UI.InputField>().text;
        if(evento.nombre_evento.Trim() != "" ){
            Debug.Log("nombre_e");
        }
        if(evento.lugar_key.Trim() != "" ){
            Debug.Log("lugar_ke");
        }
        if(evento.hora_inicio.Trim() != "" ){
            Debug.Log("hora_ini");
        }
        if(evento.hora_fin.Trim() != "" ){
            Debug.Log("hora_fin");
        }
        if(evento.dia != 0 ){
            Debug.Log("dia != 0");
        }
        if(evento.hora_inicio != evento.hora_fin){
            Debug.Log("hora_ini");
        }
        

        if (evento.nombre_evento.Trim() != "" 
            && evento.lugar_key.Trim() != "" 
        && evento.hora_inicio.Trim() != "" 
        && evento.hora_fin.Trim() != "" 
        && evento.dia != 0 
        && evento.hora_inicio != evento.hora_fin)
        {
            //print all evento 
            print("evento key: " + evento.evento_key + " nombre evento: " + evento.nombre_evento + " descripcion: " + evento.descripcion + " lugar key: " + evento.lugar_key + " hora inicio: " + evento.hora_inicio + " hora fin: " + evento.hora_fin + " dia: " + evento.dia);

            //fc.addEvento(miHorario);
        }
        else
        {
            RetroalimentacionNE.GetComponent<Text>().text = "Faltan campos por llenar";
        }
    }

}

