using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;

public class ControllerAmb : MonoBehaviour
{
    DatabaseReference reference;
    private void iniciarDB()
    {
        FirebaseDatabase db = FirebaseDatabase.GetInstance("https://ar-position-u-default-rtdb.firebaseio.com/");
        reference = db.RootReference;
    }

    private string name_collection = "ambiente";

    [Header("Agregar")]
    public InputField salon_id;
    public InputField facultad;
    public InputField nombre;
    public Text res;

    [Header("View")]
    public GameObject panelAgregar;
    public GameObject panelListar;

    // Start is called before the first frame update
    void Start()
    {
        all();
    }

    // Update is called once per frame
    void Update() { }

    public void guardarAmbiente()
    {
        string ambSalonid = salon_id.text.Trim();
        string ambFacultad = facultad.text.Trim();
        string ambNombre = nombre.text.Trim();
        add(ambSalonid, ambFacultad, ambNombre);
        clearAmb();
    }

    private void clearAmb()
    {
        salon_id.text = "";
        facultad.text = "";
        nombre.text = "";
    }
    
    public void openPanel(GameObject panel){
        panelAgregar.SetActive(false);
        panelListar.SetActive(false);
        panel.SetActive(true);
    }

    //Firebase Realtime
    private async void add(string salon_id, string facultad, string nombre)
    {
        Ambiente ambiente = new Ambiente(salon_id, facultad, nombre);
        string json = JsonUtility.ToJson(ambiente);
        iniciarDB();
        string ID = reference.Child(name_collection).Push().Key;

        string res = await reference.Child(name_collection).Child(ID).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                return "Cancelado";
            }
            if (task.IsFaulted)
            {
                return "Error: " + task.Exception;
            }
            return "Ambiente Creado";
        });
    }
    
    private async void all()
    {
        string msm="";
        iniciarDB();
        await reference.Child(name_collection).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de "+name_collection);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Ambiente ambiente = JsonUtility.FromJson<Ambiente>(item.GetRawJsonValue());
                    Debug.Log("Llave "+item.Key);                    
                    msm +="\n Ambiente "+ambiente.facultad+" Nombre: "
                    +ambiente.nombre+" salon "+ambiente.salon_id;                    
                }
            }
        });
        res.text=msm;
    }   
}
