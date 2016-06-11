using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DataReader.BusinessLogic;
using DataReader.Models;

namespace DataReader.Controllers
{
    [RoutePrefix("api")]
    public class NameQueryController : ApiController
    {
        private readonly INameBusinessLogic _nameBusinessLogic;

        public NameQueryController(INameBusinessLogic nameBusinessLogic)
        {
            _nameBusinessLogic = nameBusinessLogic;
        }

        [Route("fullName/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetData(string id)
        {
            var result = default(NameDTO);
            try
            {
                result = _nameBusinessLogic.GetById(id);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

            return Ok(result);
        }
    }
}