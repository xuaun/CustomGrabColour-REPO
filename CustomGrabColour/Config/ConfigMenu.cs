using MenuLib;
using MenuLib.MonoBehaviors;
using System;
using CustomGrabColour.PlayerGrabBeam;
using UnityEngine;
using UnityEngine.UI;
using static CustomGrabColour.PlayerGrabBeam.GrabBeamColourSettings;

namespace CustomGrabColour.Config;

internal static class ConfigMenu
{
    private static Image _neutralGrabColourPreviewImage;
    private static GameObject _neutralGrabColourPreviewParent;

    private static Image _rotatingGrabColourPreviewImage;
    private static GameObject _rotatingGrabColourPreviewParent;

    private static Image _healingGrabColourPreviewImage;
    private static GameObject _healingGrabColourPreviewParent;

    private static Image _climbingGrabColourPreviewImage;
    private static GameObject _climbingGrabColourPreviewParent;

    private const float HorizontalPos = 70f;

    public static void Init()
    {
        MenuAPI.AddElementToEscapeMenu(parent =>
        {
            MenuAPI.CreateREPOButton("Change Grab Colour", OpenPopup, parent, localPosition: new Vector2(28.3f, 350.0f));
        });
        MenuAPI.AddElementToColorMenu(parent =>
        {
            MenuAPI.CreateREPOButton("Change Grab Colour", OpenPopup, parent, localPosition: new Vector2(28.3f, 350.0f));
        });

        if (CustomGrabColourConfig.DebugAddButtonToMainMenu.Value)
        {
            MenuAPI.AddElementToMainMenu(parent =>
            {
                MenuAPI.CreateREPOButton("Change Grab Colour", OpenPopup, parent, localPosition: new Vector2(28.3f, 350.0f));
            });
        }
    }

