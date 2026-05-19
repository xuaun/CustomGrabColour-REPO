using CustomGrabColour.PlayerGrabBeam;
using HarmonyLib;

namespace CustomGrabColour.Patch;

internal abstract class MetaManagerPatches
{
    [HarmonyPatch(typeof(MetaManager), "CosmeticColorSet")]
    class MetaManager_CosmeticColorSet_Patch
    {
        private static readonly int GrabberIndex = (int)GrabBeamColourSettings.AvatarColourSource.Grabber;
        private static readonly int ArmRightIndex = (int)GrabBeamColourSettings.AvatarColourSource.ArmRight;

        public static void Postfix(int _index)
        {
            if (_index != GrabberIndex && _index != ArmRightIndex) return;
            if (PlayerAvatar.instance == null || MetaManager.instance?.colorsEquipped == null) return;

            PlayerCosmetics cosmetics = PlayerAvatar.instance.playerCosmetics;
            if (cosmetics == null) return;

            Plugin.LogMessageIfDebug($"[MetaManager.CosmeticColorSet/Postfix] Colour slot {_index} changed, updating avatar beam colour");

            try
            {
                CustomGrabBeamColour.TryUpdateAvatarColours(cosmetics, MetaManager.instance.colorsEquipped);
            }
            catch (System.Exception e)
            {
                Plugin.LogErrorIfDebug("[MetaManager.CosmeticColorSet/Postfix] Exception:\n" + e);
            }
        }
    }
}
