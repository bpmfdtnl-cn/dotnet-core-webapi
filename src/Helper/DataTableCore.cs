/**
 * Created by w on 2017/7/10.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections;
using Newtonsoft.Json;

namespace W.WebApi.Helper
{
    #region DataTable to core
    public class DataTableCore
    {

        #region
        /// <summary>
        /// 整个查询语句结果的总条数，而非本DataTable的条数
        /// </summary>
        public int TotalCount { get; set; }

        public List<DataColumnCore> Columns { get; set; } = new List<DataColumnCore>();

        public List<DataRowCore> Rows { get; set; } = new List<DataRowCore>();

        public DataColumnCore[] PrimaryKey { get; set; }
        #endregion

        public DataRowCore NewRow()
        {
            return new DataRowCore(this.Columns, new object[Columns.Count]);
        }

        public ArrayList ToArray()
        {
            ArrayList arrayList = new ArrayList();
            foreach (DataRowCore dataRow in Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (DataColumnCore dataColumn in Columns)
                {
                    //if (dataColumn.ColumnType == typeof(DateTime))
                    //    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName] == null ? "" : DateTime.Parse(dataRow[dataColumn.ColumnName].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    //else
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName]);
                }
                arrayList.Add(dictionary);
            }
            return arrayList;
        }
    }

    public class DataColumnCore
    {
        #region
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        #endregion
    }

    public class DataRowCore
    {
        #region
        private object[] _ItemArray;
        public List<DataColumnCore> Columns { get; private set; }
        #endregion

        #region
        public DataRowCore(List<DataColumnCore> columns, object[] itemArray)
        {
            this.Columns = columns;
            this._ItemArray = itemArray;
        }

        public object this[int index]
        {
            get { return _ItemArray[index]; }
            set { _ItemArray[index] = value; }
        }
        public object this[string columnName]
        {
            get
            {
                int i = 0;
                foreach (DataColumnCore column in Columns)
                {
                    if (column.ColumnName == columnName)
                        break;
                    i++;
                }
                return _ItemArray[i];
            }
            set
            {
                int i = 0;
                foreach (DataColumnCore column in Columns)
                {
                    if (column.ColumnName == columnName)
                        break;
                    i++;
                }
                _ItemArray[i] = value;
            }
        }
        #endregion
    }
    #endregion

    public static class DataTableCoreHelper
    {
        public static DataTableCore ExecuteDataTable(this DbContext context, string sql, params object[] parameters)
        {
            //using (context)
            {
                var concurrencyDetector = context.Database.GetService<IConcurrencyDetector>();

                using (concurrencyDetector.EnterCriticalSection())
                {
                    var rawSqlCommand = context.Database.GetService<IRawSqlCommandBuilder>().Build(sql, parameters);

                    RelationalDataReader query = rawSqlCommand.RelationalCommand.ExecuteReader(context.Database.GetService<IRelationalConnection>(), parameterValues: rawSqlCommand.ParameterValues);

                    return FillDataTable(query.DbDataReader, 0, int.MaxValue);
                }
            }
        }

        public static DataTableCore ExecuteDataTable(this DbContext context, string sql, int pageIndex, int pageSize, params object[] parameters)
        {
            //using (context)
            {
                var concurrencyDetector = context.Database.GetService<IConcurrencyDetector>();

                using (concurrencyDetector.EnterCriticalSection())
                {
                    var rawSqlCommand = context.Database.GetService<IRawSqlCommandBuilder>().Build(sql, parameters);

                    RelationalDataReader query = rawSqlCommand.RelationalCommand.ExecuteReader(context.Database.GetService<IRelationalConnection>(), parameterValues: rawSqlCommand.ParameterValues);

                    return FillDataTable(query.DbDataReader, pageIndex, pageSize);
                }
            }
        }

        public static DataTableCore FillDataTable(DbDataReader reader, int pageIndex, int pageSize)
        {
            bool defined = false;

            DataTableCore table = new DataTableCore();

            int index = 0;
            int beginIndex = pageSize * pageIndex;
            int endIndex = pageSize * (pageIndex + 1) - 1;

            while (reader.Read())
            {
                object[] values = new object[reader.FieldCount];

                if (!defined)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        DataColumnCore column = new DataColumnCore()
                        {
                            ColumnName = reader.GetName(i),
                            ColumnType = reader.GetFieldType(i)
                        };

                        table.Columns.Add(column);
                    }

                    defined = true;
                }

                if (index >= beginIndex && index <= endIndex)
                {
                    reader.GetValues(values);

                    table.Rows.Add(new DataRowCore(table.Columns, values));
                }

                index++;
            }

            table.TotalCount = index;

            return table;
        }
    }
}
