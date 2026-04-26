using NUnit.Framework;
using FluentValidation;
using GuestService.Models;
using GuestService.Validators;
using System.Threading.Tasks;

namespace GuestService.Tests;


[TestFixture]
public class GuestValidatorTests
{
    private GuestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new GuestValidator();
    }

    #region FirstName Validation Tests

    [Test]
    public async Task FirstName_WhenEmpty_FailsValidation()
    {
      
        var guest = new Guest { FirstName = "", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

      
        var result = await _validator.ValidateAsync(guest);

       
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task FirstName_WhenValid_PassesValidation()
    {
    
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

   
        var result = await _validator.ValidateAsync(guest);

   
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task FirstName_ExceedsMaxLength_FailsValidation()
    {
     
        var guest = new Guest { FirstName = new string('A', 101), LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

      
        var result = await _validator.ValidateAsync(guest);

      
        Assert.That(result.IsValid, Is.False);
    }

    #endregion

    #region LastName Validation Tests

    [Test]
    public async Task LastName_WhenEmpty_FailsValidation()
    {
      
        var guest = new Guest { FirstName = "John", LastName = "", Email = "john@test.com", Phone = "1234567890" };

     
        var result = await _validator.ValidateAsync(guest);

       
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task LastName_WhenValid_PassesValidation()
    {
      
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

     
        var result = await _validator.ValidateAsync(guest);

     
        Assert.That(result.IsValid, Is.True);
    }

    #endregion

    #region Email Validation Tests

    [Test]
    public async Task Email_WhenEmpty_FailsValidation()
    {
      
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "", Phone = "1234567890" };

       
        var result = await _validator.ValidateAsync(guest);

   
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task Email_WithInvalidFormat_FailsValidation()
    {
    
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "invalidemail", Phone = "1234567890" };

       
        var result = await _validator.ValidateAsync(guest);

      
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task Email_WithValidFormat_PassesValidation()
    {
       
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

       
        var result = await _validator.ValidateAsync(guest);

       
        Assert.That(result.IsValid, Is.True);
    }

    #endregion

    #region Phone Validation Tests

    [Test]
    public async Task Phone_WhenEmpty_FailsValidation()
    {
      
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "" };

       
        var result = await _validator.ValidateAsync(guest);

       
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task Phone_WithValidFormat_PassesValidation()
    {
       
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "1234567890" };

    
        var result = await _validator.ValidateAsync(guest);

      
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task Phone_WithInternationalFormat_PassesValidation()
    {
       
        var guest = new Guest { FirstName = "John", LastName = "Doe", Email = "john@test.com", Phone = "+1 (123) 456-7890" };

       
        var result = await _validator.ValidateAsync(guest);

       
        Assert.That(result.IsValid, Is.True);
    }

    #endregion

    #region Complete Guest Validation Tests

    [Test]
    public async Task CompleteGuest_WithAllValidData_PassesValidation()
    {
      
        var guest = new Guest 
        { 
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@test.com", 
            Phone = "1234567890" 
        };

     
        var result = await _validator.ValidateAsync(guest);

       
        Assert.That(result.IsValid, Is.True);
    }

    #endregion
}


[TestFixture]
public class GuestDtoValidatorTests
{
    private GuestDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new GuestDtoValidator();
    }

    #region DTO Validation Tests

    [Test]
    public async Task GuestDto_WithValidData_PassesValidation()
    {
       
        var guestDto = new GuestDto 
        { 
            Id = 1,
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@test.com", 
            Phone = "1234567890" 
        };

      
        var result = await _validator.ValidateAsync(guestDto);

      
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task GuestDto_WithInvalidEmail_FailsValidation()
    {
    
        var guestDto = new GuestDto 
        { 
            Id = 1,
            FirstName = "John", 
            LastName = "Doe", 
            Email = "invalidemail", 
            Phone = "1234567890" 
        };

      
        var result = await _validator.ValidateAsync(guestDto);

     
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task GuestDto_WithEmptyFirstName_FailsValidation()
    {
       
        var guestDto = new GuestDto 
        { 
            Id = 1,
            FirstName = "", 
            LastName = "Doe", 
            Email = "john@test.com", 
            Phone = "1234567890" 
        };

    
        var result = await _validator.ValidateAsync(guestDto);

      
        Assert.That(result.IsValid, Is.False);
    }

    #endregion
}
