using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lugar
{
    public string lugar_key;
    public string nombre_lugar;
    public string facultad;

    public Lugar(){}

    public Lugar(string lugar_key,string nombre_lugar, string facultad){
       this.lugar_key = lugar_key;
       this.nombre_lugar = nombre_lugar;
       this.facultad = facultad;
    }
}
