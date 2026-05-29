# Melee NPC Spawner Mod - Webhook Edition

A custom GTA V mod that spawns NPCs with random melee weapons triggered by HTTP Webhook signals from TikTok interactions, built on the GTA5-TikTok-Integration framework.

## Features

- **Webhook-Triggered**: Spawns NPCs in response to TikTok interactions via HTTP webhooks
- **Random Melee Weapons**: Spawns NPCs with randomly selected melee weapons including:
  - Baseball Bat
  - Crowbar
  - Knife
  - Machete
  - Flashlight
  - Hammer
  - Hatchet
  - Pool Cue
  - Golf Club
  - Wrench
  - Pipe
  - Night Stick
  - Switchblade
  - Dagger
  - Katana
  - Unarmed

- **Flexible Spawning**: 
  - Spawn 1 or multiple NPCs per command
  - Spawn multiple NPCs in a circular formation
  - Clear all NPCs on demand
  - Automatic NPC cleanup for dead NPCs
  - Display NPC names with TikTok username

## Files Used

1. **Types/MeleeNPCSpawner.cs** - Core spawner class with melee weapon logic
2. **MeleeNPCMod.cs** - Webhook handler that listens for HTTP signals

## Webhook Commands

### Spawn Melee NPCs
```
GET /cmd?cmd=spawnmelee&custom=1&username=TikTokUser
```
- `cmd`: `spawnmelee` - Spawn NPC command
- `custom`: Number of NPCs to spawn (1-10, default: 1)
- `username`: TikTok username of the person triggering the command

**Example:**
- `/cmd?cmd=spawnmelee&custom=1&username=john_doe` - Spawns 1 NPC
- `/cmd?cmd=spawnmelee&custom=5&username=jane_smith` - Spawns 5 NPCs

### Clear All Melee NPCs
```
GET /cmd?cmd=clearmelee&username=TikTokUser
```
- `cmd`: `clearmelee` - Clear all NPCs command
- `username`: TikTok username

## How It Works

1. **TikTok Integration**: TikTok events (follows, gifts, donations, etc.) are sent to your webhook server
2. **HTTP Request**: The webhook server receives HTTP GET requests with command parameters
3. **NPC Spawning**: The mod receives the command and spawns melee NPCs around the player
4. **Combat**: Spawned NPCs automatically engage in combat with the player
5. **Tracking**: The mod tracks all spawned NPCs and removes dead ones automatically
6. **Display**: NPC names are displayed on-screen with the TikTok username

## Installation

1. Copy the files to your GTA5-TikTok-Integration project:
   - `Types/MeleeNPCSpawner.cs`
   - `MeleeNPCMod.cs`

2. Add the new files to your .csproj file:
   ```xml
   <Compile Include="Types/MeleeNPCSpawner.cs" />
   <Compile Include="MeleeNPCMod.cs" />
   ```

3. Build the project in Visual Studio

4. Integrate the command handler in your webhook processing system

5. Load the compiled DLL into GTA V using Script Hook V .NET

## Integration with TikTok Webhook Server

In your webhook server configuration, add routes for melee NPC commands:

```csharp
// In your webhook server setup
server.RegisterCommand("spawnmelee", HandleMeleeSpawn);
server.RegisterCommand("clearmelee", HandleMeleeClear);
```

### Example TikTok Interaction Mappings

- **Follow**: `spawnmelee&custom=1`
- **Gift (Gold Coins)**: `spawnmelee&custom=3`
- **Super Follow**: `spawnmelee&custom=5`
- **Large Gift/Donation**: `spawnmelee&custom=10` (capped at 10)
- **Host/Raid**: `clearmelee` (clears the arena)

## Configuration

### Spawn Limits
- Maximum 10 NPCs per single command
- NPCs are capped to prevent performance issues
- Dead NPCs are automatically cleaned up

### Positioning
- NPCs spawn in a circular formation around the player
- Distance increases with each NPC to avoid overlap
- Elevation adjusted based on player location (vehicle or ground)