    private static void OpenPopup()
    {
        // build colour config screen
        var changeGrabColourPage = MenuAPI.CreateREPOPopupPage("Change Grab Beam Colour", REPOPopupPage.PresetSide.Left, false, pageDimmerVisibility: true, spacing: 1.5f);

        // colour previews
        changeGrabColourPage.AddElement(parent =>
        {
            var row1Y = GetVerticalPos(0);
            var row2Y = GetVerticalPos(1);
            const float col1X = 455f;
            const float col2X = 605f;
            const float labelYOffset = 60f;
            var labelScale = new Vector2(0.5f, 0.5f);
            
            // neutral
            var neutralLabel = MenuAPI.CreateREPOLabel("Neutral Colour Preview", parent, new Vector2(col1X, row1Y - labelYOffset));
            neutralLabel.transform.localScale = labelScale;
            neutralLabel.transform.SetPositionAndRotation(new Vector3(col1X - (neutralLabel.labelTMP.GetPreferredWidth() / 4), neutralLabel.transform.position.y, neutralLabel.transform.position.z), neutralLabel.transform.rotation);
            if (!_neutralGrabColourPreviewParent)
            {
                CreateBeamPreviewColourRectangle(parent, new Vector2(col1X, row1Y), out _neutralGrabColourPreviewParent, out _neutralGrabColourPreviewImage);
            }

            // rotating
            var rotatingLabel = MenuAPI.CreateREPOLabel("Rotating Colour Preview", parent, new Vector2(col1X, row2Y - labelYOffset));
            rotatingLabel.transform.localScale = labelScale;
            rotatingLabel.transform.SetPositionAndRotation(new Vector3(col1X - (rotatingLabel.labelTMP.GetPreferredWidth() / 4), rotatingLabel.transform.position.y, rotatingLabel.transform.position.z), rotatingLabel.transform.rotation);
            if (!_rotatingGrabColourPreviewParent)
            {
                CreateBeamPreviewColourRectangle(parent, new Vector2(col1X, row2Y), out _rotatingGrabColourPreviewParent, out _rotatingGrabColourPreviewImage);
            }

            // healing
            var healingLabel = MenuAPI.CreateREPOLabel("Healing Colour Preview", parent, new Vector2(col2X, row1Y - labelYOffset));
            healingLabel.transform.localScale = labelScale;
            healingLabel.transform.SetPositionAndRotation(new Vector3(col2X - (healingLabel.labelTMP.GetPreferredWidth() / 4), healingLabel.transform.position.y, healingLabel.transform.position.z), healingLabel.transform.rotation);
            if (!_healingGrabColourPreviewParent)
            {
                CreateBeamPreviewColourRectangle(parent, new Vector2(col2X, row1Y), out _healingGrabColourPreviewParent, out _healingGrabColourPreviewImage);
            }

            // climbing
            var climbingLabel = MenuAPI.CreateREPOLabel("Climbing Colour Preview", parent, new Vector2(col2X, row2Y - labelYOffset));
            climbingLabel.transform.localScale = labelScale;
            climbingLabel.transform.SetPositionAndRotation(new Vector3(col2X - (climbingLabel.labelTMP.GetPreferredWidth() / 4), climbingLabel.transform.position.y, climbingLabel.transform.position.z), climbingLabel.transform.rotation);
            if (!_climbingGrabColourPreviewParent)
            {
                CreateBeamPreviewColourRectangle(parent, new Vector2(col2X, row2Y), out _climbingGrabColourPreviewParent, out _climbingGrabColourPreviewImage);
            }
        });

        // neutral grab beam colours
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var beamLabel = MenuAPI.CreateREPOLabel("Neutral Beam Colour", parent, new Vector2(0, GetVerticalOffsetForScrollChildren(0)));
            beamLabel.transform.localScale = new Vector2(0.5f, 0.5f);
            return beamLabel.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var redSlider = CreateColourSlider(
                "Red",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalNeutralColour.r = f;
                    if (CustomGrabBeamColour.LocalNeutralColour.UseAvatarColour) { ApplyNeutralChange(); return; }
                    Color col = _neutralGrabColourPreviewImage.color;
                    col.r = f;
                    _neutralGrabColourPreviewImage.color = col;
                    ApplyNeutralChange();
                },
                CustomGrabBeamColour.LocalNeutralColour.r,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(1))
            );
            return redSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var greenSlider = CreateColourSlider(
                "Green",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalNeutralColour.g = f;
                    if (CustomGrabBeamColour.LocalNeutralColour.UseAvatarColour) { ApplyNeutralChange(); return; }
                    Color col = _neutralGrabColourPreviewImage.color;
                    col.g = f;
                    _neutralGrabColourPreviewImage.color = col;
                    ApplyNeutralChange();
                },
                CustomGrabBeamColour.LocalNeutralColour.g,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(2))
            );
            return greenSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var blueSlider = CreateColourSlider(
                "Blue",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalNeutralColour.b = f;
                    if (CustomGrabBeamColour.LocalNeutralColour.UseAvatarColour) { ApplyNeutralChange(); return; }
                    Color col = _neutralGrabColourPreviewImage.color;
                    col.b = f;
                    _neutralGrabColourPreviewImage.color = col;
                    ApplyNeutralChange();
                },
                CustomGrabBeamColour.LocalNeutralColour.b,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(3))
            );
            return blueSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var opacitySlider = CreateColourSlider(
                "Opacity",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalNeutralColour.a = f;
                    Color col = _neutralGrabColourPreviewImage.color;
                    col.a = f;
                    _neutralGrabColourPreviewImage.color = col;
                    ApplyNeutralChange();
                },
                CustomGrabBeamColour.LocalNeutralColour.a,
                CustomGrabColourConfig.MaxOpacity,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(4))
            );
            return opacitySlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var useAvatarColourToggle = MenuAPI.CreateREPOToggle(
                "Use Avatar Colour",
                (val) =>
                {
                    CustomGrabBeamColour.LocalNeutralColour.UseAvatarColour = val;
                    ApplyNeutralChange();
                },
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(5)),
                "Yes",
                "No",
                CustomGrabBeamColour.LocalNeutralColour.UseAvatarColour
            );
            return useAvatarColourToggle.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var avatarColourSourceToggle = CreateAvatarColourSourceToggle(
                val =>
                {
                    CustomGrabBeamColour.LocalNeutralColour.CurrentAvatarColourSource = val ? AvatarColourSource.Grabber : AvatarColourSource.ArmRight;
                    ApplyNeutralChange();
                },
                CustomGrabBeamColour.LocalNeutralColour.CurrentAvatarColourSource,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(6))
            );
            return avatarColourSourceToggle.rectTransform;
        });

        // rotating grab beam colours
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var beamLabel = MenuAPI.CreateREPOLabel("Rotating Beam Colour", parent, new Vector2(0, GetVerticalOffsetForScrollChildren(7)));
            beamLabel.transform.localScale = new Vector2(0.5f, 0.5f);
            return beamLabel.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var redSlider = CreateColourSlider(
                "Red",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalRotatingColour.r = f;
                    if (CustomGrabBeamColour.LocalRotatingColour.UseAvatarColour) { ApplyRotatingChange(); return; }
                    Color col = _rotatingGrabColourPreviewImage.color;
                    col.r = f;
                    _rotatingGrabColourPreviewImage.color = col;
                    ApplyRotatingChange();
                },
                CustomGrabBeamColour.LocalRotatingColour.r,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(8))
            );
            return redSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var greenSlider = CreateColourSlider(
                "Green",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalRotatingColour.g = f;
                    if (CustomGrabBeamColour.LocalRotatingColour.UseAvatarColour) { ApplyRotatingChange(); return; }
                    Color col = _rotatingGrabColourPreviewImage.color;
                    col.g = f;
                    _rotatingGrabColourPreviewImage.color = col;
                    ApplyRotatingChange();
                },
                CustomGrabBeamColour.LocalRotatingColour.g,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(9))
            );
            return greenSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var blueSlider = CreateColourSlider(
                "Blue",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalRotatingColour.b = f;
                    if (CustomGrabBeamColour.LocalRotatingColour.UseAvatarColour) { ApplyRotatingChange(); return; }
                    Color col = _rotatingGrabColourPreviewImage.color;
                    col.b = f;
                    _rotatingGrabColourPreviewImage.color = col;
                    ApplyRotatingChange();
                },
                CustomGrabBeamColour.LocalRotatingColour.b,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(10))
            );
            return blueSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var opacitySlider = CreateColourSlider(
                "Opacity",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalRotatingColour.a = f;
                    Color col = _rotatingGrabColourPreviewImage.color;
                    col.a = f;
                    _rotatingGrabColourPreviewImage.color = col;
                    ApplyRotatingChange();
                },
                CustomGrabBeamColour.LocalRotatingColour.a,
                CustomGrabColourConfig.MaxOpacity,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(11))
            );
            return opacitySlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var useAvatarColourToggle = MenuAPI.CreateREPOToggle(
                "Use Avatar Colour",
                (val) =>
                {
                    CustomGrabBeamColour.LocalRotatingColour.UseAvatarColour = val;
                    ApplyRotatingChange();
                },
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(12)),
                "Yes",
                "No",
                CustomGrabBeamColour.LocalRotatingColour.UseAvatarColour
            );
            return useAvatarColourToggle.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var avatarColourSourceToggle = CreateAvatarColourSourceToggle(
                val =>
                {
                    CustomGrabBeamColour.LocalRotatingColour.CurrentAvatarColourSource = val ? AvatarColourSource.Grabber : AvatarColourSource.ArmRight;
                    ApplyRotatingChange();
                },
                CustomGrabBeamColour.LocalRotatingColour.CurrentAvatarColourSource,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(13))
            );
            return avatarColourSourceToggle.rectTransform;
        });

        // healing grab beam colours
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var beamLabel = MenuAPI.CreateREPOLabel("Healing Beam Colour", parent, new Vector2(0, GetVerticalOffsetForScrollChildren(14)));
            beamLabel.transform.localScale = new Vector2(0.5f, 0.5f);
            return beamLabel.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var redSlider = CreateColourSlider(
                "Red",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalHealingColour.r = f;
                    if (CustomGrabBeamColour.LocalHealingColour.UseAvatarColour) { ApplyHealingChange(); return; }
                    Color col = _healingGrabColourPreviewImage.color;
                    col.r = f;
                    _healingGrabColourPreviewImage.color = col;
                    ApplyHealingChange();
                },
                CustomGrabBeamColour.LocalHealingColour.r,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(15))
            );
            return redSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var greenSlider = CreateColourSlider(
                "Green",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalHealingColour.g = f;
                    if (CustomGrabBeamColour.LocalHealingColour.UseAvatarColour) { ApplyHealingChange(); return; }
                    Color col = _healingGrabColourPreviewImage.color;
                    col.g = f;
                    _healingGrabColourPreviewImage.color = col;
                    ApplyHealingChange();
                },
                CustomGrabBeamColour.LocalHealingColour.g,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(16))
            );
            return greenSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var blueSlider = CreateColourSlider(
                "Blue",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalHealingColour.b = f;
                    if (CustomGrabBeamColour.LocalHealingColour.UseAvatarColour) { ApplyHealingChange(); return; }
                    Color col = _healingGrabColourPreviewImage.color;
                    col.b = f;
                    _healingGrabColourPreviewImage.color = col;
                    ApplyHealingChange();
                },
                CustomGrabBeamColour.LocalHealingColour.b,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(17))
            );
            return blueSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var opacitySlider = CreateColourSlider(
                "Opacity",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalHealingColour.a = f;
                    Color col = _healingGrabColourPreviewImage.color;
                    col.a = f;
                    _healingGrabColourPreviewImage.color = col;
                    ApplyHealingChange();
                },
                CustomGrabBeamColour.LocalHealingColour.a,
                CustomGrabColourConfig.MaxOpacity,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(18))
            );
            return opacitySlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var useAvatarColourToggle = MenuAPI.CreateREPOToggle(
                "Use Avatar Colour",
                (val) =>
                {
                    CustomGrabBeamColour.LocalHealingColour.UseAvatarColour = val;
                    ApplyHealingChange();
                },
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(19)),
                "Yes",
                "No",
                CustomGrabBeamColour.LocalHealingColour.UseAvatarColour
            );
            return useAvatarColourToggle.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var avatarColourSourceToggle = CreateAvatarColourSourceToggle(
                val =>
                {
                    CustomGrabBeamColour.LocalHealingColour.CurrentAvatarColourSource = val ? AvatarColourSource.Grabber : AvatarColourSource.ArmRight;
                    ApplyHealingChange();
                },
                CustomGrabBeamColour.LocalHealingColour.CurrentAvatarColourSource,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(20))
            );
            return avatarColourSourceToggle.rectTransform;
        });
        
        
        // climbing grab beam colours
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var beamLabel = MenuAPI.CreateREPOLabel("Climbing Beam Colour", parent, new Vector2(0, GetVerticalOffsetForScrollChildren(21)));
            beamLabel.transform.localScale = new Vector2(0.5f, 0.5f);
            return beamLabel.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var redSlider = CreateColourSlider(
                "Red",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalClimbingColour.r = f;
                    if (CustomGrabBeamColour.LocalClimbingColour.UseAvatarColour) { ApplyClimbingChange(); return; }
                    Color col = _climbingGrabColourPreviewImage.color;
                    col.r = f;
                    _climbingGrabColourPreviewImage.color = col;
                    ApplyClimbingChange();
                },
                CustomGrabBeamColour.LocalClimbingColour.r,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(22))
            );
            return redSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var greenSlider = CreateColourSlider(
                "Green",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalClimbingColour.g = f;
                    if (CustomGrabBeamColour.LocalClimbingColour.UseAvatarColour) { ApplyClimbingChange(); return; }
                    Color col = _climbingGrabColourPreviewImage.color;
                    col.g = f;
                    _climbingGrabColourPreviewImage.color = col;
                    ApplyClimbingChange();
                },
                CustomGrabBeamColour.LocalClimbingColour.g,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(23))
            );
            return greenSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var blueSlider = CreateColourSlider(
                "Blue",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalClimbingColour.b = f;
                    if (CustomGrabBeamColour.LocalClimbingColour.UseAvatarColour) { ApplyClimbingChange(); return; }
                    Color col = _climbingGrabColourPreviewImage.color;
                    col.b = f;
                    _climbingGrabColourPreviewImage.color = col;
                    ApplyClimbingChange();
                },
                CustomGrabBeamColour.LocalClimbingColour.b,
                1,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(24))
            );
            return blueSlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var opacitySlider = CreateColourSlider(
                "Opacity",
                "",
                f =>
                {
                    CustomGrabBeamColour.LocalClimbingColour.a = f;
                    Color col = _climbingGrabColourPreviewImage.color;
                    col.a = f;
                    _climbingGrabColourPreviewImage.color = col;
                    ApplyClimbingChange();
                },
                CustomGrabBeamColour.LocalClimbingColour.a,
                CustomGrabColourConfig.MaxOpacity,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(25))
            );
            return opacitySlider.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var useAvatarColourToggle = MenuAPI.CreateREPOToggle(
                "Use Avatar Colour",
                (val) =>
                {
                    CustomGrabBeamColour.LocalClimbingColour.UseAvatarColour = val;
                    ApplyClimbingChange();
                },
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(26)),
                "Yes",
                "No",
                CustomGrabBeamColour.LocalClimbingColour.UseAvatarColour
            );
            return useAvatarColourToggle.rectTransform;
        });
        changeGrabColourPage.AddElementToScrollView(parent =>
        {
            var avatarColourSourceToggle = CreateAvatarColourSourceToggle(
                val =>
                {
                    CustomGrabBeamColour.LocalClimbingColour.CurrentAvatarColourSource = val ? AvatarColourSource.Grabber : AvatarColourSource.ArmRight;
                    ApplyClimbingChange();
                },
                CustomGrabBeamColour.LocalClimbingColour.CurrentAvatarColourSource,
                parent,
                new Vector2(0, GetVerticalOffsetForScrollChildren(27))
            );
            return avatarColourSourceToggle.rectTransform;
        });

        // close and reset buttons
        changeGrabColourPage.AddElement(parent =>
        {
            var closeButton = MenuAPI.CreateREPOButton("Done", () => {
                changeGrabColourPage.ClosePage(true);
                CustomGrabBeamColour.UpdateBeamColourForAllBeams();
                CustomGrabBeamColour.SaveLocalColoursToConfig();
            },
                parent,
                new Vector2(HorizontalPos, 30f)
            );

            var resetButton = MenuAPI.CreateREPOButton("Reset", () => {
                changeGrabColourPage.ClosePage(true);
                CustomGrabBeamColour.ResetBeamColours();
                CustomGrabBeamColour.UpdateBeamColourForAllBeams();
                CustomGrabBeamColour.SaveLocalColoursToConfig();
            },
                parent,
                new Vector2(590f, 30f)
            );
            resetButton.overrideButtonSize = resetButton.GetLabelSize() / 2;
            resetButton.transform.localScale = new Vector2(0.5f, 0.5f);
        });

        SetupPreviewRectangleColours(CustomGrabBeamColour.LocalNeutralColour.Colour, CustomGrabBeamColour.LocalRotatingColour.Colour, CustomGrabBeamColour.LocalHealingColour.Colour, CustomGrabBeamColour.LocalClimbingColour.Colour);
        changeGrabColourPage.OpenPage(false);
    }


    private static void SetupPreviewRectangleColours(Color neutralColour, Color rotatingColour, Color healingColour, Color climbingColour)
    {
        _neutralGrabColourPreviewImage.color = neutralColour;
        _rotatingGrabColourPreviewImage.color = rotatingColour;
        _healingGrabColourPreviewImage.color = healingColour;
        _climbingGrabColourPreviewImage.color = climbingColour;
        HandleNeutralAvatarColourChange();
        HandleRotatingAvatarColourChange();
        HandleHealingAvatarColourChange();
        HandleClimbingAvatarColourChange();
    }

    private static void HandleNeutralAvatarColourChange()
    {
        _neutralGrabColourPreviewImage.color = CustomGrabBeamColour.LocalNeutralColour.UseAvatarColour ? CustomGrabBeamColour.GetLocalAvatarColour(CustomGrabBeamColour.LocalNeutralColour.CurrentAvatarColourSource, Color.black) : CustomGrabBeamColour.LocalNeutralColour.Colour;
        Color col = _neutralGrabColourPreviewImage.color;
        col.a = CustomGrabBeamColour.LocalNeutralColour.Colour.a;
        _neutralGrabColourPreviewImage.color = col;
    }
    private static void HandleRotatingAvatarColourChange()
    {
        _rotatingGrabColourPreviewImage.color = CustomGrabBeamColour.LocalRotatingColour.UseAvatarColour ? CustomGrabBeamColour.GetLocalAvatarColour(CustomGrabBeamColour.LocalRotatingColour.CurrentAvatarColourSource, Color.black) : CustomGrabBeamColour.LocalRotatingColour.Colour;
        Color col = _rotatingGrabColourPreviewImage.color;
        col.a = CustomGrabBeamColour.LocalRotatingColour.Colour.a;
        _rotatingGrabColourPreviewImage.color = col;
    }
    private static void HandleHealingAvatarColourChange()
    {
        _healingGrabColourPreviewImage.color = CustomGrabBeamColour.LocalHealingColour.UseAvatarColour ? CustomGrabBeamColour.GetLocalAvatarColour(CustomGrabBeamColour.LocalHealingColour.CurrentAvatarColourSource, Color.black) : CustomGrabBeamColour.LocalHealingColour.Colour;
        Color col = _healingGrabColourPreviewImage.color;
        col.a = CustomGrabBeamColour.LocalHealingColour.Colour.a;
        _healingGrabColourPreviewImage.color = col;
    }
    private static void HandleClimbingAvatarColourChange()
    {
        _climbingGrabColourPreviewImage.color = CustomGrabBeamColour.LocalClimbingColour.UseAvatarColour ? CustomGrabBeamColour.GetLocalAvatarColour(CustomGrabBeamColour.LocalClimbingColour.CurrentAvatarColourSource, Color.black) : CustomGrabBeamColour.LocalClimbingColour.Colour;
        Color col = _climbingGrabColourPreviewImage.color;
        col.a = CustomGrabBeamColour.LocalClimbingColour.Colour.a;
        _climbingGrabColourPreviewImage.color = col;
    }

    private static void ApplyNeutralChange()
    {
        HandleNeutralAvatarColourChange();
        ApplyBeamChange(BeamType.Neutral, CustomGrabBeamColour.LocalNeutralColour);
    }

    private static void ApplyRotatingChange()
    {
        HandleRotatingAvatarColourChange();
        ApplyBeamChange(BeamType.Rotate, CustomGrabBeamColour.LocalRotatingColour);
    }

    private static void ApplyHealingChange()
    {
        HandleHealingAvatarColourChange();
        ApplyBeamChange(BeamType.Heal, CustomGrabBeamColour.LocalHealingColour);
    }

    private static void ApplyClimbingChange()
    {
        HandleClimbingAvatarColourChange();
        ApplyBeamChange(BeamType.Climb, CustomGrabBeamColour.LocalClimbingColour);
    }

    private static void ApplyBeamChange(BeamType beamType, GrabBeamColourSettings settings)
    {
        CustomGrabBeamColour.UpdateBeamColour(beamType);
        CustomGrabColourConfig.SaveColour(settings);
    }

    private static void CreateBeamPreviewColourRectangle(Transform parent, Vector2 position, out GameObject colourPreviewParent, out Image colourPreviewImage)
    {
        colourPreviewParent = new GameObject();
        colourPreviewParent.name = "Grab Beam Colour Preview Rectangle Parent";
        colourPreviewParent.transform.SetParent(parent);

        Canvas canvas = colourPreviewParent.AddComponent<Canvas>();
        canvas.transform.SetParent(colourPreviewParent.gameObject.transform.parent);

        GameObject imageGameObject = new GameObject();
        imageGameObject.transform.SetParent(canvas.gameObject.transform.parent);

        colourPreviewImage = imageGameObject.AddComponent<Image>();
        colourPreviewImage.name = "Grab Beam Colour Preview Rectangle Image";
        colourPreviewImage.transform.SetParent(imageGameObject.gameObject.transform.parent);

        colourPreviewImage.color = new Color(1.0F, 0.0F, 0.0F);
        colourPreviewImage.gameObject.transform.position = position;
        colourPreviewImage.gameObject.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
    }

    private static REPOSlider CreateColourSlider(string name, string desc, Action<float> onChange, float initial, float max, Transform parent, Vector2 localPosition)
    {
        return MenuAPI.CreateREPOSlider(
            name,
            desc,
            onChange,
            parent,
            localPosition,
            0,
            max,
            2,
            initial
        );
    }

    private static REPOToggle CreateAvatarColourSourceToggle(Action<bool> onChange, AvatarColourSource initialSource, Transform parent, Vector2 localPosition)
    {
        return MenuAPI.CreateREPOToggle(
            "Avatar Colour Source",
            onChange,
            parent,
            localPosition,
            "Grabber",
            "Arm",
            initialSource == AvatarColourSource.Grabber
        );
    }

    private static float GetVerticalOffsetForScrollChildren(int index)
    {
        return 40 * index;
    }

    private const float VerticalBase = -160f;
    private const float VerticalSpacing = 150f;

    private static float GetVerticalPos(int offset)
    {
        return VerticalBase + VerticalSpacing * (3 - offset);
    }
}
