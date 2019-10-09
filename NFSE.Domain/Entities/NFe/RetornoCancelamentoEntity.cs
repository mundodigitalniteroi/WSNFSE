using System.Collections.Generic;

namespace NFSE.Domain.Entities.NFe
{
    public class RetornoCancelamentoEntity
    {
        public string status { get; set; }

        public List<Erros> erros { get; set; }
    }
}