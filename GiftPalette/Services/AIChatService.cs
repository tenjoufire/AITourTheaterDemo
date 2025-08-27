using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.Options;

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

    public AIChatService(IOptions<AIChatConfiguration> configuration, ILogger<AIChatService> logger)
    {
        _logger = logger;
        
        var config = configuration.Value;
        if (!string.IsNullOrEmpty(config.Endpoint) && !string.IsNullOrEmpty(config.AgentId))
        {
            try
            {
                _agentsClient = new PersistentAgentsClient(config.Endpoint, new DefaultAzureCredential());
                _azureAIAgentID = config.AgentId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure AI Agents client");
            }
        }
    }

    public Task<string> CreateThreadAsync()
    {
        if (_agentsClient == null)
        {
            _logger.LogWarning("Azure AI Agents client not initialized");
            return Task.FromResult(string.Empty);
        }

        try
        {
            // For now, return a mock thread ID until we can test with actual Azure AI service
            return Task.FromResult(Guid.NewGuid().ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create thread");
            return Task.FromResult(string.Empty);
        }
    }

    public async Task<string> SendMessageAsync(string message, string threadId = "")
    {
        if (_agentsClient == null || string.IsNullOrEmpty(_azureAIAgentID))
        {
            _logger.LogWarning("Azure AI Agents client or Agent ID not configured");
            
            // Return a helpful mock response until actual service is configured
            return await GenerateMockResponseAsync(message);
        }

        try
        {
            // When actual Azure AI service is configured, the implementation will be:
            // 1. Create thread if not provided
            // 2. Send message to thread
            // 3. Create and poll run
            // 4. Get response
            
            // For now, return mock response
            return await GenerateMockResponseAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to AI agent");
            return "エラーが発生しました。もう一度お試しください。";
        }
    }

    private async Task<string> GenerateMockResponseAsync(string message)
    {
        await Task.Delay(1000); // Simulate API delay
        
        var lowerMessage = message.ToLower();
        
        if (lowerMessage.Contains("おすすめ") || lowerMessage.Contains("ギフト"))
        {
            return "こんにちは！ギフト選びのお手伝いをさせていただきます。どのような方への贈り物をお探しですか？年代や性別、ご予算などを教えていただけると、最適な商品をご提案いたします！";
        }
        else if (lowerMessage.Contains("予算") || lowerMessage.Contains("価格"))
        {
            return "ご予算に応じて幅広い商品をご用意しております。3,000円～50,000円の範囲で様々なギフトアイテムがございます。具体的なご予算を教えていただけますでしょうか？";
        }
        else if (lowerMessage.Contains("女性") || lowerMessage.Contains("女の人"))
        {
            return "女性向けのギフトでしたら、アクセサリーや美容・リラックス用品、インテリア雑貨などが人気です。特にネックレスやアロマキャンドル、おしゃれなマグカップなどはよくお選びいただいております。";
        }
        else if (lowerMessage.Contains("男性") || lowerMessage.Contains("男の人"))
        {
            return "男性向けのギフトでしたら、テクノロジー関連商品やスポーツ用品、実用的なアイテムが喜ばれることが多いです。スマートウォッチやワイヤレス充電器などはいかがでしょうか？";
        }
        else if (lowerMessage.Contains("配送") || lowerMessage.Contains("届け"))
        {
            return "配送については、最短翌日お届けが可能です。お急ぎの場合はお気軽にお申し付けください。大切な記念日にも間に合うよう配送いたします。";
        }
        else
        {
            return $"ご質問ありがとうございます。「{message}」について詳しくご案内いたします。より具体的にお聞かせいただけると、より良いアドバイスができます。商品選びや配送についてなど、何でもお気軽にお尋ねください！";
        }
    }
}