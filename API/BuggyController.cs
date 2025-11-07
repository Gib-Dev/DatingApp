using System;
using Microsoft.AspNetCore.Mvc;
using API.Controllers;

namespace API;

public class BuggyController : BaseApiController
{
    [HttpGet("auth")]
    public IActionResult GetAuth()
    {
        return Unauthorized();
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }

    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("This is a server error");
    }
    
    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        throw new Exception("This is a bad request");
    }
}
