# SmartMeterProject
# Smart Meter System

A C# smart-meter simulation. 
Console clients act as smart meters and send randomly spaced electricity readings (every 15-60 seconds) to a server over a secure TLS
connection. The server verifies, stores and bills the readings, pushes live bills and
grid alerts back to the clients, and logs communication errors. Messages are sent as JSON.

## Technologies
- C# on .NET 8 (console applications)
- TCP sockets with TLS (`TcpListener`, `TcpClient`, `SslStream`)
- JSON messages (`System.Text.Json`)
- NUnit for automated tests

## Docker commands
```
docker run --rm --name server-box smartmeter-server
docker run --rm --network container:server-box smartmeter-client
```

## Requirements
- Windows with Visual Studio 2022 
- .NET 8 SDK 

## Get the code
```
git clone https://github.com/riyashanmugaraj-shu/SmartMeterProject.git
cd SmartMeterProject
```

## Build
```
dotnet build client.slnx
```
Or in Visual Studio: open `client.slnx`, then **Build > Rebuild Solution**.

## Run
1. **Start the server first**:
   ```
   cd server
   dotnet run
   ```
   Wait for `Server has started listening on port: 7000`.
   (The server loads `server.pfx` from its own folder, so run it from inside `server`,
   or start it from Visual Studio.)

2. **Start the clients.**
   - One client, from the repo root: `dotnet run --project client -- client-01`
   - 12 clients at once: run `run_12_clients.bat` from the repo root, or in PowerShell:
 
   


Each client window shows the readings it sends, the server's `ACK` replies, and the live
bill, which updates every time a reading is sent.

## Run the tests
```
dotnet test client.slnx
```
The tests include black-box unit tests, white-box unit tests, and integration tests over a real TCP connection.

## Project structure
| Folder | Purpose |
|---|---|
| `client` | Console smart-meter client: generates and sends readings, shows live bills and alerts |
| `server` | TLS server: concurrent client handling, storage, billing, alerts, error logging |
| `SmartMeter.Shared` | Code used by both sides (`Reading`, JSON adapter) |
| `SmartMeter.Tests` | Automated tests |

## Architecture
Client-server (2-tier), with a layered server: communication, service and data layers.


## Design patterns
| Pattern |
|---|---|
| Adapter | converts readings to and from JSON on the client and the server |
| Singleton | stores all readings, shared by every client handler |
| Strategy | the bill calculation can be swapped |
| Observer | the client reacts to bills and alerts pushed by the server |
| Factory Method | creates the grid alerts |

## Security
Connections use TLS (`SslStream`). A self-signed development certificate (`server/server.pfx`)
is included so the project runs straight after cloning. It is for this coursework only.
[Hana: describe how each connection is authenticated before the client connects.]

## Team
| Member | Contribution |
|---|---|
| Hana | Client-server connection; per-connection server authentication; command-line design; Docker integration; Factory tests |
| Daniya | Architecture diagram; realistic spaced readings from 12 smart meters (client, Adapter); concurrent server that receives readings; reading storage (Singleton); tests |
| Riya | Live bill calculation pushed to clients (Strategy); server error logging for communication problems; grid alerts; command-line design; Docker integration |
| Celyn | Test strategy; client live-bill display; grid alerts pushed to clients; unit and black-box testing |

## Sources
