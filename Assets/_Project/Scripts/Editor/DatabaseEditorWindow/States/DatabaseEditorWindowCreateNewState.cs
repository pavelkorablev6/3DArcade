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
    internal sealed class DatabaseEditorWindowCreateNewState<T> : DatabaseEditorWindowState<T> where T : DatabaseEntry
    {
        private T _configuration;

        public DatabaseEditorWindowCreateNewState(DatabaseEditorWindowContext<T> context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            EditorGUI.FocusTextInControl(null);
            _scrollPosition = Vector2.zero;
            _context.SetSerializedFields(null);
            _configuration             = (_context.TempCfgObject.targetObject as ConfigurationSO<T>).Value;
            _configuration.Id          = string.Empty;
            _configuration.Description = string.Empty;
        }

        public override void OnExit()
        {
            _configuration = null;
            _context.ClearSerializedFields();
        }

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

                if (GUILayout.Button(new GUIContent("Add", "Create this configuration file")))
                {
                    AddButtonClicked();
                    return;
                }

                GUI.backgroundColor = Color.yellow;
                GUI.enabled         = true;
                if (GUILayout.Button(new GUIContent("Cancel", "Cancel the creation of this configuration file")))
                {
                    CancelButtonClicked();
                    return;
                }

                GUI.backgroundColor = originalBackgroundColor;
            }

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                _configuration.Id = EditorGUILayout.TextField("Id", _configuration.Id);
            }

            int indentLevel = EditorGUI.indentLevel;
            GUILayout.Space(8f);
            using (GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox))
            {
                _scrollPosition       = scrollView.scrollPosition;
                EditorGUI.indentLevel = -1;

                _ = EditorGUILayout.PropertyField(_context.TempValueProperty, GUIContent.none, true);
                _context.TempValueProperty.isExpanded = true;
                _ = _context.TempCfgObject.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel = indentLevel;

            if (!isIdValid)
                EditorGUILayout.HelpBox("Id must be set", MessageType.Error);
            else if (!isDescriptionValid)
                EditorGUILayout.HelpBox("Description must be set", MessageType.Error);
        }

        private void AddButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            if (_context.AddEntry(_configuration))
                _context.TransitionTo<DatabaseEditorWindowNormalState<T>>();
        }

        private void CancelButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            _context.TransitionTo<DatabaseEditorWindowNormalState<T>>();
        }
    }
}
