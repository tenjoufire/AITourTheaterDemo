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
            },
            // 新しい商品ライン（ID 9-48）- 幅広い年代・性別向け
            new Product
            {
                Id = 9,
                Name = "レザービジネス手帳",
                Description = "上質な本革製のビジネス手帳。仕事の効率化とスタイルを両立できる実用的なアイテム。",
                Price = 8500,
                ImageUrl = "https://via.placeholder.com/300x300/8B4513/FFFFFF?text=レザー手帳",
                Category = "ステーショナリー",
                Stock = 15
            },
            new Product
            {
                Id = 10,
                Name = "多機能スマートウォッチ",
                Description = "健康管理から通知確認まで、様々な機能を搭載したスマートウォッチ。忙しい日常をサポート。",
                Price = 15800,
                ImageUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=スマートウォッチ",
                Category = "テクノロジー",
                Stock = 10
            },
            new Product
            {
                Id = 11,
                Name = "高級コーヒーセット",
                Description = "世界各地の厳選されたコーヒー豆3種類のセット。コーヒー愛好家への特別な贈り物。",
                Price = 4200,
                ImageUrl = "https://via.placeholder.com/300x300/6F4E37/FFFFFF?text=コーヒーセット",
                Category = "キッチン用品",
                Stock = 20
            },
            new Product
            {
                Id = 12,
                Name = "フィットネストラッカー",
                Description = "歩数・心拍数・睡眠を記録する軽量フィットネストラッカー。健康的な生活をサポート。",
                Price = 7200,
                ImageUrl = "https://via.placeholder.com/300x300/FF6347/FFFFFF?text=フィットネス",
                Category = "スポーツ・フィットネス",
                Stock = 25
            },
            new Product
            {
                Id = 13,
                Name = "アロマディフューザー",
                Description = "リビングや寝室に最適な超音波式アロマディフューザー。癒しの香りで空間を演出。",
                Price = 5400,
                ImageUrl = "https://via.placeholder.com/300x300/DDA0DD/FFFFFF?text=ディフューザー",
                Category = "美容・リラックス",
                Stock = 18
            },
            new Product
            {
                Id = 14,
                Name = "ブルートゥーススピーカー",
                Description = "コンパクトながら高音質なワイヤレススピーカー。アウトドアでも室内でも楽しめる。",
                Price = 6800,
                ImageUrl = "https://via.placeholder.com/300x300/4682B4/FFFFFF?text=スピーカー",
                Category = "テクノロジー",
                Stock = 22
            },
            new Product
            {
                Id = 15,
                Name = "料理本とエプロンセット",
                Description = "初心者向け料理本と機能的なエプロンのセット。料理を始めたい方への応援ギフト。",
                Price = 3800,
                ImageUrl = "https://via.placeholder.com/300x300/FF4500/FFFFFF?text=料理セット",
                Category = "キッチン用品",
                Stock = 16
            },
            new Product
            {
                Id = 16,
                Name = "ヨガマット",
                Description = "滑り止め加工された高品質なヨガマット。初心者から上級者まで快適に使える。",
                Price = 4500,
                ImageUrl = "https://via.placeholder.com/300x300/32CD32/FFFFFF?text=ヨガマット",
                Category = "スポーツ・フィットネス",
                Stock = 20
            },
            new Product
            {
                Id = 17,
                Name = "読書灯付きブックスタンド",
                Description = "角度調整可能な読書スタンドとLEDライトのセット。快適な読書時間を提供。",
                Price = 2900,
                ImageUrl = "https://via.placeholder.com/300x300/696969/FFFFFF?text=読書スタンド",
                Category = "インテリア",
                Stock = 24
            },
            new Product
            {
                Id = 18,
                Name = "ワイヤレス充電器",
                Description = "置くだけで充電できるスタイリッシュなワイヤレス充電パッド。デスクをすっきり整理。",
                Price = 3200,
                ImageUrl = "https://via.placeholder.com/300x300/708090/FFFFFF?text=充電器",
                Category = "テクノロジー",
                Stock = 30
            },
            new Product
            {
                Id = 19,
                Name = "保温タンブラー",
                Description = "オフィスでも外出先でも温度をキープする高性能タンブラー。忙しい毎日の相棒に。",
                Price = 2800,
                ImageUrl = "https://via.placeholder.com/300x300/B22222/FFFFFF?text=タンブラー",
                Category = "キッチン用品",
                Stock = 35
            },
            new Product
            {
                Id = 20,
                Name = "デスク整理トレー",
                Description = "竹素材の美しいデスク整理トレー。文房具や小物をスマートに収納できる。",
                Price = 2100,
                ImageUrl = "https://via.placeholder.com/300x300/DEB887/FFFFFF?text=整理トレー",
                Category = "インテリア",
                Stock = 28
            },
            new Product
            {
                Id = 21,
                Name = "メンズシルバーネックレス",
                Description = "シンプルで上品なシルバーチェーンネックレス。ビジネスからカジュアルまで対応。",
                Price = 9800,
                ImageUrl = "https://via.placeholder.com/300x300/C0C0C0/FFFFFF?text=シルバーチェーン",
                Category = "アクセサリー",
                Stock = 12
            },
            new Product
            {
                Id = 22,
                Name = "ガーデニングツールセット",
                Description = "初心者向けガーデニング道具3点セット。植物を育てる楽しさを始められる。",
                Price = 3600,
                ImageUrl = "https://via.placeholder.com/300x300/9ACD32/FFFFFF?text=ガーデニング",
                Category = "ホビー・ガーデニング",
                Stock = 14
            },
            new Product
            {
                Id = 23,
                Name = "電子書籍リーダー",
                Description = "目に優しいE-inkディスプレイ搭載の電子書籍リーダー。読書好きへの最適ギフト。",
                Price = 12800,
                ImageUrl = "https://via.placeholder.com/300x300/2F2F2F/FFFFFF?text=電子書籍",
                Category = "テクノロジー",
                Stock = 8
            },
            new Product
            {
                Id = 24,
                Name = "アロマ入浴剤セット",
                Description = "天然精油を使用した5種類の入浴剤セット。一日の疲れを癒すバスタイムに。",
                Price = 2400,
                ImageUrl = "https://via.placeholder.com/300x300/E0E6F8/FFFFFF?text=入浴剤",
                Category = "美容・リラックス",
                Stock = 40
            },
            new Product
            {
                Id = 25,
                Name = "ボードゲームセット",
                Description = "家族や友人と楽しめる戦略ボードゲーム。コミュニケーションを深める時間を提供。",
                Price = 4800,
                ImageUrl = "https://via.placeholder.com/300x300/8B0000/FFFFFF?text=ボードゲーム",
                Category = "ホビー・ガーデニング",
                Stock = 15
            },
            new Product
            {
                Id = 26,
                Name = "キャンプ用LEDランタン",
                Description = "アウトドア活動に最適な充電式LEDランタン。災害時の備えとしても活用できる。",
                Price = 3900,
                ImageUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=ランタン",
                Category = "スポーツ・フィットネス",
                Stock = 18
            },
            new Product
            {
                Id = 27,
                Name = "高級万年筆",
                Description = "なめらかな書き心地の高級万年筆。重要な書類や特別な手紙に格式ある一本。",
                Price = 15000,
                ImageUrl = "https://via.placeholder.com/300x300/191970/FFFFFF?text=万年筆",
                Category = "ステーショナリー",
                Stock = 6
            },
            new Product
            {
                Id = 28,
                Name = "マッサージクッション",
                Description = "肩や腰の疲れをほぐす電動マッサージクッション。在宅ワークの疲労回復に。",
                Price = 8900,
                ImageUrl = "https://via.placeholder.com/300x300/8FBC8F/FFFFFF?text=マッサージ",
                Category = "美容・リラックス",
                Stock = 10
            },
            new Product
            {
                Id = 29,
                Name = "釣り具ビギナーセット",
                Description = "釣りを始めたい方向けの基本道具セット。週末のリフレッシュタイムに最適。",
                Price = 6500,
                ImageUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=釣り具",
                Category = "スポーツ・フィットネス",
                Stock = 12
            },
            new Product
            {
                Id = 30,
                Name = "ワイングラスセット",
                Description = "エレガントなクリスタルワイングラス2脚セット。特別な日の乾杯を華やかに。",
                Price = 5200,
                ImageUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=ワイングラス",
                Category = "キッチン用品",
                Stock = 8
            },
            new Product
            {
                Id = 31,
                Name = "写真立てセット",
                Description = "様々なサイズの写真立て3点セット。大切な思い出を美しく飾れる。",
                Price = 2700,
                ImageUrl = "https://via.placeholder.com/300x300/CD853F/FFFFFF?text=写真立て",
                Category = "インテリア",
                Stock = 22
            },
            new Product
            {
                Id = 32,
                Name = "テニスラケット",
                Description = "初心者から中級者向けの軽量テニスラケット。健康的なスポーツライフの始まりに。",
                Price = 9200,
                ImageUrl = "https://via.placeholder.com/300x300/FFD700/FFFFFF?text=テニス",
                Category = "スポーツ・フィットネス",
                Stock = 7
            },
            new Product
            {
                Id = 33,
                Name = "キャンドルメイキングキット",
                Description = "自分だけのオリジナルキャンドルが作れるDIYキット。創作の楽しさを体験。",
                Price = 3400,
                ImageUrl = "https://via.placeholder.com/300x300/FF69B4/FFFFFF?text=キャンドル",
                Category = "ホビー・ガーデニング",
                Stock = 16
            },
            new Product
            {
                Id = 34,
                Name = "スマートフォンスタンド",
                Description = "角度調整可能な高品質スマートフォンスタンド。動画視聴やビデオ通話に最適。",
                Price = 1800,
                ImageUrl = "https://via.placeholder.com/300x300/36454F/FFFFFF?text=スマホスタンド",
                Category = "テクノロジー",
                Stock = 45
            },
            new Product
            {
                Id = 35,
                Name = "ハーブティーセット",
                Description = "リラックス効果のある5種類のハーブティーセット。心身の疲れを優しく癒す。",
                Price = 2800,
                ImageUrl = "https://via.placeholder.com/300x300/98FB98/FFFFFF?text=ハーブティー",
                Category = "美容・リラックス",
                Stock = 30
            },
            new Product
            {
                Id = 36,
                Name = "レザーキーホルダー",
                Description = "上質な革製のシンプルなキーホルダー。使うほどに味が出る長く愛用できるアイテム。",
                Price = 2200,
                ImageUrl = "https://via.placeholder.com/300x300/8B4513/FFFFFF?text=キーホルダー",
                Category = "ファッション",
                Stock = 38
            },
            new Product
            {
                Id = 37,
                Name = "ジグソーパズル",
                Description = "美しい風景の1000ピースジグソーパズル。集中力と達成感を味わえるホビーアイテム。",
                Price = 2500,
                ImageUrl = "https://via.placeholder.com/300x300/87CEEB/FFFFFF?text=パズル",
                Category = "ホビー・ガーデニング",
                Stock = 20
            },
            new Product
            {
                Id = 38,
                Name = "バックパック",
                Description = "通勤・通学・アウトドアに使える多機能バックパック。耐久性と機能性を兼ね備えている。",
                Price = 7800,
                ImageUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=バックパック",
                Category = "ファッション",
                Stock = 14
            },
            new Product
            {
                Id = 39,
                Name = "お香セット",
                Description = "天然素材の上質なお香10本セット。瞑想や集中したい時間に心を落ち着かせる。",
                Price = 1900,
                ImageUrl = "https://via.placeholder.com/300x300/DDA0DD/FFFFFF?text=お香",
                Category = "美容・リラックス",
                Stock = 35
            },
            new Product
            {
                Id = 40,
                Name = "デジタルフォトフレーム",
                Description = "Wi-Fi対応のデジタルフォトフレーム。離れた家族と写真を共有できる現代的なギフト。",
                Price = 11500,
                ImageUrl = "https://via.placeholder.com/300x300/4B0082/FFFFFF?text=フォトフレーム",
                Category = "テクノロジー",
                Stock = 9
            },
            new Product
            {
                Id = 41,
                Name = "バスローブ",
                Description = "肌触りの良いコットン100%のバスローブ。バスタイム後の贅沢なリラックスタイムに。",
                Price = 6200,
                ImageUrl = "https://via.placeholder.com/300x300/F0F8FF/FFFFFF?text=バスローブ",
                Category = "美容・リラックス",
                Stock = 12
            },
            new Product
            {
                Id = 42,
                Name = "木製時計",
                Description = "天然木を使用したシンプルで温かみのある掛け時計。どんなインテリアにもマッチ。",
                Price = 4600,
                ImageUrl = "https://via.placeholder.com/300x300/D2B48C/FFFFFF?text=木製時計",
                Category = "インテリア",
                Stock = 18
            },
            new Product
            {
                Id = 43,
                Name = "スケッチブック＆鉛筆セット",
                Description = "高品質なスケッチブックと鉛筆セット。絵を描く楽しさを始められるアートキット。",
                Price = 2300,
                ImageUrl = "https://via.placeholder.com/300x300/F5DEB3/FFFFFF?text=スケッチ",
                Category = "ステーショナリー",
                Stock = 26
            },
            new Product
            {
                Id = 44,
                Name = "スポーツタオルセット",
                Description = "吸水性抜群のマイクロファイバースポーツタオル3枚セット。ジムや旅行に最適。",
                Price = 2900,
                ImageUrl = "https://via.placeholder.com/300x300/4682B4/FFFFFF?text=タオル",
                Category = "スポーツ・フィットネス",
                Stock = 32
            },
            new Product
            {
                Id = 45,
                Name = "ルームフレグランス",
                Description = "上品な香りが長時間続くリードディフューザー。お部屋の雰囲気を格上げする。",
                Price = 3700,
                ImageUrl = "https://via.placeholder.com/300x300/E6E6FA/FFFFFF?text=フレグランス",
                Category = "美容・リラックス",
                Stock = 24
            },
            new Product
            {
                Id = 46,
                Name = "ポータブルチェア",
                Description = "軽量でコンパクトに折りたためるアウトドアチェア。キャンプやピクニックに最適。",
                Price = 5400,
                ImageUrl = "https://via.placeholder.com/300x300/228B22/FFFFFF?text=チェア",
                Category = "スポーツ・フィットネス",
                Stock = 11
            },
            new Product
            {
                Id = 47,
                Name = "カードゲームセット",
                Description = "定番から新しいルールまで楽しめるカードゲーム3種セット。世代を超えて楽しめる。",
                Price = 2600,
                ImageUrl = "https://via.placeholder.com/300x300/DC143C/FFFFFF?text=カードゲーム",
                Category = "ホビー・ガーデニング",
                Stock = 28
            },
            new Product
            {
                Id = 48,
                Name = "プランターセット",
                Description = "おしゃれなセラミック製プランター3点セット。室内ガーデニングを気軽に始められる。",
                Price = 4100,
                ImageUrl = "https://via.placeholder.com/300x300/8FBC8F/FFFFFF?text=プランター",
                Category = "ホビー・ガーデニング",
                Stock = 16
            }
        };
    }
}