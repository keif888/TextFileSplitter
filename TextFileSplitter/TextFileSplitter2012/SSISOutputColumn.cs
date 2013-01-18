using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace Martin.SQLServer.Dts
{
    internal class SSISOutputColumn
    {
        //public SSISOutputColumn()
        //{
        //    _customPropertyCollection = new Dictionary<string, SSISProperty>();
        //}

        public SSISOutputColumn(IDTSOutputColumn100 outputColumn, IDTSBufferManager100 bufferManager, int bufferID)
        {
            _customPropertyCollection = new Dictionary<string, SSISProperty>();
            _name = "_" + outputColumn.Name.Replace(" ", String.Empty).Replace("_", String.Empty).Replace("@", String.Empty);
            //_truncationRowDisposition = outputColumn.TruncationRowDisposition;
            _identificationString = outputColumn.IdentificationString;
            //_errorRowDisposition = outputColumn.ErrorRowDisposition;
            _lineageID = outputColumn.LineageID;
            _datatype = outputColumn.DataType;
            for (int j = 0; j < outputColumn.CustomPropertyCollection.Count; j++)
            {
                SSISProperty newProperty = new SSISProperty();
                newProperty.Name = outputColumn.CustomPropertyCollection[j].Name;
                newProperty.Value = outputColumn.CustomPropertyCollection[j].Value;
                _customPropertyCollection.Add(newProperty.Name, newProperty);
            }
            
            switch ((Utilities.usageOfColumnEnum)_customPropertyCollection[ManageProperties.usageOfColumn].Value)
            {
                case Utilities.usageOfColumnEnum.RowType:
                    _isMasterOrKey = false;
                    _isRowData = false;
                    _isRowType = true;
                    break;
                case Utilities.usageOfColumnEnum.Passthrough:
                case Utilities.usageOfColumnEnum.Ignore:
                    _isMasterOrKey = false;
                    _isRowData = false;
                    _isRowType = false;
                    break;
                case Utilities.usageOfColumnEnum.Key:
                case Utilities.usageOfColumnEnum.MasterValue:
                    _isMasterOrKey = true;
                    _isRowData = false;
                    _isRowType = false;
                    break;
                case Utilities.usageOfColumnEnum.RowData:
                    _isMasterOrKey = false;
                    _isRowData = true;
                    _isRowType = false;
                    break;
                default:
                    _isMasterOrKey = false;
                    _isRowData = false;
                    _isRowType = false;
                    break;
            }

            if ((int)_customPropertyCollection[ManageProperties.keyOutputColumnID].Value > 0)
            {
                _isDerived = true;
            }
            else
            {
                _isDerived = false;
            }
            _outputBufferID = bufferManager.FindColumnByLineageID(bufferID, outputColumn.LineageID);
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
            set { _name = value.Replace(" ", String.Empty); }
        }

        //private DTSRowDisposition _truncationRowDisposition;

        //public DTSRowDisposition TruncationRowDisposition
        //{
        //    get { return _truncationRowDisposition; }
        //    set { _truncationRowDisposition = value; }
        //}

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

        //private DTSRowDisposition _errorRowDisposition;

        //public DTSRowDisposition ErrorRowDisposition
        //{
        //    get { return _errorRowDisposition; }
        //    set { _errorRowDisposition = value; }
        //}

        private DataType _datatype;

        public DataType SSISDataType
        {
            get { return _datatype; }
            set { _datatype = value; }
        }

        private int _outputBufferID;

        public int OutputBufferID
        {
            get { return _outputBufferID; }
            set { _outputBufferID = value; }
        }

        private System.Reflection.FieldInfo _fileHelperField;

        public System.Reflection.FieldInfo FileHelperField
        {
            get { return _fileHelperField; }
            set { _fileHelperField = value; }
        }

        private Boolean _isMasterOrKey;

        public Boolean IsMasterOrKey
        {
            get { return _isMasterOrKey; }
        }

        private Boolean _isRowData;

        public Boolean IsRowData
        {
            get { return _isRowData; }
        }

        private Boolean _isRowType;

        public Boolean IsRowType
        {
            get { return _isRowType; }
        }

        private Boolean _isDerived;

        public Boolean IsDerived
        {
            get { return _isDerived; }
            set { _isDerived = value; }
        }
        

    }
}
