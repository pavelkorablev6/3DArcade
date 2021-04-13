/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using UnityEditor;
using UnityEngine;

namespace Arcade.UnityEditor
{
    [CustomPropertyDrawer(typeof(DatabaseVector3))]
    internal sealed class DatabaseVector3Drawer : PropertyDrawer
    {
        private const float SUBLABEL_SPACING = 4f;
        private const float BOTTOM_SPACING   = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height -= BOTTOM_SPACING;

            label = EditorGUI.BeginProperty(position, label, property);

            Rect contentRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUIContent[] labels = new []
            {
                new GUIContent("X"),
                new GUIContent("Y"),
                new GUIContent("Z")
            };

            SerializedProperty[] properties = new []
            {
                property.FindPropertyRelative("X"),
                property.FindPropertyRelative("Y"),
                property.FindPropertyRelative("Z")
            };

            int propsCount  = properties.Length;
            float width     = (contentRect.width - (propsCount - 1) * SUBLABEL_SPACING) / propsCount;
            Rect contentPos = new Rect(contentRect.x, contentRect.y, width, contentRect.height);

            int indent       = EditorGUI.indentLevel;
            float labelWidth = EditorGUIUtility.labelWidth;

            EditorGUI.indentLevel = 0;
            for (int i = 0; i < propsCount; ++i)
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(labels[i]).x;
                _ = EditorGUI.PropertyField(contentPos, properties[i], labels[i]);
                contentPos.x += width + SUBLABEL_SPACING;
            }
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel       = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => base.GetPropertyHeight(property, label) + BOTTOM_SPACING;
    }
}
