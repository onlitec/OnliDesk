# 🤝 Contribuindo para o OnliDesk

Obrigado por considerar contribuir para o OnliDesk! Este documento fornece diretrizes para contribuições.

## 📋 Código de Conduta

Este projeto adere ao código de conduta da OnliTec. Ao participar, você deve manter este código.

## 🚀 Como Contribuir

### 1. 🍴 Fork do Repositório
```bash
git clone https://github.com/onlitec/OnliDesk.git
cd OnliDesk
```

### 2. 🌿 Criar Branch
```bash
git checkout -b feature/nova-funcionalidade
# ou
git checkout -b fix/correcao-bug
```

### 3. 💻 Fazer Mudanças
- Siga os padrões de código existentes
- Adicione testes para novas funcionalidades
- Atualize a documentação se necessário

### 4. 🧪 Testar
```bash
# Testar servidor
cd RiderProjects/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor/OliAcessoRemoto.Servidor/
dotnet test

# Testar cliente
cd RiderProjects/OliAcessoRemoto/
dotnet build
```

### 5. 📝 Commit
```bash
git add .
git commit -m "feat: adicionar nova funcionalidade"
```

### 6. 📤 Push e Pull Request
```bash
git push origin feature/nova-funcionalidade
```

## 📏 Padrões de Código

### C# (.NET)
- Use PascalCase para classes, métodos e propriedades
- Use camelCase para variáveis locais
- Use async/await para operações assíncronas
- Adicione comentários XML para APIs públicas

### Commits
Siga o padrão Conventional Commits:
- `feat:` - Nova funcionalidade
- `fix:` - Correção de bug
- `docs:` - Documentação
- `style:` - Formatação
- `refactor:` - Refatoração
- `test:` - Testes
- `chore:` - Manutenção

## 🏗️ Estrutura do Projeto

```
OnliDesk/
├── RiderProjects/
│   ├── OliAcessoRemoto.Servidor/  # Servidor ASP.NET Core
│   └── OliAcessoRemoto/           # Cliente WPF
├── .github/                       # Configurações GitHub
├── docs/                          # Documentação
└── tests/                         # Testes
```

## 🧪 Testes

### Servidor
- Testes unitários para serviços
- Testes de integração para API
- Testes de banco de dados

### Cliente
- Testes unitários para ViewModels
- Testes de integração para serviços
- Testes de UI (quando aplicável)

## 📚 Documentação

- Atualize README.md se necessário
- Adicione comentários XML para APIs públicas
- Documente mudanças breaking changes
- Atualize guias de integração

## 🐛 Reportando Bugs

Use o template de bug report no GitHub Issues:
1. Descreva o problema claramente
2. Inclua passos para reproduzir
3. Adicione screenshots se aplicável
4. Inclua informações do ambiente

## ✨ Solicitando Funcionalidades

Use o template de feature request:
1. Descreva a funcionalidade desejada
2. Explique o caso de uso
3. Considere alternativas
4. Defina critérios de aceitação

## 🔍 Processo de Review

1. **Automated Checks**: CI/CD deve passar
2. **Code Review**: Pelo menos 1 aprovação
3. **Testing**: Testes devem estar incluídos
4. **Documentation**: Documentação atualizada

## 📞 Contato

- **Email**: galvatec@gmail.com
- **GitHub**: @onlitec
- **Issues**: Use GitHub Issues para discussões

## 📄 Licença

Ao contribuir, você concorda que suas contribuições serão licenciadas sob a mesma licença do projeto.

---

**Obrigado por contribuir para o OnliDesk! 🎉**
