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
    internal static class CurrentArcadeMenuItems
    {
#pragma warning disable IDE0051 // Remove unused private members
        [MenuItem("3DArcade/Reload Current Arcade", false, 103)]
        private static void ReloadCurrentArcade() => ArcadeManager.Instance.ReloadCurrentArcade();

        [MenuItem("3DArcade/Close Current Arcade", false, 103)]
        private static void CloseCurrentArcade()
        {
            ArcadeManager.ClearCurrentArcadeStateFromEditorPrefs();
            UE_Utilities.CloseAllScenes();
        }

        // ************************************************************************************************
        // * Validation
        // ************************************************************************************************
        [MenuItem("3DArcade/Reload Current Arcade", true)]
        private static bool ReloadCurrentArcadeValidation() => !Application.isPlaying && !string.IsNullOrEmpty(EditorPrefs.GetString("LoadedArcadeId"));

        [MenuItem("3DArcade/Close Current Arcade", true)]
        private static bool CloseCurrentArcadeValidation() => ReloadCurrentArcadeValidation();
#pragma warning restore IDE0051 // Remove unused private members
    }
}
