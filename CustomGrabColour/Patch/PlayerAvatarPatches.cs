using CustomGrabColour.PlayerGrabBeam;
using HarmonyLib;

namespace CustomGrabColour.Patch;

internal abstract class PlayerAvatarPatches
{
    [HarmonyPatch(typeof(PlayerAvatar))]
    [HarmonyPatch("Awake")]
    class PlayerAvatar_Awake_Patch
    {
        public static void Postfix(PlayerAvatar __instance)
        {
            Plugin.LogMessageIfDebug($"[PlayerAvatar.Awake/Postfix] Called for avatar={__instance}");

            if (__instance.GetComponent<CustomGrabBeamColour>() != null)
            {
                Plugin.LogMessageIfDebug("[PlayerAvatar.Awake/Postfix] CustomGrabBeamColour already exists, skipping AddComponent");
                return;
            }

            __instance.gameObject.AddComponent<CustomGrabBeamColour>();
            Plugin.LogMessageIfDebug("[PlayerAvatar.Awake/Postfix] Added CustomGrabBeamColour component");
        }
    }

    [HarmonyPatch(typeof(PlayerCosmetics))]
    [HarmonyPatch("SetupColorsLogic")]
    class PlayerCosmetics_SetupColorsLogic_Patch
    {
        public static void Postfix(PlayerCosmetics __instance, int[] _colors)
        {
            Plugin.LogMessageIfDebug(
                $"[PlayerCosmetics.SetupColorsLogic/Postfix] Called, colors length={_colors?.Length ?? -1}");

            try
            {
                CustomGrabBeamColour.TryUpdateAvatarColours(__instance, _colors);
            }
            catch (System.Exception e)
            {
                Plugin.LogErrorIfDebug("[PlayerCosmetics.SetupColorsLogic/Postfix] Exception:\n" + e);
            }
        }
    }
}
