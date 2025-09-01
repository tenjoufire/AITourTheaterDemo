using GiftPalette.API.Services;
using GiftPalette.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddSingleton<ICartService, CartService>();

// Add CORS for local development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// Inventory API endpoints
app.MapGet("/api/products", async (IInventoryService inventoryService) =>
{
    var products = await inventoryService.GetProductsAsync();
    return Results.Ok(products);
})
.WithName("GetProducts")
.WithOpenApi();

app.MapGet("/api/products/{id:int}", async (int id, IInventoryService inventoryService) =>
{
    var product = await inventoryService.GetProductAsync(id);
    return product != null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProduct")
.WithOpenApi();

app.MapGet("/api/products/category/{category}", async (string category, IInventoryService inventoryService) =>
{
    var products = await inventoryService.GetProductsByCategoryAsync(category);
    return Results.Ok(products);
})
.WithName("GetProductsByCategory")
.WithOpenApi();

app.MapPost("/api/inventory/check", async (CheckStockRequest request, IInventoryService inventoryService) =>
{
    var isInStock = await inventoryService.IsInStockAsync(request.ProductId, request.Quantity);
    return Results.Ok(new { IsInStock = isInStock });
})
.WithName("CheckStock")
.WithOpenApi();

// Cart ID issue endpoint (demo: fixed cart ID)
app.MapGet("/api/cart/issue", () =>
{
    var issued = new IssueCartIdResponse("demo-cart-001");
    return Results.Ok(issued);
})
.WithName("IssueCartId")
.WithOpenApi();

// Cart API endpoints
app.MapGet("/api/cart/{cartId}", async (string cartId, ICartService cartService) =>
{
    var cart = await cartService.GetCartAsync(cartId);
    return Results.Ok(cart);
})
.WithName("GetCart")
.WithOpenApi();

app.MapPost("/api/cart/{cartId}/add", async (string cartId, AddToCartRequest request, ICartService cartService) =>
{
    var cart = await cartService.AddToCartAsync(cartId, request.ProductId, request.Quantity);
    return Results.Ok(cart);
})
.WithName("AddToCart")
.WithOpenApi();

app.MapPut("/api/cart/{cartId}/update", async (string cartId, UpdateCartRequest request, ICartService cartService) =>
{
    var cart = await cartService.UpdateCartItemAsync(cartId, request.ProductId, request.Quantity);
    return Results.Ok(cart);
})
.WithName("UpdateCartItem")
.WithOpenApi();

app.MapDelete("/api/cart/{cartId}/remove/{productId:int}", async (string cartId, int productId, ICartService cartService) =>
{
    var cart = await cartService.RemoveFromCartAsync(cartId, productId);
    return Results.Ok(cart);
})
.WithName("RemoveFromCart")
.WithOpenApi();

app.MapDelete("/api/cart/{cartId}/clear", async (string cartId, ICartService cartService) =>
{
    var success = await cartService.ClearCartAsync(cartId);
    return Results.Ok(new { Success = success });
})
.WithName("ClearCart")
.WithOpenApi();

app.Run();

// Request/Response DTOs
public record CheckStockRequest(int ProductId, int Quantity);
public record AddToCartRequest(int ProductId, int Quantity);
public record UpdateCartRequest(int ProductId, int Quantity);
public record IssueCartIdResponse(string CartId);
