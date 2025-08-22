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
                ImageUrl = "/images/001.png",
                Category = "キッチン用品",
                Stock = 15
            },
            new Product
            {
                Id = 2,
                Name = "ローズゴールドネックレス",
                Description = "エレガントなローズゴールドのハートネックレス。大切な人への特別なギフトに。",
                Price = 8900,
                ImageUrl = "/images/002.png",
                Category = "アクセサリー",
                Stock = 8
            },
            new Product
            {
                Id = 3,
                Name = "アロマキャンドルセット",
                Description = "リラックス効果抜群のラベンダー&バニラの香りのキャンドルセット。癒しの時間をプレゼント。",
                Price = 3500,
                ImageUrl = "/images/003.png",
                Category = "美容・リラックス",
                Stock = 20
            },
            new Product
            {
                Id = 4,
                Name = "花柄ハンドクリームセット",
                Description = "しっとり潤う花の香りのハンドクリーム3本セット。手肌に優しい天然成分配合。",
                Price = 2400,
                ImageUrl = "/images/004.png",
                Category = "美容・リラックス",
                Stock = 25
            },
            new Product
            {
                Id = 5,
                Name = "パステルカラーブランケット",
                Description = "ふわふわで暖かいパステルピンクのブランケット。読書タイムやリラックスタイムに。",
                Price = 4800,
                ImageUrl = "/images/005.png",
                Category = "インテリア",
                Stock = 12
            },
            new Product
            {
                Id = 6,
                Name = "ユニコーンスリッパ",
                Description = "キュートなユニコーンデザインのふわふわスリッパ。お家時間を楽しく彩ります。",
                Price = 2200,
                ImageUrl = "/images/006.png",
                Category = "ファッション",
                Stock = 18
            },
            new Product
            {
                Id = 7,
                Name = "桜柄手鏡セット",
                Description = "美しい桜模様の手鏡とポーチのセット。持ち歩きに便利で実用的なギフト。",
                Price = 3200,
                ImageUrl = "/images/007.png",
                Category = "美容・リラックス",
                Stock = 14
            },
            new Product
            {
                Id = 8,
                Name = "キラキラ星座ノート",
                Description = "夜空の星座が美しくデザインされたキラキラ表紙のノート。日記や思い出の記録に。",
                Price = 1600,
                ImageUrl = "/images/008.png",
                Category = "ステーショナリー",
                Stock = 30
            },
            // 新しい商品ライン（ID 9-48）- 幅広い年代・性別向け
            new Product
            {
                Id = 9,
                Name = "レザービジネス手帳",
                Description = "上質な本革製のビジネス手帳。仕事の効率化とスタイルを両立できる実用的なアイテム。",
                Price = 8500,
                ImageUrl = "/images/009.png",
                Category = "ステーショナリー",
                Stock = 15
            },
            new Product
            {
                Id = 10,
                Name = "多機能スマートウォッチ",
                Description = "健康管理から通知確認まで、様々な機能を搭載したスマートウォッチ。忙しい日常をサポート。",
                Price = 15800,
                ImageUrl = "/images/010.png",
                Category = "テクノロジー",
                Stock = 10
            },
            new Product
            {
                Id = 11,
                Name = "高級コーヒーセット",
                Description = "世界各地の厳選されたコーヒー豆3種類のセット。コーヒー愛好家への特別な贈り物。",
                Price = 4200,
                ImageUrl = "/images/011.png",
                Category = "キッチン用品",
                Stock = 20
            },
            new Product
            {
                Id = 12,
                Name = "フィットネストラッカー",
                Description = "歩数・心拍数・睡眠を記録する軽量フィットネストラッカー。健康的な生活をサポート。",
                Price = 7200,
                ImageUrl = "/images/012.png",
                Category = "スポーツ・フィットネス",
                Stock = 25
            },
            new Product
            {
                Id = 13,
                Name = "アロマディフューザー",
                Description = "リビングや寝室に最適な超音波式アロマディフューザー。癒しの香りで空間を演出。",
                Price = 5400,
                ImageUrl = "/images/013.png",
                Category = "美容・リラックス",
                Stock = 18
            },
            new Product
            {
                Id = 14,
                Name = "ブルートゥーススピーカー",
                Description = "コンパクトながら高音質なワイヤレススピーカー。アウトドアでも室内でも楽しめる。",
                Price = 6800,
                ImageUrl = "/images/014.png",
                Category = "テクノロジー",
                Stock = 22
            },
            new Product
            {
                Id = 15,
                Name = "料理本とエプロンセット",
                Description = "初心者向け料理本と機能的なエプロンのセット。料理を始めたい方への応援ギフト。",
                Price = 3800,
                ImageUrl = "/images/015.png",
                Category = "キッチン用品",
                Stock = 16
            },
            new Product
            {
                Id = 16,
                Name = "ヨガマット",
                Description = "滑り止め加工された高品質なヨガマット。初心者から上級者まで快適に使える。",
                Price = 4500,
                ImageUrl = "/images/016.png",
                Category = "スポーツ・フィットネス",
                Stock = 20
            },
            new Product
            {
                Id = 17,
                Name = "読書灯付きブックスタンド",
                Description = "角度調整可能な読書スタンドとLEDライトのセット。快適な読書時間を提供。",
                Price = 2900,
                ImageUrl = "/images/017.png",
                Category = "インテリア",
                Stock = 24
            },
            new Product
            {
                Id = 18,
                Name = "ワイヤレス充電器",
                Description = "置くだけで充電できるスタイリッシュなワイヤレス充電パッド。デスクをすっきり整理。",
                Price = 3200,
                ImageUrl = "/images/018.png",
                Category = "テクノロジー",
                Stock = 30
            },
            new Product
            {
                Id = 19,
                Name = "保温タンブラー",
                Description = "オフィスでも外出先でも温度をキープする高性能タンブラー。忙しい毎日の相棒に。",
                Price = 2800,
                ImageUrl = "/images/019.png",
                Category = "キッチン用品",
                Stock = 35
            },
            new Product
            {
                Id = 20,
                Name = "デスク整理トレー",
                Description = "竹素材の美しいデスク整理トレー。文房具や小物をスマートに収納できる。",
                Price = 2100,
                ImageUrl = "/images/020.png",
                Category = "インテリア",
                Stock = 28
            },
            new Product
            {
                Id = 21,
                Name = "メンズシルバーネックレス",
                Description = "シンプルで上品なシルバーチェーンネックレス。ビジネスからカジュアルまで対応。",
                Price = 9800,
                ImageUrl = "/images/021.png",
                Category = "アクセサリー",
                Stock = 12
            },
            new Product
            {
                Id = 22,
                Name = "ガーデニングツールセット",
                Description = "初心者向けガーデニング道具3点セット。植物を育てる楽しさを始められる。",
                Price = 3600,
                ImageUrl = "/images/022.png",
                Category = "ホビー・ガーデニング",
                Stock = 14
            },
            new Product
            {
                Id = 23,
                Name = "電子書籍リーダー",
                Description = "目に優しいE-inkディスプレイ搭載の電子書籍リーダー。読書好きへの最適ギフト。",
                Price = 12800,
                ImageUrl = "/images/023.png",
                Category = "テクノロジー",
                Stock = 8
            },
            new Product
            {
                Id = 24,
                Name = "アロマ入浴剤セット",
                Description = "天然精油を使用した5種類の入浴剤セット。一日の疲れを癒すバスタイムに。",
                Price = 2400,
                ImageUrl = "/images/024.png",
                Category = "美容・リラックス",
                Stock = 40
            },
            new Product
            {
                Id = 25,
                Name = "ボードゲームセット",
                Description = "家族や友人と楽しめる戦略ボードゲーム。コミュニケーションを深める時間を提供。",
                Price = 4800,
                ImageUrl = "/images/025.png",
                Category = "ホビー・ガーデニング",
                Stock = 15
            },
            new Product
            {
                Id = 26,
                Name = "キャンプ用LEDランタン",
                Description = "アウトドア活動に最適な充電式LEDランタン。災害時の備えとしても活用できる。",
                Price = 3900,
                ImageUrl = "/images/026.png",
                Category = "スポーツ・フィットネス",
                Stock = 18
            },
            new Product
            {
                Id = 27,
                Name = "高級万年筆",
                Description = "なめらかな書き心地の高級万年筆。重要な書類や特別な手紙に格式ある一本。",
                Price = 15000,
                ImageUrl = "/images/027.png",
                Category = "ステーショナリー",
                Stock = 6
            },
            new Product
            {
                Id = 28,
                Name = "マッサージクッション",
                Description = "肩や腰の疲れをほぐす電動マッサージクッション。在宅ワークの疲労回復に。",
                Price = 8900,
                ImageUrl = "/images/028.png",
                Category = "美容・リラックス",
                Stock = 10
            },
            new Product
            {
                Id = 29,
                Name = "釣り具ビギナーセット",
                Description = "釣りを始めたい方向けの基本道具セット。週末のリフレッシュタイムに最適。",
                Price = 6500,
                ImageUrl = "/images/029.png",
                Category = "スポーツ・フィットネス",
                Stock = 12
            },
            new Product
            {
                Id = 30,
                Name = "ワイングラスセット",
                Description = "エレガントなクリスタルワイングラス2脚セット。特別な日の乾杯を華やかに。",
                Price = 5200,
                ImageUrl = "/images/030.png",
                Category = "キッチン用品",
                Stock = 8
            },
            new Product
            {
                Id = 31,
                Name = "写真立てセット",
                Description = "様々なサイズの写真立て3点セット。大切な思い出を美しく飾れる。",
                Price = 2700,
                ImageUrl = "/images/031.png",
                Category = "インテリア",
                Stock = 22
            },
            new Product
            {
                Id = 32,
                Name = "テニスラケット",
                Description = "初心者から中級者向けの軽量テニスラケット。健康的なスポーツライフの始まりに。",
                Price = 9200,
                ImageUrl = "/images/032.png",
                Category = "スポーツ・フィットネス",
                Stock = 7
            },
            new Product
            {
                Id = 33,
                Name = "キャンドルメイキングキット",
                Description = "自分だけのオリジナルキャンドルが作れるDIYキット。創作の楽しさを体験。",
                Price = 3400,
                ImageUrl = "/images/033.png",
                Category = "ホビー・ガーデニング",
                Stock = 16
            },
            new Product
            {
                Id = 34,
                Name = "スマートフォンスタンド",
                Description = "角度調整可能な高品質スマートフォンスタンド。動画視聴やビデオ通話に最適。",
                Price = 1800,
                ImageUrl = "/images/034.png",
                Category = "テクノロジー",
                Stock = 45
            },
            new Product
            {
                Id = 35,
                Name = "ハーブティーセット",
                Description = "リラックス効果のある5種類のハーブティーセット。心身の疲れを優しく癒す。",
                Price = 2800,
                ImageUrl = "/images/035.png",
                Category = "美容・リラックス",
                Stock = 30
            },
            new Product
            {
                Id = 36,
                Name = "レザーキーホルダー",
                Description = "上質な革製のシンプルなキーホルダー。使うほどに味が出る長く愛用できるアイテム。",
                Price = 2200,
                ImageUrl = "/images/036.png",
                Category = "ファッション",
                Stock = 38
            },
            new Product
            {
                Id = 37,
                Name = "ジグソーパズル",
                Description = "美しい風景の1000ピースジグソーパズル。集中力と達成感を味わえるホビーアイテム。",
                Price = 2500,
                ImageUrl = "/images/037.png",
                Category = "ホビー・ガーデニング",
                Stock = 20
            },
            new Product
            {
                Id = 38,
                Name = "バックパック",
                Description = "通勤・通学・アウトドアに使える多機能バックパック。耐久性と機能性を兼ね備えている。",
                Price = 7800,
                ImageUrl = "/images/038.png",
                Category = "ファッション",
                Stock = 14
            },
            new Product
            {
                Id = 39,
                Name = "お香セット",
                Description = "天然素材の上質なお香10本セット。瞑想や集中したい時間に心を落ち着かせる。",
                Price = 1900,
                ImageUrl = "/images/039.png",
                Category = "美容・リラックス",
                Stock = 35
            },
            new Product
            {
                Id = 40,
                Name = "デジタルフォトフレーム",
                Description = "Wi-Fi対応のデジタルフォトフレーム。離れた家族と写真を共有できる現代的なギフト。",
                Price = 11500,
                ImageUrl = "/images/040.png",
                Category = "テクノロジー",
                Stock = 9
            },
            new Product
            {
                Id = 41,
                Name = "バスローブ",
                Description = "肌触りの良いコットン100%のバスローブ。バスタイム後の贅沢なリラックスタイムに。",
                Price = 6200,
                ImageUrl = "/images/041.png",
                Category = "美容・リラックス",
                Stock = 12
            },
            new Product
            {
                Id = 42,
                Name = "木製時計",
                Description = "天然木を使用したシンプルで温かみのある掛け時計。どんなインテリアにもマッチ。",
                Price = 4600,
                ImageUrl = "/images/042.png",
                Category = "インテリア",
                Stock = 18
            },
            new Product
            {
                Id = 43,
                Name = "スケッチブック＆鉛筆セット",
                Description = "高品質なスケッチブックと鉛筆セット。絵を描く楽しさを始められるアートキット。",
                Price = 2300,
                ImageUrl = "/images/043.png",
                Category = "ステーショナリー",
                Stock = 26
            },
            new Product
            {
                Id = 44,
                Name = "スポーツタオルセット",
                Description = "吸水性抜群のマイクロファイバースポーツタオル3枚セット。ジムや旅行に最適。",
                Price = 2900,
                ImageUrl = "/images/044.png",
                Category = "スポーツ・フィットネス",
                Stock = 32
            },
            new Product
            {
                Id = 45,
                Name = "ルームフレグランス",
                Description = "上品な香りが長時間続くリードディフューザー。お部屋の雰囲気を格上げする。",
                Price = 3700,
                ImageUrl = "/images/045.png",
                Category = "美容・リラックス",
                Stock = 24
            },
            new Product
            {
                Id = 46,
                Name = "ポータブルチェア",
                Description = "軽量でコンパクトに折りたためるアウトドアチェア。キャンプやピクニックに最適。",
                Price = 5400,
                ImageUrl = "/images/046.png",
                Category = "スポーツ・フィットネス",
                Stock = 11
            },
            new Product
            {
                Id = 47,
                Name = "カードゲームセット",
                Description = "定番から新しいルールまで楽しめるカードゲーム3種セット。世代を超えて楽しめる。",
                Price = 2600,
                ImageUrl = "/images/047.png",
                Category = "ホビー・ガーデニング",
                Stock = 28
            },
            new Product
            {
                Id = 48,
                Name = "プランターセット",
                Description = "おしゃれなセラミック製プランター3点セット。室内ガーデニングを気軽に始められる。",
                Price = 4100,
                ImageUrl = "/images/048.png",
                Category = "ホビー・ガーデニング",
                Stock = 16
            }
        };
    }
}