using Microsoft.AspNetCore.Mvc;
using SVM_SP.Model;
using System;

namespace SVM_SP.Util
{
    public class Response: Controller
    {
        public ErrorDTO IResponse (int error=0,int statuscode=200,string message="Respuesta",string data="")
        {
            ErrorDTO dTO = new ErrorDTO();
            try
            {            
                dTO.StatusCode = statuscode;
                dTO.Message = message;
                dTO.Error = error;
                dTO.Data = data;
            }
            catch (Exception ex)
            {
                dTO.Message = ex.Message;
                dTO.Error = -15;
                dTO.StatusCode = 500;
            }          
            return dTO;
        }

        public JsonResult CResponse(ErrorDTO obj)
        {
            JsonResult jr = new JsonResult(obj);
            jr.StatusCode = 200;
            return jr;
        }
    }
}
