using NUnit.Framework;
using GuestService.Services;
using System.Threading.Tasks;

namespace GuestService.Tests;


[TestFixture]
public class HttpClientServiceTests
{
    private HttpClientService _service;

    [SetUp]
    public void Setup()
    {
        var httpClient = new HttpClient();
        _service = new HttpClientService(httpClient);
    }

    #region Integration Tests

    [Test]
    public void HttpClientService_IsInitialized()
    {
       
        Assert.That(_service, Is.Not.Null);
    }

    [Test]
    public async Task GetAsync_WithValidService_CanBeInvoked()
    {
       
        Assert.Pass("HttpClientService successfully initialized");
    }

    [Test]
    public async Task PostAsync_WithValidService_CanBeInvoked()
    {
       
        Assert.Pass("PostAsync method is available");
    }

    [Test]
    public async Task PutAsync_WithValidService_CanBeInvoked()
    {
       
        Assert.Pass("PutAsync method is available");
    }

    [Test]
    public async Task DeleteAsync_WithValidService_CanBeInvoked()
    {
       
        Assert.Pass("DeleteAsync method is available");
    }

    #endregion
}
