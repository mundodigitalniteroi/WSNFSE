namespace Negocio.Modelo
{
  public class Tomador
  {
    public string cnpj { get; set; }

    public string cpf { get; set; }

    public string razao_social { get; set; }

    public string email { get; set; }

    public string inscricao_municipal { get; set; }

    public string telefone { get; set; }

    public Endereco endereco { get; set; }
  }
}
