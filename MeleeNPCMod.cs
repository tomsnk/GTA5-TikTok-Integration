using GTA;
using GTA.Math;
using GTA.UI;
using GTAVWebhook.Types;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GTAVWebhook.Mods
{
    /// <summary>
    /// Example mod: Spawn NPCs with random melee weapons
    /// Press F5 to spawn a single melee NPC
    /// Press F6 to spawn 5 melee NPCs in a circle
    /// Press F7 to remove all spawned NPCs
    /// </summary>
    public class MeleeNPCMod : Script
    {
        private List<MeleeNPCSpawner> spawnedNPCs = new List<MeleeNPCSpawner>();
        private bool showHelpText = true;

        public MeleeNPCMod()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
            
            UI.Notify("Melee NPC Spawner Mod Loaded");
            UI.Notify("Press F5: Spawn 1 NPC");
            UI.Notify("Press F6: Spawn 5 NPCs");
            UI.Notify("Press F7: Remove All");
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Draw names of all spawned NPCs
            foreach (var npc in spawnedNPCs)
            {
                npc.DrawName();
            }

            // Remove dead NPCs from the list
            spawnedNPCs.RemoveAll(n => !n.IsAlive());

            // Display help text on screen
            if (showHelpText)
            {
                var textElements = new List<string>
                {
                    "~r~[MELEE NPC SPAWNER]",
                    "~b~F5~s~ - Spawn Single NPC",
                    "~b~F6~s~ - Spawn 5 NPCs",
                    "~b~F7~s~ - Remove All NPCs",
                    $"~g~Active NPCs: {spawnedNPCs.Count}",
                };

                float yOffset = 0.1f;
                foreach (var text in textElements)
                {
                    new TextElement(text, new PointF(10, 100 + (yOffset * 20)), 0.5f).Draw();
                    yOffset += 1f;
                }
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F5:
                    // Spawn a single melee NPC
                    SpawnSingleNPC();
                    e.Handled = true;
                    break;

                case Keys.F6:
                    // Spawn multiple melee NPCs
                    SpawnMultipleNPCs(5);
                    e.Handled = true;
                    break;

                case Keys.F7:
                    // Remove all spawned NPCs
                    RemoveAllNPCs();
                    e.Handled = true;
                    break;

                case Keys.F8:
                    // Toggle help text
                    showHelpText = !showHelpText;
                    e.Handled = true;
                    break;
            }
        }

        private void SpawnSingleNPC()
        {
            try
            {
                var spawner = new MeleeNPCSpawner($"Melee_{spawnedNPCs.Count + 1}", 5f);
                spawnedNPCs.Add(spawner);

                WeaponHash weapon = spawner.GetCurrentWeapon();
                UI.Notify($"~g~Spawned NPC with {weapon}!");
            }
            catch (Exception ex)
            {
                UI.Notify($"~r~Error: {ex.Message}");
            }
        }

        private void SpawnMultipleNPCs(int count)
        {
            try
            {
                var spawners = MeleeNPCSpawner.SpawnMultiple(count, $"Wave_{spawnedNPCs.Count}");
                spawnedNPCs.AddRange(spawners);

                UI.Notify($"~g~Spawned {count} NPCs!");
            }
            catch (Exception ex)
            {
                UI.Notify($"~r~Error: {ex.Message}");
            }
        }

        private void RemoveAllNPCs()
        {
            foreach (var npc in spawnedNPCs)
            {
                npc.Remove();
            }
            spawnedNPCs.Clear();

            UI.Notify("~r~All NPCs removed!");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RemoveAllNPCs();
                Tick -= OnTick;
                KeyDown -= OnKeyDown;
            }

            base.Dispose(disposing);
        }
    }
}
