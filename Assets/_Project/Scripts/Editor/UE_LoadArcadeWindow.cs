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
    internal sealed class UE_LoadArcadeWindow : EditorWindow
    {
        private static bool _spawnEntities = true;

        private UE_ArcadeManager _editorArcadeManager;
        private Vector2 _scrollPos = Vector2.zero;

        [MenuItem("3DArcade/Load Arcade", false, 101), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEditor")]
        private static void ShowWindow()
        {
            UE_LoadArcadeWindow window = GetWindow<UE_LoadArcadeWindow>("Load Arcade");
            window.minSize = new Vector2(310f, 100f);
        }

        private void OnEnable() => _editorArcadeManager = new UE_ArcadeManager();

        private void OnGUI()
        {
            EditorGUILayout.Space(8f);
            _spawnEntities = GUILayout.Toggle(_spawnEntities, "Spawn Entities");
            EditorGUILayout.Space(8f);
            DrawConfigurationsList();
        }

        private void DrawConfigurationsList()
        {
            using EditorGUILayout.ScrollViewScope scope = new EditorGUILayout.ScrollViewScope(_scrollPos, false, false);
            foreach (string name in _editorArcadeManager.ArcadeNames)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(name);
                    if (GUILayout.Button("FPSArcade"))
                        _editorArcadeManager.LoadArcade(name, ArcadeType.Fps, _spawnEntities);
                    if (GUILayout.Button("CYLArcade"))
                        _editorArcadeManager.LoadArcade(name, ArcadeType.Cyl, _spawnEntities);
                }
            }
        }

        [MenuItem("3DArcade/Load Arcade", true), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEditor")]
        private static bool ShowWindowValidation() => !Application.isPlaying;
    }
}
