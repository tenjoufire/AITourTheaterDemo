# Azure App Service ���ϐ��ݒ�K�C�h

���̃A�v���P�[�V�����́AAzure App Service �Ƀf�v���C����ۂɊ��ϐ�����ݒ��ǂݎ��悤�ɍ\������Ă��܂��B

## �ݒ�̗D�揇��

1. **���ϐ�** (�ŗD��)
2. **�\���t�@�C��** (appsettings.json, appsettings.Development.json)

## Azure App Service �ł̊��ϐ��ݒ�

### Azure Portal �ł̐ݒ���@

1. Azure Portal �� App Service ���\�[�X���J��
2. �����̃��j���[����u�\���v��I��
3. �u�A�v���P�[�V�����ݒ�v�^�u�Łu+ �V�����A�v���P�[�V�����ݒ�v���N���b�N
4. �ȉ��̐ݒ��ǉ��F

#### AI Chat Configuration
- **���O**: `AIChatConfiguration__Endpoint`
- **�l**: Azure AI Foundry �̃G���h�|�C���g URL
  - ��: `https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway`

- **���O**: `AIChatConfiguration__AgentId`  
- **�l**: Azure AI Agent ID
  - ��: `asst_vANDQMMb7i0FQK8Eh3JFkPCd`

#### API Configuration
- **���O**: `ApiConfiguration__BaseUrl`
- **�l**: �o�b�N�G���h API �̃x�[�X URL
  - �J����: `http://localhost:5062`
  - �{�Ԋ�: `https://your-api.azurewebsites.net`

### Azure CLI �ł̐ݒ���@

```bash
# ���\�[�X�O���[�v��App Service����ݒ�
az webapp config appsettings set --resource-group <your-resource-group> --name <your-app-name> --settings \
  "AIChatConfiguration__Endpoint=https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway" \
  "AIChatConfiguration__AgentId=asst_vANDQMMb7i0FQK8Eh3JFkPCd" \
  "ApiConfiguration__BaseUrl=https://your-api.azurewebsites.net"
```

### PowerShell �ł̐ݒ���@

```powershell
# Azure PowerShell ���g�p
Set-AzWebAppConfig -ResourceGroupName "<your-resource-group>" -Name "<your-app-name>" -AppSettings @{
    "AIChatConfiguration__Endpoint" = "https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway"
    "AIChatConfiguration__AgentId" = "asst_vANDQMMb7i0FQK8Eh3JFkPCd"
    "ApiConfiguration__BaseUrl" = "https://your-api.azurewebsites.net"
}
```

## ���[�J���J���ł̊��ϐ��ݒ�

### Windows (PowerShell)
```powershell
$env:AIChatConfiguration__Endpoint="https://katsujim-norway-resource.services.ai.azure.com/api/projects/katsujim-norway"
$env:AIChatConfiguration__AgentId="asst_vANDQMMb7i0FQK8Eh3JFkPCd"
$env:ApiConfiguration__BaseUrl="http://localhost:5062"
```

### Windows (�R�}���h�v�����v�g)
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

## �f�v���C�����g�V�i���I

### ����App Service�ɗ������f�v���C����ꍇ
API �� UI ������ App Service �Ƀf�v���C�����ꍇ�AAPI �� BaseUrl �͎������g���w���悤�ɂ��܂��F

```bash
# �����T�[�r�X���ł� API �Ăяo���̏ꍇ
az webapp config appsettings set --resource-group <your-resource-group> --name <your-app-name> --settings \
  "ApiConfiguration__BaseUrl=https://<your-app-name>.azurewebsites.net"
```

### �ʁX�� App Service �Ƀf�v���C����ꍇ
API �� UI ���ʁX�� App Service �Ƀf�v���C�����ꍇ�A���ꂼ��� URL ���w�肵�܂��F

```bash
# UI App Service �̐ݒ�iAPI ��ʂ̃T�[�r�X����Ăяo���ꍇ�j
az webapp config appsettings set --resource-group <your-resource-group> --name <your-ui-app-name> --settings \
  "ApiConfiguration__BaseUrl=https://<your-api-app-name>.azurewebsites.net"
```

## ���ӎ���

- ���ϐ����ł́AJSON�̊K�w�\���� `__` (�A���_�[�X�R�A2��) �ŕ\�����܂�
- ��: `AIChatConfiguration:Endpoint` �� `AIChatConfiguration__Endpoint`
- ��: `ApiConfiguration:BaseUrl` �� `ApiConfiguration__BaseUrl`
- ���ϐ����ݒ肳��Ă��Ȃ��ꍇ�́A�����I�� appsettings.json �̒l���g�p����܂�
- �@������ Azure Key Vault ���g�p���邱�Ƃ𐄏����܂�
- �{�Ԋ��ł� HTTPS URL ���g�p���Ă�������

## �ݒ�m�F

�A�v���P�[�V�����̃��O�ňȉ��̃��b�Z�[�W���m�F�ł��܂��F

### AI Chat Service
- ������: "Azure AI Foundry Agent Service initialized successfully with endpoint: {Endpoint}, AgentId: {AgentId}"
- �ݒ�s����: "Azure AI Foundry Agent Service not configured. Missing Endpoint or AgentId"

### API Service
- ProductService, CartService �ł�API�Ăяo���G���[���Ȃ����Ƃ��m�F
- �G���[������ꍇ�́A`ApiConfiguration__BaseUrl` �̐ݒ���m�F���Ă�������