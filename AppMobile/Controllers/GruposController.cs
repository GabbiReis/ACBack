using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppMobile.Models;
using AppMobile.AppMobile.Dados;
using AppMobile.AppMobile.Dados.Dto.Group;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AppMobile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GruposController : ControllerBase
    {
        private readonly AppMobileContext _context;

        public GruposController(AppMobileContext context)
        {
            _context = context;
        }

        // GET: api/Grupos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Grupos>>> GetGrupos()
        {
            return await _context.Grupo.ToListAsync();
        }

        // GET: api/Grupos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Grupos>> GetGrupo(int id)
        {
            var grupo = await _context.Grupo.FindAsync(id);

            if (grupo == null)
            {
                return NotFound();
            }

            return grupo;
        }
        [HttpGet("UsuariosDoGrupo/{grupoId}")]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuariosDoGrupo(int grupoId)
        {
            // Consultar a tabela de associação para os usuários associados ao grupo específico
            var usuariosAssociados = await _context.UsuariosGrupos
                .Where(ug => ug.GrupoId == grupoId)
                .Select(ug => ug.Usuario)
                .ToListAsync();

            return Ok(usuariosAssociados);
        }
        [HttpGet("DoUsuario")]
        public async Task<ActionResult<IEnumerable<Grupos>>> GetGruposDoUsuario()
        {
            // Obter o ID do usuário logado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                // Se não for possível extrair o ID do usuário, retorne um BadRequest
                return BadRequest("O ID do usuário não pôde ser identificado.");
            }

            // Buscar os grupos dos quais o usuário faz parte
            var gruposDoUsuario = await _context.UsuariosGrupos
                .Where(ug => ug.UsuarioId == userId)
                .Select(ug => ug.Grupo)
                .ToListAsync();

            return Ok(gruposDoUsuario);
        }



        // POST: api/Grupos/Cadastrar
        [HttpPost("Cadastrar")]
        public async Task<ActionResult<Grupos>> CriarGrupo([FromForm] CreateGroupDto createGroupDto)
        {
            try
            {
                // Obter o ID do usuário logado
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized("Usuário não está autenticado.");
                }

                var grupo = new Grupos
                {
                    Nome = createGroupDto.Nome,
                    QuantidadeMaxParticipantes = createGroupDto.QuantidadeMaxParticipantes,
                    Valor = createGroupDto.Valor,
                    DataRevelacao = createGroupDto.DataRevelacao,
                    Descricao = createGroupDto.Descricao,
                    AdministradorId = userId // Definir o usuário logado como administrador
                };

                if (createGroupDto.Icone != null && createGroupDto.Icone.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await createGroupDto.Icone.CopyToAsync(memoryStream);
                        var fotoBytes = memoryStream.ToArray();

                        // Certifique-se de que os bytes da foto são válidos antes de atribuí-los ao grupo
                        if (fotoBytes != null && fotoBytes.Length > 0)
                        {
                            grupo.Icone = fotoBytes;
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Grupo.Add(grupo);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetGrupo), new { id = grupo.ID }, grupo);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // Log detalhado da exceção
                Console.WriteLine($"Erro ao criar grupo: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }

                // Retorne um erro interno do servidor genérico para o cliente
                return StatusCode(500, "Erro interno do servidor ao criar grupo.");
            }
        }

        // POST: api/Grupos/TransferirAdmin/{grupoId}
        [HttpPost("TransferirAdmin/{grupoId}")]
        public async Task<IActionResult> TransferirAdmin(int grupoId, [FromBody] string novoAdminId)
        {
            // Busque o grupo pelo ID
            var grupo = await _context.Grupo.FindAsync(grupoId);
            if (grupo == null)
            {
                return NotFound("Grupo não encontrado.");
            }

            // Verifique se o usuário atual é o administrador do grupo
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != grupo.AdministradorId)
            {
                return Forbid("Você não tem permissão para realizar esta operação.");
            }

            // Verifique se o novo administrador é um usuário válido
            var novoAdmin = await _context.Usuario.FindAsync(novoAdminId);
            if (novoAdmin == null)
            {
                return NotFound("Novo administrador não encontrado.");
            }

            // Atualize o ID do administrador no grupo
            grupo.AdministradorId = novoAdminId;
            _context.Grupo.Update(grupo);
            await _context.SaveChangesAsync();

            return Ok("Administração do grupo transferida com sucesso.");
        }

    }
}
