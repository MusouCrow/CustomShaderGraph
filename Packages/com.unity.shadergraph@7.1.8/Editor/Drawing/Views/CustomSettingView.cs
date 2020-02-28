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

            ps.Add(new PropertyRow(new Label("Outline")), (row) =>
                {
                    row.Add(new Toggle(), (toggle) =>
                    {
                        toggle.value = node.Outline.isOn;
                        toggle.OnToggleChanged(ChangeOutline);
                    });
                });

            Add(ps);
        }

        private void ChangeOutline(ChangeEvent<bool> evt) {
            this.node.owner.owner.RegisterCompleteObjectUndo("Outline");
            ToggleData td = this.node.Outline;
            td.isOn = evt.newValue;
            this.node.Outline = td;
        }
    }
}
