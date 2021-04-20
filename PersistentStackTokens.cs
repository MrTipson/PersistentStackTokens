using System;
using System.Collections.ObjectModel;
using System.Timers;
using BepInEx;
using RoR2;
using UnityEngine.Networking;
namespace MrTipson
{
    [BepInPlugin("com.MrTipson.PersistentStackTokens", "Keep Bandit stack tokens between stages", "1.0.0")]
    public class PersistentStackTokens : BaseUnityPlugin
    {
        // Keep track of the stack count for all players in the lobby (as the host)
        private int[] stackCount;
        private double multiplier;
        private string config;
        private Timer timer;
        public void Awake()
        {
            // Save stack counts as stage ends
            SceneExitController.onBeginExit += saveStackTokens;
            // Apply saved counts as stage begins
            Stage.onStageStartGlobal += waitForPlayers;
            // Reset stack count when run starts
            Run.onRunStartGlobal += resetSavedTokens;
            // Read/write the config file
            config = BepInEx.Paths.ConfigPath + "/PersistentStackTokens.cfg";
            if (!(System.IO.File.Exists(config) && double.TryParse(System.IO.File.ReadAllText(config), out multiplier) && multiplier >= 0))
            {
                System.IO.File.WriteAllText(config, "1");
                multiplier = 1;
            }
            // Initialize timer for checking CharacterBody objects
            timer = new Timer(250);
            timer.Elapsed += applySavedTokens;
        }
        private void saveStackTokens(SceneExitController exitController)
        {
            // We shouldnt try to do anything if we arent the host
            if (!NetworkServer.active)
            {
                return;
            }
            ReadOnlyCollection<PlayerCharacterMasterController> players = PlayerCharacterMasterController.instances;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].master.hasBody)
                {
                    CharacterBody body = players[i].master.GetBody();
                    stackCount[i] = (int)(body.GetBuffCount(RoR2Content.Buffs.BanditSkull) * multiplier);
                }
            }
        }
        private void waitForPlayers(Stage stage)
        {
            // We shouldnt try to do anything if we arent the host
            // or in the outro
            if (!NetworkServer.active || stage.sceneDef.cachedName == "outro")
            {
                return;
            }
            timer.Enabled = true;
        }
        private void applySavedTokens(Object source, System.Timers.ElapsedEventArgs e)
        {
            // This will stay in for now
            Logger.LogInfo("Hello my name timer");
            // Keep waiting for players to load
            if(PlayerCharacterMasterController.instances.Count > PlayerCharacterMasterController.GetPlayersWithBodiesCount())
            {
                return;
            }
            // All players are loaded
            // Stop the timer
            timer.Enabled = false;
            ReadOnlyCollection<PlayerCharacterMasterController> players = PlayerCharacterMasterController.instances;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].master.hasBody)
                {
                    CharacterBody body = players[i].master.GetBody();
                    for (int j = 0; j < stackCount[i]; j++)
                    {
                        body.AddBuff(RoR2Content.Buffs.BanditSkull);
                    }
                }
            }
        }
        private void resetSavedTokens(Run run)
        {
            // We shouldnt try to do anything if we arent the host
            if (!NetworkServer.active)
            {
                return;
            }
            stackCount = new int[PlayerCharacterMasterController.instances.Count];
        }
    }
}
