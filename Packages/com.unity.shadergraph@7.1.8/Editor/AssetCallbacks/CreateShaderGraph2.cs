using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    static class CreateShaderGraph2
    {
        [MenuItem("Assets/Create/Shader/Custom Graph", false, 208)]
        public static void CreateCustomMasterMaterialGraph()
        {
            GraphUtil.CreateNewGraph(new CustomMasterNode());
        }
    }
}
