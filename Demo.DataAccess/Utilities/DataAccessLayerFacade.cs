using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Demo.DataAccess.Base;

namespace Demo.DataAccess.Utilities
{
    public class DataAccessLayerFacade : IDataAccessLayerFacade
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private const int DefaultCommandTimeout = 300;
        private readonly int _retryCount;

        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacade"/> with a
        /// specifed function to create a connection to the data-base.
        /// </summary>
        /// <param name="connectionFactory">The function used to generate a connection for the data-base.</param>
        public DataAccessLayerFacade(Func<IDbConnection> connectionFactory)
            :this(connectionFactory, 3)
        {        
        }

        /// <summary>
        /// Initialises a new instance of <see cref="DataAccessLayerFacade"/> with a
        /// specifed function to create a connection to the data-base and number of times to
        /// to retry a failed call to the data-base before returning an error.
        /// </summary>
        /// <param name="connectionFactory">The function used to generate a connection for the data-base.</param>
        /// <param name="retryCount">The number of retry allowed. By default this will be 3.</param>
        public DataAccessLayerFacade(Func<IDbConnection> connectionFactory, int retryCount)
        {
            _connectionFactory = connectionFactory;
            _retryCount = retryCount;
        }

        #region IDataAccessLayerFacade Members
        /// <summary>
        /// Gets the function for generating a connection to the data-base.
        /// </summary>
        public Func<IDbConnection> ConnectionFactory
        {
            get { return _connectionFactory; }
        }

        /// <summary>
        /// Executes a query against the configured data-base, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        public DataTable FillTable(string query, string dataTableName)
        {
            return FillTable(query, dataTableName, new DbParameter[0], DefaultCommandTimeout, 1);
        }

        /// <summary>
        /// Executes a query against the configured data-base using a specified time-out, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <param name="commandTimeout">The time-out for the SQL query to execute.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        public DataTable FillTable(string query, string dataTableName, int commandTimeout)
        {
            return FillTable(query, dataTableName, new DbParameter[0], commandTimeout, 1);
        }

        /// <summary>
        /// Executes a query against the configured data-base using specified query parameters, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <param name="queryParameters">Any parameters to be used in the SQL query.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        public DataTable FillTable(string query, string dataTableName, DbParameter[] queryParameters)
        {
            return FillTable(query, dataTableName, queryParameters, DefaultCommandTimeout, 1);
        }

        /// <summary>
        /// Executes a query against the configured data-base using a specified time-out and query parameters, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <param name="queryParameters">Any parameters to be used in the SQL query.</param>
        /// <param name="commandTimeout">The time-out for the SQL query to execute.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        public DataTable FillTable(string query, string dataTableName, DbParameter[] queryParameters, int commandTimeout)
        {
            return FillTable(query, dataTableName, queryParameters, commandTimeout, 1);
        }
        #endregion IDataAccessLayerFacade Members

        private DataTable FillTable(string query, string dataTableName, DbParameter[] queryParameters, int commandTimeout, int retryCount)
        {
            using (IDbConnection connection = ConnectionFactory())
            {
                try
                {
                    return FillTable(connection, query, dataTableName, queryParameters, commandTimeout);
                }
                catch (System.Data.SqlClient.SqlException sqlEx)
                {
                    if (RetriesExceeded(retryCount))
                    {
                        throw new DataAccessLayerException(
                            string.Format("Failed calls to datasource exceeded allowed retries {0}.", retryCount), 
                            sqlEx);
                    }
                }
                catch (System.Data.Common.DbException dbEx)
                {
                    if (RetriesExceeded(retryCount))
                    {
                        throw new DataAccessLayerException(
                            string.Format("Failed calls to datasource exceeded allowed retries {0}", retryCount), 
                            dbEx);
                    }
                }
            }
            //retry
            return FillTable(query, dataTableName, queryParameters, commandTimeout, ++retryCount);
        }

        private DataTable FillTable(IDbConnection connection, string query, string dataTableName, DbParameter[] queryParameters, int commandTimeout)
        {
            OpenConnection(connection);
            using (IDbCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.CommandTimeout = commandTimeout;
                queryParameters.ToList().ForEach(p => cmd.Parameters.Add(p));

                using (IDataReader reader = cmd.ExecuteReader())
                {
                    return PopulateDataTable(reader, dataTableName);
                }
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

        private bool RetriesExceeded(int retryCount)
        {
            if (retryCount == _retryCount)
            {
                return true;
            }
            return false;
        }
    }
}
