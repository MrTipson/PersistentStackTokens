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
        public void Awake()
        {
            // Save stack counts as stage ends
            SceneExitController.onBeginExit += (SceneExitController exitController) =>
            {
                // We shouldnt try to do anything if we arent the host
                if (!NetworkServer.active)
                {
                    return;
                }

                for(int i = 0; i < PlayerCharacterMasterController.instances.Count; i++)
                {
                    CharacterBody body = PlayerCharacterMasterController.instances[i].master.GetBody();
                    if (body)
                    {
                        stackCount[i] = (int)(body.GetBuffCount(RoR2Content.Buffs.BanditSkull) * multiplier);
                        Logger.LogDebug(stackCount[i]);
                    }
                }
            };
            // Apply saved counts as stage begins
            Stage.onStageStartGlobal += (Stage stage) =>
            {
                // We shouldnt try to do anything if we arent the host
                if (!NetworkServer.active)
                {
                    return;
                }

                for(int i = 0; i < PlayerCharacterMasterController.instances.Count; i++)
                {
                    CharacterBody body = PlayerCharacterMasterController.instances[i].master.GetBody();
                    if (body)
                    {
                        for (int j = 0; j < stackCount[i]; j++)
                        {
                            body.AddBuff(RoR2Content.Buffs.BanditSkull);
                        }
                    }
                }
            };
            // Reset stack count when run starts
            Run.onRunStartGlobal += (Run run) =>
            {
                // We shouldnt try to do anything if we arent the host
                if (!NetworkServer.active)
                {
                    return;
                }

                stackCount = new int[PlayerCharacterMasterController.instances.Count];
            };

            config = BepInEx.Paths.ConfigPath + "/PersistentStackTokens.cfg";
            if (!(System.IO.File.Exists(config) && double.TryParse(System.IO.File.ReadAllText(config), out multiplier) && multiplier >= 0))
            {
                System.IO.File.WriteAllText(config, "1");
                multiplier = 1;
            }
        }
    }
}
