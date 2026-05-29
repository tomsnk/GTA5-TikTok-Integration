using GTA;
using GTA.Math;
using GTA.UI;
using GTAVWebhook.Types;
using System;
using System.Collections.Generic;
using System.Drawing;

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

        // ==========================================
        // KONFIGURASI BATAS DARAH (HEALTH LIMIT)
        // ==========================================
        // Nilai standar darah NPC GTA V adalah 200. Anda bisa mengubah angka ini
        // untuk membatasi atau mempertebal darah maksimal NPC kiriman TikTok.
        private int npcMaxHealthLimit = 350;

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

                // Draw names and health bars of all spawned NPCs
                foreach (var npc in spawnedNPCs)
                {
                    npc.DrawName();
                    DrawHealthBar(npc); // <--- MENAMPILKAN BAR DARAH DI ATAS KEPALA
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
        /// Menggambar Bar Darah di atas kepala NPC menggunakan koordinat layar 3D ke 2D
        /// </summary>
        private void DrawHealthBar(MeleeNPCSpawner npc)
        {
            // PENTING: Kode ini mengasumsikan kelas 'MeleeNPCSpawner' Anda memiliki properti 'GetPed()' 
            // yang mengekspos objek GTA.Ped asli. Jika namanya berbeda (misal .Character / .Npc), sesuaikan di bawah ini.
            if (npc == null || npc.GetPed() == null || !npc.GetPed().Exists() || !npc.IsAlive())
                return;

            // Mengambil posisi kepala NPC dan memberi sedikit jarak ke atas (Z + 1.2f)
            Vector3 worldPosition = npc.GetPed().Position + new Vector3(0, 0, 1.2f);
            PointF screenPosition = Screen.WorldToScreen(worldPosition);

            // Jika NPC tidak terlihat di layar kamera game, lewati proses render
            if (screenPosition.X == 0 && screenPosition.Y == 0)
                return;

            // Menghitung rasio darah tersisa
            float maxHealth = npc.GetPed().MaxHealth;
            float currentHealth = npc.GetPed().Health;
            
            if (currentHealth < 0) currentHealth = 0;
            float healthRatio = Math.Max(0f, Math.Min(1f, currentHealth / maxHealth));

            // Pengaturan dimensi ukuran Bar Darah (Canvas GTA menggunakan skala standar 1280x720)
            float barWidth = 45f;
            float barHeight = 5f;

            // Menyelaraskan posisi bar agar tepat berada di tengah atas kepala
            PointF barPos = new PointF(screenPosition.X - (barWidth / 2f), screenPosition.Y);

            // 1. Gambar Background Bar (Warna Hitam Transparan)
            new RectangleElement(barPos, new SizeF(barWidth, barHeight), Color.FromArgb(120, 0, 0, 0)).Draw();

            // 2. Tentukan warna bar dinamis berdasarkan sisa darah
            Color healthColor = Color.FromArgb(200, 0, 255, 0); // Hijau (Darah Sehat)
            if (healthRatio < 0.25f)
                healthColor = Color.FromArgb(200, 255, 0, 0);  // Merah (Kritis)
            else if (healthRatio < 0.60f)
                healthColor = Color.FromArgb(200, 255, 230, 0); // Kuning (Setengah Darah)

            // 3. Gambar Isi Bar Darah Utama (Sesuai sisa healthRatio)
            float currentBarWidth = barWidth * healthRatio;
            new RectangleElement(barPos, new SizeF(currentBarWidth, barHeight), healthColor).Draw();
        }

        /// <summary>
        /// Spawns melee NPCs based on webhook command
        /// </summary>
        private void HandleSpawnMeleeCommand(CommandInfo command, string username)
        {
            int count = 1;

            if (!string.IsNullOrEmpty(command.custom))
            {
                if (int.TryParse(command.custom, out int parsedCount) && parsedCount > 0)
                {
                    count = Math.Min(parsedCount, 10); 
                }
            }

            wave++;
            var spawners = MeleeNPCSpawner.SpawnMultiple(count, $"{username}_Wave{wave}");

            // ==========================================
            // MENERAPKAN BATAS DARAH MAKSIMAL (HEALTH LIMIT)
            // ==========================================
            foreach (var spawner in spawners)
            {
                if (spawner != null && spawner.GetPed() != null && spawner.GetPed().Exists())
                {
                    spawner.GetPed().MaxHealth = npcMaxHealthLimit;
                    spawner.GetPed().Health = npcMaxHealthLimit;
                }
            }

            spawnedNPCs.AddRange(spawners);

            if (showNotifications)
            {
                UI.Notify($"~g~{username} spawned {count} melee attacker(s)!");
            }

            Logger.Log($"[TikTok] {username} spawned {count} melee NPC(s) dengan batas darah {npcMaxHealthLimit}");
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
