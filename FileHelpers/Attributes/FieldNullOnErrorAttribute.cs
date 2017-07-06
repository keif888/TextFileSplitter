using System;
using System.Collections.Generic;
using System.Text;

namespace FileHelpers
{
    /// <summary>
    /// Indicates that any conversion errors for this field will make the result NULL
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldNullOnErrorAttribute : Attribute
    {
        /// <summary>
        /// Enables NULL values to be returned on data type conversion errors.
        /// </summary>
        public FieldNullOnErrorAttribute()
        {
        }
    }
}
