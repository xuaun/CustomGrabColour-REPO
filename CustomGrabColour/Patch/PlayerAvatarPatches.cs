using CustomGrabColour.PlayerGrabBeam;
using HarmonyLib;
using UnityEngine;
using static CustomGrabColour.PlayerGrabBeam.GrabBeamColourSettings;

namespace CustomGrabColour.Patch;

internal abstract class PlayerAvatarPatches
{
    // SemiFunc.CosmeticType.ArmRightMesh = 13 (body/skin color of the right arm grabber) is the color we want to match for the grab beam
    // ArmRightMesh = 9 is the color of the right arm mesh
    // ArmRight = 1 is the color of cosmetics (sleeves/gloves) on the right arm
    private const int CosmeticTypeArmRight = 13;

    private static readonly AccessTools.FieldRef<PlayerCosmetics, PlayerAvatarVisuals> PlayerAvatarVisualsRef =
        AccessTools.FieldRefAccess<PlayerCosmetics, PlayerAvatarVisuals>("playerAvatarVisuals");

    private static readonly AccessTools.FieldRef<PlayerAvatarVisuals, bool> IsMenuAvatarRef =
        AccessTools.FieldRefAccess<PlayerAvatarVisuals, bool>("isMenuAvatar");

    private static readonly AccessTools.FieldRef<PlayerAvatarVisuals, PlayerAvatar> PlayerAvatarRef =
        AccessTools.FieldRefAccess<PlayerAvatarVisuals, PlayerAvatar>("playerAvatar");

    private static string FormatColor(Color color)
    {
        return $"({color.r:F3}, {color.g:F3}, {color.b:F3}, {color.a:F3})";
    }

    private static PlayerAvatar GetPlayerAvatar(PlayerCosmetics cosmetics)
    {
        PlayerAvatarVisuals visuals = PlayerAvatarVisualsRef(cosmetics);
        if (visuals == null) return null;
        if (IsMenuAvatarRef(visuals)) return null;
        return PlayerAvatarRef(visuals);
    }

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

            PlayerAvatar playerAvatar = GetPlayerAvatar(__instance);
            if (playerAvatar == null)
            {
                Plugin.LogMessageIfDebug("[PlayerCosmetics.SetupColorsLogic/Postfix] Not a game avatar, skipping");
                return;
            }

            if (PlayerAvatar.instance == null || playerAvatar != PlayerAvatar.instance)
            {
                Plugin.LogMessageIfDebug("[PlayerCosmetics.SetupColorsLogic/Postfix] Not local player, skipping");
                return;
            }

            if (_colors == null || _colors.Length <= CosmeticTypeArmRight)
            {
                Plugin.LogWarning($"[PlayerCosmetics.SetupColorsLogic/Postfix] _colors is null or too short (length={_colors?.Length ?? -1}), aborting");
                return;
            }

            if (MetaManager.instance?.colors == null)
            {
                Plugin.LogWarning("[PlayerCosmetics.SetupColorsLogic/Postfix] MetaManager.instance.colors is null, aborting");
                return;
            }

            int colorIndex = _colors[CosmeticTypeArmRight];
            Plugin.LogMessageIfDebug($"[PlayerCosmetics.SetupColorsLogic/Postfix] Right arm colorIndex={colorIndex}");

            if (colorIndex < 0 || colorIndex >= MetaManager.instance.colors.Count)
            {
                Plugin.LogWarning($"[PlayerCosmetics.SetupColorsLogic/Postfix] colorIndex {colorIndex} out of range 0..{MetaManager.instance.colors.Count - 1}");
                return;
            }

            Color beamColor = MetaManager.instance.colors[colorIndex].color;
            Plugin.LogMessageIfDebug($"[PlayerCosmetics.SetupColorsLogic/Postfix] Right arm color={FormatColor(beamColor)}");

            var newSettings = new GrabBeamColourSettings(beamColor, matchSkin: false, BeamType.Neutral);

            try
            {
                CustomGrabBeamColour.UpdateBeamColour(newSettings);
                Plugin.LogMessageIfDebug("[PlayerCosmetics.SetupColorsLogic/Postfix] Updated beam colour from right arm color");
            }
            catch (System.Exception e)
            {
                Plugin.LogErrorIfDebug("[PlayerCosmetics.SetupColorsLogic/Postfix] Exception:\n" + e);
            }
        }
    }
}