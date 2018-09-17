using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvioTeste
{
    class Program
    {
        static void Main(string[] args)
        {           
            
            
            
            nfse.CapaAutorizacaoNfse obj = new nfse.CapaAutorizacaoNfse();
            nfse.WSnfseSoapClient zxk = new nfse.WSnfseSoapClient();
            /*obj.homologacao = true;
           
           

            obj.autorizar = new nfse.Autorizar();
            obj.autorizar.data_emissao = "2018-08-08T09:34:43";
            obj.autorizar.natureza_operacao = "1";
            obj.autorizar.optante_simples_nacional = "false";

           
            obj.autorizar.prestador = new nfse.Prestador();
            obj.autorizar.prestador.cnpj = "27947093000112";
            obj.autorizar.prestador.inscricao_municipal = "60376000190";
            obj.autorizar.prestador.codigo_municipio = "2927408";

            obj.autorizar.tomador = new nfse.Tomador();
            obj.autorizar.tomador.cpf = "34950246976";
            obj.autorizar.tomador.razao_social = "NICOLAS FELIPE PEIXOTO";
            obj.autorizar.tomador.email = "contato@mob-link.net.br";


            obj.autorizar.tomador.endereco = new nfse.Endereco();
            obj.autorizar.tomador.endereco.logradouro = "2ª TRAVESSA OGUNJA";
            obj.autorizar.tomador.endereco.numero="32";
            obj.autorizar.tomador.endereco.complemento = "...";
            obj.autorizar.tomador.endereco.bairro = "ACUPE DE BROTAS";
            obj.autorizar.tomador.endereco.codigo_municipio = null;
            obj.autorizar.tomador.endereco.uf="BA";
            obj.autorizar.tomador.endereco.cep = "40290650";

            obj.autorizar.servico = new nfse.Servico();
            obj.autorizar.servico.aliquota = null;
            obj.autorizar.servico.discriminacao = "SERVIÇOS DE REMOÇÃO E ACAUTELAMENTO DE VEÍCULOS CONFORME PROCESSO 89163";
            obj.autorizar.servico.iss_retido = "false";
            obj.autorizar.servico.valor_iss = "0";
            obj.autorizar.servico.item_lista_servico = "1101";
            obj.autorizar.servico.codigo_tributario_municipio = "1101";
            obj.autorizar.servico.valor_servicos = "1.0";
            
            

            //obj.homologacao = true;
            obj.identificador_nota = 547959;*/





            //string asa = zxk.Autorizar(obj);

            //Console.WriteLine(asa);

            
            Console.WriteLine("Testando consulta nota");

            nfse.Consultar objConsulta = new nfse.Consultar();

            objConsulta.homologacao = false;
            objConsulta.referencia = "280470";
            objConsulta.cnpj_prestador = "13053777000247";
            objConsulta.id_usuario = 0;
            string testeconsulta = zxk.Consultar(objConsulta);

            Console.WriteLine(testeconsulta);
            

            Console.WriteLine("Testando cancela nota");

            /*nfse.Cancelar objcanc = new nfse.Cancelar();

            objcanc.homologacao = false;
            objcanc.justificativa = "Testes de canecelamentos";
            objcanc.referencia = "12361";



            string testecancela = zxk.Cancelar(objcanc);

            Console.WriteLine(testecancela);*/

            Console.ReadLine();

        }
    }
}
