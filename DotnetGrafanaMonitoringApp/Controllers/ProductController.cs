using Bogus;
using DotnetGrafanaMonitoringApp.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DotnetGrafanaMonitoringApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    [HttpGet("get-all")]
    public IActionResult GetAll()
    {
        int idCounter = 0;

        var faker = new Faker<Product>()
            .RuleFor(x => x.Id, f =>
            {
                idCounter++;

                return idCounter;
            })
            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
            .RuleFor(x => x.Price, f => f.Random.Decimal(0, 9999))
            .Generate(10);

        return Ok(faker);
    }
}
