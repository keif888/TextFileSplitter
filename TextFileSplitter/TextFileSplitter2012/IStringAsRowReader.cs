using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Martin.SQLServer.Dts
{
    internal interface IStringAsRowReader
    {
        char GetNextChar();

        bool IsEOF { get; }
    }

    internal class StringAsRowReader : IStringAsRowReader
    {
        const int BufferSize = 65536;
        private int currentPos = 0;
        private String internalBuffer = string.Empty;
        private bool endOfString = false;

        public StringAsRowReader(string buffer)
        {
            internalBuffer = buffer;
        }

        public char GetNextChar()
        {
            if (currentPos == internalBuffer.Length)
            {
                endOfString = true;
                return ' ';
            }
            else
            {
                return internalBuffer[currentPos++];
            }
        }

        public bool IsEOF
        {
            get { return endOfString; }
        }
    }
}
