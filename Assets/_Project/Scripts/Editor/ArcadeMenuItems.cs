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

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Arcade.UnityEditor
{
    internal static class ArcadeMenuItems
    {
#pragma warning disable IDE0051 // Remove unused private members

        [MenuItem("3DArcade/Spawn New Game #1", false, 41)]
        private static void AddNewGameMenuItem() => ArcadeManager.SpawnGame();

        [MenuItem("3DArcade/Spawn New Prop #2", false, 42)]
        private static void AddNewPropMenuItem() => ArcadeManager.SpawnProp();

        [MenuItem("3DArcade/Reload Arcade", false, 101)]
        private static void ReloadArcade() => ArcadeManager.Instance.ReloadCurrentArcade();

        [MenuItem("3DArcade/Save Arcade", false, 102)]
        private static void SaveArcade()
        {
            ArcadeManager.Instance.SaveCurrentArcade();
            if (EditorWindow.HasOpenInstances<ArcadeDatabaseEditorWindow>())
            {
                EditorWindow.GetWindow<ArcadeDatabaseEditorWindow>().Close();
                ArcadeDatabaseEditorWindow.ShowWindow();
            }
        }

        [MenuItem("3DArcade/Close Arcade", false, 103)]
        private static void CloseArcade()
        {
            ArcadeManager.ClearCurrentArcadeStateFromEditorPrefs();
            UE_Utilities.CloseAllScenes();
        }

        // ************************************************************************************************
        // * Menu Items Validation
        // ************************************************************************************************
        [MenuItem("3DArcade/Spawn New Game #1", true)]
        private static bool AddNewGameMenuItemValidation() => ApplicationIsNotPlayingAndCurrentArcadeIsLoaded();

        [MenuItem("3DArcade/Spawn New Prop #2", true)]
        private static bool AddNewPropMenuItemValidation() => ApplicationIsNotPlayingAndCurrentArcadeIsLoaded();

        [MenuItem("3DArcade/Reload Arcade", true)]
        private static bool ReloadArcadeValidation() => ApplicationIsNotPlayingAndCurrentArcadeIsLoaded();

        [MenuItem("3DArcade/Save Arcade", true)]
        private static bool SaveValidation() => ApplicationIsNotPlayingAndCurrentArcadeIsLoaded() && EntitiesSceneHasChanges();
        [MenuItem("3DArcade/Close Arcade", true)]
        private static bool CloseArcadeValidation() => ApplicationIsNotPlayingAndCurrentArcadeIsLoaded();

        private static bool ApplicationIsNotPlayingAndCurrentArcadeIsLoaded()
            => !Application.isPlaying && !string.IsNullOrEmpty(EditorPrefs.GetString("LoadedArcadeId"));

        private static bool EntitiesSceneHasChanges()
        {
            Scene entititesScene = SceneManager.GetSceneByName(EntitiesScene.ARCADE_SETUP_SCENE_NAME);
            return entititesScene.IsValid() && entititesScene.isLoaded && entititesScene.isDirty;
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
