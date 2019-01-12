using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SVM_SA.Util
{
    class GenericFunction
    {
        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName+@"\";
        }



    }
}
