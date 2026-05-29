using GTA;
using GTA.Math;
using GTA.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GTAVWebhook.Types
{
    /// <summary>
    /// Spawns NPCs with random melee weapons
    /// </summary>
    public class MeleeNPCSpawner
    {
        private Ped npc = null;
        private string name = null;

        // Array of melee weapons for random selection
        private static readonly WeaponHash[] MeleeWeapons = new WeaponHash[]
        {
            WeaponHash.Unarmed,
            WeaponHash.Knife,
            WeaponHash.Crowbar,
            WeaponHash.Hammer,
            WeaponHash.Hatchet,
            WeaponHash.Machete,
            WeaponHash.Flashlight,
            WeaponHash.Wrench,
            WeaponHash.BaseballBat,
            WeaponHash.PoolCue,
            WeaponHash.GolfClub,
            WeaponHash.Pipe,
            WeaponHash.NightStick,
            WeaponHash.Switchblade,
            WeaponHash.Dagger,
            WeaponHash.Katana,
        };

        /// <summary>
        /// Spawns an NPC with a random melee weapon
        /// </summary>
        /// <param name="name">Name/identifier for the NPC</param>
        /// <param name="spawnDistance">Distance from player to spawn (default: 5)</param>
        public MeleeNPCSpawner(string name, float spawnDistance = 5f)
        {
            this.name = name;

            try
            {
                Vector3 spawnPosition;

                // Calculate spawn position based on player's position and direction
                if (Game.Player.Character.IsInVehicle())
                {
                    spawnPosition = Game.Player.Character.Position + new Vector3(0, 0, 2) + Game.Player.Character.ForwardVector * spawnDistance;
                }
                else
                {
                    spawnPosition = Game.Player.Character.Position + Game.Player.Character.ForwardVector * spawnDistance;
                }

                // Create a random ped at the spawn position
                npc = World.CreateRandomPed(spawnPosition);

                if (npc != null)
                {
                    // Select a random melee weapon
                    WeaponHash randomWeapon = MeleeWeapons[new Random().Next(MeleeWeapons.Length)];

                    // Give the weapon to the NPC
                    if (randomWeapon != WeaponHash.Unarmed)
                    {
                        npc.Weapons.Give(randomWeapon, 100, true, true);
                    }

                    // Make the NPC fight against the player
                    npc.Task.FightAgainst(Game.Player.Character);

                    // Set health
                    npc.MaxHealth = 100;
                    npc.Health = 100;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error spawning melee NPC: {ex.Message}");
            }
        }

        /// <summary>
        /// Spawns multiple NPCs with random melee weapons
        /// </summary>
        /// <param name="count">Number of NPCs to spawn</param>
        /// <param name="prefix">Name prefix for the NPCs</param>
        /// <returns>List of spawned MeleeNPCSpawner instances</returns>
        public static List<MeleeNPCSpawner> SpawnMultiple(int count, string prefix = "Attacker")
        {
            List<MeleeNPCSpawner> spawners = new List<MeleeNPCSpawner>();

            for (int i = 0; i < count; i++)
            {
                float spawnDistance = 5f + (i * 3f); // Space them out in a circle
                float angle = (360f / count) * i * (float)Math.PI / 180f;
                
                // Adjust spawn position to form a circle around the player
                var offset = new Vector3((float)Math.Cos(angle) * spawnDistance, (float)Math.Sin(angle) * spawnDistance, 0);
                
                // Create individual spawners
                spawners.Add(new MeleeNPCSpawner($"{prefix}_{i + 1}", spawnDistance));
            }

            return spawners;
        }

        /// <summary>
        /// Gets the spawned NPC
        /// </summary>
        public Ped GetPed()
        {
            return npc;
        }

        /// <summary>
        /// Gets the NPC's name
        /// </summary>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Checks if the NPC is still alive
        /// </summary>
        public bool IsAlive()
        {
            return npc != null && npc.IsAlive;
        }

        /// <summary>
        /// Removes the spawned NPC
        /// </summary>
        public void Remove()
        {
            if (npc != null)
            {
                npc.Delete();
                npc = null;
            }
        }

        /// <summary>
        /// Draws the NPC's name on screen if in range and visible
        /// </summary>
        public void DrawName()
        {
            if (npc != null && name != null && World.GetDistance(npc.Position, Game.Player.Character.Position) <= 30 && npc.IsOnScreen)
            {
                PointF pointF = Screen.WorldToScreen(npc.Position, false);
                new TextElement(name, pointF, 0.6f, Color.White, GTA.UI.Font.Pricedown, Alignment.Center).Draw();
            }
        }

        /// <summary>
        /// Gets the weapon the NPC currently has
        /// </summary>
        public WeaponHash GetCurrentWeapon()
        {
            return npc?.Weapons.Current.Hash ?? WeaponHash.Unarmed;
        }

        /// <summary>
        /// Gets a random melee weapon
        /// </summary>
        public static WeaponHash GetRandomMeleeWeapon()
        {
            return MeleeWeapons[new Random().Next(MeleeWeapons.Length)];
        }
    }
}
