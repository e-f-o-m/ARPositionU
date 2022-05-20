using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseController
{
    string LUGARES_REF = "lugares";
    string EVENTOS_REF = "eventos";
    string HORARIO_REF = "horario";
    string USUARIOS_REF = "usuarios";
    string TOKEN_USER = "";
    private Firebase.Auth.FirebaseUser user = null;
    private Firebase.Auth.FirebaseAuth auth = null;
    private string eventKeySelected = null;
    private DatabaseReference reference;


    public FirebaseController()
    {
        FirebaseDatabase db = FirebaseDatabase.GetInstance("https://ar-position-u-default-rtdb.firebaseio.com/");
        //Unity persistence firebase
        db.SetPersistenceEnabled(false);
        reference = db.RootReference;
        reference.ValueChanged += HandleValueChanged;
    }

    public async Task<Boolean> CheckUser()
    {
        return await Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
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
                            Boolean isVerifiedEmail = user.IsEmailVerified;
                            if (isVerifiedEmail)
                            {
                                Debug.Log("User is verified");
                                return true;
                            }else{
                                Debug.Log("User is not verified");
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (System.Exception)
                {
                    Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                }
                return false;

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

    public async Task<string> addHorario(MiHorario miHorario)
    {
        var newRef = reference.Child(USUARIOS_REF).Child(TOKEN_USER).Child(HORARIO_REF);
        string ID = newRef.Push().Key;
        miHorario.horario_key = ID;
        string json = JsonUtility.ToJson(miHorario);
        Debug.Log(json);
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
                Debug.Log("Error al recuperar datos de evento eventos: " + EVENTOS_REF);
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

    public async Task<Evento> getEvento(string evento_key)
    {
        Evento evento = new Evento();
        await reference.Child(EVENTOS_REF).Child(evento_key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de evento eventos: " + EVENTOS_REF);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                evento = JsonUtility.FromJson<Evento>(snapshot.GetRawJsonValue());
            }
        });
        return evento;
    }

    public async Task<string> addEvento(Evento _evento)
    {
        var newRef = reference.Child(EVENTOS_REF);
        string ID = newRef.Push().Key;
        _evento.evento_key = ID;
        string json = JsonUtility.ToJson(_evento);
        Debug.Log(json);
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

    public async Task<String> updateEvento(Evento _evento)
    {
        string json = JsonUtility.ToJson(_evento);
        Debug.Log(json);
        return await reference.Child(EVENTOS_REF).Child(_evento.evento_key).SetRawJsonValueAsync(json).ContinueWith(task =>
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
            return _evento.evento_key;
        });
    }


    public async Task<List<Evento>> getMiHorario()
    {
        List<MiHorario> miHorarios = new List<MiHorario>();
        List<Evento> eventos = new List<Evento>();
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
                    MiHorario miHorario = JsonUtility.FromJson<MiHorario>(item.GetRawJsonValue());
                    miHorarios.Add(miHorario);
                }
            }
        });


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
                    evento.evento_key = miHorario.horario_key;
                    eventos.Add(evento);
                }

            });
        }
        return eventos;
    }

    
    public async Task<List<Evento>> getMiHorarioEvent()
    {
        List<MiHorario> miHorarios = new List<MiHorario>();
        List<Evento> eventos = new List<Evento>();
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
                    MiHorario miHorario = JsonUtility.FromJson<MiHorario>(item.GetRawJsonValue());
                    miHorarios.Add(miHorario);
                }
            }
        });


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
        return eventos;
    }



    public async Task<List<Lugar>> getLugares()
    {
        List<Lugar> lugares = new List<Lugar>();

        await reference.Child(LUGARES_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de lugar" + LUGARES_REF);
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

    public async Task<string> deleteHorario(string horario_key)
    {
        var newRef = reference.Child(USUARIOS_REF).Child(TOKEN_USER).Child(HORARIO_REF).Child(horario_key);
        return await newRef.RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("RemoveValueAsync was canceled.");
                return null;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("RemoveValueAsync encountered an error: " + task.Exception);
                return null;
            }
            return horario_key;
        });
    }



    // ------------------------- LOGIN -------------------------
    public void logout()
    {
        Debug.Log("logout");
        auth.SignOut();
        user = null;
    }

    public async Task<string> resetPassword(string emailAddress)
    {

        return await auth.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return null;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " +
                        task.Exception);
                    return null;
                }

                return "Password reset email sent successfully.";

            });
    }


    public async Task<Boolean> SingIn(string email, string password)
    {
        return await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return false;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return false;
            }
            user = task.Result;
            Boolean isVerifiedEmail = user.IsEmailVerified;
            if (isVerifiedEmail)
            {
                Debug.Log("Email is verified");
                return true;
            }
            else
            {
                Debug.Log("Email is not verified");
                return false;
            }
        });
    }


    public async Task<string> registerNewUser(string email, string password, string stNombreEtRe, string stCodigoEtRe)
    {
        logout();
        string res = await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return null;
                }
                if (task.IsFaulted)
                {
                    return null;
                }
                user = task.Result;
                return "Nuevo Usuario";
            });

        res = await user.SendEmailVerificationAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    return null;
                }
                if (task.IsFaulted)
                {
                    return null;
                }
                return res + "\nCorreo valido.\nVerifique la cuenta en su correo.";
            });

        if (res != null)
        {
            Usuario usuario = new Usuario();
            usuario.correo = email;
            usuario.nombre = stNombreEtRe;
            usuario.codigo = stCodigoEtRe;
            usuario.rol = 0;
            usuario.usuario_key = user.UserId;

            Debug.Log("usuario_key: " + usuario.usuario_key);
            res = await reference.Child(USUARIOS_REF).Child(user.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(usuario)).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SetRawJsonValueAsync was canceled.");
                    return null;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SetRawJsonValueAsync Registrarse: " + task.Exception);
                    return null;
                }
                return res + "\n⏳";
            });
        }
        if (res == null)
        {
            logout();
        }
        return res;

    }

    public async Task<Boolean> deleteUser(){
        await reference.Child(USUARIOS_REF).Child(user.UserId).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("RemoveValueAsync was canceled.");
            }
            if (task.IsFaulted)
            {
                Debug.LogError("RemoveValueAsync encountered an error: " + task.Exception);
            }
        });


        return await user.DeleteAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("DeleteAsync was canceled.");
                return false;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                return false;
            }
            return true;
        });
    }


    // ------------------------- Users -------------------------
    public async Task<List<Usuario>> getUsuarios()
    {
        List<Usuario> usuarios = new List<Usuario>();
        await reference.Child(USUARIOS_REF).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de usuarios");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (var item in snapshot.Children)
                {
                    Usuario usuario = JsonUtility.FromJson<Usuario>(item.GetRawJsonValue());
                    usuarios.Add(usuario);
                }
            }
        });
        return usuarios;
    }

    public async Task<Usuario> getUserData()
    {
        Usuario usuario = new Usuario();
        await reference.Child(USUARIOS_REF).Child(TOKEN_USER).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Error al recuperar datos de usuario");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                usuario = JsonUtility.FromJson<Usuario>(snapshot.GetRawJsonValue());
            }
        });
        return usuario;
    }

    public async Task<Boolean> updateUsuario(Usuario usuario)
    {
        await reference.Child(USUARIOS_REF).Child(usuario.usuario_key).SetRawJsonValueAsync(JsonUtility.ToJson(usuario)).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SetRawJsonValueAsync was canceled.");
                return false;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SetRawJsonValueAsync encountered an error: " + task.Exception);
                return false;
            }
            return true;
        });
        return true;
    }


}