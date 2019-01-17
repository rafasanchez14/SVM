using SVM_SA.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVM_SA.Util
{
    class SVMLogger
    {
        public static void Write(string message,
            string filename="SVMLogger" , 
            Boolean addDate=true)
        {
            string path = GenericFunction.GetExecutingDirectoryNameIp()+filename+".txt";

            if (addDate)
            {
                message = message + DateTime.Now.ToLongDateString();
            }
                         
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(message);
                }
           

        }

    }
}
