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
    public abstract class ManagerWindow<T> : ManagerWindowbase where T : DatabaseEntry
    {
        public T[] Entries;

        protected abstract string DirectoryAlias { get; }
        protected abstract string DirectoryPath { get; }
        protected abstract MultiFileDatabase<T> DerivedDatabase { get; }

        private SerializedObject _serializedObject;
        private SerializedProperty _serializedProperty;

        private MultiFileDatabase<T> _database;

        private Vector2 _scrollPos;

        protected override void Initialize()
        {
            _ = _virtualFileSystem.MountDirectory(DirectoryAlias, DirectoryPath);

            _database = DerivedDatabase;
            _database.Initialize();

            _serializedObject   = new SerializedObject(this);
            _serializedProperty = _serializedObject.FindProperty(nameof(Entries));

            Load();
        }

        private void OnGUI()
        {
            GUILayout.Space(8f);

            Color originalColor = GUI.backgroundColor;

            using (new GUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Save", GUILayout.MaxWidth(200f)))
                    Save();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Reload (Discard changes)", GUILayout.MaxWidth(200f)))
                    Load();
            }

            using (new GUILayout.HorizontalScope())
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Save & Close", GUILayout.MaxWidth(200f)))
                {
                    Save();
                    Close();
                    return;
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Cancel & Close", GUILayout.MaxWidth(200f)))
                {
                    Close();
                    return;
                }
            }

            GUI.backgroundColor = originalColor;

            GUILayout.Space(8f);
            using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_scrollPos, EditorStyles.helpBox))
            {
                _scrollPos = scrollView.scrollPosition;

                if (EditorGUILayout.PropertyField(_serializedProperty, true))
                    _ = _serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(8f);
        }

        private void Load()
        {
            EditorGUI.FocusTextInControl(null);

            if (_database.LoadAll())
            {
                Entries = _database.GetValues();
                _serializedObject.Update();
            }
        }

        private void Save()
        {
            EditorGUI.FocusTextInControl(null);

            _database.DeleteAll();
            foreach (T entry in Entries)
                _ = _database.Add(entry);
            _ = _database.SaveAll();
        }
    }
}
