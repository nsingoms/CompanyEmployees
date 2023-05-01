using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api")]
[ApiController]
public class RootController : ControllerBase
{
   private readonly LinkGenerator _linkGenerator;
   public RootController(LinkGenerator linkGenerator) => 
      _linkGenerator = linkGenerator;
}