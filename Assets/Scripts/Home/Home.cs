using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
    public ScrollRect scrollRect;
    [Header ("Views Home")]
    public GameObject AceptarBtnH;

    [Header ("Dialog Select")]
    public GameObject DialogoValiHo;
    public GameObject DescripcionTvDiHo;

    private List<GameObject> horarios = new List<GameObject>();

    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();

    private Boolean isLogged = false;
    private string eventKeySelected = null;


    private FirebaseController fc;
    void Start()
    {
    }
    void OnEnable()
    {
        horarios = horarios = new List<GameObject>();
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
        isLogged = await fc.CheckUser();
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

            Color selectedColor = new Color(79/255.0f, 140/255.0f, 238/255.0f);
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
        deleteHorario(eventKeySelected);
    }

    private async Task deleteHorario(string key_horario){
        string res = await fc.deleteHorario(key_horario);
        if(res != null){
            Debug.Log("Evento eliminado res :"+res);
            for(int i=0; i<horarios.Count; i++){
                if(horarios[i].transform.Find("marginPanel/id").GetComponent<Text>().text == key_horario){
                    Destroy(horarios[i].gameObject);
                    horarios.RemoveAt(i);
                    break;
                }
            }

        }
    }
}
