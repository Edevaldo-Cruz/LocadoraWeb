using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LocadoraWeb.Model
{
    public class Cliente
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "O campo nome deve ser preenchido.")]
        //[Index("idx_NOME" ,IsUnique = true)]
        [StringLength(200, ErrorMessage = "O campo suporta até 200 caracteres.")]
        
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo CPF deve ser preenchido.")]
        //[Index(IsUnique = true)]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF invalido")]
        
        public string CPF { get; set; }

        [Required(ErrorMessage = "O campo data de nascimento deve ser preenchido.")]
        public DateTime DataNascimento { get; set; }
    }
}
