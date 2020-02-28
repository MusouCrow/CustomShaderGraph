using UnityEngine;
using UnityEngine.Rendering;

namespace Game {
    public static class CustomShaderSvc {
        public static void DoRenderType(Material material, bool opaque, bool additive) {
            if (opaque) {
                material.renderQueue = (int)RenderQueue.Geometry;
                material.SetOverrideTag("RenderType", "Opaque");
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else {
                material.renderQueue = (int)RenderQueue.Transparent;
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_ZWrite", 0);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

                if (additive) {
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                }
                else {
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                }
            }
        }

        public static void DoBlendMode(Material material, bool opaque, bool additive) {
            int src = material.GetInt("_SrcBlend");
            int dst = material.GetInt("_DstBlend");

            if (additive) {
                src = (int)BlendMode.One;
                dst = (int)BlendMode.One;
            }
            else {
                if (opaque) {
                    src = (int)BlendMode.One;
                    dst = (int)BlendMode.Zero;
                }
                else {
                    src = (int)BlendMode.SrcAlpha;
                    dst = (int)BlendMode.OneMinusSrcAlpha;
                }
            }

            material.SetInt("_SrcBlend", src);
            material.SetInt("_DstBlend", dst);
        }

        public static void DoAlphaClip(Material material) {
            if (material.HasProperty("_AlphaClip")) {
                float alphaClip = material.GetFloat("_AlphaClip");

                if (alphaClip > 0) {
                    material.EnableKeyword("_ALPHATEST_ON");
                }
                else {
                    material.DisableKeyword("_ALPHATEST_ON");
                }
            }
            else {
                material.DisableKeyword("_ALPHATEST_ON");
            }
        }

        public static void DoCull(Material material, bool twoSided) {
            CullMode cull = twoSided ? CullMode.Off : CullMode.Back;
            material.SetInt("_Cull", (int)cull);
            material.doubleSidedGI = twoSided;
        }
    }
}