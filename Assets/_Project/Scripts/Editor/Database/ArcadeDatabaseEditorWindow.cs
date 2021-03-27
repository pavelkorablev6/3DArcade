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
    internal sealed class ArcadeDatabaseEditorWindow : DatabaseEditorWindowBase<ArcadeConfiguration>
    {
        private ArcadeConfigurationSO _tempCfg;

        [MenuItem("3DArcade/Arcades"), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Editor")]
        private static void ShowWindow()
        {
            ArcadeDatabaseEditorWindow window = GetWindow<ArcadeDatabaseEditorWindow>("Arcade Manager", true);
            window.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
        }

        protected override MultiFileDatabase<ArcadeConfiguration> Database => UE_ArcadeManager.ArcadeDatabase;

        protected override ArcadeConfiguration DefaultConfiguration => ArcadeConfiguration.DummyArcade;

        protected override void DrawInlineButtons(ArcadeConfiguration entry)
        {
            if (GUILayout.Button("Load (FPS)", GUILayout.Width(85f)))
                UE_ArcadeManager.LoadArcade(entry.Id, ArcadeType.Fps, true);

            if (GUILayout.Button("Load (CYL)", GUILayout.Width(85f)))
                UE_ArcadeManager.LoadArcade(entry.Id, ArcadeType.Cyl, true);
        }

        protected override SerializedObject GetSerializedObject(ArcadeConfiguration cfg)
        {
            _tempCfg       = CreateInstance<ArcadeConfigurationSO>();
            _tempCfg.Value = cfg ?? DefaultConfiguration;
            return new SerializedObject(_tempCfg);
        }
    }
}
