using System;
using UnityEngine;
using UnityEditor.ShaderGraph.Internal;

namespace UnityEditor.ShaderGraph {
    static class PropertyUtil2 {
        public static void AddProperty(PropertyCollector properties, string displayName, string referenceName, float value, bool hidden) {
            var property = new Vector1ShaderProperty() {
                displayName = displayName,
                overrideReferenceName = referenceName,
                value = value,
                hidden = hidden
            };

            properties.AddShaderProperty(property);
        }

        public static void AddProperty(PropertyCollector properties, string displayName, string referenceName, bool value, bool hidden) {
            var property = new BooleanShaderProperty() {
                displayName = displayName,
                overrideReferenceName = referenceName,
                value = value,
                hidden = hidden
            };

            properties.AddShaderProperty(property);
        }
    }
}
