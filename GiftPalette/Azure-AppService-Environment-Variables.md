# Azure App Service 環境変数設定ガイド

このアプリケーションは、Azure App Service にデプロイする際に環境変数から設定を読み取るように構成されています。

## 設定の優先順位

1. **環境変数** (最優先)
2. **構成ファイル** (appsettings.json, appsettings.Development.json)

## Azure App Service での環境変数設定

### Azure Portal での設定方法

1. Azure Portal で App Service リソースを開く
2. 左側のメニューから「構成」を選択
3. 「アプリケーション設定」タブで「+ 新しいアプリケーション設定」をクリック
4. 以下の設定を追加：

#### AI Chat Configuration
- **名前**: `AIChatConfiguration__Endpoint`
- **値**: Azure AI Foundry のエンドポイント URL
  - 例: `https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway`

- **名前**: `AIChatConfiguration__AgentId`  
- **値**: Azure AI Agent ID
  - 例: `asst_vANDQMMb7i0FQK8Eh3JFkPCd`

#### API Configuration
- **名前**: `ApiConfiguration__BaseUrl`
- **値**: バックエンド API のベース URL
  - 開発環境: `http://localhost:5062`
  - 本番環境: `https://your-api.azurewebsites.net`

### Azure CLI での設定方法

```bash
# リソースグループとApp Service名を設定
az webapp config appsettings set --resource-group <your-resource-group> --name <your-app-name> --settings \
  "AIChatConfiguration__Endpoint=https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway" \
  "AIChatConfiguration__AgentId=asst_vANDQMMb7i0FQK8Eh3JFkPCd" \
  "ApiConfiguration__BaseUrl=https://your-api.azurewebsites.net"
```

### PowerShell での設定方法

```powershell
# Azure PowerShell を使用
Set-AzWebAppConfig -ResourceGroupName "<your-resource-group>" -Name "<your-app-name>" -AppSettings @{
    "AIChatConfiguration__Endpoint" = "https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway"
    "AIChatConfiguration__AgentId" = "asst_vANDQMMb7i0FQK8Eh3JFkPCd"
    "ApiConfiguration__BaseUrl" = "https://your-api.azurewebsites.net"
}
```

## ローカル開発での環境変数設定

### Windows (PowerShell)
```powershell
$env:AIChatConfiguration__Endpoint="https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway"
$env:AIChatConfiguration__AgentId="asst_vANDQMMb7i0FQK8Eh3JFkPCd"
$env:ApiConfiguration__BaseUrl="http://localhost:5062"
```

### Windows (コマンドプロンプト)
```cmd
set AIChatConfiguration__Endpoint=https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway
set AIChatConfiguration__AgentId=asst_vANDQMMb7i0FQK8Eh3JFkPCd
set ApiConfiguration__BaseUrl=http://localhost:5062
```

### Linux/macOS
```bash
export AIChatConfiguration__Endpoint="https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway"
export AIChatConfiguration__AgentId="asst_vANDQMMb7i0FQK8Eh3JFkPCd"
export ApiConfiguration__BaseUrl="http://localhost:5062"
```

## デプロイメントシナリオ

### 同じApp Serviceに両方をデプロイする場合
API と UI が同じ App Service にデプロイされる場合、API の BaseUrl は自分自身を指すようにします：

```bash
# 同じサービス内での API 呼び出しの場合
az webapp config appsettings set --resource-group <your-resource-group> --name <your-app-name> --settings \
  "ApiConfiguration__BaseUrl=https://<your-app-name>.azurewebsites.net"
```

### 別々の App Service にデプロイする場合
API と UI が別々の App Service にデプロイされる場合、それぞれの URL を指定します：

```bash
# UI App Service の設定（API を別のサービスから呼び出す場合）
az webapp config appsettings set --resource-group <your-resource-group> --name <your-ui-app-name> --settings \
  "ApiConfiguration__BaseUrl=https://<your-api-app-name>.azurewebsites.net"
```

## 注意事項

- 環境変数名では、JSONの階層構造を `__` (アンダースコア2つ) で表現します
- 例: `AIChatConfiguration:Endpoint` → `AIChatConfiguration__Endpoint`
- 例: `ApiConfiguration:BaseUrl` → `ApiConfiguration__BaseUrl`
- 環境変数が設定されていない場合は、自動的に appsettings.json の値が使用されます
- 機密情報は Azure Key Vault を使用することを推奨します
- 本番環境では HTTPS URL を使用してください

## 設定確認

アプリケーションのログで以下のメッセージを確認できます：

### AI Chat Service
- 成功時: "Azure AI Foundry Agent Service initialized successfully with endpoint: {Endpoint}, AgentId: {AgentId}"
- 設定不足時: "Azure AI Foundry Agent Service not configured. Missing Endpoint or AgentId"

### API Service
- ProductService, CartService でのAPI呼び出しエラーがないことを確認
- エラーがある場合は、`ApiConfiguration__BaseUrl` の設定を確認してください