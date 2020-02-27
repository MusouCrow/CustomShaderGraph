using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI {
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
        EditorGUI.BeginChangeCheck();
        
        base.OnGUI(materialEditor, properties);

        Material material = materialEditor.target as Material;

        this.DoRenderType(material, out bool opaque);
        this.DoBlendMode(material, out bool addtive);

        if (EditorGUI.EndChangeCheck()) {
            this.Adjust(material, opaque, addtive);
        }
    }

    private bool IsAddtiveMode(Material material) {
        int src = material.GetInt("_SrcBlend");
        int dst = material.GetInt("_DstBlend");

        return src == (int)BlendMode.One && dst == (int)BlendMode.One;
    }

    private void DoRenderType(Material material, out bool opaque) {
        string renderType = material.GetTag("RenderType", false);
        opaque = renderType == "Opaque";
        opaque = GUILayout.Toggle(opaque, "Opaque");
    }

    private void DoBlendMode(Material material, out bool addtive) {
        addtive = this.IsAddtiveMode(material);
        addtive = GUILayout.Toggle(addtive, "Addtive Mode");
    }

    private void Adjust(Material material, bool opaque, bool addtive) {
        if (opaque) {
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            material.SetOverrideTag("RenderType", "Opaque");
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        else {
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_ZWrite", 0);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            if (addtive) {
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            }
            else {
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            }
        }

        int src = material.GetInt("_SrcBlend");
        int dst = material.GetInt("_DstBlend");

        if (addtive) {
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
}
