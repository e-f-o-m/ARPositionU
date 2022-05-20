using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lugar
{
    public string lugar_key;
    public string nombre_lugar;

    public Lugar(){}

    public Lugar(string lugar_key,string nombre_lugar){
       this.lugar_key = lugar_key;
       this.nombre_lugar = nombre_lugar;
    }
}
