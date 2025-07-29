#!/bin/bash

# OnliDesk Server Deployment Script
# Build and deploy to Ubuntu server

set -e

echo "📦 OnliDesk Server Deployment Script"
echo "===================================="

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Configuration
SERVER_IP="172.20.120.40"
SERVER_USER="your-username"  # Change this to your username
DEPLOY_PATH="/opt/onlidesk-server/app"
BUILD_PATH="./publish"

# Get server user
read -p "Enter server username [$SERVER_USER]: " input_user
SERVER_USER=${input_user:-$SERVER_USER}

print_step "1. Building OnliDesk Server..."
dotnet clean OnliDesk-Server/
dotnet restore OnliDesk-Server/
dotnet publish OnliDesk-Server/ \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained false \
    --output $BUILD_PATH

print_status "✅ Build completed successfully!"

print_step "2. Creating deployment package..."
cd $BUILD_PATH
tar -czf ../onlidesk-server-deploy.tar.gz *
cd ..

print_status "✅ Deployment package created: onlidesk-server-deploy.tar.gz"

print_step "3. Uploading to server..."
scp onlidesk-server-deploy.tar.gz $SERVER_USER@$SERVER_IP:/tmp/

print_step "4. Deploying on server..."
ssh $SERVER_USER@$SERVER_IP << 'EOF'
    # Stop service if running
    sudo systemctl stop onlidesk-server || true
    
    # Backup current version
    sudo cp -r /opt/onlidesk-server/app /opt/onlidesk-server/app.backup.$(date +%Y%m%d_%H%M%S) || true
    
    # Clear app directory
    sudo rm -rf /opt/onlidesk-server/app/*
    
    # Extract new version
    cd /opt/onlidesk-server/app
    sudo tar -xzf /tmp/onlidesk-server-deploy.tar.gz
    
    # Set permissions
    sudo chown -R onlidesk:onlidesk /opt/onlidesk-server/app
    sudo chmod +x OnliDesk.Server || true
    
    # Clean up
    rm /tmp/onlidesk-server-deploy.tar.gz
    
    # Start service
    sudo systemctl start onlidesk-server
    sudo systemctl status onlidesk-server --no-pager
EOF

print_step "5. Verifying deployment..."
sleep 5

# Test health endpoint
if curl -f -s http://$SERVER_IP/health > /dev/null; then
    print_status "✅ Deployment successful! Server is responding."
    echo ""
    echo "🌐 OnliDesk Server is now running at:"
    echo "   http://$SERVER_IP"
    echo "   http://$SERVER_IP/health"
    echo "   http://$SERVER_IP/swagger"
else
    print_warning "⚠️  Server might still be starting. Check status with:"
    echo "   ssh $SERVER_USER@$SERVER_IP 'sudo systemctl status onlidesk-server'"
fi

# Cleanup local files
rm -f onlidesk-server-deploy.tar.gz
rm -rf $BUILD_PATH

print_status "✅ Deployment completed!"
echo ""
echo "📝 Useful commands:"
echo "   Check status: ssh $SERVER_USER@$SERVER_IP 'sudo systemctl status onlidesk-server'"
echo "   View logs:    ssh $SERVER_USER@$SERVER_IP 'sudo journalctl -u onlidesk-server -f'"
echo "   Restart:      ssh $SERVER_USER@$SERVER_IP 'sudo systemctl restart onlidesk-server'"
