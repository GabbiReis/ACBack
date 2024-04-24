using AppMobile.Models;
using AppMobile.AppMobile.Dados;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppMobile.AppMobile.Dados.Dto.User;

namespace AppMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppMobileContext _contexto;

        public UsuariosController(AppMobileContext contexto)
        {
            _contexto = contexto;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _contexto.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuario(int id)
        {
            var usuario = await _contexto.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost("Cadastrar")]
        public async Task<ActionResult<Usuarios>> CadastrarUsuario([FromForm] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid) // Check for validation errors
                {
                    return BadRequest(ModelState);
                }

                var usuario = new Usuarios
                {
                    Nome = createUserDto.Nome,
                    Email = createUserDto.Email,
                    Senha = createUserDto.Senha, // Assuming you have logic to hash the password
                    Foto = await ProcessFoto(createUserDto.Foto) // Handle photo upload (optional)
                };

                _contexto.Usuarios.Add(usuario);
                await _contexto.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.ID }, usuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exceção interna ao cadastrar usuário: {ex.InnerException?.Message}");
                return StatusCode(500, "Erro interno do servidor ao cadastrar usuário.");
            }
        }

        // Helper method to process photo upload (optional)
        private async Task<byte[]> ProcessFoto(IFormFile foto)
        {
            if (foto != null && foto.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await foto.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            return null;  // No photo uploaded
        }
    }
}

