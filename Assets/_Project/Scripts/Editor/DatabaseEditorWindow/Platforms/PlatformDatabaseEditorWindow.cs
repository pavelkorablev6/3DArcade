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
    internal sealed class PlatformDatabaseEditorWindow : DatabaseEditorWindowBase<PlatformConfiguration, PlatformConfigurationSO>
    {
        public override MultiFileDatabase<PlatformConfiguration> Database => new ArcadeManager().ArcadeContext.Databases.Platforms;

        public override PlatformConfiguration DefaultConfiguration => Database.DefaultValues;

        [MenuItem("3DArcade/Platforms", false, 11), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Editor")]
        private static void ShowWindow()
        {
            UE_Utilities.OpenMainScene();
            GetWindow<PlatformDatabaseEditorWindow>("Platform Manager", true).minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
        }

        [MenuItem("3DArcade/Platforms", true), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Editor")]
        private static bool ShowMenuValidation() => !Application.isPlaying;
    }
}
