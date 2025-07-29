# 📋 Changelog

Todas as mudanças notáveis neste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### 🔄 Em Desenvolvimento
- SignalR Hub para comunicação em tempo real
- Controle remoto com captura de tela
- Transferência de arquivos
- Sistema de chat durante sessões

## [1.0.0-beta] - 2024-12-19

### ✨ Adicionado
- **Servidor OnliDek**: Implementação completa do servidor ASP.NET Core 9.0
- **API REST**: Endpoints completos para gerenciamento de clientes
- **Autenticação JWT**: Sistema seguro de tokens para autenticação
- **Banco de Dados**: Integração com SQLite usando Entity Framework Core
- **Geração de IDs**: Sistema automático de IDs únicos no formato XXX XXX XXX
- **Cliente WPF**: Interface desktop para Windows
- **Integração**: Comunicação completa cliente-servidor
- **Documentação**: Guias completos de instalação e integração
- **CI/CD**: Pipeline automatizado com GitHub Actions
- **Templates**: Templates para Issues e Pull Requests

### 🏗️ Arquitetura
- **Servidor**: ASP.NET Core 9.0 com arquitetura em camadas
- **Cliente**: WPF .NET 9.0 com padrão MVVM
- **Banco**: SQLite com Entity Framework Core
- **Comunicação**: HTTP REST + SignalR (preparado)
- **Autenticação**: JWT com refresh tokens

### 📊 Funcionalidades Implementadas
- ✅ Registro automático de clientes
- ✅ Sistema de autenticação JWT
- ✅ API REST com todos os endpoints
- ✅ Banco SQLite com modelos completos
- ✅ Interface WPF funcional
- ✅ Integração cliente-servidor
- ✅ Documentação completa
- ✅ Testes automatizados
- ✅ CI/CD pipeline

### 🔧 Endpoints da API
- `GET /health` - Verificação de saúde do servidor
- `POST /api/client/register` - Registrar novo cliente
- `GET /api/client/status/{clientId}` - Obter status do cliente
- `POST /api/client/connection/request` - Solicitar conexão
- `GET /api/client/clients/online` - Listar clientes online

### 📦 Estrutura do Projeto
```
OnliDesk/
├── RiderProjects/
│   ├── OliAcessoRemoto.Servidor/     # Servidor ASP.NET Core
│   │   ├── Controllers/              # API Controllers
│   │   ├── Models/                   # Modelos de dados
│   │   ├── Services/                 # Serviços de negócio
│   │   ├── Data/                     # Entity Framework
│   │   └── Hubs/                     # SignalR Hubs
│   └── OliAcessoRemoto/              # Cliente WPF
│       ├── Models/                   # Modelos do cliente
│       ├── Services/                 # Serviços de integração
│       └── Views/                    # Interface WPF
├── .github/                          # Configurações GitHub
├── docs/                             # Documentação
└── tests/                            # Testes
```

### 🧪 Testes
- ✅ Testes de saúde do servidor
- ✅ Testes de registro de clientes
- ✅ Testes de autenticação JWT
- ✅ Testes de integração API
- ✅ Validação de build cliente/servidor

### 📚 Documentação
- ✅ README.md principal
- ✅ Guia de integração cliente-servidor
- ✅ Documentação de API
- ✅ Scripts de teste automatizado
- ✅ Guias de instalação e configuração

### 🔒 Segurança
- ✅ Autenticação JWT com chaves seguras
- ✅ Validação de entrada em todos os endpoints
- ✅ Sanitização de dados
- ✅ Logs de auditoria para atividades

### 🚀 Deploy
- ✅ Build automatizado via GitHub Actions
- ✅ Publicação de artefatos
- ✅ Releases automatizados
- ✅ Suporte a múltiplas plataformas (Linux/Windows)

## [0.1.0] - 2024-12-18

### ✨ Adicionado
- Estrutura inicial do projeto
- Configuração do repositório GitHub
- Documentação básica

---

## 📝 Tipos de Mudanças

- **✨ Added** - para novas funcionalidades
- **🔄 Changed** - para mudanças em funcionalidades existentes
- **⚠️ Deprecated** - para funcionalidades que serão removidas
- **🗑️ Removed** - para funcionalidades removidas
- **🐛 Fixed** - para correções de bugs
- **🔒 Security** - para correções de vulnerabilidades

## 🔗 Links

- [Repositório](https://github.com/onlitec/OnliDesk)
- [Issues](https://github.com/onlitec/OnliDesk/issues)
- [Releases](https://github.com/onlitec/OnliDesk/releases)
