using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Martin.SQLServer.Dts
{
    internal class SSISOutputColumn
    {
        public SSISOutputColumn()
        {
            _customPropertyCollection = new Dictionary<string, SSISProperty>();
        }

        public SSISOutputColumn(IDTSOutputColumn100 outputColumn)
        {
            _customPropertyCollection = new Dictionary<string, SSISProperty>();
            _name = outputColumn.Name;
            _truncationRowDisposition = outputColumn.TruncationRowDisposition;
            _identificationString = outputColumn.IdentificationString;
            _errorRowDisposition = outputColumn.ErrorRowDisposition;
            _lineageID = outputColumn.LineageID;
            for (int j = 0; j < outputColumn.CustomPropertyCollection.Count; j++)
            {
                SSISProperty newProperty = new SSISProperty();
                newProperty.Name = outputColumn.CustomPropertyCollection[j].Name;
                newProperty.Value = outputColumn.CustomPropertyCollection[j].Value;
                _customPropertyCollection.Add(newProperty.Name, newProperty);
            }
        }

        private Dictionary<String, SSISProperty> _customPropertyCollection;

        public Dictionary<String, SSISProperty> CustomPropertyCollection
        {
            get { return _customPropertyCollection; }
            set { _customPropertyCollection = value; }
        }

        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private DTSRowDisposition _truncationRowDisposition;

        public DTSRowDisposition TruncationRowDisposition
        {
            get { return _truncationRowDisposition; }
            set { _truncationRowDisposition = value; }
        }

        private int _lineageID;

        public int LineageID
        {
            get { return _lineageID; }
            set { _lineageID = value; }
        }

        private String _identificationString;

        public String IdentificationString
        {
            get { return _identificationString; }
            set { _identificationString = value; }
        }

        private DTSRowDisposition _errorRowDisposition;

        public DTSRowDisposition ErrorRowDisposition
        {
            get { return _errorRowDisposition; }
            set { _errorRowDisposition = value; }
        }
        
        
    }
}