### Combat
- All NPCs engage in melee combat with the player
- Weapon selection is random from the available pool
- NPC health is set to 100 by default

## Class Reference

### MeleeNPCWebhookHandler

#### Constructor
```csharp
public MeleeNPCWebhookHandler()
```
- Initializes the webhook handler
- Listens for incoming HTTP commands

#### Private Methods
- `OnTick()` - Main game loop handler
- `HandleWebhookCommand(CommandInfo)` - Routes commands
- `HandleSpawnMeleeCommand(CommandInfo, string)` - Spawns NPCs
- `HandleClearMeleeCommand(string)` - Clears all NPCs

### MeleeNPCSpawner

#### Constructor
```csharp
public MeleeNPCSpawner(string name, float spawnDistance = 5f)
```
- `name`: Display name for the NPC (includes TikTok username)
- `spawnDistance`: Distance to spawn from player (default: 5 units)

#### Public Methods
- `SpawnMultiple(int count, string prefix)` - Static method to spawn multiple NPCs
- `GetPed()` - Returns the spawned Ped object
- `GetName()` - Returns the NPC's name
- `IsAlive()` - Checks if NPC is still alive
- `Remove()` - Deletes the NPC
- `DrawName()` - Renders NPC name on screen
- `GetCurrentWeapon()` - Returns the weapon hash the NPC has
- `GetRandomMeleeWeapon()` - Static method to get a random melee weapon

## Requirements

- GTA V
- Script Hook V
- Script Hook V .NET 3
- Visual Studio 2017+ (for compilation)
- .NET Framework 4.8+
- TikTok Webhook Server configured and running

## Logging

All webhook commands are logged to the in-game logger:
- Successful spawns: `[TikTok] {username} spawned {count} melee NPC(s)`
- Successful clears: `[TikTok] {username} cleared {cleared} melee NPC(s)`
- Errors are logged with exception details

## Performance Considerations

- Each NPC spawned adds to game memory usage
- Spawning many NPCs rapidly may cause performance issues
- Use `clearmelee` command periodically to clean up
- Server-side rate limiting recommended to prevent spam

## Advanced Usage

### Custom Command Parameters

Extend the webhook handler to accept custom parameters:

```csharp
private void HandleSpawnMeleeCommand(CommandInfo command, string username)
{
    // Parse custom parameters
    var parameters = command.custom.Split(',');
    int count = int.Parse(parameters[0]);
    float distance = parameters.Length > 1 ? float.Parse(parameters[1]) : 5f;
    
    // Spawn with custom parameters
}
```

### Event Integration

Map specific TikTok events to different NPC counts:

```csharp
// In your TikTok event handler
if (eventType == "follow")
    SendWebhookCommand("spawnmelee", "1", username);
else if (eventType == "donation" && amount > 100)
    SendWebhookCommand("spawnmelee", "5", username);
```

## Troubleshooting

### NPCs not spawning
- Check that the webhook server is running
- Verify the command format is correct
- Ensure the mod DLL is loaded in GTA V
- Check logs for error messages

### Performance issues
- Reduce the number of NPCs spawned per command
- Clear NPCs more frequently
- Check system resources

### Webhook not connecting
- Verify the correct port is configured
- Check firewall settings
- Ensure TikTok integration server is sending requests

## Future Enhancement Ideas

- Customize NPC behavior per TikTok event type
- Add difficulty scaling with wave progression
- Support for ranged weapon variation
- NPC squad tactics and formations
- Boss NPC spawning for special events
- Leaderboard tracking for event triggers

## License

Built on the GTA5-TikTok-Integration framework. Follow the license terms of the original project.

## Notes

- This mod is for entertainment purposes
- Use responsibly and respect Rockstar Games' terms of service
- Based on Script Hook V by Alexander Blade
- Uses GTA.NET by crosire
- Part of the GTA5-TikTok-Integration project

