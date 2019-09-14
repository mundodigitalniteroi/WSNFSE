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
            if (dataTable == null || dataTable.Rows.Count.Equals(0) || dataTable.Columns.Count.Equals(0))
            {
                return null;
            }

            var lines = new List<DataRow>();

            foreach (DataRow row in dataTable.Rows)
            {
                lines.Add(row);
            }

            return ConvertDataRowToList<T>(lines);
        }

        private static List<T> ConvertDataRowToList<T>(List<DataRow> lines)
        {
            var list = new List<T>();

            foreach (DataRow linha in lines)
            {
                T item = CreateItem<T>(linha);

                list.Add(item);
            }

            return list;
        }

        private static T CreateItem<T>(DataRow row)
        {
            T objectInstance = Activator.CreateInstance<T>();

            string columnName;

            foreach (DataColumn dataColumn in row.Table.Columns)
            {
                columnName = dataColumn.ColumnName;

                var property = objectInstance.GetType().GetProperty(columnName);

                if (property != null)
                {
                    Type type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    object value = default;

                    value = row[columnName];

                    try
                    {
                        object safeValue = (value == DBNull.Value) ? null : Convert.ChangeType(value, type);

                        property.SetValue(objectInstance, safeValue, null);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return objectInstance;
        }
    }
}