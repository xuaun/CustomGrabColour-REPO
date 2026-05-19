using System;
using System.Reflection;
using CustomGrabColour.Config;
using Photon.Pun;
using UnityEngine;
using static CustomGrabColour.PlayerGrabBeam.GrabBeamColourSettings;

namespace CustomGrabColour.PlayerGrabBeam;

// handles local and other players grab beam colours
public class CustomGrabBeamColour : MonoBehaviour
{
    internal static GrabBeamColourSettings LocalNeutralColour;
    internal static GrabBeamColourSettings LocalHealingColour;
    internal static GrabBeamColourSettings LocalRotatingColour;
    internal static GrabBeamColourSettings LocalClimbingColour;

    internal GrabBeamColourSettings CurrentNeutralColour;
    internal GrabBeamColourSettings CurrentHealingColour;
    internal GrabBeamColourSettings CurrentRotatingColour;
    internal GrabBeamColourSettings CurrentClimbingColour;
    internal bool SentInitialColourUpdate = false;

    private static bool _hasLocalAvatarColours;
    private static Color _localAvatarGrabberColour;
    private static Color _localAvatarArmRightColour;

    private bool _hasAvatarColours;
    private Color _avatarGrabberColour;
    private Color _avatarArmRightColour;

    public PlayerAvatar player;

    public static GrabBeamColourSettings LocalBeamColour
    {
        set
        {
            switch (value.CurrentBeamType)
            {
                case BeamType.Neutral:
                    {
                        LocalNeutralColour = value; break;
                    }
                case BeamType.Heal:
                    {
                        LocalHealingColour = value; break;
                    }
                case BeamType.Rotate:
                    {
                        LocalRotatingColour = value; break;
                    }
                case BeamType.Climb:
                    {
                        LocalClimbingColour = value; break;
                    }
            }
        }
    }
    public static GrabBeamColourSettings GetLocalSettingsForBeamType(BeamType beamType)
    {
        switch (beamType)
        {
            case BeamType.Heal:
                {
                    return LocalHealingColour;
                }
            case BeamType.Rotate:
                {
                    return LocalRotatingColour;
                }
            case BeamType.Climb:
                {
                    return LocalClimbingColour;
                }
            default:
                {
                    return LocalNeutralColour;
                }
        }
    }

    public GrabBeamColourSettings CurrentBeamColour
    {
        set
        {
            switch (value.CurrentBeamType)
            {
                case BeamType.Neutral:
                    {
                        CurrentNeutralColour = value; break;
                    }
                case BeamType.Heal:
                    {
                        CurrentHealingColour = value; break;
                    }
                case BeamType.Rotate:
                    {
                        CurrentRotatingColour = value; break;
                    }
                case BeamType.Climb:
                    {
                        CurrentClimbingColour = value; break;
                    }
            }
        }
    }
    public GrabBeamColourSettings GetSettingsForBeamType(BeamType beamType)
    {
        switch (beamType)
        {
            case BeamType.Heal:
                {
                    return CurrentHealingColour;
                }
            case BeamType.Rotate:
                {
                    return CurrentRotatingColour;
                }
            case BeamType.Climb:
                {
                    return CurrentClimbingColour;
                }
            default:
                {
                    return CurrentNeutralColour;
                }
        }
    }

    void Awake()
    {
        player = gameObject.GetComponent<PlayerAvatar>();
    }

    public static void SaveLocalColoursToConfig()
    {
        CustomGrabColourConfig.SaveColour(LocalNeutralColour);
        CustomGrabColourConfig.SaveColour(LocalRotatingColour);
        CustomGrabColourConfig.SaveColour(LocalHealingColour);
        CustomGrabColourConfig.SaveColour(LocalClimbingColour);
    }

    public static void ResetBeamColours()
    {
        LocalNeutralColour = new GrabBeamColourSettings(CustomGrabColourConfig.NeutralDefaultColour, (bool)CustomGrabColourConfig.NeutralGrabBeam.UseAvatarColour.DefaultValue, BeamType.Neutral, CustomGrabColourConfig.DefaultAvatarColourSource);
        LocalRotatingColour = new GrabBeamColourSettings(CustomGrabColourConfig.RotatingDefaultColour, (bool)CustomGrabColourConfig.RotatingGrabBeam.UseAvatarColour.DefaultValue, BeamType.Rotate, CustomGrabColourConfig.DefaultAvatarColourSource);
        LocalHealingColour = new GrabBeamColourSettings(CustomGrabColourConfig.HealingDefaultColour, (bool)CustomGrabColourConfig.HealingGrabBeam.UseAvatarColour.DefaultValue, BeamType.Heal, CustomGrabColourConfig.DefaultAvatarColourSource);
        LocalClimbingColour = new GrabBeamColourSettings(CustomGrabColourConfig.ClimbingDefaultColour, (bool)CustomGrabColourConfig.ClimbingGrabBeam.UseAvatarColour.DefaultValue, BeamType.Climb, CustomGrabColourConfig.DefaultAvatarColourSource);
        UpdateBeamColourForAllBeams();
    }

    public static void UpdateBeamColour(GrabBeamColourSettings newColour)
    {
        newColour.a = Mathf.Clamp(newColour.a, 0f, CustomGrabColourConfig.MaxOpacity);
        LocalBeamColour = newColour;
        UpdateBeamColour(newColour.CurrentBeamType);
    }
    public static void UpdateBeamColourForAllBeams()
    {
        foreach (BeamType beamType in Enum.GetValues(typeof(BeamType)))
        {
            UpdateBeamColour(beamType);
        }
    }

