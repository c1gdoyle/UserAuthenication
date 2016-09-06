using System;
using System.Data.Common;

namespace Demo.DataAccess.Utilities
{
    /// <summary>
    /// An exception class that is thrown when a call to the datasource returns a warning or error.
    /// </summary>
    public class DataAccessLayerException : Exception
    {
        /// <summary>
        /// Intialises a new instance of <see cref="DataAccessLayerException"/> with a specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DataAccessLayerException(string message)
            :base(message)
        {
        }

        /// <summary>
        /// Intialises a new instance of <see cref="DataAccessLayerException"/> with a specified message
        /// and a reference to inner exception the was the cause of this exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The exception thrown by the datasource that is the cause of this exception.</param>
        public DataAccessLayerException(string message, DbException innerException)
            :base(message, innerException)
        {
        }
    }
}
