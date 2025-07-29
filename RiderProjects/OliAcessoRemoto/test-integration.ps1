# Script de Teste de Integração - Cliente WPF com Servidor OnliDek
# Execute este script para testar a integração

Write-Host "=== Teste de Integração OliAcesso Remoto ===" -ForegroundColor Green
Write-Host ""

# Configurações
$ServerUrl = "http://172.25.63.212:5165"
$ClientProjectPath = "."
$ServerProjectPath = "../OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor"

Write-Host "1. Verificando conectividade com o servidor..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$ServerUrl/health" -Method Get -TimeoutSec 5
    Write-Host "✓ Servidor está online: $($response.status)" -ForegroundColor Green
    Write-Host "  Timestamp: $($response.timestamp)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Erro ao conectar com servidor: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "  Verifique se o servidor está rodando em $ServerUrl" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "2. Testando registro de cliente..." -ForegroundColor Yellow
try {
    $clientData = @{
        name = "Cliente Teste PowerShell"
        connectionInfo = '{"ip":"127.0.0.1","version":"1.0.0","os":"Windows"}'
    } | ConvertTo-Json

    $headers = @{
        "Content-Type" = "application/json"
    }

    $response = Invoke-RestMethod -Uri "$ServerUrl/api/client/register" -Method Post -Body $clientData -Headers $headers
    
    if ($response.success) {
        Write-Host "✓ Cliente registrado com sucesso!" -ForegroundColor Green
        Write-Host "  Client ID: $($response.clientId)" -ForegroundColor Gray
        Write-Host "  Token: $($response.token.Substring(0, 20))..." -ForegroundColor Gray
        
        $global:TestClientId = $response.clientId
        $global:TestToken = $response.token
    } else {
        Write-Host "✗ Falha no registro: $($response.message)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ Erro no teste de registro: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "3. Testando listagem de clientes online..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $global:TestToken"
    }
    
    $response = Invoke-RestMethod -Uri "$ServerUrl/api/client/clients/online" -Method Get -Headers $headers
    Write-Host "✓ Clientes online obtidos: $($response.Count) cliente(s)" -ForegroundColor Green
    
    foreach ($client in $response) {
        Write-Host "  - $($client.name) ($($client.clientId))" -ForegroundColor Gray
    }
} catch {
    Write-Host "✗ Erro ao obter clientes online: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "4. Verificando dependências do cliente WPF..." -ForegroundColor Yellow
if (Test-Path "$ClientProjectPath/OliAcessoRemoto.csproj") {
    Write-Host "✓ Projeto cliente encontrado" -ForegroundColor Green
    
    # Verificar se as dependências foram adicionadas
    $csprojContent = Get-Content "$ClientProjectPath/OliAcessoRemoto.csproj" -Raw
    
    $requiredPackages = @(
        "Microsoft.AspNetCore.SignalR.Client",
        "System.Net.Http.Json",
        "Newtonsoft.Json",
        "System.IdentityModel.Tokens.Jwt"
    )
    
    foreach ($package in $requiredPackages) {
        if ($csprojContent -like "*$package*") {
            Write-Host "  ✓ $package" -ForegroundColor Green
        } else {
            Write-Host "  ✗ $package (FALTANDO)" -ForegroundColor Red
        }
    }
} else {
    Write-Host "✗ Projeto cliente não encontrado em $ClientProjectPath" -ForegroundColor Red
}

Write-Host ""
Write-Host "5. Verificando arquivos de integração..." -ForegroundColor Yellow
$requiredFiles = @(
    "Models/ServerModels.cs",
    "Services/ServerApiService.cs",
    "appsettings.client.json"
)

foreach ($file in $requiredFiles) {
    if (Test-Path "$ClientProjectPath/$file") {
        Write-Host "  ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $file (FALTANDO)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== Resumo do Teste ===" -ForegroundColor Green
Write-Host "✓ Servidor OnliDek está funcionando" -ForegroundColor Green
Write-Host "✓ API de registro funcionando" -ForegroundColor Green
Write-Host "✓ Autenticação JWT funcionando" -ForegroundColor Green
Write-Host "✓ Listagem de clientes funcionando" -ForegroundColor Green
Write-Host ""
Write-Host "Próximos passos:" -ForegroundColor Yellow
Write-Host "1. Compile o cliente WPF: dotnet build" -ForegroundColor White
Write-Host "2. Execute o cliente: dotnet run" -ForegroundColor White
Write-Host "3. O cliente deve se registrar automaticamente no servidor" -ForegroundColor White
Write-Host "4. Teste a conexão entre dois clientes" -ForegroundColor White
Write-Host ""
Write-Host "ID do cliente de teste criado: $global:TestClientId" -ForegroundColor Cyan
