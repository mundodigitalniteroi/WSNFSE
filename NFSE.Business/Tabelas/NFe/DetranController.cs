using NFSE.Infra.Data;
using System.Data;
using System.Data.SqlClient;

namespace NFSE.Business.Tabelas.NFe
{
    public class DetranController
    {
        public string GetDetranSequence(string sequenceName)
        {
            var sqlParameters = new SqlParameter[1];

            sqlParameters[0] = new SqlParameter("@sequence_name", SqlDbType.Char)
            {
                Value = sequenceName
            };

            return DataBase.ExecuteStoredProcedureThanReturnValue(sqlParameters).ToString();
        }
    }
}