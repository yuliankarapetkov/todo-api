using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Web.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Todos")]
    public class TodosController : Controller
    {
        public IActionResult Get()
        {
            return Ok(new[] { new { Description = "Hard-coded Todo", IsCompleted = false } });
        }
    }
}