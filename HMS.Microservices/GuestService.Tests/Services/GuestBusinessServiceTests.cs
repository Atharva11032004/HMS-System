using NUnit.Framework;
using Moq;
using Microsoft.EntityFrameworkCore;
using GuestService.Data;
using GuestService.Models;
using GuestService.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace GuestService.Tests;


[TestFixture]
public class GuestBusinessServiceTests
{
    private ApplicationDbContext _context;
    private GuestBusinessService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "GuestServiceTestDb_" + Guid.NewGuid())
            .Options;
        
        _context = new ApplicationDbContext(options);
        _service = new GuestBusinessService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    #region GetGuestsAsync Tests

    [Test]
    public async Task GetGuestsAsync_ReturnsAllGuests()
    {
      
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.Guests.Add(new Guest { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "0987654321" });
        _context.SaveChanges();

     
        var result = await _service.GetGuestsAsync();

       
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].FirstName, Is.EqualTo("John"));
        Assert.That(result[1].FirstName, Is.EqualTo("Jane"));
    }

    [Test]
    public async Task GetGuestsAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
   
        var result = await _service.GetGuestsAsync();

   
        Assert.That(result.Count, Is.EqualTo(0));
    }

    #endregion

    #region GetGuestAsync Tests

    [Test]
    public async Task GetGuestAsync_WithValidId_ReturnsGuest()
    {
       
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();

         
        var result = await _service.GetGuestAsync(1);

        
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("John"));
    }

    [Test]
    public async Task GetGuestAsync_WithInvalidId_ReturnsNull()
    {
      
        var result = await _service.GetGuestAsync(999);

        
        Assert.That(result, Is.Null);
    }

    #endregion

    #region CreateGuestAsync Tests

    [Test]
    public async Task CreateGuestAsync_CreatesNewGuest()
    {
    
        var guestDto = new GuestDto { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

       
        var result = await _service.CreateGuestAsync(guestDto);

    
        Assert.That(result.FirstName, Is.EqualTo("John"));
        Assert.That(_context.Guests.Count(), Is.EqualTo(1));
    }

    #endregion

    #region UpdateGuestAsync Tests

    [Test]
    public async Task UpdateGuestAsync_WithValidId_UpdatesGuest()
    {
       
 

        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();
        
        var updateDto = new GuestDto { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "9876543210" };

       
        var result = await _service.UpdateGuestAsync(1, updateDto);

       
        Assert.That(result, Is.True);
        var updated = _context.Guests.Find(1);
        Assert.That(updated.FirstName, Is.EqualTo("Jane"));
    }

    [Test]
    public async Task UpdateGuestAsync_WithInvalidId_ReturnsFalse()
    {
       
        var updateDto = new GuestDto { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "9876543210" };

     
        var result = await _service.UpdateGuestAsync(999, updateDto);

   
        Assert.That(result, Is.False);
    }

    #endregion

    #region DeleteGuestAsync Tests

    [Test]
    public async Task DeleteGuestAsync_WithValidId_DeletesGuest()
    {
        
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.SaveChanges();

        
        var result = await _service.DeleteGuestAsync(1);

        
        Assert.That(result, Is.True);
        Assert.That(_context.Guests.Find(1), Is.Null);
    }

    [Test]
    public async Task DeleteGuestAsync_WithInvalidId_ReturnsFalse()
    {
       
        var result = await _service.DeleteGuestAsync(999);

        
        Assert.That(result, Is.False);
    }

    #endregion

    #region SearchGuestsAsync Tests

    [Test]
    public async Task SearchGuestsAsync_ByEmail_ReturnsMatchingGuests()
    {
        
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.Guests.Add(new Guest { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "0987654321" });
        _context.SaveChanges();

        
        var result = await _service.SearchGuestsAsync("john@test.com", null, null);

        
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Email, Is.EqualTo("john@test.com"));
    }

    [Test]
    public async Task SearchGuestsAsync_ByPhone_ReturnsMatchingGuests()
    {
        
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.Guests.Add(new Guest { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "0987654321" });
        _context.SaveChanges();

        
        var result = await _service.SearchGuestsAsync(null, "1234567890", null);

        
        Assert.That(result.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task SearchGuestsAsync_ByName_ReturnsMatchingGuests()
    {
        
        _context.Guests.Add(new Guest { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" });
        _context.Guests.Add(new Guest { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Phone = "0987654321" });
        _context.SaveChanges();

        
        var result = await _service.SearchGuestsAsync(null, null, "John");

        
        Assert.That(result.Count, Is.EqualTo(1));
    }

    #endregion
}
