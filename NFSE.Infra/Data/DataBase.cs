using MSDAL;
using NFSE.Domain.Enum;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;

namespace NFSE.Infra.Data
{
    public class DataBase
    {
        public static SystemEnvironment SystemEnvironment { get; set; } = SystemEnvironment.Development;

        public static string GetNfeDatabase()
        {
            if (SystemEnvironment == SystemEnvironment.Development)
            {
                return "db_NfseDev";
            }
            else
            {
                return "db_Nfse";
            }
        }

        #region Set Connection String
        private static void SetConnectionString()
        {
            if (SystemEnvironment == SystemEnvironment.Development)
            {
                ConnectionFactory.connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringDev"].ConnectionString;
            }
            else if (SystemEnvironment == SystemEnvironment.Production)
            {
                ConnectionFactory.connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringProd"].ConnectionString;
            }
        }
        #endregion Set Connection String

        public static void ConnectDataBase()
        {
            SetConnectionString();

            ConnectionFactory.ConnectDataBase();
        }

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
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            SetConnectionString();

            return ConnectionFactory.Consultar(SQL.ToString());
        }

        public static DataTable Select(StringBuilder SQL)
        {
            return Select(SQL.ToString());
        }

        public static DataTable Select(string SQL, SqlParameter[] parameters)
        {
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            SetConnectionString();

            return ConnectionFactory.Select(SQL, parameters);
        }

        public static DataTable Select(StringBuilder SQL, SqlParameter[] parameters)
        {
            return Select(SQL.ToString(), parameters);
        }
        #endregion Select

        #region Parameters
        public static SqlParameter BuildParameter(string parameterName, object value, DbType type, int size = 0)
        {
            return ParameterFactory.BuildParameter(parameterName, value, type, size);
        }
        public static SqlParameter BuildParameter(string parameterName, object value, SqlDbType type, int size = 0)
        {
            return ParameterFactory.BuildParameter(parameterName, value, type, size);
        }

        public static SqlParameter[] AddNewParameter(SqlParameter[] parameterArray, string parameterName, object value, DbType type, int size = 0)
        {
            return ParameterFactory.AddNewParameter(parameterArray, parameterName, value, type, size);
        }

        public static SqlParameter[] AddNewParameter(SqlParameter[] parameterArray, string parameterName, object value, SqlDbType type, int size = 0)
        {
            return ParameterFactory.AddNewParameter(parameterArray, parameterName, value, type, size);
        }
        #endregion Parameters

        #region Execute
        public static int Execute(string SQL)
        {
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.Execute(SQL.ToString());
        }

        public static int Execute(StringBuilder SQL)
        {
            return Execute(SQL.ToString());
        }

        public static int Execute(string SQL, SqlParameter[] parameters)
        {
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.Execute(SQL, parameters);
        }

        public static int Execute(StringBuilder SQL, SqlParameter[] parameters)
        {
            return Execute(SQL.ToString(), parameters);
        }
        #endregion Execute

        #region Execute Scalar
        public static int ExecuteScopeIdentity(string SQL)
        {
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.ExecuteScopeIdentity(SQL.ToString());
        }

        public static int ExecuteScopeIdentity(StringBuilder SQL)
        {
            return ExecuteScopeIdentity(SQL.ToString());
        }

        public static int ExecuteScopeIdentity(string SQL, SqlParameter[] parameters)
        {
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.ExecuteScopeIdentity(SQL, parameters);
        }

        public static int ExecuteScopeIdentity(StringBuilder SQL, SqlParameter[] parameters)
        {
            return ExecuteScopeIdentity(SQL.ToString(), parameters);
        }
        #endregion Execute Scalar

        public static int ExecuteStoredProcedureThanReturnValue(SqlParameter[] parameters)
        {
            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            return ConnectionFactory.ExecuteStoredProcedureThanReturnValue("dbo.SequenceGetNewValue", parameters);
        }

        public static void SetContextInfo(int usuarioId)
        {
            if (!ConnectionFactory.IsConnected())
            {
                ConnectDataBase();
            }

            ConnectionFactory.SetContextInfo(usuarioId.ToString());
        }

        #region Set NULL if empty
        public static string SetNullIfEmpty(char input)
        {
            if (input == '\0')
            {
                return "NULL";
            }
            else
            {
                return "'" + input + "'";
            }
        }

        public static string SetNullIfEmpty(DateTime inputDateTime, string outputDateTimeFormat = "yyyyMMdd HH:mm")
        {
            if (inputDateTime == DateTime.MinValue)
            {
                return "NULL";
            }
            else
            {
                return "'" + String.Format("{0:" + outputDateTimeFormat + "}", inputDateTime) + "'";
            }
        }

        public static string SetNullIfEmpty(DateTime? inputDateTime, string outputDateTimeFormat = "yyyyMMdd HH:mm")
        {
            if (inputDateTime == null)
            {
                return "NULL";
            }
            else
            {
                return "'" + String.Format("{0:" + outputDateTimeFormat + "}", inputDateTime) + "'";
            }
        }

        public static string SetNullIfEmpty(decimal input)
        {
            return (input == 0 ? "NULL" : input.ToString());
        }

        public static string SetNullIfEmpty(double input)
        {
            return (input == 0 ? "NULL" : input.ToString());
        }

        public static string SetNullIfEmpty(int input)
        {
            return (input == 0 ? "NULL" : input.ToString());
        }

        public static string SetNullIfEmpty(short input)
        {
            return (input == 0 ? "NULL" : input.ToString());
        }

        public static string SetNullIfEmpty(string input)
        {
            if (input == null) return "NULL";

            return (String.IsNullOrWhiteSpace(input) ? "NULL" : "'" + input + "'");
        }

        public static string SetNullIfEmpty(string input, string compareValue)
        {
            if (input == null) return "NULL";

            return (String.IsNullOrWhiteSpace(compareValue) ? "NULL" : "'" + input + "'");
        }

        public static string SetNullIfEmpty(int input, string compareValue)
        {
            return (String.IsNullOrWhiteSpace(compareValue) ? "NULL" : input.ToString());
        }

        public static string SetNullIfEmpty(string input, int compareValue)
        {
            if (input == null) return "NULL";

            return (compareValue == 0 ? "NULL" : "'" + input + "'");
        }

        public static string SetNullIfEmpty(int input, int compareValue)
        {
            return (compareValue == 0 ? "NULL" : input.ToString());
        }
        #endregion Set NULL if empty
    }
}