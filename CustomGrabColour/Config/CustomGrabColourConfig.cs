using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using CustomGrabColour.PlayerGrabBeam;
using UnityEngine;

namespace CustomGrabColour.Config;

public static class CustomGrabColourConfig
{
    private static readonly Dictionary<GrabBeamColourSettings.BeamType, BeamConfigEntries> BeamTypeToConfigEntries = [];
    private static bool _suppressConfigChangeEvents;

    public struct BeamConfigEntries(
        ConfigEntry<string> beamColour,
        ConfigEntry<bool> useAvatarColour,
        ConfigEntry<GrabBeamColourSettings.AvatarColourSource> avatarColourSource
    )
    {
        public readonly ConfigEntry<string> BeamColour = beamColour;
        public readonly ConfigEntry<bool> UseAvatarColour = useAvatarColour;
        public readonly ConfigEntry<GrabBeamColourSettings.AvatarColourSource> AvatarColourSource = avatarColourSource;
    }

    public const float DefaultOpacity = 0.2f;
    public const float MaxOpacity = 0.5f;
    public const GrabBeamColourSettings.AvatarColourSource DefaultAvatarColourSource = GrabBeamColourSettings.AvatarColourSource.Grabber;
    public static readonly Color NeutralDefaultColour = new(1f, 0.58f, 0.19f, 0.35f);
    public static readonly Color HealingDefaultColour = new(0.17f, 1f, 0.17f, DefaultOpacity);
    public static readonly Color RotatingDefaultColour = new(0.65f, 0.06f, 0.8f, DefaultOpacity);
    public static readonly Color ClimbingDefaultColour = new(0.0f, 0.7f, 1f, DefaultOpacity);

    public static BeamConfigEntries NeutralGrabBeam;
    public static BeamConfigEntries HealingGrabBeam;
    public static BeamConfigEntries RotatingGrabBeam;
    public static BeamConfigEntries ClimbingGrabBeam;

    public static ConfigEntry<bool> EnableDebugLogs;
    public static ConfigEntry<bool> DebugAddButtonToMainMenu;

    private const string ColourNotes = "\nStored as R,G,B,A values in 0-1 range";

