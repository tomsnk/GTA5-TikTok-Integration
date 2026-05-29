using GTA;
using GTA.Math;
using GTA.UI;
using GTAVWebhook.Types;
using System;
using System.Collections.Generic;

namespace GTAVWebhook.Mods
{
    /// <summary>
    /// Webhook-triggered mod: Spawn NPCs with random melee weapons via TikTok interactions
    /// 
    /// HTTP Webhook Commands:
    /// - GET /cmd?cmd=spawnmelee&custom=1 - Spawn 1 NPC
    /// - GET /cmd?cmd=spawnmelee&custom=5 - Spawn 5 NPCs
    /// - GET /cmd?cmd=clearmelee - Remove all NPCs
    /// 
    /// Usage with TikTok:
    /// Send webhook from TikTok integration server to trigger melee NPC spawning
    /// </summary>
    public class MeleeNPCWebhookHandler : Script
    {
        private List<MeleeNPCSpawner> spawnedNPCs = new List<MeleeNPCSpawner>();
        private HttpServer httpServer = new HttpServer();
        private int wave = 0;
        private bool showNotifications = true;

        public MeleeNPCWebhookHandler()
        {
            Tick += OnTick;
            
            UI.Notify("~g~Melee NPC Webhook Handler Loaded");
            UI.Notify("~b~Listening for webhook commands...");
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                // Process incoming webhook commands
                CommandInfo command = httpServer.DequeueCommand();

                if (command != null)
                {
                    HandleWebhookCommand(command);
                }

                // Draw names of all spawned NPCs
                foreach (var npc in spawnedNPCs)
                {
                    npc.DrawName();
                }

                // Remove dead NPCs from the list
                spawnedNPCs.RemoveAll(n => !n.IsAlive());

                // Display active NPC count on screen
                if (spawnedNPCs.Count > 0)
                {
                    new TextElement($"~g~Active Melee NPCs: {spawnedNPCs.Count}", new PointF(10, 100), 0.5f).Draw();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in MeleeNPCWebhookHandler.OnTick: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles incoming webhook commands from TikTok
        /// </summary>
        private void HandleWebhookCommand(CommandInfo command)
        {
            if (command == null)
                return;

            string cmd = command.cmd.ToLower();
            string username = command.username ?? "TikTok";

            try
            {
                switch (cmd)
                {
                    case "spawnmelee":
                        HandleSpawnMeleeCommand(command, username);
                        break;

                    case "clearmelee":
                        HandleClearMeleeCommand(username);
                        break;

                    default:
                        // Command not recognized by this handler
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error handling webhook command '{cmd}': {ex.Message}");
                if (showNotifications)
                {
                    UI.Notify($"~r~Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Spawns melee NPCs based on webhook command
        /// Custom parameter: number of NPCs to spawn (default: 1)
        /// </summary>
        private void HandleSpawnMeleeCommand(CommandInfo command, string username)
        {
            int count = 1;

            // Parse the custom parameter for NPC count
            if (!string.IsNullOrEmpty(command.custom))
            {
                if (int.TryParse(command.custom, out int parsedCount) && parsedCount > 0)
                {
                    count = Math.Min(parsedCount, 10); // Cap at 10 NPCs per command
                }
            }

            wave++;
            var spawners = MeleeNPCSpawner.SpawnMultiple(count, $"{username}_Wave{wave}");
            spawnedNPCs.AddRange(spawners);

            if (showNotifications)
            {
                UI.Notify($"~g~{username} spawned {count} melee attacker(s)!");
            }

            Logger.Log($"[TikTok] {username} spawned {count} melee NPC(s)");
        }

        /// <summary>
        /// Clears all spawned melee NPCs based on webhook command
        /// </summary>
        private void HandleClearMeleeCommand(string username)
        {
            int cleared = spawnedNPCs.Count;

            foreach (var npc in spawnedNPCs)
            {
                npc.Remove();
            }
            spawnedNPCs.Clear();

            if (showNotifications)
            {
                UI.Notify($"~y~{username} cleared {cleared} attacker(s)!");
            }

            Logger.Log($"[TikTok] {username} cleared {cleared} melee NPC(s)");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var npc in spawnedNPCs)
                {
                    npc.Remove();
                }
                spawnedNPCs.Clear();

                Tick -= OnTick;
            }

            base.Dispose(disposing);
        }
    }
}
