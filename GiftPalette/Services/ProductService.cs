using GiftPalette.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GiftPalette.Services;

public interface IProductService
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductAsync(int id);
    Task<List<Product>> GetProductsByCategoryAsync(string category);
}

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _baseUrl;

    public ProductService(HttpClient httpClient, IOptions<ApiConfiguration> apiConfig)
    {
        _httpClient = httpClient;
        _baseUrl = apiConfig.Value.BaseUrl;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(json, _jsonOptions);
                return products ?? new List<Product>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching products: {ex.Message}");
        }
        
        return new List<Product>();
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Product>(json, _jsonOptions);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching product {id}: {ex.Message}");
        }
        
        return null;
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/products/category/{category}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(json, _jsonOptions);
                return products ?? new List<Product>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching products by category {category}: {ex.Message}");
        }
        
        return new List<Product>();
    }
}