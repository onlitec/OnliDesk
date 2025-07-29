# 🔗 Guia de Integração - Cliente WPF com Servidor OnliDek

## 📋 Visão Geral
Este guia detalha como integrar o cliente WPF OliAcesso Remoto com o servidor OnliDek para testes em ambiente local.

## 🌐 Configuração do Ambiente

### Dados do Servidor
- **IP**: `172.25.63.212` (eth0)
- **Porta**: `5165` (HTTP)
- **URL Base**: `http://172.25.63.212:5165`
- **Health Check**: `http://172.25.63.212:5165/health`

### Endpoints Disponíveis
- `POST /api/client/register` - Registrar cliente
- `GET /api/client/status/{clientId}` - Status do cliente
- `POST /api/client/connection/request` - Solicitar conexão
- `GET /api/client/clients/online` - Listar clientes online
- `GET /health` - Verificação de saúde

## 🚀 Passos de Integração Implementados

### 1. ✅ Dependências Adicionadas
```xml
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="9.0.7" />
<PackageReference Include="System.Net.Http.Json" Version="9.0.7" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
```

### 2. ✅ Modelos de Comunicação
- `Models/ServerModels.cs` - DTOs para comunicação com servidor
- Modelos para registro, conexão, status e eventos

### 3. ✅ Serviço de API
- `Services/ServerApiService.cs` - Cliente HTTP para comunicação
- Métodos para todas as operações do servidor
- Tratamento de erros e autenticação JWT

### 4. ✅ Integração no MainWindow
- Registro automático no servidor ao iniciar
- Obtenção de ID real do servidor (formato XXX XXX XXX)
- Verificação de status de clientes remotos
- Solicitação de conexão através da API

### 5. ✅ Configuração
- `appsettings.client.json` - Configurações do cliente
- URLs do servidor e parâmetros de conexão

## 🧪 Como Testar

### Pré-requisitos
1. **Servidor OnliDek rodando**:
   ```bash
   cd /opt/onlidesk/RiderProjects/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor
   export PATH="$PATH:$HOME/.dotnet" && dotnet run
   ```

2. **Verificar servidor online**:
   ```bash
   curl http://172.25.63.212:5165/health
   ```

### Teste Automatizado
Execute o script de teste:
```powershell
cd RiderProjects/OliAcessoRemoto
powershell -ExecutionPolicy Bypass -File test-integration.ps1
```

### Teste Manual do Cliente

1. **Compilar o cliente**:
   ```bash
   cd RiderProjects/OliAcessoRemoto
   dotnet build
   ```

2. **Executar o cliente**:
   ```bash
   dotnet run
   ```

3. **Verificar integração**:
   - Cliente deve se registrar automaticamente
   - ID deve ser obtido do servidor (não mais simulado)
   - Status deve mostrar "Conectado ao servidor"

## 🔄 Fluxo de Funcionamento

### Inicialização do Cliente
1. Cliente inicia e verifica saúde do servidor
2. Se servidor online, registra cliente automaticamente
3. Recebe ID único (XXX XXX XXX) e token JWT
4. Configura autenticação para próximas chamadas
5. Atualiza UI com status de conexão

### Solicitação de Conexão
1. Usuário digita ID do cliente remoto
2. Cliente verifica se ID de destino está online
3. Se online, envia solicitação através da API
4. Servidor processa e notifica cliente de destino
5. Aguarda resposta do usuário remoto

### Tratamento de Erros
- Servidor offline: Modo offline com funcionalidade limitada
- Cliente de destino offline: Notificação ao usuário
- Erros de rede: Mensagens informativas
- Falhas de autenticação: Re-registro automático

## 📊 Monitoramento

### Logs do Servidor
O servidor registra todas as atividades:
- Registro de clientes
- Solicitações de conexão
- Status de clientes
- Erros e exceções

### Logs do Cliente
- Conexão com servidor
- Registro e autenticação
- Solicitações de conexão
- Erros de comunicação

## 🔧 Configurações Avançadas

### Alterar URL do Servidor
Edite `Services/ServerApiService.cs`:
```csharp
public ServerApiService(string baseUrl = "http://SEU_IP:PORTA")
```

### Configurar Timeout
```csharp
_httpClient.Timeout = TimeSpan.FromSeconds(30);
```

### Habilitar Logs Detalhados
```csharp
// Adicionar logging HTTP
services.AddHttpClient<ServerApiService>()
    .AddLogging();
```

## 🐛 Solução de Problemas

### Cliente não conecta ao servidor
1. Verificar se servidor está rodando
2. Testar conectividade: `curl http://172.25.63.212:5165/health`
3. Verificar firewall/rede
4. Conferir URL no código

### Erro de autenticação
1. Verificar se token JWT está sendo enviado
2. Conferir configuração de chaves JWT
3. Verificar expiração do token

### ID não é gerado
1. Verificar resposta da API de registro
2. Conferir logs do servidor
3. Testar endpoint manualmente

## 📈 Próximos Passos

### Funcionalidades Pendentes
1. **SignalR Hub**: Comunicação em tempo real
2. **Controle Remoto**: Captura de tela e eventos
3. **Transferência de Arquivos**: Upload/download
4. **Chat**: Mensagens durante sessão
5. **Gravação**: Registro de sessões

### Melhorias Sugeridas
1. **Reconexão Automática**: Em caso de perda de conexão
2. **Cache Local**: Para funcionamento offline
3. **Criptografia**: Dados sensíveis
4. **Compressão**: Otimização de tráfego
5. **Multi-monitor**: Suporte a múltiplas telas

## ✅ Status da Integração

- ✅ **Comunicação HTTP**: Funcionando
- ✅ **Registro de Cliente**: Funcionando  
- ✅ **Autenticação JWT**: Funcionando
- ✅ **Verificação de Status**: Funcionando
- ✅ **Solicitação de Conexão**: Funcionando
- ⏳ **SignalR Real-time**: Pendente
- ⏳ **Controle Remoto**: Pendente
- ⏳ **Transferência de Arquivos**: Pendente

A integração básica está **100% funcional** para testes de conectividade e gerenciamento de clientes!
