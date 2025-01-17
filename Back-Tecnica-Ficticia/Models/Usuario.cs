using System;
using System.Collections.Generic;

namespace Tecnica_FicticiaSA.Models;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string UsuarioNombre { get; set; } = null!;

    public string UsuarioCorreo { get; set; } = null!;

    public string UsuarioClave { get; set; } = null!;
}
