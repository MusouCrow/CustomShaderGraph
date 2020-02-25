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

        [SerializeField]
        private SurfaceType surfaceType;

        public SurfaceType SurfaceType {
            get {
                return surfaceType;
            }
            set {
                if (surfaceType == value)
                    return;
                
                surfaceType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private AlphaMode alphaMode;

        public AlphaMode AlphaMode {
            get {
                return alphaMode;
            }
            set {
                if (alphaMode == value)
                    return;

                alphaMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private bool twoSided;

        public ToggleData TwoSided {
            get {
                return new ToggleData(twoSided);
            }
            set {
                if (twoSided == value.isOn)
                    return;
                
                twoSided = value.isOn;
                Dirty(ModificationScope.Graph);
            }
        }

        public CustomMasterNode() {
            this.UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization() {
            base.UpdateNodeAfterDeserialization();
            this.name = "Custom Master";

            this.AddSlot(new ColorRGBMaterialSlot(ColorSlotId, ColorSlotName, ColorSlotName, SlotType.Input, Color.grey.gamma, ColorMode.Default, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, 1, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(AlphaThresholdSlotId, AlphaClipThresholdSlotName, AlphaClipThresholdSlotName, SlotType.Input, 0, ShaderStageCapability.Fragment));

            this.RemoveSlotsNameNotMatching(new[] {
                ColorSlotId,
                AlphaSlotId,
                AlphaThresholdSlotId
            });
        }

        protected override VisualElement CreateCommonSettingsElement() {
            return new CustomSettingsView(this);
        }
    }
}