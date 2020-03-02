using System;
using System.Linq;
using System.Collections.Generic;
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
        
        private static int Sort(MaterialProperty a, MaterialProperty b) {
            return a.displayName.Length - b.displayName.Length;
        }

        private Dictionary<string, bool> foldoutMap = new Dictionary<string, bool>();

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
            EditorGUI.BeginChangeCheck();
            
            Material material = materialEditor.target as Material;

            DrawRenderType(material, out bool opaque);
            DrawBlendMode(material, out bool additive);
            DrawCull(material, out bool twoSided);

            var typeMap = new Dictionary<string, List<MaterialProperty>>();
            var keys = new List<string>();

            foreach (var v in properties) {
                int index = material.shader.FindPropertyIndex(v.name);
                var flags = material.shader.GetPropertyFlags(index);

                if (flags == ShaderPropertyFlags.HideInInspector) {
                    continue;
                }

                int pos = v.displayName.IndexOf(" ");
                string key = pos == -1 ? v.displayName : v.displayName.Substring(0, pos);

                if (!typeMap.ContainsKey(key)) {
                    typeMap.Add(key, new List<MaterialProperty>());
                    keys.Add(key);
                }
                
                typeMap[key].Add(v);
            }

            keys.Sort((string a, string b) => {
                return typeMap[a].Count - typeMap[b].Count;
            });

            foreach (var k in keys) {
                if (typeMap[k].Count > 1) {
                    typeMap[k].Sort(Sort);

                    if (!this.foldoutMap.ContainsKey(k)) {
                        this.foldoutMap[k] = false;
                    }

                    this.foldoutMap[k] = EditorGUILayout.BeginFoldoutHeaderGroup(this.foldoutMap[k], k);

                    if (this.foldoutMap[k]) {
                        foreach (var v in typeMap[k]) {
                            int pos = v.displayName.IndexOf(" ");
                            string name = pos == -1 ? v.displayName : v.displayName.Substring(pos + 1);

                            materialEditor.ShaderProperty(v, name);
                        }
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
                else {
                    foreach (var v in typeMap[k]) {
                        materialEditor.ShaderProperty(v, v.displayName);
                    }
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