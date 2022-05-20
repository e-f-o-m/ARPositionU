using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

/* 
Crear flechas para ir a la siguiente hora o día 
*/
public class ARVuforia : MonoBehaviour
{

    private List<Evento> horarios = new List<Evento>();
    private List<Evento> eventos = new List<Evento>();
    private List<Lugar> lugares = new List<Lugar>();

    private Boolean isLogged = false;
    private string eventKeySelected = null;
    private string placeKeySelected = null;

    private FirebaseController fc;

    private int day = 1;
    void OnEnable()
    {
        //day = (int)DateTime.Now.DayOfWeek;
        iniciarDB();
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        isLogged = await fc.CheckUser();
        eventos = await fc.getEventos();
        horarios = await fc.getMiHorario();
        lugares = await fc.getLugares();
        if (isLogged)
        {
            horarios = await fc.getMiHorario();
        }
        else
        {
            Debug.Log("No hay horarios");
        }
    }

    public void OnClickDay(GameObject button)
    {
        var colors = button.GetComponent<Button>().colors;
        colors.normalColor = Color.red;
        button.GetComponent<Button>().colors = colors;
    }

    public void OnTargetFound(string targetName)
    {
        double timeNow = DateTime.Now.Hour + (DateTime.Now.Minute / 60); 
        Evento evento = getProximityEvent(targetName, timeNow);

        setDataAR(evento);
    }

    public void OnTargetLost(string targetName)
    {
        Debug.Log("targetName lost: " + targetName);
    }

    private void setDataAR(Evento evento)
    {

        if (evento != null)
        {

            string namePlace = lugares.Find(x => x.lugar_key == evento.lugar_key).nombre_lugar;
            Debug.Log("4 xxxxxxx Nombre lugar: " + namePlace);
            GameObject.Find("CanvasHologram/Panel/PanelMain/PanelLugar/Text").GetComponent<Text>().text = namePlace;
            GameObject.Find("CanvasHologram/Panel/PanelMain/PanelResponsble/Text").GetComponent<Text>().text = evento.responsable;

            GameObject.Find("CanvasHologram/Panel/PanelMain/PanelEvento/Text").GetComponent<Text>().text = evento.nombre_evento;
            Debug.Log("5 xxxxxxx Nombre: " + evento.nombre_evento);

            GameObject.Find("CanvasHologram/Panel/PanelClock/PanelStart/Text").GetComponent<Text>().text = evento.hora_inicio;
            GameObject.Find("CanvasHologram/Panel/PanelClock/PanelEnd/Text").GetComponent<Text>().text = evento.hora_fin;
        }
        else
        {
            Debug.Log("No hay eventos");
        }
    }

    public void OnNextEvent()
    {
        otherEvent(1);
    }
    public void OnPreviousEvent()
    {
        otherEvent(-1);
    }

    private void otherEvent(int nextToggle)
    {
        if (eventKeySelected != null)
        {
            Evento newEvent = null;

            Evento mEvent = eventos.Find(x => x.evento_key == eventKeySelected);

            double hoursAdd = srtTimeTo24(mEvent.hora_fin) + nextToggle;

            for (int i = 0; i < 24; i++)
            {
                newEvent = getProximityEvent(mEvent.lugar_key, hoursAdd);
                if (newEvent != null)
                {
                    if (newEvent.evento_key != mEvent.evento_key)
                    {
                        eventKeySelected = newEvent.evento_key;
                        day = newEvent.dia;
                        setDataAR(newEvent);
                        break;
                    }
                }
                hoursAdd += nextToggle * 2 ;
            }
        }
        else
        {
            Debug.Log("Siguiente evento no encontrado");
        }
    }

    private Evento getProximityEvent(string targetName, double timeNow)
    {
        int previousDay = 0;
        double proximityStarHour = 1000.0;
        Evento evento = null;

        foreach (Evento _evento in eventos)
        {
            //validate teget place and event place
            if (_evento.lugar_key == targetName)
            {
                double hourEventFull = srtTimeTo24(_evento.hora_fin);

                //break if add a day and exist event selected
                if (previousDay < _evento.dia && proximityStarHour < 500)
                {
                    break;
                }

                previousDay = _evento.dia;

                int daysSpace = _evento.dia - day;

                /* Debug.Log("11 uuuuuuuu daysSpace: " + daysSpace);
                Debug.Log("22 uuuuuuuu day: " + day);
                Debug.Log("32 uuuuuuuu evento.dia: " + _evento.dia); */

                if (daysSpace < 0)
                {
                    hourEventFull += 24 * (7 + Math.Abs(daysSpace) - day);
                }else if (daysSpace > 0){
                    hourEventFull += 24 * daysSpace;    
                }


                if (timeNow <= hourEventFull)
                {
                    if (Math.Abs(hourEventFull - timeNow) <= proximityStarHour)
                    {
                        proximityStarHour = Math.Abs(hourEventFull - timeNow);

                        evento = _evento;
                        
                        //save event key (for nexr of previous btn)
                        eventKeySelected = evento.evento_key;
                        
                        Debug.Log("ppppppro prox: " + proximityStarHour);
                        Debug.Log("nnnnname name :  " + evento.nombre_evento);

                        if (proximityStarHour == 0)
                        {
                            break;
                        }
                    }


                }
            }
        }
        return evento;
    }

    private double srtTimeTo24(string time)
    {
        int hour = 12;
        int minute = 0;
        if (time.Split(':')[1].Split(' ')[1].Equals("p."))
        {
            //is PM
            hour += int.Parse(time.Split(':')[0]);
        }
        else
        {
            hour = int.Parse(time.Split(':')[0]);
        }
        minute = int.Parse(time.Split(':')[1].Split(' ')[0]);
        return hour + (minute / 60.0);
    }
}