using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseController {
    string LUGARES_REF = "lugares";
    string EVENTOS_REF = "eventos";
    string HORARIO_REF = "horario";
    string USUARIOS_REF = "usuarios";
    string TOKEN_USER = "";
    private Firebase.Auth.FirebaseUser user = null;
    private Firebase.Auth.FirebaseAuth auth = null;
    private string eventKeySelected = null;
    private DatabaseReference reference;


    public FirebaseController() {
        FirebaseDatabase db = FirebaseDatabase.GetInstance("https://ar-position-u-default-rtdb.firebaseio.com/");
        //Unity persistence firebase
        db.SetPersistenceEnabled(false);
        reference = db.RootReference;
        reference.ValueChanged += HandleValueChanged;   
    }


    public async Task CheckUser()
    {
        await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                try
                {
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                        if (auth.CurrentUser != null)
                        {
                            user = auth.CurrentUser;
                            TOKEN_USER = user.UserId;
                            Debug.Log("User logged in");
                        }
                        else
                        {
                            Debug.Log("User not logged in");
                        }
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
                return;

            });
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log(args.Snapshot);
    }



    public async Task<string> addEvento(MiHorario miHorario)
    {
        var newRef = reference.Child(USUARIOS_REF).Child(TOKEN_USER).Child(HORARIO_REF);
        string ID = newRef.Push().Key;
        miHorario.horario_key = ID;
        string json = JsonUtility.ToJson(miHorario);
        return await newRef.Child(ID).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCanceled)
            { 
                Debug.LogError("SetRawJsonValueAsync was canceled.");
                return null;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SetRawJsonValueAsync encountered an error: " + task.Exception);
                return null;
            }
            return ID;
        });
    }

    public async Task<List<Evento>> getEventos()
    {
        List<Evento> eventos = new List<Evento>();

        await reference.Child(EVENTOS_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de evento eventos: "+EVENTOS_REF);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Evento evento = JsonUtility.FromJson<Evento>(item.GetRawJsonValue());
                    eventos.Add(evento);
                }
            }
        });
        return eventos;
    }


    public async Task<List<Evento>> getMiHorario()
    {
        Debug.Log("xxxxx Home a1");
        List<MiHorario> miHorarios = new List<MiHorario>();
        List<Evento> eventos = new List<Evento>();

        Debug.Log("xxxxx ===== " + USUARIOS_REF + "/" + TOKEN_USER + "/" + HORARIO_REF);
        await reference.Child(USUARIOS_REF).Child(TOKEN_USER).Child(HORARIO_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de evento eventos: Mi horario");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Debug.Log("xxxxx Home a2=== "+item.GetRawJsonValue());
                    MiHorario miHorario = JsonUtility.FromJson<MiHorario>(item.GetRawJsonValue());
                    miHorarios.Add(miHorario);
                }
            }
        });

        Debug.Log("xxxxx Home a3");

        foreach (MiHorario miHorario in miHorarios)
        {
            await reference.Child(EVENTOS_REF).Child(miHorario.evento_key).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error al recuperar datos de evento eventos: Mi horario");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    Evento evento = JsonUtility.FromJson<Evento>(snapshot.GetRawJsonValue());
                    eventos.Add(evento);
                }
                
            });
        }
        Debug.Log("xxxxx Home a4");
        return eventos;
    }

    
    public async Task<List<Lugar>> getLugares()
    {
        List<Lugar> lugares = new List<Lugar>();

        await reference.Child(LUGARES_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de lugar"+LUGARES_REF);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Lugar lugar = JsonUtility.FromJson<Lugar>(item.GetRawJsonValue());
                    lugares.Add(lugar);
                }
            }
        });
        return lugares;
    }
}