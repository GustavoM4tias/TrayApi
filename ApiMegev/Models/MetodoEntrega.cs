using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace megev.Models
{
    public class MetodoEntrega
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }

        private MetodoEntrega() { }
        public MetodoEntrega(string nome)
        {
            Nome = nome;
        }
    }
}