    public static void Init(ConfigFile config)
    {
        ConfigEntry<string> neutralGrabBeamColour = config.Bind(
            "General",
            "NeutralGrabBeamColour",
            ConfigUtil.ColorToString(NeutralDefaultColour),
            "The default colour of the grab beam when holding an item." + ColourNotes
        );
        ConfigEntry<bool> neutralGrabBeamUseAvatarColour = config.Bind(
            "General",
            "NeutralGrabBeamMatchSkin",
            true,
            "Should the neutral grab beam use the colour of your avatar grabber?"
        );
        ConfigEntry<GrabBeamColourSettings.AvatarColourSource> neutralGrabBeamAvatarColourSource = config.Bind(
            "General",
            "NeutralGrabBeamAvatarColourSource",
            DefaultAvatarColourSource,
            "Which avatar colour should the neutral grab beam use when Use Avatar Colour is enabled?"
        );
        NeutralGrabBeam = new BeamConfigEntries(neutralGrabBeamColour, neutralGrabBeamUseAvatarColour, neutralGrabBeamAvatarColourSource);
        BeamTypeToConfigEntries.Add(GrabBeamColourSettings.BeamType.Neutral, NeutralGrabBeam);


        ConfigEntry<string> rotatingGrabBeamColour = config.Bind(
            "General",
            "RotatingGrabBeamColour",
            ConfigUtil.ColorToString(RotatingDefaultColour),
            "The colour of the grab beam when rotating an item or monster." + ColourNotes
        );
        ConfigEntry<bool> rotatingGrabBeamUseAvatarColour = config.Bind(
            "General",
            "RotatingGrabBeamMatchSkin",
            false,
            "Should the rotating grab beam use the colour of your avatar grabber?"
        );
        ConfigEntry<GrabBeamColourSettings.AvatarColourSource> rotatingGrabBeamAvatarColourSource = config.Bind(
            "General",
            "RotatingGrabBeamAvatarColourSource",
            DefaultAvatarColourSource,
            "Which avatar colour should the rotating grab beam use when Use Avatar Colour is enabled?"
        );
        RotatingGrabBeam = new BeamConfigEntries(rotatingGrabBeamColour, rotatingGrabBeamUseAvatarColour, rotatingGrabBeamAvatarColourSource);
        BeamTypeToConfigEntries.Add(GrabBeamColourSettings.BeamType.Rotate, RotatingGrabBeam);


        ConfigEntry<string> healingGrabBeamColour = config.Bind(
            "General",
            "HealingGrabBeamColour",
            ConfigUtil.ColorToString(HealingDefaultColour),
            "The colour of the grab beam when healing another player." + ColourNotes
        );
        ConfigEntry<bool> healingGrabBeamUseAvatarColour = config.Bind(
            "General",
            "HealingGrabBeamMatchSkin",
            false,
            "Should the healing grab beam use the colour of your avatar grabber?"
        );
        ConfigEntry<GrabBeamColourSettings.AvatarColourSource> healingGrabBeamAvatarColourSource = config.Bind(
            "General",
            "HealingGrabBeamAvatarColourSource",
            DefaultAvatarColourSource,
            "Which avatar colour should the healing grab beam use when Use Avatar Colour is enabled?"
        );
        HealingGrabBeam = new BeamConfigEntries(healingGrabBeamColour, healingGrabBeamUseAvatarColour, healingGrabBeamAvatarColourSource);
        BeamTypeToConfigEntries.Add(GrabBeamColourSettings.BeamType.Heal, HealingGrabBeam);
        
        
        ConfigEntry<string> climbingGrabBeamColour = config.Bind(
            "General",
            "ClimbingGrabBeamColour",
            ConfigUtil.ColorToString(ClimbingDefaultColour),
            "The colour of the grab beam when climbing with the tumble climb upgrade." + ColourNotes
        );
        ConfigEntry<bool> climbingGrabBeamUseAvatarColour = config.Bind(
            "General",
            "ClimbingGrabBeamMatchSkin",
            false,
            "Should the climbing grab beam use the colour of your avatar grabber?"
        );
        ConfigEntry<GrabBeamColourSettings.AvatarColourSource> climbingGrabBeamAvatarColourSource = config.Bind(
            "General",
            "ClimbingGrabBeamAvatarColourSource",
            DefaultAvatarColourSource,
            "Which avatar colour should the climbing grab beam use when Use Avatar Colour is enabled?"
        );
        ClimbingGrabBeam = new BeamConfigEntries(climbingGrabBeamColour, climbingGrabBeamUseAvatarColour, climbingGrabBeamAvatarColourSource);
        BeamTypeToConfigEntries.Add(GrabBeamColourSettings.BeamType.Climb, ClimbingGrabBeam);


        EnableDebugLogs = config.Bind(
            "Debug",
            "EnableDebugLogs",
            false,
            "Outputs additional debugging information to the log"
        );
        
        DebugAddButtonToMainMenu = config.Bind(
            "Debug",
            "AddConfigToMainMenu",
            false,
            "Adds the config menu button to the main menu"
        );

        LoadValuesFromConfig();
        RegisterConfigChangeHandlers();
    }

    private static void RegisterConfigChangeHandlers()
    {
        RegisterBeamConfigChangeHandlers(NeutralGrabBeam);
        RegisterBeamConfigChangeHandlers(RotatingGrabBeam);
        RegisterBeamConfigChangeHandlers(HealingGrabBeam);
        RegisterBeamConfigChangeHandlers(ClimbingGrabBeam);
    }

