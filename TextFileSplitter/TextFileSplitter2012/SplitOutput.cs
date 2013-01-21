using FileHelpers;
using Microsoft.SqlServer.Dts.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Martin.SQLServer.Dts
{
    internal class SplitOutput
    {
        public SplitOutput()
        {
            _numberOrRecords = 0;
            _foundInBuffers = false;
        }

        private PipelineBuffer _dataBuffer;
        public PipelineBuffer DataBuffer
        {
            get { return _dataBuffer; }
            set 
            { 
                _dataBuffer = value;
                _foundInBuffers = true;
            }
        }

        private SSISOutput _dataOutput;
        public SSISOutput DataOutput
        {
            get { return _dataOutput; }
            set { _dataOutput = value; }
        }

        public Type ClassBuffer;
        public FileHelperEngine Engine;

        private Int64 _numberOrRecords;
        public Int64 IncrementRowCount()
        {
            return ++_numberOrRecords;
        }
        public Int64 NumberOfRecords
        {
            get { return _numberOrRecords; }
        }

        private Boolean _foundInBuffers;
        public Boolean FoundInBuffers
        {
            get { return _foundInBuffers; }
        }
    }
}
