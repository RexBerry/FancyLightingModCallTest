using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FancyLightingModCallTest
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class FancyLightingModCallTest : Mod
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("FancyLighting", out var fancyLighting))
            {
                fancyLighting.Call("AddCustomTileLighting", TileID.Dirt, MakeCheckerboard);
                fancyLighting.Call("AddHook", "PostUpdateLightMap", LightMapUpdateCallback);
                fancyLighting.Call(
                    "AddHook",
                    "ModifyFancyAtmosphereColors",
                    ModifyFancyAtmosphereColors
                );
                fancyLighting.Call(
                    "AddHook",
                    "ModifyFancySkyLightShadingStrength",
                    ModifyFancySkyLightShadingStrength
                );
                // fancyLighting.Call("RemoveCustomTileLighting", TileID.Dirt);
                Console.WriteLine(fancyLighting.Call("GarbageCommand"));
            }
        }

        private static void MakeCheckerboard(Tile tile, int x, int y, ref Vector3 lightColor)
        {
            var brightness = (x + y) % 2 == 0 ? 0f : 1f;
            lightColor.X = brightness;
            lightColor.Y = brightness;
            lightColor.Z = brightness;
        }

        private static void LightMapUpdateCallback(
            Texture2D lightMapTexture,
            Matrix samplingTransformation,
            Rectangle lightMapArea,
            bool cameraMode
        )
        {
            Main.NewText(
                $"{lightMapTexture.Size()}, {samplingTransformation}, {lightMapArea}, {cameraMode}"
            );
        }

        private static void ModifyFancyAtmosphereColors(
            ref Vector3 lowSkyColor,
            ref Vector3 middleSkyColor,
            ref Vector3 highSkyColor,
            ref Vector3 skyColorMult
        )
        {
            middleSkyColor = Vector3.Zero;
            skyColorMult *= 0.5f;
        }

        private static void ModifyFancySkyLightShadingStrength(ref float weatherMultiplier) =>
            weatherMultiplier = 0f;
    }
}
