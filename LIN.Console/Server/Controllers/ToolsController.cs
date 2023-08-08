using LIN.Console.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace LIN.Console.Server.Controllers;


[Route("api")]
public class ToolsController : Microsoft.AspNetCore.Mvc.Controller
{


    [HttpGet("myip")]
    public async Task<ActionResult> WhatsMyIP([FromServices] IHttpContextAccessor http)
    {

        // Cual es mi IP
        var myIP = Firewall.HttpIPv4(http);

        var isvalid = LIN.Shared.Validations.IP.ValidateIPv4(myIP);

        if (isvalid)
            return Ok(myIP);


        return StatusCode(503, myIP);

    }


}
