﻿using System;
using System.Data;
using System.Data.Common;

namespace Demo.DataAccess.Base
{
    /// <summary>
    /// Describes the behaviour of a facade class the supports
    /// read and write operations to a data-base.
    /// </summary>
    public interface IDataAccessLayerFacade
    {
        /// <summary>
        /// Gets the function for generating a connection to the data-base.
        /// </summary>
        Func<IDbConnection> ConnectionFactory { get; }

        /// <summary>
        /// Executes a query against the configured data-base, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        DataTable FillTable(string query, string dataTableName);

        /// <summary>
        /// Executes a query against the configured data-base using a specified time-out, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <param name="commandTimeout">The time-out for the SQL query to execute.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        DataTable FillTable(string query, string dataTableName, int commandTimeout);

        /// <summary>
        /// Executes a query against the configured data-base using specified query parameters, creates a <see cref="DataTable"/> of the
        /// given name and fills that table with the results of the query.
        /// </summary>
        /// <param name="query">The Sql query to execute against the Database.</param>
        /// <param name="dataTableName">The name of the DataTable to be created.</param>
        /// <param name="queryParameters">Any parameters to be used in the SQL query.</param>
        /// <returns>The newly created table.</returns>
        /// <exception cref="Demo.DataAccess.Utilities.DataAccessLayerException">Call to the data-base returned a warning or error.</exception>
        DataTable FillTable(string query, string dataTableName, DbParameter[] queryParameters);

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
        DataTable FillTable(string query, string dataTableName, DbParameter[] queryParameters, int commandTimeout);
    }
}
