using System;
using System.Collections.Generic;
using System.Linq;
using Data.Util;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal {
    class CustomSubShader : ICustomSubShader {
        private ShaderPass litPass;
        private ShaderPass depthOnlyPass;
        private ShaderPass shadowCasterPass;

        private static KeywordDescriptor SmoothnessChannelKeyword = new KeywordDescriptor() {
            displayName = "Smoothness Channel",
            referenceName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        private static ActiveFields GetActiveFieldsFromMasterNode(CustomMasterNode masterNode, ShaderPass pass) {
            var activeFields = new ActiveFields();
            var baseActiveFields = activeFields.baseInstance;

            baseActiveFields.Add("features.graphPixel");

            if (masterNode.IsSlotConnected(CustomMasterNode.AlphaThresholdSlotId) || masterNode.GetInputSlots<Vector1MaterialSlot>().First(x => x.id == CustomMasterNode.AlphaThresholdSlotId).value > 0.0f) {
                baseActiveFields.Add("AlphaClip");
            }

            if (masterNode.SurfaceType != ShaderGraph.SurfaceType.Opaque) {
                baseActiveFields.Add("SurfaceType.Transparent");

                if (masterNode.AlphaMode == AlphaMode.Alpha) {
                    baseActiveFields.Add("BlendMode.Alpha");
                }
                else if (masterNode.AlphaMode == AlphaMode.Additive) {
                    baseActiveFields.Add("BlendMode.Add");
                }
                else if (masterNode.AlphaMode == AlphaMode.Premultiply) {
                    baseActiveFields.Add("BlendMode.Premultiply");
                }
            }

            return activeFields;
        }

        private static bool GenerateShaderPass(CustomMasterNode masterNode, ShaderPass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths) {
            UniversalShaderGraphUtilities.SetRenderState(masterNode.SurfaceType, masterNode.AlphaMode, masterNode.TwoSided.isOn, ref pass);

            var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);
            
            return ShaderGraph.GenerationUtils.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths,
                UniversalShaderGraphResources.s_Dependencies, UniversalShaderGraphResources.s_ResourceClassName, UniversalShaderGraphResources.s_AssemblyName);
        }

        public CustomSubShader() {
            this.litPass = new ShaderPass() {
                displayName = "Lit",
                referenceName = "CUSTOM_LIT_PASS",
                lightMode = "UniversalForward",
                passInclude = "Assets/Shader/Include/LitPass.hlsl",
                varyingsInclude = "Assets/Shader/Include/Varyings.hlsl",

                pixelPorts = new List<int>() {
                    CustomMasterNode.ColorSlotId,
                    CustomMasterNode.AlphaSlotId,
                    CustomMasterNode.AlphaThresholdSlotId
                },

                includes = new List<string>() {
                    "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                },
                pragmas = new List<string>() {
                    "prefer_hlslcc gles",
                    "exclude_renderers d3d11_9x",
                    "target 2.0",
                    "multi_compile_instancing",
                },
            };

            this.depthOnlyPass = new ShaderPass() {
                displayName = "DepthOnly",
                referenceName = "SHADERPASS_DEPTHONLY",
                lightMode = "DepthOnly",
                passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl",
                varyingsInclude = "Assets/Shader/Include/Varyings.hlsl",

                pixelPorts = new List<int>() {
                    CustomMasterNode.AlphaSlotId,
                    CustomMasterNode.AlphaThresholdSlotId
                },

                includes = new List<string>() {
                    "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                },
                pragmas = new List<string>() {
                    "prefer_hlslcc gles",
                    "exclude_renderers d3d11_9x",
                    "target 2.0",
                    "multi_compile_instancing",
                },

                ZWriteOverride = "ZWrite On",
                ColorMaskOverride = "ColorMask 0",
            };

            this.shadowCasterPass = new ShaderPass() {
                displayName = "ShadowCaster",
                referenceName = "SHADERPASS_SHADOWCASTER",
                lightMode = "ShadowCaster",
                passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl",
                varyingsInclude = "Assets/Shader/Include/Varyings.hlsl",

                pixelPorts = new List<int>() {
                    CustomMasterNode.AlphaSlotId,
                    CustomMasterNode.AlphaThresholdSlotId
                },

                requiredAttributes = new List<string>() {
                    "Attributes.normalOS", 
                },

                includes = new List<string>() {
                    "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                    "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                },
                pragmas = new List<string>() {
                    "prefer_hlslcc gles",
                    "exclude_renderers d3d11_9x",
                    "target 2.0",
                    "multi_compile_instancing",
                },
                keywords = new KeywordDescriptor[] {
                    SmoothnessChannelKeyword,
                },

                ZWriteOverride = "ZWrite On",
                ZTestOverride = "ZTest LEqual",
            };
        }

        public int GetPreviewPassIndex() {
            return 0;
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset) {
            return renderPipelineAsset is UniversalRenderPipelineAsset;
        }

        public string GetSubshader(IMasterNode masterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null) {
            if (sourceAssetDependencyPaths != null) {
                // 添加本文件的GUID
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("4b67b0462483b4352b2413cbacaf6549"));
            }

            var customMasterNode = masterNode as CustomMasterNode;
            var subShader = new ShaderGenerator();

            subShader.AddShaderChunk("SubShader {", true);
            subShader.Indent();

            {
                var surfaceTags = ShaderGenerator.BuildMaterialTags(customMasterNode.SurfaceType);
                var tagsBuilder = new ShaderStringBuilder(0);
                surfaceTags.GetTags(tagsBuilder, "UniversalPipeline");
                subShader.AddShaderChunk(tagsBuilder.ToString());
                
                CustomSubShader.GenerateShaderPass(customMasterNode, litPass, mode, subShader, sourceAssetDependencyPaths);
            }

            subShader.Deindent();
            subShader.AddShaderChunk("}", true);

            return subShader.GetShaderString(0);
        }
    }
}