using MSDAL;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Negocio.Util
{
    public class GlobalDataBaseController
    {
        private static EnvironmentEnum databaseEnvironment;

        public static EnvironmentEnum DatabaseEnvironment
        {
            get
            {
                return DatabaseEnvironment;
            }
            set
            {
                databaseEnvironment = value;

                SetConnectionString();
            }
        }

        #region Set Connection String
        private static void SetConnectionString()
        {
            if (databaseEnvironment == EnvironmentEnum.Development)
            {
                ConnectionFactory.connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringDev"].ConnectionString;
            }
            else if (databaseEnvironment == EnvironmentEnum.Production)
            {
                ConnectionFactory.connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringProd"].ConnectionString;
            }
        }
        #endregion Set Connection String

        public static void ConnectDataBase() => ConnectionFactory.ConnectDataBase();

        public static void DisconnectDataBase() => ConnectionFactory.DisconnectDataBase();

        #region Begin Transaction
        public static void BeginTransaction()
        {
            ConnectionFactory.ConnectDataBase();

            ConnectionFactory.BeginTransaction();
        }
        #endregion Begin Transaction

        public static void CommitTransaction() => ConnectionFactory.CommitTransaction();

        public static void RollbackTransaction() => ConnectionFactory.RollbackTransaction();

        #region Select
        public static DataTable Select(string SQL)
        {
            return ConnectionFactory.Consultar(SQL.ToString());
        }

        public static DataTable Select(StringBuilder SQL)
        {
            return Select(SQL.ToString());
        }

        public static DataTable Select(string SQL, SqlParameter[] parameters)
        {
            return ConnectionFactory.SelectWithParameters(SQL, parameters);
        }
        #endregion Select

        #region Parameters
        public static SqlParameter BuildParameter(string parameterName, object value, DbType type, int size = 0)
        {
            return ConnectionFactory.BuildParameter(parameterName, value, type, size);
        }
        public static SqlParameter BuildParameter(string parameterName, object value, SqlDbType type, int size = 0)
        {
            return ConnectionFactory.BuildParameter(parameterName, value, type, size);
        }

        public static SqlParameter[] AddNewParameter(SqlParameter[] parameterArray, string parameterName, object value, DbType type, int size = 0)
        {
            return ConnectionFactory.AddNewParameter(parameterArray, parameterName, value, type, size);
        }

        public static SqlParameter[] AddNewParameter(SqlParameter[] parameterArray, string parameterName, object value, SqlDbType type, int size = 0)
        {
            return ConnectionFactory.AddNewParameter(parameterArray, parameterName, value, type, size);
        }
        #endregion Parameters

        #region Execute
        public static int Execute(string SQL)
        {
            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.ExecuteScalar(SQL.ToString());
        }

        public static int Execute(StringBuilder SQL)
        {
            return Execute(SQL.ToString());
        }

        public static int Execute(string SQL, SqlParameter[] parameters)
        {
            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.ExecuteWithParameters(SQL, parameters);
        }

        public static int Execute(StringBuilder SQL, SqlParameter[] parameters)
        {
            return Execute(SQL.ToString(), parameters);
        }
        #endregion Execute
    }
}
