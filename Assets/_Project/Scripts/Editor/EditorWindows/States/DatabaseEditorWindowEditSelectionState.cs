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
    internal sealed class DatabaseEditorWindowEditSelectionState<T> : DatabaseEditorWindowState<T>
        where T : DatabaseEntry
    {
        private T _configuration;
        private string _newId;

        public override void OnEnter()
        {
            EditorGUI.FocusTextInControl(null);
            _scrollPosition = Vector2.zero;
            _configuration  = (Context.TempCfgObject.targetObject as ConfigurationSO<T>).Value;
            _newId          = string.Empty;
        }

        public override void OnExit() => Context.ClearSerializedFields();

        public override void OnUpdate(float dt)
        {
            Color originalBackgroundColor = GUI.backgroundColor;

            bool saveAsNew          = !string.IsNullOrEmpty(_newId);
            bool isIdValid          = _newId != _configuration.Id;
            bool isDescriptionValid = !string.IsNullOrEmpty(_configuration.Description);

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (saveAsNew)
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

                    if (GUILayout.Button(new GUIContent("Save As...", "Save as a new configuration")))
                    {
                        SaveButtonClicked(_newId);
                        return;
                    }
                }
                else
                {
                    if (isDescriptionValid)
                    {
                        GUI.backgroundColor = Color.green;
                        GUI.enabled         = true;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.red;
                        GUI.enabled         = false;
                    }

                    if (GUILayout.Button(new GUIContent("Save", "Save this configuration")))
                    {
                        SaveButtonClicked();
                        return;
                    }
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

            GUILayout.Space(8f);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Editing:{new string(' ', 37)}{_configuration.Id}");
                _newId = EditorGUILayout.TextField("Save As:", _newId);
            }

            int indentLevel = EditorGUI.indentLevel;
            using (GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox))
            {
                _scrollPosition       = scrollView.scrollPosition;
                EditorGUI.indentLevel = -1;

                _ = EditorGUILayout.PropertyField(Context.TempValueProperty, GUIContent.none, true);
                Context.TempValueProperty.isExpanded = true;
                _ = Context.TempCfgObject.ApplyModifiedProperties();
            }
            EditorGUI.indentLevel = indentLevel;

            if (saveAsNew && !isIdValid)
                EditorGUILayout.HelpBox("The new Id must be of a different value than the current configuration Id", MessageType.Error);
            else if (!isDescriptionValid)
                EditorGUILayout.HelpBox("Description must be set", MessageType.Error);
        }

        private void SaveButtonClicked(string newId = null)
        {
            EditorGUI.FocusTextInControl(null);

            if (!string.IsNullOrEmpty(newId))
                _configuration.Id = _newId;

            if (Context.SaveEntry(_configuration))
                Context.TransitionTo<DatabaseEditorWindowNormalState<T>>();
        }

        private void CancelButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            Context.TransitionTo<DatabaseEditorWindowNormalState<T>>();
        }
    }
}
