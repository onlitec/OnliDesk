# 🖥️ OnliDesk Server - Sistema de Acesso Remoto

Servidor ASP.NET Core 9.0 para sistema de acesso remoto OnliDesk desenvolvido pela OnliTec.

## 🏗️ Arquitetura

### Servidor OnliDek
- **Tecnologia**: ASP.NET Core 9.0
- **Banco de Dados**: SQLite
- **Autenticação**: JWT
- **Comunicação**: REST API + SignalR
- **Localização**: `RiderProjects/OliAcessoRemoto.Servidor/`

### Cliente WPF
- **Tecnologia**: WPF .NET 9.0
- **Interface**: Windows Presentation Foundation
- **Comunicação**: HTTP Client + SignalR Client
- **Localização**: `RiderProjects/OliAcessoRemoto/`

## 🚀 Funcionalidades

### ✅ Implementadas
- **Registro de Clientes**: Geração automática de IDs únicos (XXX XXX XXX)
- **Autenticação JWT**: Sistema seguro de tokens
- **API REST**: Endpoints para gerenciamento de clientes
- **Banco de Dados**: Persistência com SQLite
- **Interface WPF**: Cliente desktop funcional
- **Integração**: Comunicação cliente-servidor

### 🔄 Em Desenvolvimento
- **SignalR Hub**: Comunicação em tempo real
- **Controle Remoto**: Captura de tela e eventos
- **Transferência de Arquivos**: Upload/download
- **Chat**: Mensagens durante sessão

## 🛠️ Requisitos

- **.NET 9.0 SDK**
- **Windows** (para cliente WPF)
- **Linux/Windows** (para servidor)
- **SQLite** (incluído)

## 📦 Instalação

### Servidor
```bash
cd RiderProjects/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor
dotnet restore
dotnet run
```

### Cliente
```bash
cd RiderProjects/OliAcessoRemoto
dotnet restore
dotnet run
```

## 🔧 Configuração

### Servidor (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=onlidek.db"
  },
  "Jwt": {
    "Key": "OnliDekRemoteAccessSecretKey2024!",
    "Issuer": "OnliDekServer",
    "Audience": "OnliDekClients"
  }
}
```

### Cliente
- Configuração automática via `ServerApiService`
- URL padrão: `http://localhost:5165`

## 🧪 Testes

### Verificar Servidor
```bash
curl http://localhost:5165/health
```

### Registrar Cliente
```bash
curl -X POST http://localhost:5165/api/client/register \
  -H "Content-Type: application/json" \
  -d '{"name": "Cliente Teste", "connectionInfo": "{}"}'
```

## 📊 API Endpoints

- `GET /health` - Verificação de saúde
- `POST /api/client/register` - Registrar cliente
- `GET /api/client/status/{clientId}` - Status do cliente
- `POST /api/client/connection/request` - Solicitar conexão
- `GET /api/client/clients/online` - Listar clientes online

## 🏷️ Branches

- `main` - Versão estável
- `server-implementation` - Implementação do servidor
- `client-integration` - Integração do cliente
- `feature/*` - Novas funcionalidades
- `hotfix/*` - Correções urgentes

## 👥 Desenvolvimento

### Estrutura do Projeto
```
OnliDesk/
├── RiderProjects/
│   ├── OliAcessoRemoto.Servidor/     # Servidor ASP.NET Core
│   │   └── OliAcessoRemoto.Servidor/
│   │       └── OliAcessoRemoto.Servidor/
│   └── OliAcessoRemoto/              # Cliente WPF
├── docs/                             # Documentação
├── tests/                            # Testes
└── scripts/                          # Scripts de build/deploy
```

### Padrões de Commit
- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Documentação
- `style:` - Formatação
- `refactor:` - Refatoração
- `test:` - Testes
- `chore:` - Manutenção

## 📝 Licença

Propriedade da OnliTec - Todos os direitos reservados.

## 📞 Contato

- **Email**: galvatec@gmail.com
- **Desenvolvedor**: onlitec
- **Repositório**: https://github.com/onlitec/OnliDesk

---

**Status**: 🟢 Servidor funcional, Cliente em integração
**Versão**: 1.0.0-beta
**Última Atualização**: $(date)
