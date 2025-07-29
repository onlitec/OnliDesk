# 🚀 OnliDesk Server - Guia de Instalação

Guia completo para instalar o OnliDesk Server no Ubuntu Server 22.04 LTS.

## 📋 Pré-requisitos

### Servidor de Destino
- **IP**: 172.20.120.40
- **OS**: Ubuntu Server 22.04 LTS
- **RAM**: Mínimo 2GB (recomendado 4GB+)
- **Disco**: Mínimo 10GB livres
- **Usuário**: Com privilégios sudo

### Máquina de Desenvolvimento
- .NET 8.0 SDK instalado
- Acesso SSH ao servidor
- Git (para clonar o repositório)

## 🔧 Instalação Passo a Passo

### 1. **Preparar Scripts de Instalação**

```bash
# Tornar scripts executáveis
chmod +x install-server.sh
chmod +x configure-server.sh
chmod +x deploy-server.sh
```

### 2. **Copiar Scripts para o Servidor**

```bash
# Copiar scripts de instalação
scp install-server.sh configure-server.sh your-username@172.20.120.40:~/
```

### 3. **Executar Instalação no Servidor**

```bash
# Conectar ao servidor
ssh your-username@172.20.120.40

# Executar instalação do sistema
./install-server.sh

# Executar configuração
./configure-server.sh
```

### 4. **Fazer Deploy da Aplicação**

```bash
# Na máquina de desenvolvimento
./deploy-server.sh
```

## 📊 Verificação da Instalação

### Verificar Status do Serviço
```bash
sudo systemctl status onlidesk-server
```

### Verificar Logs
```bash
sudo journalctl -u onlidesk-server -f
```

### Testar API
```bash
# Health check
curl http://172.20.120.40/health

# Swagger UI
curl http://172.20.120.40/swagger
```

## 🌐 Endpoints Disponíveis

| Endpoint | Descrição |
|----------|-----------|
| `http://172.20.120.40/` | Página principal |
| `http://172.20.120.40/health` | Health check |
| `http://172.20.120.40/swagger` | Documentação da API |
| `http://172.20.120.40/api/client/register` | Registrar cliente |
| `http://172.20.120.40/remotehub` | SignalR Hub |

## 🔧 Comandos de Gerenciamento

### Controlar Serviço
```bash
# Iniciar
sudo systemctl start onlidesk-server

# Parar
sudo systemctl stop onlidesk-server

# Reiniciar
sudo systemctl restart onlidesk-server

# Status
sudo systemctl status onlidesk-server

# Habilitar inicialização automática
sudo systemctl enable onlidesk-server
```

### Logs
```bash
# Logs em tempo real
sudo journalctl -u onlidesk-server -f

# Logs das últimas 100 linhas
sudo journalctl -u onlidesk-server -n 100

# Logs de hoje
sudo journalctl -u onlidesk-server --since today
```

### Nginx
```bash
# Status do Nginx
sudo systemctl status nginx

# Recarregar configuração
sudo systemctl reload nginx

# Testar configuração
sudo nginx -t
```

## 📁 Estrutura de Arquivos

```
/opt/onlidesk-server/
├── app/                          # Aplicação
│   ├── OnliDesk.Server.dll       # Executável principal
│   ├── appsettings.json          # Configuração
│   └── ...                       # Outros arquivos
├── data/                         # Banco de dados
│   └── onlidesk.db              # SQLite database
└── logs/                         # Logs (se configurado)

/var/log/onlidesk/               # Logs do sistema
├── onlidesk-server.log          # Log da aplicação

/etc/systemd/system/             # Serviços
└── onlidesk-server.service      # Serviço systemd

/etc/nginx/sites-available/      # Nginx
└── onlidesk-server              # Configuração do site
```

## 🔒 Segurança

### Firewall (UFW)
```bash
# Verificar status
sudo ufw status

# Permitir portas necessárias
sudo ufw allow 22/tcp   # SSH
sudo ufw allow 80/tcp   # HTTP
sudo ufw allow 443/tcp  # HTTPS (futuro)
```

### SSL/HTTPS (Opcional)
```bash
# Instalar certificado Let's Encrypt
sudo certbot --nginx -d your-domain.com
```

## 🔄 Atualizações

### Deploy de Nova Versão
```bash
# Na máquina de desenvolvimento
./deploy-server.sh
```

### Backup Antes da Atualização
```bash
# Backup do banco de dados
sudo cp /opt/onlidesk-server/data/onlidesk.db /opt/onlidesk-server/data/onlidesk.db.backup.$(date +%Y%m%d)

# Backup da aplicação
sudo cp -r /opt/onlidesk-server/app /opt/onlidesk-server/app.backup.$(date +%Y%m%d)
```

## 🆘 Solução de Problemas

### Serviço Não Inicia
```bash
# Verificar logs detalhados
sudo journalctl -u onlidesk-server -n 50

# Verificar permissões
sudo chown -R onlidesk:onlidesk /opt/onlidesk-server

# Verificar se .NET está instalado
dotnet --version
```

### Erro de Conexão com Banco
```bash
# Verificar se o diretório existe
ls -la /opt/onlidesk-server/data/

# Verificar permissões
sudo chown -R onlidesk:onlidesk /opt/onlidesk-server/data/
```

### Nginx Não Funciona
```bash
# Testar configuração
sudo nginx -t

# Verificar se a porta 5000 está em uso
sudo netstat -tlnp | grep :5000
```

## 📞 Suporte

Para suporte técnico:
- **Email**: galvatec@gmail.com
- **GitHub**: https://github.com/onlitec/OnliDesk
- **Issues**: https://github.com/onlitec/OnliDesk/issues

---

**OnliDesk Server v1.0.0 - OnliTec © 2024**
