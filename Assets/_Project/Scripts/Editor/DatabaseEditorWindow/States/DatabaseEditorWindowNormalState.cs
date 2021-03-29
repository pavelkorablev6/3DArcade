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
    internal sealed class DatabaseEditorWindowNormalState<T> : DatabaseEditorWindowState<T> where T : DatabaseEntry
    {
        public DatabaseEditorWindowNormalState(DatabaseEditorWindowContext<T> context)
        : base(context)
        {
        }

        public override void OnEnter()
        {
            EditorGUI.FocusTextInControl(null);
            _scrollPosition = Vector2.zero;
            _context.RefreshDatabase();
        }

        public override void OnUpdate(float dt)
        {
            Color originalBackgroundColor = GUI.backgroundColor;

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.cyan;
                if (GUILayout.Button(new GUIContent("Add", "Create a new configuration")))
                {
                    AddButtonClicked();
                    return;
                }

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(new GUIContent("Reload", "Reload all configurations")))
                {
                    ReloadButtonClicked();
                    return;
                }

                GUI.backgroundColor = originalBackgroundColor;
            }

            using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox);
            _scrollPosition = scrollView.scrollPosition;

            foreach (T entry in _context.Entries)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(new GUIContent(entry.ToString(), "Edit this configuration")))
                    {
                        ListItemEditButtonClicked(entry);
                        return;
                    }

                    _context.DatabaseEditorWindow.DrawInlineButtons(entry);

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button(new GUIContent("x", "Delete this configuration"), GUILayout.Width(35f)))
                        ListItemDeleteButtonClicked(entry);
                    GUI.backgroundColor = originalBackgroundColor;
                }
            }
        }

        private void AddButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            _context.TransitionTo<DatabaseEditorWindowCreateNewState<T>>();
        }

        private void ReloadButtonClicked()
        {
            EditorGUI.FocusTextInControl(null);
            _context.RefreshDatabase();
        }

        private void ListItemEditButtonClicked(T entry)
        {
            EditorGUI.FocusTextInControl(null);
            _context.SetSerializedFields(entry);
            _context.TransitionTo<DatabaseEditorWindowEditSelectionState<T>>();
        }

        private void ListItemDeleteButtonClicked(T entry)
        {
            EditorGUI.FocusTextInControl(null);
            _context.DeleteEntry(entry);
        }
    }
}
