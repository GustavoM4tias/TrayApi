using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace megev.Models
{
    public class ObjetivoLoja
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }

        private ObjetivoLoja() { }

        public ObjetivoLoja(string nome)
        {
            Nome = nome;
        }
    }
}