using System;
using System.Reflection;
using CustomGrabColour.Config;
using Photon.Pun;
using UnityEngine;
using static CustomGrabColour.PlayerGrabBeam.GrabBeamColourSettings;

namespace CustomGrabColour.PlayerGrabBeam;

// handles local and other players grab beam colours
public class CustomGrabBeamColour : MonoBehaviour, IPunObservable
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
        get => throw new NotImplementedException("Tried to get field LocalBeamColour, call GetLocalSettingsForBeamType instead");
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
        get => throw new NotImplementedException("Tried to get field CurrentBeamColour, call GetSettingsForBeamType instead");
    }
    public GrabBeamColourSettings GetSettingsForBeamType(BeamType beamType)
    {
        switch (beamType)
        {
            case BeamType.Heal:
                {
                    return CurrentNeutralColour;
                }
            case BeamType.Rotate:
                {
                    return CurrentHealingColour;
                }
            case BeamType.Climb:
                {
                    return CurrentClimbingColour;
                }
            default:
                {
                    return CurrentRotatingColour;
                }
        }
    }

    void Awake()
    {
        player = gameObject.GetComponent<PlayerAvatar>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new NotImplementedException();
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
        LocalNeutralColour = new GrabBeamColourSettings(CustomGrabColourConfig.NeutralDefaultColour, (bool)CustomGrabColourConfig.NeutralGrabBeam.MatchSkin.DefaultValue, BeamType.Neutral);
        LocalRotatingColour = new GrabBeamColourSettings(CustomGrabColourConfig.RotatingDefaultColour, (bool)CustomGrabColourConfig.RotatingGrabBeam.MatchSkin.DefaultValue, BeamType.Rotate);
        LocalHealingColour = new GrabBeamColourSettings(CustomGrabColourConfig.HealingDefaultColour, (bool)CustomGrabColourConfig.HealingGrabBeam.MatchSkin.DefaultValue, BeamType.Heal);
        LocalClimbingColour = new GrabBeamColourSettings(CustomGrabColourConfig.ClimbingDefaultColour, (bool)CustomGrabColourConfig.ClimbingGrabBeam.MatchSkin.DefaultValue, BeamType.Climb);
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

        if (GameManager.Multiplayer())
        {
            PlayerAvatar.instance.photonView.RPC("SetBeamColourRPC", RpcTarget.AllBuffered, ToRPCBuffer(settings));
        }
        else
        {
            PlayerAvatar.instance.GetComponent<CustomGrabBeamColour>().SetBeamColourRPC(ToRPCBuffer(settings));
        }
    }

    [PunRPC]
    public void SetBeamColourRPC(object[] beamColourParts)
    {
        GrabBeamColourSettings newBeamColour = FromRPCBuffer(beamColourParts);
        Plugin.LogMessageIfDebug("SetBeamColourRPC called with values: r:" + newBeamColour.r + ", g:" + newBeamColour.g + ", b:" + newBeamColour.b + ", a:" + newBeamColour.a + ", matchSkin:" + newBeamColour.MatchSkin + ", beamType:" + newBeamColour.CurrentBeamType);

        newBeamColour.a = Mathf.Clamp(newBeamColour.a, 0f, CustomGrabColourConfig.MaxOpacity);

        CurrentBeamColour = newBeamColour;

        if (newBeamColour.CurrentBeamType != BeamType.Neutral) return;

        // invoke ColorStates method to make sure the beam colour updates properly
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

    public Color GetBodyColour(Color fallbackColour)
    {
        return fallbackColour;
    }

    public static Color GetLocalBodyColour(Color fallbackColour)
    {
        return fallbackColour;
    }
}