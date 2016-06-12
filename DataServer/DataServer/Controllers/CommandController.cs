using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using DataServer.BusinessLogic;
using DataServer.Models;
using DataServer.Utils;

namespace DataServer.Controllers
{
    [RoutePrefix("api")]
    public class CommandController : ApiController
    {
        private readonly ICommandFullName _commandFullNameBusinessLogic;
        
        public CommandController(ICommandFullName commandFullNameBusinessLogic)
        {
            _commandFullNameBusinessLogic = commandFullNameBusinessLogic;
        }
        
        [Route("fullName")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateData(string firstName, string lastName)
        {
            var resultId = default(string);
            try
            {
                resultId = await _commandFullNameBusinessLogic.CreateFullName(firstName, lastName);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }
            
            return Ok(resultId);
        }

        [Route("fullName/{id}")]
        [HttpPatch]
        public async Task<IHttpActionResult> UpdateData(string id, string firstName, string lastName)
        {
            var result = default(bool);
            try
            {
                result = await _commandFullNameBusinessLogic.UpdateData(id, firstName, lastName);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

            return Ok(result);
        }

        [Route("fullName/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetData(string id)
        {
            var result = default(FullName);
            try
            {
                result = await _commandFullNameBusinessLogic.GetById(id);
            }
            catch (Exception ex)
            {
                return StatusCode(HttpStatusCode.ExpectationFailed);
            }

            return Ok(result);
        }
    }
}