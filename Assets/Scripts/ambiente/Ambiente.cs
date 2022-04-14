using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambiente
{
    public string salon_id;
    public string facultad;
    public string nombre;

    public Ambiente(){}

    public Ambiente(string salon_id,string facultad,string nombre){
       this.salon_id=salon_id;
       this.facultad=facultad;
       this.nombre=nombre;
    }
}
