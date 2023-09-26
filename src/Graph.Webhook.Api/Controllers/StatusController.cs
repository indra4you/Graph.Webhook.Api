using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Graph.Webhook.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public sealed class StatusController : ControllerBase
{
    [HttpGet()]
    [AllowAnonymous()]
    public IActionResult Get(
    )
    {
        return base.Ok("Graph Webhook API Running Fine");
    }
}