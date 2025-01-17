using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tecnica_FicticiaSA.Custom;
using Tecnica_FicticiaSA.Models;
using Tecnica_FicticiaSA.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;
using Microsoft.AspNetCore.JsonPatch;

namespace Tecnica_FicticiaSA.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly FicticiaDbContext _db;
        public ClienteController(FicticiaDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("Clientes")]
        public async Task<IActionResult> ListarClientes()
        {
            var lista = await _db.Clientes
            .Include(c => c.AtributosAdicionales)
            .Select(c => new
            {
                c.ClienteId,
                c.ClienteNombre,
                c.ClienteIdentificacion,
                c.ClienteEdad,
                c.ClienteGenero,
                c.ClienteEstado,
                c.ClienteManeja,
                c.ClienteLentes,
                c.ClienteDiabetico,
                c.ClienteOtros,
                AtributosAdicionales = c.AtributosAdicionales.Select(a => new
                {
                    a.AaAtributo
                })
            })
            .ToListAsync();
            return Ok(new { clientes = lista });
        }

        [HttpGet]
        [Route("Clientes/{id}")]
        public async Task<IActionResult> ObtenerCliente(int id)
        {
            var cliente = await _db.Clientes
            .Include(c => c.AtributosAdicionales)
            .Where(c => c.ClienteId == id)
            .Select(c => new
            {
                c.ClienteId,
                c.ClienteNombre,
                c.ClienteIdentificacion,
                c.ClienteEdad,
                c.ClienteGenero,
                c.ClienteEstado,
                c.ClienteManeja,
                c.ClienteLentes,
                c.ClienteDiabetico,
                c.ClienteOtros,
                AtributosAdicionales = c.AtributosAdicionales.Select(a => new
                {
                    a.AaAtributo
                })
            })
            .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound(new { isSuccess = false, message = "Cliente no encontrado" });
            }

            return Ok(new { isSuccess = true, cliente });
        }

        [HttpPost]
        [Route("Clientes")]
        public async Task<IActionResult> RegistrarCliente(ClienteDTO cliente)
        {
            if (cliente == null ||
                string.IsNullOrWhiteSpace(cliente.ClienteNombre) ||
                string.IsNullOrWhiteSpace(cliente.ClienteIdentificacion) ||
                cliente.ClienteEdad >99 && cliente.ClienteEdad <18)
                return BadRequest(new { isSuccess = false, message = "El cliente está sin datos o con datos inválidos" });

            var clienteModelo = new Cliente
            {
                ClienteNombre = cliente.ClienteNombre,
                ClienteIdentificacion = cliente.ClienteIdentificacion,
                ClienteEdad = cliente.ClienteEdad,
                ClienteGenero = cliente.ClienteGenero,
                ClienteEstado = cliente.ClienteEstado,
                ClienteManeja = cliente.ClienteManeja,
                ClienteLentes = cliente.ClienteLentes,
                ClienteDiabetico = cliente.ClienteDiabetico,
                ClienteOtros = cliente.ClienteOtros,
                AtributosAdicionales = cliente.AtributosAdicionales
                                       .Select(item => new AtributosAdicionale { AaAtributo = item.AaAtributo })
                                       .ToList()
            };

            try
            {
                await _db.Clientes.AddAsync(clienteModelo);
                await _db.SaveChangesAsync();

                return Ok(new { isSuccess = true, message = "Cliente registrado", clienteId = clienteModelo.ClienteId });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Ocurrió un error al registrar el cliente.", error = ex.Message });
            }
        }

        [HttpDelete]
        [Route("Clientes/{id}")]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            try
            {
               
                var cliente = await _db.Clientes
                    .Include(c => c.AtributosAdicionales) 
                    .FirstOrDefaultAsync(c => c.ClienteId == id);

                
                if (cliente == null)
                {
                    return NotFound(new { isSuccess = false, message = "Cliente no encontrado" });
                }

                _db.Clientes.Remove(cliente);
                await _db.SaveChangesAsync();

                return Ok(new { isSuccess = true, message = "Cliente eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false, message = "Error al eliminar el cliente", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("Clientes/{id}")]
        public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ClienteDTO cliente)
        {
            if (cliente == null)
            {
                return BadRequest(new { isSuccess = false, message = "Datos inválidos" });
            }

            var clienteExistente = await _db.Clientes
                .Include(c => c.AtributosAdicionales)
                .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (clienteExistente == null)
            {
                return NotFound(new { isSuccess = false, message = "Cliente no encontrado" });
            }

            
            clienteExistente.ClienteNombre = cliente.ClienteNombre;
            clienteExistente.ClienteIdentificacion = cliente.ClienteIdentificacion;
            clienteExistente.ClienteEdad = cliente.ClienteEdad;
            clienteExistente.ClienteGenero = cliente.ClienteGenero;
            clienteExistente.ClienteEstado = cliente.ClienteEstado;
            clienteExistente.ClienteManeja = cliente.ClienteManeja;
            clienteExistente.ClienteLentes = cliente.ClienteLentes;
            clienteExistente.ClienteDiabetico = cliente.ClienteDiabetico;
            clienteExistente.ClienteOtros = cliente.ClienteOtros;

            
            clienteExistente.AtributosAdicionales.Clear();
            foreach (var atributo in cliente.AtributosAdicionales)
            {
                clienteExistente.AtributosAdicionales.Add(new AtributosAdicionale
                {
                    AaAtributo = atributo.AaAtributo
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "Cliente actualizado correctamente" });
        }


        [HttpPatch]
        [Route("Clientes/{id}")]
        public async Task<IActionResult> ActualizarParcialCliente(int id, [FromBody] ClienteDTO cliente)
        {
            if (cliente == null)
            {
                return BadRequest(new { isSuccess = false, message = "Datos inválidos" });
            }

            var clienteExistente = await _db.Clientes
                .Include(c => c.AtributosAdicionales)
                .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (clienteExistente == null)
            {
                return NotFound(new { isSuccess = false, message = "Cliente no encontrado" });
            }

            if (clienteExistente.ClienteNombre != cliente.ClienteNombre)
                clienteExistente.ClienteNombre = cliente.ClienteNombre;
            if (clienteExistente.ClienteIdentificacion != cliente.ClienteIdentificacion)
                clienteExistente.ClienteIdentificacion = cliente.ClienteIdentificacion;
            if(clienteExistente.ClienteEdad != cliente.ClienteEdad)
                clienteExistente.ClienteEdad = cliente.ClienteEdad;
            if(clienteExistente.ClienteGenero != cliente.ClienteGenero)
                clienteExistente.ClienteGenero = cliente.ClienteGenero;
            if(clienteExistente.ClienteEstado != cliente.ClienteEstado)
                clienteExistente.ClienteEstado = cliente.ClienteEstado;
            if(clienteExistente.ClienteManeja != cliente.ClienteManeja)
                clienteExistente.ClienteManeja = cliente.ClienteManeja;
            if(clienteExistente.ClienteLentes != cliente.ClienteLentes)
                clienteExistente.ClienteLentes = cliente.ClienteLentes;
            if(clienteExistente.ClienteDiabetico != cliente.ClienteDiabetico)
                clienteExistente.ClienteDiabetico = cliente.ClienteDiabetico;
            if(clienteExistente.ClienteOtros != cliente.ClienteOtros)
                clienteExistente.ClienteOtros = cliente.ClienteOtros;


            clienteExistente.AtributosAdicionales.Clear();
            foreach (var atributo in cliente.AtributosAdicionales)
            {
                clienteExistente.AtributosAdicionales.Add(new AtributosAdicionale
                {
                    AaAtributo = atributo.AaAtributo
                });
            }

            await _db.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "Cliente actualizado correctamente" });
        }
    }
}
