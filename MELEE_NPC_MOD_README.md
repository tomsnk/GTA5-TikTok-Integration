# Melee NPC Spawner Mod

A custom GTA V mod that spawns NPCs with random melee weapons, built on top of the GTA5-TikTok-Integration codebase.

## Features

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
  - Spawn single NPCs
  - Spawn multiple NPCs in a circular formation
  - Automatic NPC cleanup for dead NPCs
  - Display NPC names on screen

- **Easy Integration**: Works seamlessly with the existing webhook system

## Files Added

1. **Types/MeleeNPCSpawner.cs** - Core spawner class with melee weapon logic
2. **MeleeNPCMod.cs** - Example mod demonstrating usage with keyboard controls

## Usage

### In-Game Controls (MeleeNPCMod.cs)

- **F5**: Spawn a single NPC with a random melee weapon
- **F6**: Spawn 5 NPCs in a circle around the player
- **F7**: Remove all spawned NPCs
- **F8**: Toggle help text display

### Code Integration

```csharp
using GTAVWebhook.Types;

// Spawn a single NPC with melee weapon
var spawner = new MeleeNPCSpawner("Attacker_1", 5f);

// Spawn multiple NPCs
var spawners = MeleeNPCSpawner.SpawnMultiple(5, "Wave_1");

// Check if NPC is alive
if (spawner.IsAlive())
{
    // Do something
}

// Get the weapon the NPC has
WeaponHash weapon = spawner.GetCurrentWeapon();

// Remove the NPC
spawner.Remove();

// Get random melee weapon
WeaponHash randomWeapon = MeleeNPCSpawner.GetRandomMeleeWeapon();
```

## Installation

1. Copy the files to your GTA5-TikTok-Integration project:
   - `Types/MeleeNPCSpawner.cs`
   - `MeleeNPCMod.cs` (optional - only if you want the standalone mod)

2. Add the new files to your .csproj file:
   ```xml
   <Compile Include="Types/MeleeNPCSpawner.cs" />
   <Compile Include="MeleeNPCMod.cs" />
   ```

3. Build the project in Visual Studio

4. Load the compiled DLL into GTA V using Script Hook V .NET

## Integration with Existing Webhook System

You can also integrate this with the existing webhook system in `GTAVWebhookScript.cs`:

```csharp
public void HandleSpawnMeleeCommand(CommandInfo cmd)
{
    int count = string.IsNullOrEmpty(cmd.custom) ? 1 : int.Parse(cmd.custom);
    var spawners = MeleeNPCSpawner.SpawnMultiple(count, cmd.username);
}
```

## Class Reference

### MeleeNPCSpawner

#### Constructor
```csharp
public MeleeNPCSpawner(string name, float spawnDistance = 5f)
```
- `name`: Display name for the NPC
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

## Technical Details

- NPCs spawn at the player's position + forward vector
- If player is in a vehicle, NPCs spawn slightly elevated
- NPCs automatically engage in combat with the player
- Dead NPCs are automatically removed from tracking
- Names are displayed on screen when within 30 units and visible

## Performance Considerations

- Each NPC spawned adds to game memory usage
- Spawning too many NPCs simultaneously may cause performance issues
- Use `RemoveAllNPCs()` or individual `Remove()` calls to clean up

## Future Enhancement Ideas

- Customize NPC behavior (pacifist, aggressive, etc.)
- Add support for ranged weapons
- Group formations and tactics
- Difficulty scaling
- Wave system with progressively harder NPCs
- Webhook integration for remote spawning

## License

Built on the GTA5-TikTok-Integration framework. Follow the license terms of the original project.

## Notes

- This mod is for educational purposes
- Use responsibly and respect Rockstar Games' terms of service
- Based on Script Hook V by Alexander Blade
- Uses GTA.NET by crosire
