using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityEditor.ShaderGraph.Drawing {
    class CustomSettingsView : VisualElement {
        private CustomMasterNode node;

        public CustomSettingsView(CustomMasterNode node) {
            this.node = node;

            PropertySheet ps = new PropertySheet();

            ps.Add(new PropertyRow(new Label("Surface")), (row) =>
                {
                    row.Add(new EnumField(SurfaceType.Opaque), (field) =>
                    {
                        field.value = this.node.SurfaceType;
                        field.RegisterValueChangedCallback(ChangeSurface);
                    });
                });

            ps.Add(new PropertyRow(new Label("Blend")), (row) =>
                {
                    row.Add(new EnumField(AlphaMode.Additive), (field) =>
                    {
                        field.value = this.node.AlphaMode;
                        field.RegisterValueChangedCallback(ChangeAlphaMode);
                    });
                });

            ps.Add(new PropertyRow(new Label("Two Sided")), (row) =>
                {
                    row.Add(new Toggle(), (toggle) =>
                    {
                        toggle.value = this.node.TwoSided.isOn;
                        toggle.OnToggleChanged(ChangeTwoSided);
                    });
                });

            Add(ps);
        }

        void ChangeSurface(ChangeEvent<Enum> evt) {
            if (Equals(this.node.SurfaceType, evt.newValue))
                return;

            this.node.owner.owner.RegisterCompleteObjectUndo("Surface Change");
            this.node.SurfaceType = (SurfaceType)evt.newValue;
        }

        void ChangeAlphaMode(ChangeEvent<Enum> evt) {
            if (Equals(this.node.AlphaMode, evt.newValue))
                return;

            this.node.owner.owner.RegisterCompleteObjectUndo("Alpha Mode Change");
            this.node.AlphaMode = (AlphaMode)evt.newValue;
        }

        void ChangeTwoSided(ChangeEvent<bool> evt) {
            this.node.owner.owner.RegisterCompleteObjectUndo("Two Sided Change");
            ToggleData td = this.node.TwoSided;
            td.isOn = evt.newValue;
            this.node.TwoSided = td;
        }
    }
}
