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
    internal abstract class DatabaseEditorWindowBase<T> : EditorWindow where T : DatabaseEntry, new()
    {
        private enum WindowState
        {
            Normal,
            CreateNew,
            EditSelection
        }

        protected const float MIN_WINDOW_WIDTH = 468f;
        protected const float MIN_WINDOW_HEIGHT = 468f;

        protected abstract MultiFileDatabase<T> Database { get; }
        protected abstract T DefaultConfiguration { get; }

        private T[] _entries;
        private WindowState _windowState = WindowState.Normal;
        private Vector2 _scrollPosition;

        private SerializedObject _tempCfgObject;
        private SerializedProperty _tempValueProperty;
        private Vector2 _tempScrollPosition;

        private void OnEnable() => RefreshDatabase();

        private void OnGUI()
        {
            switch (_windowState)
            {
                case WindowState.Normal:
                    ShowNormalControls();
                    break;
                case WindowState.CreateNew:
                    ShowCreateNewControls();
                    break;
                case WindowState.EditSelection:
                    ShowEditSelectionControls();
                    break;
                default:
                    throw new System.Exception($"Unhandled WindowState case: {_windowState}");
            }
        }

        protected virtual void DrawInlineButtons(T entry)
        {
        }

        protected abstract SerializedObject GetSerializedObject(T cfg = null);

        private void ShowNormalControls()
        {
            if (_windowState != WindowState.Normal)
                return;

            DrawNormalToolbar();
            DrawNormalListView();
        }

        private void ShowCreateNewControls()
        {
            if (_windowState != WindowState.CreateNew)
                return;

            DrawCreateNewToolar();
            DrawCreateNewFields();
        }

        private void ShowEditSelectionControls()
        {
            if (_windowState != WindowState.EditSelection)
                return;

            DrawEditSelectionToolbar();
            DrawEditSelectionFields();
        }

        private void RefreshDatabase()
        {
            if (Database.LoadAll())
                _entries = Database.GetValues();
        }

        private void DrawNormalToolbar()
        {
            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add"))
                {
                    _tempCfgObject     = GetSerializedObject();
                    _tempValueProperty = _tempCfgObject.FindProperty("Value");
                    _windowState       = WindowState.CreateNew;
                }

                if (GUILayout.Button("Reload"))
                    RefreshDatabase();
            }
        }

        private void DrawNormalListView()
        {
            using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox);
            _scrollPosition = scrollView.scrollPosition;

            foreach (T entry in _entries)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(entry.ToString()))
                    {
                        _tempCfgObject     = GetSerializedObject(entry);
                        _tempValueProperty = _tempCfgObject.FindProperty("Value");
                        _windowState = WindowState.EditSelection;
                    }

                    DrawInlineButtons(entry);

                    if (GUILayout.Button("x", GUILayout.Width(35f)))
                        DeleteEntry(entry);
                }
            }
        }

        private void DrawCreateNewToolar()
        {
            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                bool addButtonPressed    = GUILayout.Button("Add");
                bool cancelButtonPressed = GUILayout.Button("Cancel");

                if (addButtonPressed && AddEntry((_tempCfgObject.targetObject as ConfigurationSO<T>).Value))
                {
                    _entries     = Database.GetValues();
                    _windowState = WindowState.Normal;
                }

                if (addButtonPressed || cancelButtonPressed)
                    _windowState = WindowState.Normal;
            }
        }

        private void DrawCreateNewFields()
        {
            if (_tempValueProperty != null)
            {
                using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_tempScrollPosition, EditorStyles.helpBox);
                _tempScrollPosition = scrollView.scrollPosition;
                _ = EditorGUILayout.PropertyField(_tempValueProperty, GUIContent.none, true);
                _tempValueProperty.isExpanded = true;
                _ = _tempCfgObject.ApplyModifiedProperties();
            }
        }

        private void DrawEditSelectionToolbar()
        {
            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                bool saveButtonPressed   = GUILayout.Button("Save");
                bool cancelButtonPressed = GUILayout.Button("Cancel");

                if (saveButtonPressed && SaveEntry((_tempCfgObject.targetObject as ConfigurationSO<T>).Value))
                    RefreshDatabase();

                if (saveButtonPressed || cancelButtonPressed)
                    _windowState = WindowState.Normal;
            }
        }

        private void DrawEditSelectionFields()
        {
            if (_tempValueProperty != null)
            {
                using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_tempScrollPosition, EditorStyles.helpBox);
                _tempScrollPosition = scrollView.scrollPosition;
                _ = EditorGUILayout.PropertyField(_tempValueProperty, GUIContent.none, true);
                _tempValueProperty.isExpanded = true;
                _ = _tempCfgObject.ApplyModifiedProperties();
            }
        }

        private bool AddEntry(T entry) => Database.Add(entry) != null;

        private bool SaveEntry(T value) => Database.Save(value);

        private void DeleteEntry(T entry)
        {
            if (EditorUtility.DisplayDialog($"Delete {typeof(T).Name}", $"Delete '{entry}' configuration?", "Yes", "No"))
                if (Database.Delete(entry.Id))
                    _entries = Database.GetValues();
        }

        //private void Save()
        //{
        //    EditorGUI.FocusTextInControl(null);
        //    Database.DeleteAll();
        //    foreach (T entry in _entries)
        //        _ = Database.Add(entry);
        //    _ = Database.SaveAll();
        //}
    }
}
