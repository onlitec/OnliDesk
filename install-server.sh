#!/bin/bash

# OnliDesk Server Installation Script
# Ubuntu Server 22.04 LTS
# Target: 172.20.120.40

set -e

echo "🚀 OnliDesk Server Installation Script"
echo "======================================="
echo "Target Server: 172.20.120.40"
echo "OS: Ubuntu Server 22.04 LTS"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

print_step() {
    echo -e "${BLUE}[STEP]${NC} $1"
}

# Check if running as root
if [[ $EUID -eq 0 ]]; then
   print_error "This script should not be run as root. Please run as a regular user with sudo privileges."
   exit 1
fi

# Update system
print_step "1. Updating system packages..."
sudo apt update && sudo apt upgrade -y

# Install required packages
print_step "2. Installing required packages..."
sudo apt install -y curl wget git unzip nginx certbot python3-certbot-nginx

# Install .NET 8.0
print_step "3. Installing .NET 8.0 SDK..."
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt update
sudo apt install -y dotnet-sdk-8.0 dotnet-runtime-8.0 aspnetcore-runtime-8.0

# Verify .NET installation
print_status "Verifying .NET installation..."
dotnet --version

# Create application directory
print_step "4. Creating application directory..."
sudo mkdir -p /opt/onlidesk-server
sudo chown $USER:$USER /opt/onlidesk-server

# Create systemd service user
print_step "5. Creating service user..."
sudo useradd --system --home /opt/onlidesk-server --shell /bin/false onlidesk || true

# Create directories
print_step "6. Creating required directories..."
sudo mkdir -p /opt/onlidesk-server/{app,logs,data}
sudo mkdir -p /var/log/onlidesk
sudo chown -R onlidesk:onlidesk /opt/onlidesk-server
sudo chown -R onlidesk:onlidesk /var/log/onlidesk

print_status "✅ System preparation completed!"
print_warning "Next steps:"
echo "1. Copy the OnliDesk Server files to /opt/onlidesk-server/app/"
echo "2. Run the configuration script: ./configure-server.sh"
echo "3. Start the service: sudo systemctl start onlidesk-server"
