namespace megev.Models
{
    public class UsuarioInputDto
    {
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public decimal SaldoConta { get; set; }
    }

    public class UsuarioOutputDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public decimal SaldoConta { get; set; }
    }
}