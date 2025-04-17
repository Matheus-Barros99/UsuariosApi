using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using UsuariosApi.Context;

namespace UsuariosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os usuários cadastrados
        /// </summary>
        /// <returns>Status code</returns>
        [HttpGet("RecuperaUsuarios")]
        public async Task<IActionResult> GetAllUsers()
        {
            var usuarios = await _context.Users.ToListAsync();

            return Ok(usuarios);
        }

        /// <summary>
        /// Retorna um usuário específico de acordo com o id solicitado
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Status code</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var usuario = await _context.Users.FindAsync(id);

            if (usuario == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return StatusCode((int)HttpStatusCode.OK, usuario);
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="usuario">Usuário a ser cadastrado</param>
        /// <returns></returns>
        [HttpPost("SalvaUsuario")]
        public async Task<IActionResult> CreateUser([FromBody] Models.User usuario)
        {
            if (usuario == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            usuario.Salt = Utilidades.PasswordHasher.SaltGenerator();
            usuario.Senha = Utilidades.PasswordHasher.HashSenha(usuario.Senha, usuario.Salt);

            await _context.Users.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = usuario.Id }, usuario);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">Id do usuário a ser atualizado</param>
        /// <param name="usuario">Modelo de usuário alterado</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Models.User usuario)
        {
            if (id != usuario.Id)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            usuario.Salt = Utilidades.PasswordHasher.SaltGenerator();
            usuario.Senha = Utilidades.PasswordHasher.HashSenha(usuario.Senha, usuario.Salt);

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!UserExists(id))
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }
                else
                {
                    throw;
                }
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Deleta um usuário existente
        /// </summary>
        /// <param name="id">Id do usuário a ser deletado</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var usuario = await _context.Users.FindAsync(id);

            if (usuario == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            _context.Users.Remove(usuario);
            await _context.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.OK);
        }

        /// <summary>
        /// Verifica se o usuário existe no banco de dados
        /// </summary>
        /// <param name="id">Id do usuário a ser verificado</param>
        /// <returns></returns>
        private bool UserExists(int id)
        {
            var usuario = _context.Users.Find(id);

            if (usuario == null)
                return false;

            else
                return true;
        }
    }
}
