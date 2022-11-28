using System.ComponentModel.DataAnnotations;

namespace LocadoraWeb.Model
{
    public class Filme
    {
        public int FilmeId { get; set; }
        [StringLength(100, ErrorMessage = "O campo suporta até 100 caracteres.")]
        public string Titulo { get; set; }
        public int ClassificacaoIndicada { get; set; }
        public byte Lancamento { get; set; }
    }
}
