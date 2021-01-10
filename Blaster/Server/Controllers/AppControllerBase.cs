using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blaster.Server.Controllers
{
    [Authorize]
    public class AppControllerBase : ControllerBase
    {
        
    }
}
