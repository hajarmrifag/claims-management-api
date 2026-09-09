using Claims.Application.Customers;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> CreateCustomer(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateAsync(
            request,
            cancellationToken);

        return Created(
            $"/api/customers/{customer.Id}",
            customer);
    }
}
