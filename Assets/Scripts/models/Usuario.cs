
public class Usuario 
{

    public string usuario_key;
    public string nombre;
    public string correo;
    public string codigo;
    public int rol;

    public Usuario(){}
    
    public Usuario(string usuario_key, string nombre, string correo, string codigo, int rol)
    {
        this.usuario_key = usuario_key;
        this.nombre = nombre;
        this.correo = correo;
        this.codigo = codigo;
        this.rol = rol;
    }
}