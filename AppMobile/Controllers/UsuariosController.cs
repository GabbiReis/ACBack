using AppMobile.Models;
using AppMobile.AppMobile.Dados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppMobile.AppMobile.Dados.Dto.User;
using System.Net.Mail;

namespace AppMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppMobileContext _contexto;
        private readonly ISendGridClient _sendGridClient;
        private readonly string _sendGridApiKey;
        private readonly JwtService _jwtService;

        public UsuariosController(AppMobileContext contexto, IConfiguration configuration, JwtService jwtService)
        {
            _contexto = contexto;
            _sendGridApiKey = configuration.GetValue<string>("SendGrid:ApiKey");
            _sendGridClient = new SendGridClient(_sendGridApiKey);
            _jwtService = jwtService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _contexto.Usuario.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            var usuario = await _contexto.Usuario.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }
        [HttpPost("MigratePasswords")]
        public async Task<IActionResult> MigratePasswordsToBCrypt()
        {
            var usuarios = await _contexto.Usuario.ToListAsync();
            foreach (var usuario in usuarios)
            {
                if (!BCrypt.Net.BCrypt.Verify(usuario.Senha, usuario.Senha)) // Verifique se a senha já não está em BCrypt
                {
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                }
            }
            await _contexto.SaveChangesAsync();
            return Ok("Senhas migradas para BCrypt com sucesso.");
        }

        [HttpPost("Cadastrar")]
        public async Task<ActionResult<Usuarios>> CadastrarUsuario([FromForm] CreateUserDto createUserDto)
        {
            try
            {
                var usuario = new Usuarios
                {
                    Nome = createUserDto.Nome,
                    Email = createUserDto.Email,
                    Senha = BCrypt.Net.BCrypt.HashPassword(createUserDto.Senha) // Hash da senha
                };
                // Gere o token de recuperação de senha
                string tokenRecuperacaoSenha = Guid.NewGuid().ToString();
                DateTime tokenExpiracao = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora

                // Atribua o token ao novo usuário
                usuario.TokenRecuperacaoSenha = tokenRecuperacaoSenha;
                usuario.TokenRecuperacaoSenhaExpiracao = tokenExpiracao;

                if (createUserDto.Foto != null && createUserDto.Foto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await createUserDto.Foto.CopyToAsync(memoryStream);
                        var fotoBytes = memoryStream.ToArray();

                        // Certifique-se de que os bytes da foto são válidos antes de atribuí-los ao usuário
                        if (fotoBytes != null && fotoBytes.Length > 0)
                        {
                            usuario.Foto = fotoBytes;
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    _contexto.Usuario.Add(usuario);
                    await _contexto.SaveChangesAsync();

                    // Retorne o usuário cadastrado sem gerar o token JWT
                    return CreatedAtAction(nameof(GetUsuario), new { id = usuario.ID }, usuario); // Retorna 201 Created
                }
                else
                {
                    return BadRequest(ModelState);
                }

            }
            catch (Exception ex)
            {
                // Log da exceção interna
                Console.WriteLine($"Exceção interna ao cadastrar usuário: {ex.InnerException?.Message}");

                // Retorne um erro interno do servidor genérico para o cliente
                return StatusCode(500, "Erro interno do servidor ao cadastrar usuário.");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Usuarios>> Login([FromForm] string email, [FromForm] string senha)
        {
            var usuario = await _contexto.Usuario
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                return Unauthorized("Email ou senha incorretos.");
            }

            // Gere o token JWT com base no usuário
            string tokenJwt = _jwtService.GenerateJwtToken(usuario);

            // Retorne o token JWT junto com os dados do usuário
            return Ok(new
            {
                usuario.ID,
                usuario.Nome,
                usuario.Email,
                usuario.Foto,
                TokenJwt = tokenJwt
            });
        }

        [HttpPost("EsqueciSenha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("O email é obrigatório.");
            }

            var usuario = await _contexto.Usuario.SingleOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            // Gere o token de recuperação de senha
            string tokenRecuperacaoSenha = Guid.NewGuid().ToString();
            DateTime tokenExpiracao = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora

            // Armazene o token e a data de expiração no banco de dados
            usuario.TokenRecuperacaoSenha = tokenRecuperacaoSenha;
            usuario.TokenRecuperacaoSenhaExpiracao = tokenExpiracao;

            _contexto.Usuario.Update(usuario);
            await _contexto.SaveChangesAsync();

            // Envie o email com o token de recuperação de senha
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admchocomigos@gmail.com", "ChocoMigos"),
                Subject = "Recuperação de Senha",
                PlainTextContent = $"Seu token de recuperação de senha é: {tokenRecuperacaoSenha} volte para a página de 'Esqueci senha' e redefina sua nova senha utilizando o token",
                HtmlContent = $"<strong>Seu token de recuperação de senha é: {tokenRecuperacaoSenha}</strong>"
            };
            msg.AddTo(new EmailAddress(usuario.Email));

            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return Ok("Email de recuperação enviado com sucesso.");
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Falha ao enviar email de recuperação.");
            }
        }

        public class EsqueciSenhaRequest
        {
            public string Email { get; set; }
        }

        [HttpPost("RedefinirSenha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
        {
            // Verifique se o email e o token são válidos
            var usuario = await _contexto.Usuario.SingleOrDefaultAsync(u =>
                u.Email == request.Email &&
                u.TokenRecuperacaoSenha == request.TokenRecuperacao &&
                u.TokenRecuperacaoSenhaExpiracao > DateTime.UtcNow);

            if (usuario == null)
            {
                return BadRequest("Token de recuperação de senha inválido ou expirado.");
            }

            // Atualize a senha do usuário
            usuario.Senha = request.NovaSenha;

            // Gere um novo token de recuperação de senha
            string novoTokenRecuperacaoSenha = Guid.NewGuid().ToString();
            DateTime novaTokenExpiracao = DateTime.UtcNow.AddHours(1); // Novo token válido por 1 hora

            // Atribua o novo token ao usuário
            usuario.TokenRecuperacaoSenha = novoTokenRecuperacaoSenha;
            usuario.TokenRecuperacaoSenhaExpiracao = novaTokenExpiracao;

            _contexto.Usuario.Update(usuario);
            await _contexto.SaveChangesAsync();

            return Ok("Senha redefinida com sucesso.");
        }

        public class RedefinirSenhaRequest
        {
            public string Email { get; set; }
            public string TokenRecuperacao { get; set; }
            public string NovaSenha { get; set; }
        }
    }
}
