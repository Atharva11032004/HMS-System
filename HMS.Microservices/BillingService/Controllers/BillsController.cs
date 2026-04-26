using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BillingService.Models;
using BillingService.Services;

namespace BillingService.Controllers;

[ApiController]
[Route("bills")]
[Authorize]
public class BillsController : ControllerBase
{
    private readonly BillingBusinessService _billingService;

    public BillsController(BillingBusinessService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBill([FromBody] CreateBillRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var bill = await _billingService.CreateBillAsync(request);
        return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBills()
    {
        var bills = await _billingService.GetAllBillsAsync();
        return Ok(bills);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBill(int id)
    {
        var bill = await _billingService.GetBillAsync(id);
        if (bill == null) return NotFound();
        return Ok(bill);
    }

    [HttpPost("{id}/lines")]
    public async Task<IActionResult> AddBillLine(int id, [FromBody] BillLineDto line)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _billingService.AddBillLineAsync(id, line);
        return Created();
    }
}

[ApiController]
[Route("payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly BillingBusinessService _billingService;

    public PaymentsController(BillingBusinessService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var payment = await _billingService.CreatePaymentAsync(request);
        return Created();
    }
}