using System;
using System.Data;

namespace Demo.DataAccess.Base
{
    /// <summary>
    /// Describes the behaviour of a facade class the supports
    /// read and write operations to a data-base.
    /// </summary>
    public interface IDataAccessLayerFacade : IDataAccessLayerReaderFacade
    {
        /// <summary>
        /// Gets the function for generating a connection to the data-base.
        /// </summary>
        Func<IDbConnection> ConnectionFactory { get; }
    }
}
