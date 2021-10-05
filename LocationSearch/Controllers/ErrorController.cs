using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace LocationSearch.Controllers {
    /// <summary>
    /// Controller to handle errors globally.
    /// </summary>
    public class ErrorController : Controller {

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error() {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // If its was a ajax request then return json object
            if(!string.IsNullOrEmpty(HttpContext.Request.Headers["x-requested-with"])) {
                if(HttpContext.Request.Headers["x-requested-with"][0]
                    .ToLower() == "xmlhttprequest") {
                    var code = HttpStatusCode.InternalServerError;
                    var result = JsonConvert.SerializeObject(new { error = exceptionDetails.Error.Message });
                    HttpContext.Response.ContentType = "application/json";
                    HttpContext.Response.StatusCode = (int)code;
                    HttpContext.Response.WriteAsync(result);
                    return null;
                }
            }

            // else return error view with required details.
            if(exceptionDetails != null) {
                ViewBag.ExceptionPath = exceptionDetails.Path;
                ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
                ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;
            }

            return View("Error");
        }
    }
}
