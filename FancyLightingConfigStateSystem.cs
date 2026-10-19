using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FancyLightingModCallTest;

public class FancyLightingConfigStateSystem : ModSystem
{
    private static int _smoothLightingEnabledIndex = 0;
    private static int _smoothLightingRenderModeIndex = 0;
    private static int _smoothLightingGlowRenderModeIndex = 0;
    private static int _smoothLightingActiveLightMapOverrideIndex = 0;

    private static int _fullHdrEnumValue = 0;
    private static int _accurateEnumValue = 0;
    private static int _fullBrightEnumValue = 0;

    private static int[] _fancyLightingConfigState = [-1]; // fallback dummy value in case the mod calls don't work

    public override void PostSetupContent()
    {
        // This example is not exhaustive

        if (Main.dedServ)
        {
            return;
        }

        if (!ModLoader.TryGetMod("FancyLighting", out var fancyLighting))
        {
            return;
        }

        {
            if (fancyLighting.Call("GetConfigStateInfo") is not int[] configState)
            {
                return;
            }

            _fancyLightingConfigState = configState;
        }

        if (fancyLighting.Call("GetConfigStateInfoMetadataNames") is not string[] configNames)
        {
            return;
        }

        if (fancyLighting.Call("GetConfigStateInfoMetadataTypes") is not string[] configTypes)
        {
            return;
        }

        if (
            fancyLighting.Call("GetConfigStateTypeMetadata")
            is not Dictionary<string, string[]> typeMetadata
        )
        {
            return;
        }

        // element 0 is a dummy value, so if Array.IndexOf() returns -1 we want it to be 0 instead
        _smoothLightingEnabledIndex = Math.Max(
            Array.IndexOf(configNames, "SmoothLightingEnabled"),
            0
        );
        _smoothLightingRenderModeIndex = Math.Max(
            Array.IndexOf(configNames, "SmoothLightingRenderMode"),
            0
        );
        _smoothLightingGlowRenderModeIndex = Math.Max(
            Array.IndexOf(configNames, "SmoothLightingGlowRenderMode"),
            0
        );
        _smoothLightingActiveLightMapOverrideIndex = Math.Max(
            Array.IndexOf(configNames, "SmoothLightingActiveLightMapOverride"),
            0
        );

        if (_smoothLightingRenderModeIndex != 0)
        {
            if (
                typeMetadata.TryGetValue(
                    configTypes[_smoothLightingRenderModeIndex],
                    out var enumValues
                )
            )
            {
                _fullHdrEnumValue = Array.IndexOf(enumValues, "FullHDR");
            }
        }

        if (_smoothLightingGlowRenderModeIndex != 0)
        {
            if (
                typeMetadata.TryGetValue(
                    configTypes[_smoothLightingGlowRenderModeIndex],
                    out var enumValues
                )
            )
            {
                _accurateEnumValue = Array.IndexOf(enumValues, "Accurate");
            }
        }

        if (_smoothLightingActiveLightMapOverrideIndex != 0)
        {
            if (
                typeMetadata.TryGetValue(
                    configTypes[_smoothLightingActiveLightMapOverrideIndex],
                    out var enumValues
                )
            )
            {
                _fullBrightEnumValue = Array.IndexOf(enumValues, "FullBright");
            }
        }

        On_Main.DrawTiles += (
            On_Main.orig_DrawTiles orig,
            Main self,
            bool solidLayer,
            bool forRenderTargets,
            bool intoRenderTargets,
            int waterStyleOverride
        ) =>
        {
            if (
                _fancyLightingConfigState[_smoothLightingActiveLightMapOverrideIndex]
                == _fullBrightEnumValue
            )
            {
                Main.NewText("The full bright pass of Smooth Lighting is active");
            }

            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        };
    }

    public override void Unload() => _fancyLightingConfigState = null;

    public override void PostUpdateEverything()
    {
        if (Main.dedServ)
        {
            return;
        }

        if (_fancyLightingConfigState[_smoothLightingEnabledIndex] == 1)
        {
            Main.NewText("Smooth Lighting is Enabled");
        }

        if (_fancyLightingConfigState[_smoothLightingRenderModeIndex] == _fullHdrEnumValue)
        {
            Main.NewText("Full HDR Rendering is Enabled");
        }

        if (_fancyLightingConfigState[_smoothLightingGlowRenderModeIndex] == _accurateEnumValue)
        {
            Main.NewText("Accurate glow rendering is Enabled");
        }
    }
}