    public static void UpdateBeamColour(BeamType beamType)
    {
        GrabBeamColourSettings settings = GetLocalSettingsForBeamType(beamType);

        if (PlayerAvatar.instance == null)
        {
            Plugin.LogMessageIfDebug("[CustomGrabBeamColour] No local PlayerAvatar instance, skipping beam colour update");
            return;
        }

        if (GameManager.Multiplayer())
        {
            PlayerAvatar.instance.photonView.RPC("SetBeamColourRPC", RpcTarget.AllBuffered, ToRPCBuffer(settings));
        }
        else
        {
            PlayerAvatar.instance.GetComponent<CustomGrabBeamColour>().SetBeamColourRPC(ToRPCBuffer(settings));
        }
    }

    public static void TryUpdateAvatarColours(PlayerCosmetics cosmetics, int[]? cosmeticColors)
    {
        if (!AvatarColourResolver.TryGetAvatarColours(
                cosmetics,
                cosmeticColors,
                out PlayerAvatar? playerAvatar,
                out Color avatarGrabberColour,
                out Color avatarArmRightColour
            ))
        {
            return;
        }

        if (playerAvatar == null)
        {
            Plugin.LogMessageIfDebug("[CustomGrabBeamColour] Avatar grabber colour resolved without a PlayerAvatar");
            return;
        }

        CustomGrabBeamColour customGrabBeamColour = playerAvatar.GetComponent<CustomGrabBeamColour>();
        if (customGrabBeamColour == null)
        {
            Plugin.LogMessageIfDebug("[CustomGrabBeamColour] PlayerAvatar has no CustomGrabBeamColour component yet");
            return;
        }

        customGrabBeamColour.SetAvatarColours(avatarGrabberColour, avatarArmRightColour);

        if (PlayerAvatar.instance == null || playerAvatar != PlayerAvatar.instance)
        {
            Plugin.LogMessageIfDebug("[CustomGrabBeamColour] Updated remote avatar colour cache");
            return;
        }

        _localAvatarGrabberColour = avatarGrabberColour;
        _localAvatarArmRightColour = avatarArmRightColour;
        _hasLocalAvatarColours = true;
        UpdateBeamColourForAvatarColourBeams();
    }

    private static void UpdateBeamColourForAvatarColourBeams()
    {
        foreach (BeamType beamType in Enum.GetValues(typeof(BeamType)))
        {
            GrabBeamColourSettings settings = GetLocalSettingsForBeamType(beamType);
            if (!settings.UseAvatarColour) continue;

            UpdateBeamColour(beamType);
        }
    }

    private void SetAvatarColours(Color avatarGrabberColour, Color avatarArmRightColour)
    {
        _avatarGrabberColour = avatarGrabberColour;
        _avatarArmRightColour = avatarArmRightColour;
        _hasAvatarColours = true;
    }

    [PunRPC]
    public void SetBeamColourRPC(object[] beamColourParts)
    {
        GrabBeamColourSettings newBeamColour = FromRPCBuffer(beamColourParts);
        Plugin.LogMessageIfDebug("SetBeamColourRPC called with values: r:" + newBeamColour.r + ", g:" + newBeamColour.g + ", b:" + newBeamColour.b + ", a:" + newBeamColour.a + ", useAvatarColour:" + newBeamColour.UseAvatarColour + ", avatarColourSource:" + newBeamColour.CurrentAvatarColourSource + ", beamType:" + newBeamColour.CurrentBeamType);

        newBeamColour.a = Mathf.Clamp(newBeamColour.a, 0f, CustomGrabColourConfig.MaxOpacity);

        CurrentBeamColour = newBeamColour;

        // reset prevColorState so ColorStates() re-applies the colour on next call
        Type physGrabberType = player.physGrabber.GetType();

        try
        {
            FieldInfo colorStatesField = physGrabberType.GetField("prevColorState", BindingFlags.Instance | BindingFlags.NonPublic);
            if (colorStatesField != null) colorStatesField.SetValue(player.physGrabber, -1);
        }
        catch (Exception e)
        {
            Plugin.LogErrorIfDebug("Error while setting field 'prevColorState', this is probably harmless.\n" + e);
        }

        try
        {
            MethodInfo colorStatesInfo = physGrabberType.GetMethod("ColorStates", BindingFlags.Instance | BindingFlags.NonPublic);
            if (colorStatesInfo != null) colorStatesInfo.Invoke(player.physGrabber, null);
        }
        catch (Exception e)
        {
            Plugin.LogErrorIfDebug("Error while calling method 'ColorStates', this is probably harmless.\n" + e);
        }
    }

    public Color GetAvatarColour(AvatarColourSource avatarColourSource, Color fallbackColour)
    {
        if (!_hasAvatarColours) return fallbackColour;

        return avatarColourSource switch
        {
            AvatarColourSource.ArmRight => _avatarArmRightColour,
            _ => _avatarGrabberColour
        };
    }

    public static Color GetLocalAvatarColour(AvatarColourSource avatarColourSource, Color fallbackColour)
    {
        if (!_hasLocalAvatarColours) return fallbackColour;

        return avatarColourSource switch
        {
            AvatarColourSource.ArmRight => _localAvatarArmRightColour,
            _ => _localAvatarGrabberColour
        };
    }
}
