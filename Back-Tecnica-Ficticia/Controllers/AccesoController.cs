using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tecnica_FicticiaSA.Custom;
using Tecnica_FicticiaSA.Models;
using Tecnica_FicticiaSA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Tecnica_FicticiaSA.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly FicticiaDbContext _db;
        private readonly Utilidades _utilidades;
        public AccesoController(FicticiaDbContext db, Utilidades utilidades)
        {
            _db = db;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO user)
        {
            var modeloUsuario = new Usuario
            {
                UsuarioNombre = user.nombre,
                UsuarioCorreo = user.correo,
                UsuarioClave = _utilidades.encriptSHA256(user.clave),
            };

            await _db.Usuarios.AddAsync(modeloUsuario);
            await _db.SaveChangesAsync();

            if (modeloUsuario.UsuarioId != 0)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, message = "usuario registrado" });
            }

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, message = "no se ha podido registrar el usuario" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO userLogging)
        {
            var usuarioEncontrado = await _db.Usuarios.Where(u =>
                                                             u.UsuarioCorreo == userLogging.correo &&
                                                             u.UsuarioClave == _utilidades.encriptSHA256(userLogging.clave)
                                                             ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "", message = "Usuario no encontrado" });
            }

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilidades.createJWT(usuarioEncontrado), message = "Usuario encontrado" });
        }
    }
}
