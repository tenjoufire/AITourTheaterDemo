using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;

namespace GiftPalette.Services;

public class AIChatConfiguration
{
    public string Endpoint { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
}

public class AIChatService : IAIChatService
{
    private readonly PersistentAgentsClient? _agentsClient;
    private readonly string? _azureAIAgentID;
    private readonly ILogger<AIChatService> _logger;
    private readonly AIChatConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public AIChatService(IOptions<AIChatConfiguration> configuration, ILogger<AIChatService> logger, IOptions<ApiConfiguration> apiConfig, HttpClient httpClient)
    {
        _logger = logger;
        _config = configuration.Value;
        _httpClient = httpClient;
        _apiBaseUrl = apiConfig.Value.BaseUrl;

        if (!string.IsNullOrEmpty(_config.Endpoint) && !string.IsNullOrEmpty(_config.AgentId))
        {
            try
            {
                // Initialize with DefaultAzureCredential for Azure AI Foundry service
#if DEBUG
                _agentsClient = new PersistentAgentsClient(_config.Endpoint, new AzureCliCredential());
#else
                _agentsClient = new PersistentAgentsClient(_config.Endpoint, new DefaultAzureCredential());
#endif
                _azureAIAgentID = _config.AgentId;
                _logger.LogInformation("Azure AI Foundry Agent Service initialized successfully with endpoint: {Endpoint}, AgentId: {AgentId}", _config.Endpoint, _config.AgentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure AI Foundry Agent Service client");
            }
        }
        else
        {
            _logger.LogWarning("Azure AI Foundry Agent Service not configured. Missing Endpoint or AgentId. Using mock responses for demonstration.");
        }
    }

    public async Task<string> CreateThreadAsync()
    {
        if (_agentsClient == null)
        {
            _logger.LogWarning("Azure AI Foundry Agent Service client not initialized, returning mock thread ID");
            return await Task.FromResult(Guid.NewGuid().ToString());
        }

        try
        {
            _logger.LogInformation("Creating new thread with Azure AI Foundry Agent Service");
            // When implemented with actual Azure AI Foundry service:
            var thread = await _agentsClient.Threads.CreateThreadAsync();
            return thread.Value.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create thread with Azure AI Foundry Agent Service");
            return $"{Guid.NewGuid().ToString()}"; // Return mock thread ID on failure
        }
    }

    public async Task<string> SendMessageAsync(string message, string threadId = "", string cartId = "")
    {
        if (_agentsClient == null || string.IsNullOrEmpty(_azureAIAgentID))
        {
            _logger.LogWarning("Azure AI Foundry Agent Service client or Agent ID not configured");
            return await GenerateEnhancedMockResponseAsync(message, cartId);
        }

        try
        {
            _logger.LogInformation("Sending message to Azure AI Foundry Agent Service - Agent: {AgentId}, Thread: {ThreadId}", _azureAIAgentID, threadId);
            var agentResponse = await _agentsClient.Administration.GetAgentAsync(_azureAIAgentID);
            var agent = agentResponse.Value;

            //create messages in Thread
            await _agentsClient.Messages.CreateMessageAsync(threadId, MessageRole.User, message);

            // Get response from the agent
            var runResponse = await _agentsClient.Runs.CreateRunAsync(threadId, _azureAIAgentID);
            var run = runResponse.Value;

            while (run.Status == RunStatus.InProgress || run.Status == RunStatus.Queued)
            {
                await Task.Delay(500); // Wait before checking status again
                var updatedRunResponse = await _agentsClient.Runs.GetRunAsync(threadId, run.Id);
                run = updatedRunResponse.Value;
            }

            //get messages from responce
            var messagesResponse = _agentsClient.Messages.GetMessages(threadId, order: ListSortOrder.Descending);

            foreach (var threadMessage in messagesResponse)
            {
                if (threadMessage.Role == MessageRole.Agent)
                {
                    foreach (var content in threadMessage.ContentItems)
                    {
                        if (content is MessageTextContent textContent)
                        {
                            _logger.LogInformation("Received response from Azure AI Foundry Agent Service");
                            return textContent.Text;
                        }
                    }
                }
            }

            return "申し訳ございません。Azure AI Foundry Agent Serviceからの応答が取得できませんでした。もう一度お試しください。";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to Azure AI Foundry Agent Service");
            return "申し訳ございません。Azure AI Foundry Agent Serviceとの通信でエラーが発生しました。もう一度お試しください。";
        }
    }

    private async Task<string> AddToCartAsync(string cartId, int productId, int quantity)
    {
        try
        {
            var request = new { ProductId = productId, Quantity = quantity };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/cart/{cartId}/add", content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully added product {ProductId} (quantity: {Quantity}) to cart {CartId}", productId, quantity, cartId);
                return "商品をカートに追加しました。";
            }
            else
            {
                _logger.LogWarning("Failed to add product to cart. Status: {StatusCode}", response.StatusCode);
                return "商品の追加に失敗しました。";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product to cart");
            return "商品の追加中にエラーが発生しました。";
        }
    }

