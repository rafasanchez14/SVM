using SVM_SP.Model;
using SVM_SP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SVM_SP.DAL
{
    public class SVMDocument
    {

        public ErrorDTO GetDocument()
        {
            var oResp = new Response(); 
            try
            {
               return oResp.IResponse(statuscode:404, data: @"C:\Users\Joselyn\source\repos");
               
            }
            catch (Exception ex)
            {
                return oResp.IResponse(error: -1, statuscode: 400, message: ex.Message);
            }
        }

    }
}
