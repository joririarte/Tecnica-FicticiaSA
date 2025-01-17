using System;
using System.Collections.Generic;

namespace Tecnica_FicticiaSA.Models;

public partial class AtributosAdicionale
{
    public int AaId { get; set; }

    public int AaClienteId { get; set; }

    public string? AaAtributo { get; set; }

    public virtual Cliente AaCliente { get; set; } = null!;
}
