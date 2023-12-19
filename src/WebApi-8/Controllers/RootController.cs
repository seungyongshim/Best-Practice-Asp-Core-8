using ClassLib_8.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using WebApi_8.Dto;

namespace WebApi_8.Controllers;

[ApiController]
[AuditLog]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpPost]
    public IActionResult PostAsync(HelloDto dto)
    {
        return Ok(dto);
    }
}
