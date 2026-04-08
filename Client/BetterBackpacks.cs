using BepInEx;
using BepInEx.Logging;
using BetterBackpacks.Patches;

namespace BetterBackpacks
{
    [BepInPlugin("com.refringe.betterbackpacks", "BetterBackpacks", "1.0.1")]
    public class BetterBackpacks : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private void Awake()
        {
            LogSource = Logger;

            new CreateGridsPatch().Enable();
            new ContainedGridsViewShowPatch().Enable();

            LogSource.LogInfo("BetterBackpacks loaded!");
        }
    }
}
