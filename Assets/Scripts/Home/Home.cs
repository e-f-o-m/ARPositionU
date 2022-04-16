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
    public GameObject EventosPanel;
    public GameObject AdminPanel;
    public GameObject cardItem;
    public GameObject contentHorario;
    [Header ("Views Home")]
    public GameObject AceptarBtnH;

    [Header ("Dialog Select")]
    public GameObject DialogoValiHo;
    public GameObject DescripcionTvDiHo;

    private List<GameObject> horarios = new List<GameObject>();

    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();

    private string eventKeySelected = null;

    private FirebaseController fc;
    void Start()
    {
        iniciarDB();
    }

    
    private async void iniciarDB()
    {
        fc = new FirebaseController();
        await fc.CheckUser();
        eventos = await fc.getMiHorario();
        lugares = await fc.getLugares();
        generateGaObEventos();
    }

    private void generateGaObEventos()
    {
        for (int i = 0; i < eventos.Count; i++)
        {
            var _evento = eventos[i];
            GameObject g = (GameObject)Instantiate(cardItem, contentHorario.transform);
            g.SetActive(true);
            
            g.transform.Find("marginPanel/id").GetComponent<Text>().text = ""+_evento.evento_key;

            string _lugar = lugares.Find(x => x.lugar_key == _evento.lugar_key).nombre_lugar;
            g.transform.Find("marginPanel/Salon").GetComponent<Text>().text = _lugar;
            
            g.transform.Find("marginPanel/Evento").GetComponent<Text>().text = _evento.nombre_evento;

            Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            string [] days  = {"Lu", "Ma", "Mi", "Ju", "Vi", "Sa", "Do"};
            string day = days[_evento.dia];
            g.transform.Find("marginPanel/"+day).GetComponent<Text>().color = selectedColor;
            horarios.Add(g);
        }          
    }

    public void selectItemHorario(GameObject id_event){
        OpenPanelDialog(id_event.GetComponent<Text>().text);
    }

    public void OpenPanelDialog(string id_event){
        eventKeySelected = id_event;
        Boolean isOpen = DialogoValiHo.activeSelf;
        DialogoValiHo.SetActive(!isOpen);
        //DescripcionTvDiHo
    }

    public void OpenPanel(GameObject panel)
    {
        //TODO: agregar estos dos
        //panelAgregar.SetActive(false);
        //panelListar.SetActive(false);
        HomePanel.SetActive(false);
        EventosPanel.SetActive(false);
        AdminPanel.SetActive(false);
        panel.SetActive(true);
    }


    public void aceptarDialog(){
        DialogoValiHo.SetActive(false);
        MiHorario miHorario = new MiHorario();
        miHorario.evento_key = eventKeySelected;
        //TODO: DELETE: fc.deleteEvento(eventKeySelected);
    }
}
