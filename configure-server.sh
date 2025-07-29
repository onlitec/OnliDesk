#!/bin/bash

# OnliDesk Server Configuration Script
# Configure systemd service, nginx, and firewall

set -e

echo "⚙️  OnliDesk Server Configuration Script"
echo "========================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Get server IP
SERVER_IP="172.20.120.40"
DOMAIN_NAME=""

echo "Server IP: $SERVER_IP"
read -p "Enter domain name (optional, press Enter to skip): " DOMAIN_NAME

# Create systemd service file
print_step "1. Creating systemd service..."
sudo tee /etc/systemd/system/onlidesk-server.service > /dev/null <<EOF
[Unit]
Description=OnliDesk Server
After=network.target

[Service]
Type=notify
User=onlidesk
Group=onlidesk
WorkingDirectory=/opt/onlidesk-server/app
ExecStart=/usr/bin/dotnet OnliDesk.Server.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=onlidesk-server
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

# Logging
StandardOutput=journal
StandardError=journal

# Security settings
NoNewPrivileges=yes
PrivateTmp=yes
ProtectSystem=strict
ReadWritePaths=/opt/onlidesk-server/data
ReadWritePaths=/var/log/onlidesk

[Install]
WantedBy=multi-user.target
EOF

# Create production appsettings
print_step "2. Creating production configuration..."
sudo tee /opt/onlidesk-server/app/appsettings.Production.json > /dev/null <<EOF
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    },
    "Console": {
      "IncludeScopes": false
    },
    "File": {
      "Path": "/var/log/onlidesk/onlidesk-server.log",
      "MinLevel": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/opt/onlidesk-server/data/onlidesk.db"
  },
  "Jwt": {
    "Key": "OnliDesk-Production-Secret-Key-2024-$(openssl rand -base64 32)",
    "Issuer": "OnliDesk.Server",
    "Audience": "OnliDesk.Client",
    "ExpiryMinutes": 60
  },
  "OnliDesk": {
    "ServerName": "OnliDesk Server",
    "Version": "1.0.0",
    "MaxClients": 1000,
    "SessionTimeoutMinutes": 30,
    "CleanupIntervalMinutes": 5,
    "ServerUrl": "http://$SERVER_IP:5000"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
EOF

# Configure Nginx
print_step "3. Configuring Nginx reverse proxy..."
if [ -n "$DOMAIN_NAME" ]; then
    SERVER_NAME="$DOMAIN_NAME"
else
    SERVER_NAME="$SERVER_IP"
fi

sudo tee /etc/nginx/sites-available/onlidesk-server > /dev/null <<EOF
server {
    listen 80;
    server_name $SERVER_NAME;
    
    # Security headers
    add_header X-Frame-Options DENY;
    add_header X-Content-Type-Options nosniff;
    add_header X-XSS-Protection "1; mode=block";
    
    # Proxy to OnliDesk Server
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }
    
    # SignalR WebSocket support
    location /remotehub {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
        
        # WebSocket timeouts
        proxy_read_timeout 86400;
    }
    
    # Health check endpoint
    location /health {
        proxy_pass http://localhost:5000/health;
        access_log off;
    }
}
EOF

# Enable nginx site
sudo ln -sf /etc/nginx/sites-available/onlidesk-server /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx

# Configure firewall
print_step "4. Configuring firewall..."
sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw --force enable

# Set permissions
print_step "5. Setting permissions..."
sudo chown -R onlidesk:onlidesk /opt/onlidesk-server
sudo chmod +x /opt/onlidesk-server/app/OnliDesk.Server.dll || true

# Enable and start services
print_step "6. Enabling services..."
sudo systemctl daemon-reload
sudo systemctl enable onlidesk-server
sudo systemctl enable nginx

print_status "✅ Configuration completed!"
echo ""
echo "🚀 To start OnliDesk Server:"
echo "   sudo systemctl start onlidesk-server"
echo ""
echo "📊 To check status:"
echo "   sudo systemctl status onlidesk-server"
echo ""
echo "📝 To view logs:"
echo "   sudo journalctl -u onlidesk-server -f"
echo ""
echo "🌐 Server will be available at:"
echo "   http://$SERVER_NAME"
echo "   http://$SERVER_NAME/health"
echo "   http://$SERVER_NAME/swagger"
