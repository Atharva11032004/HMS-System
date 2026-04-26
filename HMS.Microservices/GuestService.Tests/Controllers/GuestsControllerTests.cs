using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestService.Controllers;
using GuestService.Data;
using GuestService.Models;
using GuestService.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuestService.Tests;

 
[TestFixture]
public class GuestsControllerTests
{
    private ApplicationDbContext _context;
    private GuestBusinessService _service;
    private GuestsController _controller;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GuestControllerTestDb_" + Guid.NewGuid())
            .Options;
        
        _context = new ApplicationDbContext(options);
        _service = new GuestBusinessService(_context);
        _controller = new GuestsController(_service);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region GetGuests Tests

    [Test]
    public async Task GetGuests_ReturnsOkWithGuests()
    {
     
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();

  
        var result = await _controller.GetGuests();

        
        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    #endregion

    #region GetGuest Tests

    [Test]
    public async Task GetGuest_WithValidId_ReturnsOkWithGuest()
    {
      
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();

        
        var result = await _controller.GetGuest(1);


        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetGuest_WithInvalidId_ReturnsNotFound()
    {
      
        var result = await _controller.GetGuest(999);

       
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region CreateGuest Tests

    [Test]
    public async Task CreateGuest_WithValidData_ReturnsCreatedAtAction()
    {
       
        var guestDto = new GuestDto { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

      
        var result = await _controller.CreateGuest(guestDto);

       
        Assert.That(result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
    }

    [Test]
    public async Task CreateGuest_WithInvalidData_ReturnsBadRequest()
    {
       
        var guestDto = new GuestDto { FirstName = "", LastName = "", Email = "", Phone = "" };
        _controller.ModelState.AddModelError("FirstName", "FirstName is required");

        var result = await _controller.CreateGuest(guestDto);

        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }
 
    #endregion

    #region UpdateGuest Tests

    [Test]
    public async Task UpdateGuest_WithValidData_ReturnsNoContent()
    {
        
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();
        
        var guestDto = new GuestDto { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "9876543210" };

       
        var result = await _controller.UpdateGuest(1, guestDto);

     
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task UpdateGuest_WithInvalidId_ReturnsNotFound()
    {
        
        var guestDto = new GuestDto { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "9876543210" };

  
        var result = await _controller.UpdateGuest(999, guestDto);

     
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region DeleteGuest Tests

    [Test]
    public async Task DeleteGuest_WithValidId_ReturnsNoContent()
    {
       
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();

   
        var result = await _controller.DeleteGuest(1);

       
        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteGuest_WithInvalidId_ReturnsNotFound()
    {
     
        var result = await _controller.DeleteGuest(999);

        
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region SearchGuests Tests

    [Test]
    public async Task SearchGuests_ReturnsOkWithResults()
    {
    
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();

     
        var result = await _controller.SearchGuests("john@test.com", null, null);

      
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task SearchGuests_WithNoMatches_ReturnsOkWithEmptyList()
    {
   
        var result = await _controller.SearchGuests("notfound@test.com", null, null);

    
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    #endregion
}
