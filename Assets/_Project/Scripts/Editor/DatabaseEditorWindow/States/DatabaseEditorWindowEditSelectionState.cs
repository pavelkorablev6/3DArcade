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
    internal sealed class DatabaseEditorWindowEditSelectionState<T> : DatabaseEditorWindowState<T> where T : DatabaseEntry
    {
        private T _configuration;

        public DatabaseEditorWindowEditSelectionState(DatabaseEditorWindowContext<T> context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            EditorGUI.FocusTextInControl(null);
            _scrollPosition = Vector2.zero;
            _configuration = (_context.TempCfgObject.targetObject as ConfigurationSO<T>).Value;
        }

        public override void OnExit() => _context.ClearSerializedFields();

        public override void OnUpdate(float dt)
        {
            Color originalBackgroundColor = GUI.backgroundColor;

            bool isIdValid          = !string.IsNullOrEmpty(_configuration.Id);
            bool isDescriptionValid = !string.IsNullOrEmpty(_configuration.Description);

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (isIdValid && isDescriptionValid)
                {
                    GUI.backgroundColor = Color.green;
                    GUI.enabled         = true;
                }
                else
                {
                    GUI.backgroundColor = Color.red;
                    GUI.enabled         = false;
                }

                if (GUILayout.Button("Save"))
                {
                    SaveButtonClicked();
                    return;
                }

                GUI.backgroundColor = Color.yellow;
                GUI.enabled         = true;
                if (GUILayout.Button("Cancel"))
                {
                    CancelButtonClicked();
                    return;
                }

                GUI.backgroundColor = originalBackgroundColor;
            }

            using (GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox))
            {
                _scrollPosition = scrollView.scrollPosition;
                _ = EditorGUILayout.PropertyField(_context.TempValueProperty, GUIContent.none, true);
                _context.TempValueProperty.isExpanded = true;
                _ = _context.TempCfgObject.ApplyModifiedProperties();
            }

            if (!isIdValid)
                EditorGUILayout.HelpBox("Id must be set", MessageType.Error);
            else if (!isDescriptionValid)
                EditorGUILayout.HelpBox("Description must be set", MessageType.Error);
        }

        private void SaveButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            _ = _context.SaveEntry(_configuration);
            _context.TransitionTo<DatabaseEditorWindowNormalState<T>>();
        }

        private void CancelButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            _context.TransitionTo<DatabaseEditorWindowNormalState<T>>();
        }
    }
}
