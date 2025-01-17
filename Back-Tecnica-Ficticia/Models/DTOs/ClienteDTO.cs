namespace Tecnica_FicticiaSA.Models.DTOs
{
    public class ClienteDTO
    {
        public string ClienteNombre { get; set; } = null!;

        public string ClienteIdentificacion { get; set; } = null!;

        public int? ClienteEdad { get; set; }

        public string? ClienteGenero { get; set; }

        public bool? ClienteEstado { get; set; }

        public bool? ClienteManeja { get; set; }

        public bool? ClienteLentes { get; set; }

        public bool? ClienteDiabetico { get; set; }

        public bool? ClienteOtros { get; set; }

        public virtual ICollection<AtributosAdicionalesDTO> AtributosAdicionales { get; set; } = new List<AtributosAdicionalesDTO>();
    }
}
