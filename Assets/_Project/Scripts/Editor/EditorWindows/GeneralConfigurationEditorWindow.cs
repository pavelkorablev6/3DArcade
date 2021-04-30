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

using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace Arcade.UnityEditor
{
    internal sealed class GeneralConfigurationEditorWindow : EditorWindow
    {
        private const float MIN_WINDOW_WIDTH  = 468f;
        private const float MIN_WINDOW_HEIGHT = 468f;

        private GeneralConfigurationSO _cfg;
        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;

        private Vector2 _scrollPosition;

        [MenuItem("3DArcade/General Configuration", false, 0), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Editor Menu")]
        private static void ShowWindow()
            => GetWindow<GeneralConfigurationEditorWindow>("General Configuration", true)
               .minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);

        private void OnEnable() => Initialize();

        private void Initialize()
        {
            EditorGUI.FocusTextInControl(null);
            _scrollPosition = Vector2.zero;

            string dataPath        = SystemUtils.GetDataPath();
            IVirtualFileSystem vfs = new VirtualFileSystem().MountFile("general_cfg", $"{dataPath}/3darcade~/Configuration/GeneralConfiguration.xml");

            GeneralConfiguration cfg = new GeneralConfiguration(vfs);
            cfg.Initialize();

            _cfg       = CreateInstance<GeneralConfigurationSO>();
            _cfg.Value = cfg;

            _serializedObject   = new SerializedObject(_cfg);
            _serializedProperty = _serializedObject.FindProperty("Value");
        }

        private void OnGUI()
        {
            Color originalBackgroundColor = GUI.backgroundColor;

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button(new GUIContent("Save", "Save this configuration")))
                {
                    SaveButtonClicked();
                    return;
                }

                GUI.backgroundColor = Color.yellow;
                GUI.enabled         = true;
                if (GUILayout.Button(new GUIContent("Cancel", "Revert all the changes")))
                {
                    CancelButtonClicked();
                    return;
                }

                GUI.backgroundColor = originalBackgroundColor;
            }

            int indentLevel = EditorGUI.indentLevel;
            using (GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox))
            {
                _scrollPosition = scrollView.scrollPosition;
                EditorGUI.indentLevel = -1;

                _ = EditorGUILayout.PropertyField(_serializedProperty, GUIContent.none, true);
                _serializedProperty.isExpanded = true;
                _ = _serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel = indentLevel;
        }

        private void SaveButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            _ = _cfg.Value.Save();
        }

        private void CancelButtonClicked() => Initialize();
    }
}
