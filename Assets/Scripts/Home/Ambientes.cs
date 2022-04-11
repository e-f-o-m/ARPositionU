using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class Ambientes : MonoBehaviour
{
    public GameObject cardItem;
    public GameObject contentAmbientes;
    public GameObject HomePanel;
    public GameObject AmbientesPanel;
    public GameObject AdminPanel;
    
    GameObject AceptarBtnH;
    private List<GameObject> horarios = new List<GameObject>();

    private int count = 0;
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            horarios.Add(generateAmbientes());
        }
    }

    public GameObject generateAmbientes()
    {
        count++;
        GameObject g = (GameObject)Instantiate(cardItem, contentAmbientes.transform);
        g.SetActive(true);
        //TODO: change id, firebase database
        g.transform.Find("marginPanel/id").GetComponent<Text>().text = ""+count;
        return g;                   
    }

    public void setData(){
        horarios.Add(generateAmbientes());
    }

    public void selectItemAmbientes(GameObject id){
        Debug.Log("Selected item: " + id.GetComponent<Text>().text);
    }

    public void OpenPanel(GameObject panel)
    {
        HomePanel.SetActive(false);
        AmbientesPanel.SetActive(false);
        AdminPanel.SetActive(false);
        panel.SetActive(true);
    }


}
