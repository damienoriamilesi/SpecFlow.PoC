using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TechTalk.SpecFlow;

namespace SpecFlow.PoC.Tests;

[Binding]
public class SecondScenarioClass
{
    //private readonly HttpClient _client;
    //private readonly BddTestsApplicationFactory _factory = new();

    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly IProductService _service;
    private Product[]? Products { get; set; }

    public SecondScenarioClass()
    {
        //_client = _factory.CreateDefaultClient(new Uri($"http://localhost/"));
        _service = new ProductService(_productRepositoryMock.Object);
    }
    [Given(@"Database contains products :")]
    public async Task GivenDatabaseContainsProducts(Table table)
    {
        Products = await _service.GetAllProductsAsync();
    }

    [When(@"A call is made to '(.*)'")]
    public void WhenACallIsMadeTo(string p0)
    {
        ScenarioContext.StepIsPending();
    }

    [Then(@"The Result Should be :")]
    public void ThenTheResultShouldBe(Table table)
    {
        
    }
}

public class BddTestsApplicationFactory : WebApplicationFactory<Program>
{
    private readonly InMemoryProductRepository _productRepository = new InMemoryProductRepository();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped<IProductRepository>(_ => _productRepository);
        });

        builder.UseEnvironment("Development");
    }
}

public class InMemoryProductRepository : IProductRepository
{
    public Task<Product> GetProductByIdAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public async Task<Product[]> GetAllProductsAsync()
    {
        return await Task.FromResult(new Product[2]);
    }

    public Task InsertProductAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProductAsync(Product product)
    {
        throw new NotImplementedException();
    }
}

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(Guid productId);

    Task<Product[]> GetAllProductsAsync();
        
    Task InsertProductAsync(Product product);

    Task UpdateProductAsync(Product product);
}
public interface IProductService
{
    Task<Product[]> GetAllProductsAsync();
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<Product[]> GetAllProductsAsync()
    {
        return await _productRepository.GetAllProductsAsync();
    }
}

public abstract class Product
{
    protected Product(Guid productId, string name, string description, decimal price, int stock)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    private Guid ProductId { get; set; }

    private string Name { get; set; }

    private string Description { get; set; }

    private decimal Price { get; set; }

    private int Stock { get; set; }
}