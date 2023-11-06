using System;
using System.Collections.Generic;
using System.Text;

namespace RepoInspector.src.Exceptions
{
    /// <summary>
    /// Represents an exception thrown for configuration-related errors.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ConfigurationException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that describes the exception.</param>
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