    private async Task<string> GetCartAsync(string cartId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/cart/{cartId}");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var cart = JsonSerializer.Deserialize<GiftPalette.Models.Cart>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (cart != null && cart.Items.Count > 0)
                {
                    var itemsSummary = string.Join("\n", cart.Items.Select(item => 
                        $"• {item.ProductName} × {item.Quantity}個 (¥{item.Price:N0} × {item.Quantity} = ¥{(item.Price * item.Quantity):N0})"));
                    
                    return $"現在のカート内容:\n{itemsSummary}\n\n合計: {cart.TotalItems}個 ¥{cart.TotalAmount:N0}";
                }
                else
                {
                    return "カートは空です。";
                }
            }
            else
            {
                return "カート情報の取得に失敗しました。";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart");
            return "カート情報の取得中にエラーが発生しました。";
        }
    }

    private async Task<string> RemoveFromCartAsync(string cartId, int productId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/cart/{cartId}/remove/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully removed product {ProductId} from cart {CartId}", productId, cartId);
                return "商品をカートから削除しました。";
            }
            else
            {
                return "商品の削除に失敗しました。";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from cart");
            return "商品の削除中にエラーが発生しました。";
        }
    }

    private async Task<string> ClearCartAsync(string cartId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/cart/{cartId}/clear");
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully cleared cart {CartId}", cartId);
                return "カートをクリアしました。";
            }
            else
            {
                return "カートのクリアに失敗しました。";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart");
            return "カートのクリア中にエラーが発生しました。";
        }
    }

    private async Task<string> ProcessCartOperationAsync(string message, string cartId)
    {
        var lowerMessage = message.ToLower();
        
        // Cart operation patterns
        if ((lowerMessage.Contains("カート") || lowerMessage.Contains("追加")) && !string.IsNullOrEmpty(cartId))
        {
            // Pattern: "商品Aを2個カートに追加して"
            
            if (lowerMessage.Contains("追加"))
            {
                // Extract quantity if mentioned
                var quantityMatch = System.Text.RegularExpressions.Regex.Match(lowerMessage, @"(\d+)個?");
                var quantity = quantityMatch.Success ? int.Parse(quantityMatch.Groups[1].Value) : 1;
                
                // Map product names to IDs (simplified for demo)
                var productId = 1; // Default to first product
                
                if (lowerMessage.Contains("マグカップ") || lowerMessage.Contains("カップ"))
                    productId = 1;
                else if (lowerMessage.Contains("ネックレス") || lowerMessage.Contains("アクセサリー"))
                    productId = 2;
                else if (lowerMessage.Contains("キャンドル") || lowerMessage.Contains("アロマ"))
                    productId = 3;
                else if (lowerMessage.Contains("ハンドクリーム") || lowerMessage.Contains("クリーム"))
                    productId = 4;
                else if (lowerMessage.Contains("ブランケット") || lowerMessage.Contains("毛布"))
                    productId = 5;
                
                var result = await AddToCartAsync(cartId, productId, quantity);
                var cartStatus = await GetCartAsync(cartId);
                return $"{result}\n\n{cartStatus}";
            }
            
            if (lowerMessage.Contains("確認") || lowerMessage.Contains("中身") || lowerMessage.Contains("内容"))
            {
                return await GetCartAsync(cartId);
            }
            
            if (lowerMessage.Contains("削除") || lowerMessage.Contains("消去"))
            {
                // For demo, remove first item or specify product
                var productId = 1;
                if (lowerMessage.Contains("ネックレス")) productId = 2;
                else if (lowerMessage.Contains("キャンドル")) productId = 3;
                // Add more mappings as needed
                
                var result = await RemoveFromCartAsync(cartId, productId);
                var cartStatus = await GetCartAsync(cartId);
                return $"{result}\n\n{cartStatus}";
            }
            
            if (lowerMessage.Contains("クリア") || lowerMessage.Contains("空に"))
            {
                return await ClearCartAsync(cartId);
            }
        }
        
        return null!; // No cart operation detected
    }

    private async Task<string> GenerateEnhancedMockResponseAsync(string message, string cartId = "")
    {
        // Check for cart operations first
        if (!string.IsNullOrEmpty(cartId))
        {
            var cartOperation = await ProcessCartOperationAsync(message, cartId);
            if (cartOperation != null)
            {
                return cartOperation;
            }
        }
        
        // Simulate Azure AI Foundry processing time
        await Task.Delay(Random.Shared.Next(800, 2000));

        var lowerMessage = message.ToLower();

        // More sophisticated mock responses that simulate an AI agent trained for gift recommendations
        if (lowerMessage.Contains("こんにちは") || lowerMessage.Contains("はじめまして") || lowerMessage.Contains("初めて"))
        {
            return "こんにちは！GiftPaletteのAIアシスタントです😊\n\n" +
                   "私は、お客様に最適なギフトをご提案するために、Azure AI Foundryで訓練されたAIエージェントです。\n\n" +
                   "以下のような情報を教えていただけると、より良いご提案ができます：\n" +
                   "• 贈る相手の方（年代、性別、関係性）\n" +
                   "• ご予算の範囲\n" +
                   "• 贈るシーン（誕生日、記念日、お礼など）\n\n" +
                   "何かご質問はございますか？";
        }
        else if (lowerMessage.Contains("おすすめ") || lowerMessage.Contains("提案") || lowerMessage.Contains("選んで"))
        {
            return "🎁 **ギフト選びのお手伝いをいたします！**\n\n" +
                   "より具体的なご提案をするために、以下について教えてください：\n\n" +
                   "1. **お相手の情報**\n" +
                   "   - 年代（20代、30代など）\n" +
                   "   - 性別\n" +
                   "   - ご関係（家族、友人、同僚など）\n\n" +
                   "2. **ご予算**\n" +
                   "   - 3,000円〜5,000円\n" +
                   "   - 5,000円〜10,000円\n" +
                   "   - 10,000円以上\n\n" +
                   "3. **贈るシーン**\n" +
                   "   - 誕生日、記念日、お祝い、お礼など\n\n" +
                   "これらの情報をもとに、最適な商品をご提案いたします！";
        }
        else if (lowerMessage.Contains("予算") || lowerMessage.Contains("価格") || lowerMessage.Contains("値段"))
        {
            return "💰 **予算別おすすめギフト**\n\n" +
                   "**〜5,000円**\n" +
                   "• アロマキャンドル・入浴剤\n" +
                   "• おしゃれな文房具\n" +
                   "• 小物・アクセサリー\n\n" +
                   "**5,000円〜10,000円**\n" +
                   "• ブランドコスメ\n" +
                   "• 上質なタオル・寝具\n" +
                   "• グルメギフト\n\n" +
                   "**10,000円〜**\n" +
                   "• ジュエリー・時計\n" +
                   "• 家電・デジタル機器\n" +
                   "• 体験ギフト\n\n" +
                   "具体的なご予算をお聞かせください。その範囲でベストな商品をご提案いたします！";
        }
        else if (lowerMessage.Contains("女性") || lowerMessage.Contains("彼女") || lowerMessage.Contains("奥さん") || lowerMessage.Contains("母"))
        {
            return "👩 **女性向けギフトのご提案**\n\n" +
                   "**人気商品TOP3**\n" +
                   "1. **アクセサリー**（ネックレス、ピアス）\n" +
                   "   - 上品で毎日使える\n" +
                   "   - 価格帯：8,000円〜25,000円\n\n" +
                   "2. **美容・リラックス用品**\n" +
                   "   - アロマディフューザー、スキンケアセット\n" +
                   "   - 価格帯：3,000円〜15,000円\n\n" +
                   "3. **ライフスタイル雑貨**\n" +
                   "   - おしゃれなマグカップ、インテリア小物\n" +
                   "   - 価格帯：2,000円〜8,000円\n\n" +
                   "お相手の年代や好みをお聞かせください。より具体的にご提案いたします！";
        }
        else if (lowerMessage.Contains("男性") || lowerMessage.Contains("彼氏") || lowerMessage.Contains("旦那") || lowerMessage.Contains("父"))
        {
            return "👨 **男性向けギフトのご提案**\n\n" +
                   "**おすすめカテゴリ**\n" +
                   "1. **テック・ガジェット**\n" +
                   "   - ワイヤレス充電器、スマートウォッチ\n" +
                   "   - 価格帯：5,000円〜30,000円\n\n" +
                   "2. **ビジネス・実用品**\n" +
                   "   - 革小物、ボールペン、ネクタイ\n" +
                   "   - 価格帯：3,000円〜15,000円\n\n" +
                   "3. **趣味・エンターテイメント**\n" +
                   "   - 書籍、ゲーム、スポーツ用品\n" +
                   "   - 価格帯：2,000円〜20,000円\n\n" +
                   "どのような分野にご興味をお持ちの方でしょうか？";
        }
        else if (lowerMessage.Contains("配送") || lowerMessage.Contains("届け") || lowerMessage.Contains("発送"))
        {
            return "🚚 **配送について**\n\n" +
                   "**配送オプション**\n" +
                   "• **通常配送**：2-3営業日（無料）\n" +
                   "• **お急ぎ便**：翌日配送（+500円）\n" +
                   "• **指定日配送**：ご希望の日時（+300円）\n\n" +
                   "**ギフト包装**\n" +
                   "• 無料ギフト包装（リボン付き）\n" +
                   "• プレミアム包装（+200円）\n" +
                   "• メッセージカード（無料）\n\n" +
                   "大切な記念日にもしっかりと間に合うよう手配いたします！\n" +
                   "配送についてご不明な点がございましたら、お気軽にお尋ねください。";
        }
        else if (lowerMessage.Contains("返品") || lowerMessage.Contains("交換") || lowerMessage.Contains("キャンセル"))
        {
            return "🔄 **返品・交換について**\n\n" +
                   "**返品ポリシー**\n" +
                   "• 商品到着後14日以内\n" +
                   "• 未使用・未開封の商品\n" +
                   "• 返送料は弊社負担\n\n" +
                   "**交換について**\n" +
                   "• サイズ・色違いの交換可能\n" +
                   "• 在庫状況により代替商品をご提案\n\n" +
                   "**キャンセル**\n" +
                   "• 発送前であれば全額返金\n" +
                   "• 発送後は返品手続きとなります\n\n" +
                   "ご不明な点がございましたら、カスタマーサポートまでお気軽にお問い合わせください。";
        }
        else
        {
            return $"ご質問いただき、ありがとうございます！\n\n" +
                   $"「{message}」について、より詳しくお答えするために、もう少し具体的な情報をお聞かせください。\n\n" +
                   "**よくあるご質問**\n" +
                   "• 商品の選び方・おすすめ\n" +
                   "• 価格・予算について\n" +
                   "• 配送・ギフト包装\n" +
                   "• 返品・交換について\n\n" +
                   "Azure AI Foundryの技術を活用して、お客様に最適なギフト選びをサポートいたします。どんな小さなことでもお気軽にお尋ねください！";
        }
    }
}