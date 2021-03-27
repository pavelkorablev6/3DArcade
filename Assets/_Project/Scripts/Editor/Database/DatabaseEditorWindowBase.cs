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
            AddNew,
            Delete,
            Edit
        }

        protected const float MIN_WINDOW_WIDTH = 408f;
        protected const float MIN_WINDOW_HEIGHT = 408f;

        protected abstract MultiFileDatabase<T> Database { get; }
        protected abstract T DefaultConfiguration { get; }

        protected GameObject _tempGameObject;
        protected T _selection;

        private T[] _entries;

        private WindowState _windowState = WindowState.Normal;
        private Vector2 _scrollPosition;

        private Editor _editorSingle;
        private Vector2 _scrollPositionSingle;

        private void OnEnable() => Load();

        private void OnDestroy()
        {
            if (_tempGameObject != null)
                DestroyImmediate(_tempGameObject);
        }

        private void OnGUI()
        {
            switch (_windowState)
            {
                case WindowState.Normal:
                    ShowNormalControls();
                    break;
                case WindowState.AddNew:
                    ShowAddNewControls();
                    break;
                case WindowState.Delete:
                    ShowDeleteControls();
                    break;
                case WindowState.Edit:
                    ShowEditControls();
                    break;
                default:
                    throw new System.Exception($"Unhandled WindowState case: {_windowState}");
            }
        }

        protected virtual void DrawInlineButtons(T entry)
        {
        }

        protected virtual void DrawConfigurationExtras(T entry)
        {
        }

        protected abstract bool Add();

        protected abstract bool Save();

        protected abstract Editor GetComponentEditor();

        private void ShowNormalControls()
        {
            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add"))
                {
                    _selection = DefaultConfiguration;
                    _editorSingle = GetComponentEditor();
                    _windowState = WindowState.AddNew;
                }

                if (GUILayout.Button("Reload"))
                {
                    _selection = null;
                    Load();
                }
            }

            using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPosition, EditorStyles.helpBox);
            _scrollPosition = scrollView.scrollPosition;

            foreach (T entry in _entries)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(entry.ToString()))
                    {
                        _selection = entry;
                        _editorSingle = GetComponentEditor();
                        _windowState = WindowState.Edit;
                        return;
                    }

                    DrawInlineButtons(entry);

                    if (GUILayout.Button("x", GUILayout.Width(35f)))
                    {
                        _selection = entry;
                        _windowState = WindowState.Delete;
                        return;
                    }
                }
            }
        }

        private void ShowAddNewControls()
        {
            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add") && Add())
                {
                    if (_tempGameObject != null)
                        DestroyImmediate(_tempGameObject);
                    _selection = null;
                    _entries = Database.GetValues();
                    _windowState = WindowState.Normal;
                }
                if (GUILayout.Button("Cancel"))
                {
                    if (_tempGameObject != null)
                        DestroyImmediate(_tempGameObject);
                    _selection = null;
                    _windowState = WindowState.Normal;
                }
            }

            if (_tempGameObject != null)
            {
                using GUILayout.ScrollViewScope scrollview = new GUILayout.ScrollViewScope(_scrollPositionSingle);
                _scrollPositionSingle = scrollview.scrollPosition;
                _ = _editorSingle.DrawDefaultInspector();
                DrawConfigurationExtras(_selection);
            }
        }

        private void ShowDeleteControls()
        {
            GUILayout.Space(8f);
            GUILayout.Label($"Delete {typeof(T).Name}: {_selection} ?");

            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Yes") && Database.Delete(_selection.Id))
                {
                    _entries = Database.GetValues();
                    _selection = null;
                    _windowState = WindowState.Normal;
                }
                if (GUILayout.Button("No"))
                {
                    _selection = null;
                    _windowState = WindowState.Normal;
                }
            }
        }

        private void ShowEditControls()
        {
            GUILayout.Space(8f);
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save"))
                {
                    if (Save())
                    {
                        if (_tempGameObject != null)
                            DestroyImmediate(_tempGameObject);
                        Load();
                        _selection = null;
                        _windowState = WindowState.Normal;
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    if (_tempGameObject != null)
                        DestroyImmediate(_tempGameObject);
                    _selection = null;
                    _windowState = WindowState.Normal;
                }
            }

            if (_tempGameObject != null)
            {
                using GUILayout.ScrollViewScope scrollView = new GUILayout.ScrollViewScope(_scrollPositionSingle);
                _scrollPositionSingle = scrollView.scrollPosition;
                _ = _editorSingle.DrawDefaultInspector();
                DrawConfigurationExtras(_selection);
            }
        }

        private void Load()
        {
            if (Database.LoadAll())
                _entries = Database.GetValues();
        }

        //private bool Delete(T entry) => Database.Delete(entry.Id);

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
