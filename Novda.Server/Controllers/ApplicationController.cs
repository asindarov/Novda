using Microsoft.AspNetCore.Mvc;

namespace Novda.Server.Controllers;

[ApiController]
[Route("applications")]
public class ApplicationController : ControllerBase
{
    [HttpPost("register")]
    public async Task<string> RegisterApplicationAsync(Guid applicationId, string localApplicationUrl)
    {
        return "Registered";
    }
}
