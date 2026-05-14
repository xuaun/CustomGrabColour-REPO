using HarmonyLib;
using UnityEngine;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using CustomGrabColour.Config;
using CustomGrabColour.PlayerGrabBeam;

namespace CustomGrabColour.Patch;

internal abstract class PhysGrabberPatches
{
    private static readonly AccessTools.FieldRef<PhysGrabber, List<GameObject>> PhysGrabPointVisualGridObjectsRef =
        AccessTools.FieldRefAccess<PhysGrabber, List<GameObject>>("physGrabPointVisualGridObjects");

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [HarmonyPatch(typeof(PhysGrabber))]
    [HarmonyPatch("ColorStateSetColor")]
    class PhysGrabber_ColorState_Patch
    {
        private static readonly AccessTools.FieldRef<PhysGrabber, int> PrevColorStateRef =
            AccessTools.FieldRefAccess<PhysGrabber, int>("prevColorState");

        static void Prefix(PhysGrabber __instance, ref Color mainColor, ref Color emissionColor)
        {
            int currentColourState = PrevColorStateRef(__instance);

            if (mainColor == null || emissionColor == null)
            {
                ResetRotateBeamGridsColour(__instance);
                return;
            }

            CustomGrabBeamColour grabBeamColour = GetCurrentGrabBeamColour(__instance);
            if (!grabBeamColour)
            {
                Plugin.LogMessageIfDebug("Player has no custom beam colour");
                ResetRotateBeamGridsColour(__instance);
                return;
            }

            GrabBeamColourSettings grabBeamSettings;
            if (currentColourState == 0)
            {
                grabBeamSettings = grabBeamColour.CurrentNeutralColour;
            }
            else if (currentColourState == 1)
            {
                grabBeamSettings = grabBeamColour.CurrentHealingColour;
            }
            else if (currentColourState == 2)
            {
                grabBeamSettings = grabBeamColour.CurrentRotatingColour;
            }
            else if (currentColourState == 3)
            {
                grabBeamSettings = grabBeamColour.CurrentClimbingColour;
            }
            else
            {
                ResetRotateBeamGridsColour(__instance);
                return;
            }

            Color customColour;
            if(grabBeamSettings.UseAvatarColour)
            {
                customColour = grabBeamColour.GetAvatarColour(grabBeamSettings.CurrentAvatarColourSource, grabBeamSettings.Colour);
                customColour.a = grabBeamSettings.Colour.a;
            }
            else
            {
                customColour = grabBeamSettings.Colour;
            }

            mainColor.r = customColour.r / 3.5f; // TODO: probably find a better way to fix this
            mainColor.g = customColour.g / 4f;
            mainColor.b = customColour.b / 3.5f;
            mainColor.a = customColour.a;
            emissionColor.r = customColour.r / 3.5f;
            emissionColor.g = customColour.g / 4f;
            emissionColor.b = customColour.b / 3.5f;
            emissionColor.a = 0.3f;

            Plugin.LogMessageIfDebug("Set player beam to: (" + mainColor.r + ", " + mainColor.g + ", " + mainColor.b + ", " + mainColor.a + "). colour state is " + currentColourState);

            SetRotateBeamGridsColour(__instance, customColour);
        }
    }

    private static CustomGrabBeamColour GetCurrentGrabBeamColour(PhysGrabber grabber)
    {
        return grabber.playerAvatar.gameObject.GetComponent<CustomGrabBeamColour>();
    }

    private static void ResetRotateBeamGridsColour(PhysGrabber __instance)
    {
        SetRotateBeamGridsColour(__instance, CustomGrabColourConfig.RotatingDefaultColour);
    }

    private static void SetRotateBeamGridsColour(PhysGrabber __instance, Color gridColour)
    {
        List<GameObject> physGrabPointVisualGridObjects = PhysGrabPointVisualGridObjectsRef(__instance);

        foreach (var gridMeshObject in physGrabPointVisualGridObjects)
        {
            Material gridMeshMaterial = gridMeshObject.GetComponent<MeshRenderer>().material;
            if (!gridMeshMaterial) continue;
            
            Color col = new Color(
                gridColour.r / 3.5f,
                gridColour.g / 4f,
                gridColour.b / 3.5f,
                gridColour.a
            );
            gridMeshMaterial.color = col;

            Color emission = new Color(
                gridColour.r / 3.5f,
                gridColour.g / 4f,
                gridColour.b / 3.5f,
                0.3f
            );
            gridMeshMaterial.SetColor(EmissionColor, emission);

            Plugin.LogMessageIfDebug("Set grid mesh to: (" + gridMeshMaterial.color.r + ", " + gridMeshMaterial.color.g + ", " + gridMeshMaterial.color.b + ", " + gridMeshMaterial.color.a + ")");
        }
    }

    [HarmonyPatch(typeof(PhysGrabber))]
    [HarmonyPatch("ChangeBeamAlpha")]
    class PhysGrabber_ChangeBeamAlpha_Patch
    {
        static bool Prefix()
        {
            // cancel changing the beam alpha
            return false;
        }
    }

    [HarmonyPatch(typeof(PhysGrabber))]
    [HarmonyPatch("PhysGrabBeamActivate")]
    class PhysGrabber_PhysGrabBeamActivate_Patch
    {
        static async void Prefix(PhysGrabber __instance)
        {
            CustomGrabBeamColour grabBeamColour = GetCurrentGrabBeamColour(__instance);
            if(grabBeamColour.SentInitialColourUpdate) return;
            
            bool grabBeamActive = true;
            try
            {
                FieldInfo grabBeamActiveField = __instance.GetType().GetField("physGrabBeamActive", BindingFlags.Instance | BindingFlags.NonPublic);
                grabBeamActive = (bool)grabBeamActiveField.GetValue(__instance);
            }
            catch (Exception) {
                Plugin.LogMessageIfDebug("Failed to get value of PhysGrabber physGrabBeamActive field");
            }
    
            if (grabBeamActive) return;
    
            await Task.Delay(100);
    
            Plugin.LogMessageIfDebug("PhysGrabBeamActivate called");
    
            // if player has custom beam colour update it when they activate their beam
            GrabBeamUtil.TrySendBeamColourUpdateForAllBeams(__instance.playerAvatar);
            grabBeamColour.SentInitialColourUpdate = true;
        }
    }
}
