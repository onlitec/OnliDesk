# OnliDek Server - OliAcesso Remoto

A comprehensive ASP.NET Core server application for the OliAcesso Remoto ecosystem, providing real-time remote desktop functionality with SignalR, JWT authentication, and Entity Framework Core.

## 🚀 Features Implemented

### ✅ Core Infrastructure
- **ASP.NET Core 9.0** Web API with SignalR
- **Entity Framework Core** with SQL Server
- **JWT Authentication** with role-based access
- **Serilog** structured logging
- **Rate Limiting** for security
- **CORS** configuration for cross-origin requests

### ✅ Database Schema
- **Clients Table**: Manages client registration and status
- **ActiveSessions Table**: Tracks remote desktop sessions
- **ConnectionLogs Table**: Audit trail for all activities

### ✅ Real-time Communication
- **SignalR Hub** (`RemoteAccessHub`) for real-time messaging
- Client registration and authentication
- Connection request/response flow
- Screen sharing data transmission
- Input event forwarding (mouse/keyboard)

### ✅ API Endpoints
- `POST /api/client/register` - Register new client
- `GET /api/client/status/{clientId}` - Get client status
- `POST /api/client/connection/request` - Request connection
- `GET /api/client/clients/online` - List online clients
- `GET /health` - Health check endpoint

### ✅ Security Features
- JWT token-based authentication
- Client session management
- Connection authorization
- Rate limiting (100 requests/minute)
- Audit logging for all activities

## 🏗️ Architecture

```
OliAcessoRemoto.Servidor/
├── Controllers/          # API endpoints
├── Hubs/                # SignalR hub for real-time communication
├── Models/              # Data models and DTOs
├── Services/            # Business logic services
├── Data/                # Entity Framework context
└── Configuration/       # Server configuration
```

## 🔧 Configuration

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OnliDekRemoteAccess;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Server Settings
```json
{
  "ServerSettings": {
    "MaxConcurrentConnections": 1000,
    "ClientTimeoutMinutes": 30,
    "ScreenUpdateIntervalMs": 100,
    "CompressionLevel": "Medium"
  }
}
```

## 🚦 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server or LocalDB
- Visual Studio 2022 or JetBrains Rider

### Running the Server
1. Restore NuGet packages
2. Update database connection string in `appsettings.json`
3. Run the application - database will be created automatically
4. Server will be available at `https://localhost:7000`

### Testing the API
Use the provided `OliAcessoRemoto.Servidor.http` file to test endpoints:
- Health check: `GET /health`
- Register client: `POST /api/client/register`
- Get online clients: `GET /api/client/clients/online`

## 🔌 SignalR Hub Methods

### Client Methods
- `RegisterClient(clientInfo)` - Register client with server
- `RequestConnection(targetId, requesterId)` - Request connection to target
- `RespondToConnection(requesterId, approved)` - Approve/deny connection
- `SendScreenData(targetId, screenData)` - Send screen capture data
- `SendInputEvent(targetId, inputEvent)` - Send mouse/keyboard events

### Server Events
- `RegistrationSuccess/Failed` - Client registration result
- `IncomingConnectionRequest` - Incoming connection request
- `ConnectionApproved/Denied` - Connection response
- `ReceiveScreenData` - Screen data from remote client
- `ReceiveInputEvent` - Input events from controller

## 🔐 Authentication Flow

1. Client registers via `/api/client/register`
2. Server generates unique 9-digit ID (XXX XXX XXX format)
3. Server returns JWT token for authentication
4. Client uses token for API calls and SignalR connection
5. Token contains client ID for session management

## 📊 Logging

Structured logging with Serilog:
- Console output for development
- File logging: `logs/onlidek-server-{date}.txt`
- Connection activities and errors tracked

## 🔄 Background Services

- **CleanupService**: Automatically cleans up expired sessions every 5 minutes
- **Session Management**: Tracks active connections and timeouts

## 🎯 Next Steps

The server is ready for integration with the existing WPF client. Key integration points:
1. Update client to use real API endpoints instead of simulation
2. Implement SignalR client connection in WPF app
3. Add screen capture and input forwarding functionality
4. Implement proper error handling and reconnection logic

## 📝 API Documentation

The server includes OpenAPI/Swagger documentation available at `/swagger` in development mode.

---

**Status**: ✅ Core server implementation complete and ready for client integration
**Technology Stack**: ASP.NET Core 9.0, SignalR, Entity Framework Core, JWT, Serilog
**Database**: SQL Server with automatic schema creation
