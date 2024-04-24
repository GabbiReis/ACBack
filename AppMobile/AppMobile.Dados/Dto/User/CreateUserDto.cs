namespace AppMobile.AppMobile.Dados.Dto.User
{
    public class CreateUserDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public IFormFile Foto { get; set; }
    }
}

