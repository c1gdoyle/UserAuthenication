﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.DataAccess.Base;

namespace Demo.DataAccess.Utilities
{
    public class DataAccessLayerFacade : IDataAccessLayerFacade
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private const int DefaultCommandTimeout = 300;

        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacade"/> with a
        /// specifed function to create a connection to the data-base.
        /// </summary>
        /// <param name="connectionFactory">The function used to generate a connection for the data-base.</param>
        public DataAccessLayerFacade(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        #region IDataAccessLayerFacade Members
        /// <summary>
        /// Gets the function for generating a connection to the data-base.
        /// </summary>
        public Func<IDbConnection> ConnectionFactory
        {
            get { return _connectionFactory; }
        }

        public DataTable FillTable(string query, string dataTableName)
        {
            return FillTable(query, dataTableName, new object[0], DefaultCommandTimeout, 1);
        }

        public DataTable FillTable(string query, string dataTableName, object[] queryParameters)
        {
            return FillTable(query, dataTableName, queryParameters, DefaultCommandTimeout, 1);
        }

        public DataTable FillTable(string query, string dataTableName, int commandTimeout)
        {
            return FillTable(query, dataTableName, new object[0], commandTimeout, 1);
        }

        public DataTable FillTable(string query, string dataTableName, object[] queryParameters, int commandTimeout)
        {
            return FillTable(query, dataTableName, queryParameters, commandTimeout, 1);
        }
        #endregion IDataAccessLayerFacade Members

        private DataTable FillTable(string query, string dataTableName, object[] queryParameters, int commandTimeout, int retryCount)
        {
            using (IDbConnection connection = ConnectionFactory())
            {
                try
                {
                    return FillTable(connection, query, dataTableName, queryParameters, commandTimeout);
                }
                catch (System.Data.SqlClient.SqlException sqlEx)
                {
                    if (RetriesExceeded(sqlEx, retryCount))
                    {
                        throw;
                    }
                }
                catch (System.Data.Common.DbException dbEx)
                {
                    if (RetriesExceeded(dbEx, retryCount))
                    {
                        throw;
                    }
                }
            }
            //retry
            return FillTable(query, dataTableName, queryParameters, commandTimeout, ++retryCount);
        }

        private DataTable FillTable(IDbConnection connection, string query, string dataTableName, object[] queryParameters, int commandTimeout)
        {
            OpenConnection(connection);
            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = string.Format(query, queryParameters);
                cmd.CommandTimeout = commandTimeout;

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    return PopulateDataTable(reader, dataTableName);
                }
            }
        }

        private void ExecuteUpdate(string updateQuery)
        {
            using (IDbConnection connection = ConnectionFactory())
            {
                ExecuteUpdate(connection, updateQuery);
            }
        }

        private void ExecuteUpdate(IDbConnection connection, string updateQuery)
        {
            OpenConnection(connection);

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = updateQuery;
                command.ExecuteNonQuery();
            }
        }

        private DataTable PopulateDataTable(IDataReader reader, string dataTableName)
        {
            int count = 0;
            DataTable table = null;
            while (reader.Read())
            {
                if (count++ == 0)
                {
                    table = CreateTable(reader);
                    table.TableName = dataTableName;
                }
                AddDataRow(reader, table);

            }
            return table;
        }

        private DataTable CreateTable(IDataReader reader)
        {
            var table = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                AddDataColumn(reader, table, i);
            }
            return table;
        }

        private void AddDataColumn(IDataReader reader, DataTable table, int index)
        {
            string columnName = reader.GetName(index);
            Type columnType = reader.GetFieldType(index);
            table.Columns.Add(columnName, columnType);
        }

        private void AddDataRow(IDataReader reader, DataTable table)
        {
            DataRow row = table.NewRow();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }
            table.Rows.Add(row);
        }

        private void OpenConnection(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private bool RetriesExceeded(Exception ex, int retryCount)
        {
            if (retryCount == 3)
            {
                return true;
            }
            return false;
        }
    }
}
