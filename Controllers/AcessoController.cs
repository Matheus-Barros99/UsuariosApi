using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Text;
using UsuariosApi.Context;
using UsuariosApi.Models.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace UsuariosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcessoController : ControllerBase
    {
        private readonly AppDbContext _contexto;
        private readonly IConfiguration _configuration;
        public AcessoController(AppDbContext contexto, IConfiguration configuration)
        {
            _contexto = contexto;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginDTO dadosLogin)
        {
            var usuario = _contexto.Users.Where(u => u.Email == dadosLogin.Email).FirstOrDefault();

            if (usuario == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            var senhaCorreta = Utilidades.PasswordHasher.VerificaSenha(dadosLogin.Senha, usuario.Salt, usuario.Senha);

            if (!senhaCorreta)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized);
            }

            var claims = new[]
              {
                    new Claim(ClaimTypes.Name, usuario.Email)
              };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenLogin"]!)); // deve vir do appsettings
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                                claims: claims,
                                expires: DateTime.Now.AddHours(1),
                                signingCredentials: creds);

            return StatusCode((int)HttpStatusCode.OK, new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
