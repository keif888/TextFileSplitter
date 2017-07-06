using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Martin.SQLServer.Dts
{
    class ArgumentVerifier
    {

        public static void CheckObjectArgument(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

        }

        public static void CheckStringArgument(string stringArgument, string argumentName)
        {
            if (string.IsNullOrEmpty(stringArgument))
            {
                throw new System.ArgumentNullException(argumentName);
            }
        }

        public static void CheckFileNameArgument(string fileName)
        {
            CheckStringArgument(fileName, "fileName");
            if (!File.Exists(fileName))
            {
                throw new ArgumentException(MessageStrings.FileDoesNotExist(fileName));
            }
        }
    }
}
