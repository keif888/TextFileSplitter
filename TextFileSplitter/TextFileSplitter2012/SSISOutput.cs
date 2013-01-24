using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

#if SQL2012
using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
using IDTSOutputCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputCollection100;
using IDTSOutputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumnCollection100;
using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn100;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
using IDTSInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumn100;
using IDTSVirtualInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInputColumn100;
using IDTSVirtualInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInput100;
using IDTSInputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumnCollection100;
using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
using IDTSExternalMetadataColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSExternalMetadataColumn100;
using IDTSRuntimeConnection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSRuntimeConnection100;
using IDTSConnectionManagerFlatFile = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFile100;
using IDTSConnectionManagerFlatFileColumn = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFileColumn100;
using IDTSBufferManager = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSBufferManager100;
#endif
#if SQL2008
    using IDTSOutput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutput100;
    using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
    using IDTSOutputCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputCollection100;
    using IDTSOutputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumnCollection100;
    using IDTSOutputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSOutputColumn100;
    using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
    using IDTSInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumn100;
    using IDTSVirtualInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInputColumn100;
    using IDTSVirtualInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInput100;
    using IDTSInputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumnCollection100;
    using IDTSComponentMetaData = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData100;
    using IDTSExternalMetadataColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSExternalMetadataColumn100;
    using IDTSRuntimeConnection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSRuntimeConnection100;
    using IDTSConnectionManagerFlatFile = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFile100;
    using IDTSConnectionManagerFlatFileColumn = Microsoft.SqlServer.Dts.Runtime.Wrapper.IDTSConnectionManagerFlatFileColumn100;
    using IDTSBufferManager = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSBufferManager100;
#endif

namespace Martin.SQLServer.Dts
{
    public class SSISOutput
    {
        public SSISOutput()
        {
            _customPropertyCollection = new Dictionary<string, SSISProperty>();
            _outputColumnCollection = new List<SSISOutputColumn>();
        }

        public SSISOutput(IDTSOutput output, IDTSBufferManager bufferManager)
        {
            _customPropertyCollection = new Dictionary<string, SSISProperty>();
            _outputColumnCollection = new List<SSISOutputColumn>();
            _name = "_" + output.Name.Replace(" ", String.Empty).Replace("_", String.Empty).Replace("@", String.Empty);
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

        public Dictionary<String, SSISProperty> CustomPropertyCollection
        {
            get { return _customPropertyCollection; }
            set { _customPropertyCollection = value; }
        }

        private List<SSISOutputColumn> _outputColumnCollection;

        public List<SSISOutputColumn> OutputColumnCollection
        {
            get { return _outputColumnCollection; }
            set { _outputColumnCollection = value; }
        }

        private DTSRowDisposition _errorRowDisposition;

        public DTSRowDisposition ErrorRowDisposition
        {
            get { return _errorRowDisposition; }
            set { _errorRowDisposition = value; }
        }

        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value.Replace(" ", String.Empty); }
        }
        
    }
}
