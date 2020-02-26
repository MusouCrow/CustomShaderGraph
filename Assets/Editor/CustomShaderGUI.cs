using UnityEngine;
using UnityEditor;

public class CustomShaderGUI : ShaderGUI {
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
        EditorGUI.BeginChangeCheck();
        
        base.OnGUI(materialEditor, properties);

        if (EditorGUI.EndChangeCheck()) {
            Material material = materialEditor.target as Material;
            
            {
                float v = 0;

                if (material.HasProperty("_AddtiveMode") && material.GetFloat("_AddtiveMode") > 0) {
                    v = 1;
                }
                
                material.SetFloat("_DstBlend", v);
            }
        }
    }
}
