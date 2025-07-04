# XWopcUA - OPC UA Client

A Windows Forms application for OPC UA client functionality with support for both encrypted and non-encrypted connections.

## Features

- **Connection Types**
  - Non-encrypted (Anonymous) connections
  - Encrypted connections with various security policies
  - Username/Password authentication
  - Certificate-based authentication

- **Certificate Management**
  - Import certificates (DER, PEM, PFX formats)
  - Create self-signed certificates
  - Manage trusted/rejected certificate stores
  - Certificate validation

- **Node Operations**
  - Browse OPC UA server address space
  - Read node values
  - Write node values
  - Node search functionality

- **Data Monitoring**
  - Subscribe to node value changes (planned)
  - Historical data access (planned)

- **Logging**
  - Application-level logging
  - Log viewer in main window

## Requirements

- Visual Studio 2019 or later
- .NET Framework 4.7.2
- OPC Foundation OPC UA SDK (installed via NuGet)

## Building the Project

1. Open `XWopcUA.sln` in Visual Studio 2019
2. Restore NuGet packages (should happen automatically)
3. Build the solution (F6 or Build → Build Solution)
4. Run the application (F5)

## Usage

### Connecting to an OPC UA Server

1. **Non-encrypted Connection:**
   - Enter the server URL (e.g., `opc.tcp://localhost:4840`)
   - Select "None" for Security Mode
   - Click "Connect"

2. **Encrypted Connection:**
   - Enter the server URL
   - Select "Sign" or "SignAndEncrypt" for Security Mode
   - Choose a Security Policy (e.g., Basic256Sha256)
   - Optionally enable certificate authentication
   - Click "Connect"

### Certificate Management

1. Click "Cert Manager" to open the certificate management window
2. Import existing certificates or create self-signed certificates
3. Manage trusted and rejected certificate stores

### Node Operations

1. Click "Browse..." to open the node browser
2. Navigate through the server's address space
3. Select a node to read/write values
4. Use the main window to perform read/write operations

## Project Structure

```
XWopcUA/
├── Forms/              # Windows Forms UI
├── Services/           # Business logic services
├── Models/             # Data models
├── Utils/              # Utility classes
└── Properties/         # Project properties
```

## Security Notes

- Certificates are stored in local directories
- Passwords are not persisted
- Follow OPC UA security best practices
- Use encrypted connections for production environments

## License

This project is provided as-is for educational and development purposes.