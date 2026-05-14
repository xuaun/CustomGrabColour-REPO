using HarmonyLib;
using UnityEngine;

namespace CustomGrabColour.PlayerGrabBeam;

internal static class AvatarColourResolver
{
    // SemiFunc.CosmeticType.ArmRightMesh = 13 is the avatar grabber colour.
    // SemiFunc.CosmeticType.ArmRight = 9 is the avatar right arm colour.
    private const int AvatarGrabberCosmeticColorIndex = (int)GrabBeamColourSettings.AvatarColourSource.Grabber;
    private const int AvatarArmRightCosmeticColorIndex = (int)GrabBeamColourSettings.AvatarColourSource.ArmRight;

    private static readonly AccessTools.FieldRef<PlayerCosmetics, PlayerAvatarVisuals> PlayerAvatarVisualsRef =
        AccessTools.FieldRefAccess<PlayerCosmetics, PlayerAvatarVisuals>("playerAvatarVisuals");

    private static readonly AccessTools.FieldRef<PlayerAvatarVisuals, bool> IsMenuAvatarRef =
        AccessTools.FieldRefAccess<PlayerAvatarVisuals, bool>("isMenuAvatar");

    private static readonly AccessTools.FieldRef<PlayerAvatarVisuals, PlayerAvatar> PlayerAvatarRef =
        AccessTools.FieldRefAccess<PlayerAvatarVisuals, PlayerAvatar>("playerAvatar");

    public static bool TryGetAvatarColours(
        PlayerCosmetics cosmetics,
        int[]? cosmeticColors,
        out PlayerAvatar? playerAvatar,
        out Color grabberColour,
        out Color armRightColour
    )
    {
        playerAvatar = null;
        grabberColour = default;
        armRightColour = default;

        PlayerAvatarVisuals visuals = PlayerAvatarVisualsRef(cosmetics);
        if (visuals == null)
        {
            Plugin.LogMessageIfDebug("[AvatarColourResolver] PlayerAvatarVisuals is null, skipping");
            return false;
        }

        if (IsMenuAvatarRef(visuals))
        {
            Plugin.LogMessageIfDebug("[AvatarColourResolver] Menu avatar, skipping");
            return false;
        }

        playerAvatar = PlayerAvatarRef(visuals);
        if (playerAvatar == null)
        {
            Plugin.LogMessageIfDebug("[AvatarColourResolver] PlayerAvatar is null, skipping");
            return false;
        }

        if (!TryGetAvatarColour(cosmeticColors, AvatarGrabberCosmeticColorIndex, out grabberColour))
        {
            return false;
        }

        if (!TryGetAvatarColour(cosmeticColors, AvatarArmRightCosmeticColorIndex, out armRightColour))
        {
            return false;
        }

        Plugin.LogMessageIfDebug($"[AvatarColourResolver] Avatar grabber colour={FormatColour(grabberColour)}, right arm colour={FormatColour(armRightColour)}");
        return true;
    }

    private static bool TryGetAvatarColour(int[]? cosmeticColors, int cosmeticColorIndex, out Color colour)
    {
        colour = default;

        if (cosmeticColors == null || cosmeticColors.Length <= cosmeticColorIndex)
        {
            Plugin.LogWarning($"[AvatarColourResolver] cosmeticColors is null or too short for index {cosmeticColorIndex} (length={cosmeticColors?.Length ?? -1}), aborting");
            return false;
        }

        if (MetaManager.instance?.colors == null)
        {
            Plugin.LogWarning("[AvatarColourResolver] MetaManager.instance.colors is null, aborting");
            return false;
        }

        int colourIndex = cosmeticColors[cosmeticColorIndex];
        if (colourIndex < 0 || colourIndex >= MetaManager.instance.colors.Count)
        {
            Plugin.LogWarning($"[AvatarColourResolver] colourIndex {colourIndex} out of range 0..{MetaManager.instance.colors.Count - 1}");
            return false;
        }

        colour = MetaManager.instance.colors[colourIndex].color; 
        return true;
    }

    private static string FormatColour(Color colour)
    {
        return $"({colour.r:F3}, {colour.g:F3}, {colour.b:F3}, {colour.a:F3})";
    }
}
