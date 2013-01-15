using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Martin.SQLServer.Dts
{
    internal class SSISProperty
    {
        public SSISProperty()
        {
            _name = String.Empty;
            _value = null;
        }

        private String _name;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Object _value;

        public Object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
