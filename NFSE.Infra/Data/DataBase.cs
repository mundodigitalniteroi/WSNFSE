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
        private static SystemEnvironment databaseEnvironment;

        public static SystemEnvironment SystemEnvironment
        {
            get
            {
                return SystemEnvironment;
            }
            set
            {
                databaseEnvironment = value;

                SetConnectionString();
            }
        }

        public static string GetNfeDatabase()
        {
            if (databaseEnvironment == SystemEnvironment.Development)
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
            if (databaseEnvironment == SystemEnvironment.Development)
            {
                ConnectionFactory.connectionString = ConfigurationManager.ConnectionStrings["ConnectionStringDev"].ConnectionString;
            }
            else if (databaseEnvironment == SystemEnvironment.Production)
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
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

            return ConnectionFactory.Consultar(SQL.ToString());
        }

        public static DataTable Select(StringBuilder SQL)
        {
            return Select(SQL.ToString());
        }

        public static DataTable Select(string SQL, SqlParameter[] parameters)
        {
            Debug.WriteLine(SQL.Trim() + Environment.NewLine + "GO" + Environment.NewLine);

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

            return ConnectionFactory.Execute(SQL, parameters);
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
    }
}