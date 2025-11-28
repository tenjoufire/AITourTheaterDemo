using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.Options;
using OpenAI.Responses;
using System.Collections.Concurrent;

#pragma warning disable OPENAI001

namespace GiftPalette.Services;

public class AIChatConfiguration
{
    public string Endpoint { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
}

public class AIChatService : IAIChatService
{
    private readonly AIProjectClient? _projectClient;
    private readonly OpenAIResponseClient? _responseClient;
    private readonly string? _agentName;
    private readonly ILogger<AIChatService> _logger;
    private readonly AIChatConfiguration _config;
    private readonly ConcurrentDictionary<string, List<ResponseItem>> _conversationHistory = new();

    public AIChatService(IOptions<AIChatConfiguration> configuration, ILogger<AIChatService> logger)
    {
        _logger = logger;
        _config = configuration.Value;

        if (!string.IsNullOrEmpty(_config.Endpoint) && !string.IsNullOrEmpty(_config.AgentName))
        {
            try
            {
                // Initialize with DefaultAzureCredential for Azure AI Foundry service
#if DEBUG
                _projectClient = new AIProjectClient(endpoint: new Uri(_config.Endpoint), tokenProvider: new AzureCliCredential());
#else
                _projectClient = new AIProjectClient(endpoint: new Uri(_config.Endpoint), tokenProvider: new DefaultAzureCredential());
#endif
                _agentName = _config.AgentName;

                // Get the agent and create a response client
                AgentRecord agentRecord = _projectClient.Agents.GetAgent(_agentName);
                _responseClient = _projectClient.OpenAI.GetProjectResponsesClientForAgent(agentRecord);

                _logger.LogInformation("Azure AI Foundry Agent Service initialized successfully with endpoint: {Endpoint}, AgentName: {AgentName}, AgentId: {AgentId}", _config.Endpoint, _config.AgentName, agentRecord.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure AI Foundry Agent Service client");
            }
        }
        else
        {
            _logger.LogWarning("Azure AI Foundry Agent Service not configured. Missing Endpoint or AgentName. Using mock responses for demonstration.");
        }
    }

    public async Task<string> CreateThreadAsync()
    {
        // Create a new conversation thread by generating a unique ID
        var threadId = Guid.NewGuid().ToString();
        _conversationHistory.TryAdd(threadId, new List<ResponseItem>());
        _logger.LogInformation("Created new conversation thread: {ThreadId}", threadId);
        return await Task.FromResult(threadId);
    }

    public async Task<string> SendMessageAsync(string message, string threadId = "")
    {
        if (_responseClient == null || string.IsNullOrEmpty(_agentName))
        {
            _logger.LogWarning("Azure AI Foundry Agent Service client or Agent Name not configured");
            return await GenerateEnhancedMockResponseAsync(message);
        }

        try
        {
            _logger.LogInformation("Sending message to Azure AI Foundry Agent Service - Agent: {AgentName}, Thread: {ThreadId}", _agentName, threadId);

            // Get or create conversation history for this thread
            if (!_conversationHistory.TryGetValue(threadId, out var history))
            {
                history = new List<ResponseItem>();
                _conversationHistory.TryAdd(threadId, history);
            }

            // Create response with conversation history using PreviousResponseId if available
            OpenAIResponse response;
            if (history.Count > 0)
            {
                // Build input items from history for multi-turn conversation
                var inputItems = new List<ResponseItem>();
                inputItems.AddRange(history);
                
                var userMessageItem = ResponseItem.CreateUserMessageItem(message);
                inputItems.Add(userMessageItem);
                
                response = await _responseClient.CreateResponseAsync(inputItems);
                
                // Store user message and output items for conversation continuity
                history.Add(userMessageItem);
            }
            else
            {
                response = await _responseClient.CreateResponseAsync(message);
                
                // Store user message for first message in conversation
                history.Add(ResponseItem.CreateUserMessageItem(message));
            }

            // Store output items for conversation continuity
            history.AddRange(response.OutputItems);

            var outputText = response.GetOutputText();
            _logger.LogInformation("Received response from Azure AI Foundry Agent Service");
            return outputText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to Azure AI Foundry Agent Service");
            return "申し訳ございません。Azure AI Foundry Agent Serviceとの通信でエラーが発生しました。もう一度お試しください。";
        }
    }

    private async Task<string> GenerateEnhancedMockResponseAsync(string message)
    {
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