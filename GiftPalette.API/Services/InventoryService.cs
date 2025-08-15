using GiftPalette.API.Models;

namespace GiftPalette.API.Services;

public interface IInventoryService
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product?> GetProductAsync(int id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<bool> UpdateStockAsync(int productId, int quantity);
    Task<bool> IsInStockAsync(int productId, int quantity);
}

public class InventoryService : IInventoryService
{
    private readonly List<Product> _products;

    public InventoryService()
    {
        _products = InitializeSampleProducts();
    }

    public Task<IEnumerable<Product>> GetProductsAsync()
    {
        return Task.FromResult(_products.Where(p => p.IsAvailable).AsEnumerable());
    }

    public Task<Product?> GetProductAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id && p.IsAvailable);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        var products = _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase) && p.IsAvailable);
        return Task.FromResult(products);
    }

    public Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product == null || product.Stock < quantity)
            return Task.FromResult(false);

        product.Stock -= quantity;
        if (product.Stock <= 0)
            product.IsAvailable = false;

        return Task.FromResult(true);
    }

    public Task<bool> IsInStockAsync(int productId, int quantity)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        return Task.FromResult(product?.Stock >= quantity && product.IsAvailable);
    }

    private static List<Product> InitializeSampleProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "可愛いピンクのマグカップ",
                Description = "お気に入りの飲み物を楽しむためのキュートなピンクマグカップ。毎日の癒しタイムにぴったり♪",
                Price = 1980,
                ImageUrl = "https://via.placeholder.com/300x300/FFB6C1/FFFFFF?text=ピンクマグカップ",
                Category = "キッチン用品",
                Stock = 15
            },
            new Product
            {
                Id = 2,
                Name = "ローズゴールドネックレス",
                Description = "エレガントなローズゴールドのハートネックレス。大切な人への特別なギフトに。",
                Price = 8900,
                ImageUrl = "https://via.placeholder.com/300x300/E6C2A6/FFFFFF?text=ローズゴールド",
                Category = "アクセサリー",
                Stock = 8
            },
            new Product
            {
                Id = 3,
                Name = "アロマキャンドルセット",
                Description = "リラックス効果抜群のラベンダー&バニラの香りのキャンドルセット。癒しの時間をプレゼント。",
                Price = 3500,
                ImageUrl = "https://via.placeholder.com/300x300/E6E6FA/FFFFFF?text=アロマキャンドル",
                Category = "美容・リラックス",
                Stock = 20
            },
            new Product
            {
                Id = 4,
                Name = "花柄ハンドクリームセット",
                Description = "しっとり潤う花の香りのハンドクリーム3本セット。手肌に優しい天然成分配合。",
                Price = 2400,
                ImageUrl = "https://via.placeholder.com/300x300/FFF0F5/FFFFFF?text=ハンドクリーム",
                Category = "美容・リラックス",
                Stock = 25
            },
            new Product
            {
                Id = 5,
                Name = "パステルカラーブランケット",
                Description = "ふわふわで暖かいパステルピンクのブランケット。読書タイムやリラックスタイムに。",
                Price = 4800,
                ImageUrl = "https://via.placeholder.com/300x300/FFE4E1/FFFFFF?text=ブランケット",
                Category = "インテリア",
                Stock = 12
            },
            new Product
            {
                Id = 6,
                Name = "ユニコーンスリッパ",
                Description = "キュートなユニコーンデザインのふわふわスリッパ。お家時間を楽しく彩ります。",
                Price = 2200,
                ImageUrl = "https://via.placeholder.com/300x300/DDA0DD/FFFFFF?text=ユニコーン",
                Category = "ファッション",
                Stock = 18
            },
            new Product
            {
                Id = 7,
                Name = "桜柄手鏡セット",
                Description = "美しい桜模様の手鏡とポーチのセット。持ち歩きに便利で実用的なギフト。",
                Price = 3200,
                ImageUrl = "https://via.placeholder.com/300x300/FFC0CB/FFFFFF?text=桜手鏡",
                Category = "美容・リラックス",
                Stock = 14
            },
            new Product
            {
                Id = 8,
                Name = "キラキラ星座ノート",
                Description = "夜空の星座が美しくデザインされたキラキラ表紙のノート。日記や思い出の記録に。",
                Price = 1600,
                ImageUrl = "https://via.placeholder.com/300x300/191970/FFFFFF?text=星座ノート",
                Category = "ステーショナリー",
                Stock = 30
            }
        };
    }
}