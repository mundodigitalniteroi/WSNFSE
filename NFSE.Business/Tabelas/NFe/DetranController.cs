using NFSE.Infra.Data;
using System.Data;
using System.Data.SqlClient;

namespace NFSE.Business.Tabelas.NFe
{
    public class DetranController
    {
        public int GetDetranSequence(string sequence_name)
        {
            var sqlParameters = new SqlParameter[1];

            sqlParameters[0] = new SqlParameter("@sequence_name", SqlDbType.Char)
            {
                Value = sequence_name
            };

            return DataBase.ExecuteStoredProcedureThanReturnValue(sqlParameters);
        }
    }
}