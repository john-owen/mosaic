using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TaskListApplication.Models;

namespace TaskListApplication.Controllers
{
    public class ValuesController : ApiController
    {
        readonly ITaskList tasklist = (ITaskList) new TasklistCSVRemote();

        [AcceptVerbs("GET")]
        [Route("api/values/get/{offset}/{count}")]
        public IHttpActionResult Get(int offset, int count)
        {
            // Offset cannot be negative
            if (offset < 0) offset = 0;
            
            return Ok(tasklist.GetRange(offset, count));
        }

        [AcceptVerbs("GET")]
        [Route("api/values/insert/{title}")]
        public IHttpActionResult Insert(string title)
        {
            if (title == null) return BadRequest();
            if (title.Length > 20) return BadRequest();

            if (tasklist.Insert(title))
                return Ok();
            else
                return BadRequest();
        }

        [AcceptVerbs("GET")] 
        [Route("api/values/remove/{id}")]
        public IHttpActionResult Remove(int id)
        {
            if (tasklist.Remove(id))
                return Ok();
            else
                return BadRequest();
        }

        [AcceptVerbs("GET")]
        [Route("api/values/rebuild")]
        public IHttpActionResult Rebuild()
        {
            if (tasklist.RebuildList())
                return Ok();
            else
                return BadRequest();
        }

        [AcceptVerbs("GET")]
        [Route("api/values/toggle/{id}")]
        public IHttpActionResult ToggleChecked(int id)
        {
            if (tasklist.ToggleComplete(id))
                return Ok();
            else
                return BadRequest();
        }

    }
}
