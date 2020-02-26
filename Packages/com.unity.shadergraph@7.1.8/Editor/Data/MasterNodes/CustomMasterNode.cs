using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph {
    interface ICustomSubShader : ISubShader {}
    
    [Serializable]
    [Title("Master", "Custom")]
    class CustomMasterNode : MasterNode<ICustomSubShader> {
        public const string ColorSlotName = "Color";
        public const string AlphaSlotName = "Alpha";
        public const string AlphaClipThresholdSlotName = "AlphaClipThreshold";

        public const int ColorSlotId = 0;
        public const int AlphaSlotId = 7;
        public const int AlphaThresholdSlotId = 8;

        public CustomMasterNode() {
            this.UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization() {
            base.UpdateNodeAfterDeserialization();
            this.name = "Custom Master";

            this.AddSlot(new ColorRGBMaterialSlot(ColorSlotId, ColorSlotName, ColorSlotName, SlotType.Input, Color.grey.gamma, ColorMode.Default, ShaderStageCapability.Fragment));
            this.AddSlot(new Vector1MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, 1, ShaderStageCapability.Fragment));
            this.AddSlot(new Vector1MaterialSlot(AlphaThresholdSlotId, AlphaClipThresholdSlotName, AlphaClipThresholdSlotName, SlotType.Input, 0, ShaderStageCapability.Fragment));

            this.RemoveSlotsNameNotMatching(new[] {
                ColorSlotId,
                AlphaSlotId,
                AlphaThresholdSlotId
            });
        }
        
        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode) {
            base.CollectShaderProperties(properties, generationMode);
            
            PropertyUtil2.AddProperty(properties, "SrcBlend", "_SrcBlend", 1, true);
            PropertyUtil2.AddProperty(properties, "DstBlend", "_DstBlend", 0, true);
        }
    }
}