using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evento
{
    public string evento_key;
    public string lugar_key;
    public string nombre_evento;
    public string responsable;
    public int dia;
    public string hora_inicio;
    public string hora_fin;
    public string descripcion;
    public int version;
    
    public Evento(){}

    public Evento (string evento_key, string lugar_key, string nombre_evento, string responsable, int dia, string hora_inicio, string hora_fin, string descripcion, int version)
    {
        this.evento_key = evento_key;
        this.lugar_key = lugar_key;
        this.nombre_evento = nombre_evento;
        this.responsable = responsable;
        this.dia = dia;
        this.hora_inicio = hora_inicio;
        this.hora_fin = hora_fin;
        this.descripcion = descripcion;
        this.version = version;
    }
}
