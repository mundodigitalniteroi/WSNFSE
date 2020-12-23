namespace NFSE.Domain.Entities.NFe
{
    public class NfseDados
    {
        public int Id { get; set; }
        public string Lote { get; set; }
        public string Leilao { get; set; }
        public string Status { get; set; }
        public string Placa { get; set; }
        public string Chassi { get; set; }
        public string Renavam { get; set; }
        public string MarcaModelo { get; set; }
        public string Cor { get; set; }
        public string Combustivel { get; set; }
        public int AnoFab { get; set; }
        public int AnoMod { get; set; }
        public string NumeroMotor { get; set; }
        public string DataApreensao { get; set; }
        public string UfVeiculo { get; set; }
        public string CpfCnpj { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
        public string CodigoMunicipioIbge { get; set; }
        public bool NotaCriada { get; set; }
        public decimal ValorServico { get; set; }
        public int IdentificadorNota { get; set; }
    }
}