    private static void RegisterBeamConfigChangeHandlers(BeamConfigEntries configEntries)
    {
        configEntries.BeamColour.SettingChanged += HandleBeamConfigChanged;
        configEntries.UseAvatarColour.SettingChanged += HandleBeamConfigChanged;
        configEntries.AvatarColourSource.SettingChanged += HandleBeamConfigChanged;
    }

    private static void HandleBeamConfigChanged(object sender, EventArgs args)
    {
        if (_suppressConfigChangeEvents) return;

        LoadValuesFromConfig();
        CustomGrabBeamColour.UpdateBeamColourForAllBeams();
    }

    private static void LoadValuesFromConfig()
    {
        Color neutralColourFromConfig = ConfigUtil.StringToColor(NeutralGrabBeam.BeamColour.Value, NeutralDefaultColour);
        neutralColourFromConfig.a = Mathf.Clamp(neutralColourFromConfig.a, 0f, MaxOpacity);
        CustomGrabBeamColour.LocalNeutralColour = new GrabBeamColourSettings(neutralColourFromConfig, NeutralGrabBeam.UseAvatarColour.Value, GrabBeamColourSettings.BeamType.Neutral, NeutralGrabBeam.AvatarColourSource.Value);

        Color rotatingColourFromConfig = ConfigUtil.StringToColor(RotatingGrabBeam.BeamColour.Value, RotatingDefaultColour);
        rotatingColourFromConfig.a = Mathf.Clamp(rotatingColourFromConfig.a, 0f, MaxOpacity);
        CustomGrabBeamColour.LocalRotatingColour = new GrabBeamColourSettings(rotatingColourFromConfig, RotatingGrabBeam.UseAvatarColour.Value, GrabBeamColourSettings.BeamType.Rotate, RotatingGrabBeam.AvatarColourSource.Value);

        Color healingColourFromConfig = ConfigUtil.StringToColor(HealingGrabBeam.BeamColour.Value, HealingDefaultColour);
        healingColourFromConfig.a = Mathf.Clamp(healingColourFromConfig.a, 0f, MaxOpacity);
        CustomGrabBeamColour.LocalHealingColour = new GrabBeamColourSettings(healingColourFromConfig, HealingGrabBeam.UseAvatarColour.Value, GrabBeamColourSettings.BeamType.Heal, HealingGrabBeam.AvatarColourSource.Value);

        Color climbingColourFromConfig = ConfigUtil.StringToColor(ClimbingGrabBeam.BeamColour.Value, ClimbingDefaultColour);
        climbingColourFromConfig.a = Mathf.Clamp(climbingColourFromConfig.a, 0f, MaxOpacity);
        CustomGrabBeamColour.LocalClimbingColour = new GrabBeamColourSettings(climbingColourFromConfig, ClimbingGrabBeam.UseAvatarColour.Value, GrabBeamColourSettings.BeamType.Climb, ClimbingGrabBeam.AvatarColourSource.Value);
    }

    public static void SaveColour(GrabBeamColourSettings beamColourSettings)
    {
        Plugin.LogMessageIfDebug("Saving colour to config file: " + beamColourSettings);
        beamColourSettings.a = Mathf.Clamp(beamColourSettings.a, 0f, MaxOpacity);

        bool colourConfigValueExists = BeamTypeToConfigEntries.TryGetValue(beamColourSettings.CurrentBeamType, out BeamConfigEntries configEntries);
        if (!colourConfigValueExists)
        {
            Plugin.LogWarning("Unable to save colour value for beam type: " + beamColourSettings.CurrentBeamType);
        }

        _suppressConfigChangeEvents = true;
        try
        {
            configEntries.BeamColour.Value = ConfigUtil.ColorToString(beamColourSettings.Colour);
            configEntries.UseAvatarColour.Value = beamColourSettings.UseAvatarColour;
            configEntries.AvatarColourSource.Value = beamColourSettings.CurrentAvatarColourSource;
        }
        finally
        {
            _suppressConfigChangeEvents = false;
        }
    }
}
