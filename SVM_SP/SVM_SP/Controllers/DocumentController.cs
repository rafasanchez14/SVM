using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SVM_SP.DAL;
using System.Net.Http;
using SVM_SP.Util;
using SVM_SP.Model;

namespace SVM_SP.Controllers
{
    [Route("api/Documents")]
    public class DocumentController : Controller
    {
        [HttpGet("")]
        public JsonResult GetDoc()
        {
            SVMDocument sVM = new SVMDocument();
            Response format = new Response();
            JsonResult oResp;
            try
            {
                oResp = format.CResponse(sVM.GetDocument());     
            }
            catch (Exception ex)
            {

                oResp = Json(ex);
            }
            return oResp;
        }
    }
}