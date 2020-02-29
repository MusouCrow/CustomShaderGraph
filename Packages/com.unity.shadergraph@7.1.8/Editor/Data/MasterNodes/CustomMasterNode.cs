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

        public const int ColorSlotId = 0;

        [SerializeField]
        private bool outline;
        
        public ToggleData Outline {
            get {
                return new ToggleData(outline);
            }
            set {
                if (outline == value.isOn) {
                    return;
                }
                
                outline = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        public CustomMasterNode() {
            this.UpdateNodeAfterDeserialization();
        }

        public override void UpdateNodeAfterDeserialization() {
            base.UpdateNodeAfterDeserialization();
            this.name = "Custom Master";

            this.AddSlot(new ColorRGBAMaterialSlot(ColorSlotId, ColorSlotName, ColorSlotName, SlotType.Input, Color.grey.gamma, ShaderStageCapability.Fragment));

            this.RemoveSlotsNameNotMatching(new[] {
                ColorSlotId
            });
        }

        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode) {
            base.CollectShaderProperties(properties, generationMode);
            
            PropertyUtil2.AddProperty(properties, "SrcBlend", "_SrcBlend", 1, true);
            PropertyUtil2.AddProperty(properties, "DstBlend", "_DstBlend", 0, true);
            PropertyUtil2.AddProperty(properties, "ZWrite", "_ZWrite", 1, true);
            PropertyUtil2.AddProperty(properties, "Cull", "_Cull", 2, true);
            PropertyUtil2.AddProperty(properties, "Alpha Clip", "_AlphaClip", 1, false, FloatType.Slider);
        }

        protected override VisualElement CreateCommonSettingsElement() {
            return new CustomSettingsView(this);
        }
    }
}