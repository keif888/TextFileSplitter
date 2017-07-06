using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System;
using System.Collections.Generic;

namespace Martin.SQLServer.Dts
{
    /// <summary>
    /// Class to store the details about an IDTSOutput100, without the overhead of COM calls...
    /// </summary>
    public class SSISOutput
    {
        /// <summary>
        /// Creator of the SSISOutput class.  Will populate this class with all the relevant parts of an IDTSOutput100
        /// </summary>
        /// <param name="output">The IDTSOutput100 that you wish to collect all relevant data from</param>
        /// <param name="bufferManager">The buffer manager, or null.  This is used for the collection of column lineage into buffers.</param>
        public SSISOutput(IDTSOutput100 output, IDTSBufferManager100 bufferManager)
        {
            _customPropertyCollection = new Dictionary<string, SSISProperty>();
            _outputColumnCollection = new List<SSISOutputColumn>();
            _name = "_" + System.Text.RegularExpressions.Regex.Replace(output.Name, @"[^a-zA-Z0-9]", String.Empty);
            _errorRowDisposition = output.ErrorRowDisposition;

            // Get the Custom Properties
            for (int i = 0; i < output.CustomPropertyCollection.Count; i++)
            {
                SSISProperty newProperty = new SSISProperty();
                newProperty.Name = output.CustomPropertyCollection[i].Name;
                newProperty.Value = output.CustomPropertyCollection[i].Value;
                _customPropertyCollection.Add(newProperty.Name, newProperty);
            }

            // Get the Columns
            for (int i = 0; i < output.OutputColumnCollection.Count; i++)
            {
                SSISOutputColumn newColumn = new SSISOutputColumn(output.OutputColumnCollection[i], bufferManager, output.Buffer);
                _outputColumnCollection.Add(newColumn);
            }
        }

        private Dictionary<String, SSISProperty> _customPropertyCollection;

        /// <summary>
        /// Returns the CustomPropertyCollection
        /// </summary>
        public Dictionary<String, SSISProperty> CustomPropertyCollection
        {
            get { return _customPropertyCollection; }
        }

        private List<SSISOutputColumn> _outputColumnCollection;

        /// <summary>
        /// Returns the list of SSISOutputColumns that were attached to the IDTSOutput100
        /// </summary>
        public List<SSISOutputColumn> OutputColumnCollection
        {
            get { return _outputColumnCollection; }
        }

        private DTSRowDisposition _errorRowDisposition;

        public DTSRowDisposition ErrorRowDisposition
        {
            get { return _errorRowDisposition; }
        }

        private String _name;

        public String Name
        {
            get { return _name; }
        }
        
    }
}
