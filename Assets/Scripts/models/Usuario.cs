
public class Usuario 
{

    private string usuario_key;
    private string nombre;
    private string token;
    private string correo;
    private string codigo;
    private string rol;

    public Usuario(){}
    
    public Usuario(string usuario_key, string nombre, string token, string correo, string codigo, string rol)
    {
        this.usuario_key = usuario_key;
        this.nombre = nombre;
        this.correo = correo;
        this.codigo = codigo;
        this.rol = rol;
    }
}