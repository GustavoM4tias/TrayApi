using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace megev.Models
{
    public class Produto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Referencia { get; set; }
        public string Descricao { get; set; }
        public string Categoria { get; set; }
        public decimal Preco { get; set; }
        public bool Status { get; set; }

        public string Image { get; set; }

        private Produto() { }

        public Produto(string referencia, string descricao, string categoria, decimal preco, bool status, string image)
        {
            Referencia = referencia;
            Descricao = descricao;
            Categoria = categoria;
            Preco = preco;
            Status = status;
            Image = image;
        }
    }
}
