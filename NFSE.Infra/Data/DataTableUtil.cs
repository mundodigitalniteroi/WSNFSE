using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace NFSE.Infra.Data
{
    public static class DataTableUtil
    {
        public static List<T> DataTableToList<T>(DataTable dataTable)
        {
            if (dataTable == null | dataTable.Rows.Count.Equals(0))
                return null;

            List<DataRow> lines = new List<DataRow>();

            foreach (DataRow row in dataTable.Rows)

                lines.Add(row);

            return ConvertDataRowToList<T>(lines);
        }

        private static List<T> ConvertDataRowToList<T>(List<DataRow> linhas)
        {
            List<T> list = null;

            if (linhas != null)
            {
                list = new List<T>();

                foreach (DataRow linha in linhas)
                {
                    T item = CreateItem<T>(linha);

                    list.Add(item);
                }
            }

            return list;
        }

        private static T CreateItem<T>(DataRow row)
        {
            if (row == null)
                return default(T);

            // converte um DataRow para um objeto T
            string nomeDaColuna = default(string);

            T objeto = default(T);

            objeto = Activator.CreateInstance<T>();

            foreach (DataColumn coluna in row.Table.Columns)
            {
                nomeDaColuna = coluna.ColumnName;

                // Pega a propriedade com a mesma coluna
                Type objType = objeto.GetType();

                PropertyInfo prop = objType.GetProperty(nomeDaColuna, (BindingFlags)(BindingFlags)(int)BindingFlags.IgnoreCase + (int)BindingFlags.Public + (int)BindingFlags.Instance);

                try
                {
                    // Pega o valor da coluna
                    object value = default(object);

                    value = (row[nomeDaColuna].GetType() == typeof(DBNull)) ? null : row[nomeDaColuna];

                    // Define o valor da propriedade
                    prop.SetValue(objeto, value, null);
                }
                catch
                {
                    throw;
                }
            }

            return objeto;
        }
    }
}