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

    private FirebaseController fc;
    private string targetName;

    private int day = 0;
    void OnEnable()
    {
        day = (int)DateTime.Now.DayOfWeek;
        iniciarDB();
    }

    private async void iniciarDB()
    {
        fc = new FirebaseController();
        isLogged = await fc.CheckUser();
        eventos = await fc.getEventos();
        lugares = await fc.getLugares();
        if (isLogged)
        {
            horarios = await fc.getMiHorarioEvent();
        }
        else
        {
            Debug.Log("No hay horario");
        }
    }

    public void OnClickDay(GameObject button)
    {
        var colors = button.GetComponent<Button>().colors;
        colors.normalColor = Color.red;
        button.GetComponent<Button>().colors = colors;
    }

    public void OnTargetFound(string _targetName)
    {
        targetName = _targetName;
        Debug.Log("Hour: " +DateTime.Now.Hour + ":" + DateTime.Now.Minute);
        double timeNow = (DateTime.Now.Hour + (double)(DateTime.Now.Minute / 60.0));
        Debug.Log("Target Found: " + timeNow);
        Evento evento = getProximityEvent(timeNow);
        setDataAR(evento);
    }

    public void OnTargetLost(string targetName)
    {
        /* Debug.Log("targetName lost: " + targetName); */
    }

    private void setDataAR(Evento evento)
    {
        string[] days = { "BtnLunes", "BtnMartes", "BtnMiercoles", "BtnJueves", "BtnViernes", "BtnSabado", "BtnDomingo" };

        if (evento != null)
        {

            string namePlace = lugares.Find(x => x.lugar_key == evento.lugar_key).nombre_lugar;

            if (horarios.Count > 0)
            {
                Evento mEventoHorario = horarios.Find(x => x.evento_key == evento.evento_key);
                if (mEventoHorario != null)
                {
                    GameObject.Find(targetName + "/CanvasHologram/Panel/PanelMain/PanelMensaje/Text").GetComponent<Text>().text = "Mi Horario";
                }
                else
                {
                    GameObject.Find(targetName + "/CanvasHologram/Panel/PanelMain/PanelMensaje/Text").GetComponent<Text>().text = "- -";
                }
            }

            GameObject.Find(targetName + "/CanvasHologram/Panel/PanelMain/PanelLugar/Text").GetComponent<Text>().text = namePlace;
            GameObject.Find(targetName + "/CanvasHologram/Panel/PanelMain/PanelEvento/Text").GetComponent<Text>().text = evento.nombre_evento;
            GameObject.Find(targetName + "/CanvasHologram/Panel/PanelMain/PanelResponsble/Text").GetComponent<Text>().text = evento.responsable;

            GameObject.Find(targetName + "/CanvasHologram/Panel/PanelClock/PanelStart/Text").GetComponent<Text>().text = evento.hora_inicio;
            GameObject.Find(targetName + "/CanvasHologram/Panel/PanelClock/PanelEnd/Text").GetComponent<Text>().text = evento.hora_fin;

            foreach (string _day in days)
            {
                //restaurar los colores
                var _colors = GameObject.Find(targetName + "/CanvasHologram/Panel/PanelCalendar/" + _day).GetComponent<Button>().colors;
                _colors.normalColor = Color.white;
                GameObject.Find(targetName + "/CanvasHologram/Panel/PanelCalendar/" + _day).GetComponent<Button>().colors = _colors;

            }

            GameObject objectDay = GameObject.Find(targetName + "/CanvasHologram/Panel/PanelCalendar/" + days[evento.dia - 1]);
            var colors = objectDay.GetComponent<Button>().colors;
            colors.normalColor = new Color(0.709f, 0.425f, 0.816f, 0.816f);
            objectDay.GetComponent<Button>().colors = colors;

        }
        else
        {
            Debug.Log("Siguiente o start: No hay eventos");
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

            double hoursAdd = 0;

            // Event next or previous, add or subtract hour
            hoursAdd = (nextToggle == 1) ? srtTimeTo24(mEvent.hora_fin) + nextToggle : srtTimeTo24(mEvent.hora_inicio) + nextToggle;

            newEvent = getProximityEvent(hoursAdd);
            if (newEvent != null && newEvent.evento_key != mEvent.evento_key)
            {
                Debug.Log(" + ++ 8 other encontrao evento");
                eventKeySelected = newEvent.evento_key;
                setDataAR(newEvent);
            }
            else
            {
                Debug.Log("+ + + 9 other No hay eventos");
            }
        }
        else
        {
            Debug.Log("Error other");
        }
    }

    private Evento getProximityEvent(double timeNow)
    {
        int previousDay = 0;
        double proximityEvent = 1000.0;
        double proximityTemp = 0;

        Evento evento = null;
        //filter events by target
        List<Evento> eventosTarget = eventos.FindAll(x => x.lugar_key == targetName);

        foreach (Evento _evento in eventosTarget)
        {
            double hourEventFull = srtTimeTo24(_evento.hora_fin);
            int daysSpace = _evento.dia - day;
            if (daysSpace < 0)
            {
                proximityTemp = hourEventFull + (24 * (7 - day + _evento.dia)); // daysSpace resta
                
            }
            else if (daysSpace > 0)
            {
                proximityTemp = (24 * daysSpace) + hourEventFull;
            }else
            {
                proximityTemp = hourEventFull;
            }

            Debug.Log("hora now : "+ timeNow+ "\nhora evento: "+ hourEventFull+ " \ntemp: " + proximityTemp + " " + "\nprox: " + proximityEvent+ "\nevent: "+ _evento.nombre_evento);
            if (proximityTemp > timeNow && proximityTemp < proximityEvent)
            {
                proximityEvent = 0 + proximityTemp;
                evento = _evento;
                
                //save event key (for nexr of previous btn)
                eventKeySelected = evento.evento_key;
                if (proximityEvent == 0)
                {
                    break;
                }
            }
        }

        if (evento != null)
        {
            day = evento.dia;
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