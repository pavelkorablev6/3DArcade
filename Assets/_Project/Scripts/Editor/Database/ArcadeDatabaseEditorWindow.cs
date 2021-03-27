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
        private static GameObject _dummyCabPrefab;

        private ArcadeConfigurationComponent _configurationComponent;
        private bool _gamesVisible;
        private bool _propsVisible;
        private Vector2 _gamesScrollPosition;
        private Vector2 _propsScrollPosition;

        private GameObject _tempGamesRootGameObject;
        private GameObject _tempPropsRootGameObject;

        [MenuItem("3DArcade/Arcades"), SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Editor")]
        private static void ShowWindow()
        {
            if (_dummyCabPrefab == null)
                _dummyCabPrefab = Resources.Load<GameObject>("Prefabs/pfEditorCabDummy");

            ArcadeDatabaseEditorWindow window = GetWindow<ArcadeDatabaseEditorWindow>("Arcade Manager", true);
            window.minSize = new Vector2(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
        }

        protected override MultiFileDatabase<ArcadeConfiguration> Database => UE_ArcadeManager.ArcadeDatabase;

        protected override ArcadeConfiguration DefaultConfiguration => ArcadeConfiguration.DummyArcade;

        protected override void DrawInlineButtons(ArcadeConfiguration entry)
        {
            if (GUILayout.Button("Load (FPS)", GUILayout.Width(85f)))
            {
                _selection = entry;
                UE_ArcadeManager.LoadArcade(entry.Id, ArcadeType.Fps, true);
            }

            if (GUILayout.Button("Load (CYL)", GUILayout.Width(85f)))
            {
                _selection = entry;
                UE_ArcadeManager.LoadArcade(entry.Id, ArcadeType.Cyl, true);
            }
        }

        protected override void DrawConfigurationExtras(ArcadeConfiguration entry)
        {
            if (entry.Games != null)
            {
                GUILayout.Space(8f);
                _gamesVisible = EditorGUILayout.Foldout(_gamesVisible, "Games");
                if (_gamesVisible)
                {
                    using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_gamesScrollPosition, EditorStyles.helpBox);
                    {
                        _gamesScrollPosition = scrollView.scrollPosition;
                        foreach (ModelConfiguration cfg in entry.Games)
                            GUILayout.Label(cfg.Id);
                    }
                }
            }

            if (entry.Props != null)
            {
                GUILayout.Space(8f);
                _propsVisible = EditorGUILayout.Foldout(_propsVisible, "Props");
                if (_propsVisible)
                {
                    using EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(_propsScrollPosition, EditorStyles.helpBox);
                    {
                        _propsScrollPosition = scrollView.scrollPosition;
                        foreach (ModelConfiguration cfg in entry.Props)
                            GUILayout.Label(cfg.Id);
                    }
                }
            }
        }

        protected override bool Add() => Database.Add(_configurationComponent.ToArcadeConfiguration()) != null;

        protected override bool Save() => true;

        protected override Editor GetComponentEditor()
        {
            _tempGameObject = new GameObject("TempArcadeConfiguration");

            _configurationComponent = _tempGameObject.AddComponent<ArcadeConfigurationComponent>();
            _configurationComponent.FromArcadeConfiguration(_selection);

            _tempGamesRootGameObject = new GameObject("Games");
            _tempGamesRootGameObject.transform.SetParent(_tempGameObject.transform);
            SpawnObjects(_selection.Games, _tempGamesRootGameObject.transform);

            _tempPropsRootGameObject = new GameObject("Props");
            _tempPropsRootGameObject.transform.SetParent(_tempGameObject.transform);
            SpawnObjects(_selection.Props, _tempPropsRootGameObject.transform);

            return Editor.CreateEditor(_configurationComponent);
        }

        private void SpawnObjects(ModelConfiguration[] configurations, Transform parent)
        {
            if (configurations == null)
                return;

            foreach (ModelConfiguration cfg in configurations)
            {
                GameObject cfgGameObject = new GameObject(cfg.Id);
                cfgGameObject.transform.SetParent(parent);
                cfgGameObject.transform.SetPositionAndRotation(cfg.Position, Quaternion.Euler(cfg.Rotation));
                cfgGameObject.AddComponent<ModelConfigurationComponent>()
                             .FromModelConfiguration(cfg);
            }
        }
    }
}
