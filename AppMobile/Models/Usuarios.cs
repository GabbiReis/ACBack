namespace AppMobile.Models
{
    public class Usuarios
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public byte[] Foto { get; set; }
    }
}
