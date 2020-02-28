using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Game {
    public class CustomShaderGUI : ShaderGUI {
        private static bool IsAdditiveMode(Material material) {
            int src = material.GetInt("_SrcBlend");
            int dst = material.GetInt("_DstBlend");

            return src == (int)BlendMode.One && dst == (int)BlendMode.One;
        }

        private static void DrawRenderType(Material material, out bool opaque) {
            string renderType = material.GetTag("RenderType", false);
            opaque = renderType == "Opaque";
            opaque = EditorGUILayout.Toggle("Opaque", opaque);
        }

        private static void DrawBlendMode(Material material, out bool additive) {
            additive = IsAdditiveMode(material);
            additive = EditorGUILayout.Toggle("Addtive Mode", additive);
        }
        
        private static void DrawCull(Material material, out bool twoSided) {
            int cull = material.GetInt("_Cull");
            twoSided = cull != (int)CullMode.Back;
            twoSided = EditorGUILayout.Toggle("Two Sided", twoSided);
        }
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
            EditorGUI.BeginChangeCheck();
            
            Material material = materialEditor.target as Material;

            DrawRenderType(material, out bool opaque);
            DrawBlendMode(material, out bool additive);
            DrawCull(material, out bool twoSided);

            foreach (var v in properties) {
                int index = material.shader.FindPropertyIndex(v.name);
                var flags = material.shader.GetPropertyFlags(index);

                if (flags == ShaderPropertyFlags.HideInInspector) {
                    continue;
                }

                if (v.type == MaterialProperty.PropType.Color) {
                    materialEditor.ColorProperty(v, v.displayName);
                }
                else if (v.type == MaterialProperty.PropType.Float) {
                    materialEditor.FloatProperty(v, v.displayName);
                }
                else if (v.type == MaterialProperty.PropType.Range) {
                    materialEditor.RangeProperty(v, v.displayName);
                }
                else if (v.type == MaterialProperty.PropType.Texture) {
                    materialEditor.TextureProperty(v, v.displayName);
                }
                else if (v.type == MaterialProperty.PropType.Vector) {
                    materialEditor.VectorProperty(v, v.displayName);
                }
            }

            if (EditorGUI.EndChangeCheck()) {
                this.Adjust(material, opaque, additive, twoSided);
            }
        }

        private void Adjust(Material material, bool opaque, bool additive, bool twoSided) {
            CustomShaderSvc.DoRenderType(material, opaque, additive);
            CustomShaderSvc.DoBlendMode(material, opaque, additive);
            CustomShaderSvc.DoAlphaClip(material);
            CustomShaderSvc.DoCull(material, twoSided);
        }
    }
}