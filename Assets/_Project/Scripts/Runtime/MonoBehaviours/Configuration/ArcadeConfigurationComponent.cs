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

using UnityEngine;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class ArcadeConfigurationComponent : MonoBehaviour
    {
        [Tooltip("Second priority arcade scene name lookup, after ArcadeProperties.Scene.\nAlso used to match external artworks.\n")]
        public string Id;
        [Tooltip("Used for UI texts")]
        public string Description;
        [Tooltip("FPSArcade specific settings")]
        public FpsArcadeProperties FpsArcadeProperties;
        [Tooltip("CYLArcade specific settings")]
        public CylArcadeProperties CylArcadeProperties;

        //public bool Save(MultiFileDatabase<ArcadeConfiguration> arcadeDatabase, CameraSettings fpsCameraSettings, CameraSettings cylCameraSettings, bool saveGameTransforms)
        //{
        //    GetChildNodes(out Transform tGames, out Transform tProps);

        //    ArcadeConfiguration cfg = new ArcadeConfiguration
        //    {
        //        Id                  = Id,
        //        Description         = Description,
        //        RenderSettings      = RenderSettings,
        //        FpsArcadeProperties = FpsArcadeProperties,
        //        CylArcadeProperties = CylArcadeProperties,
        //        Games               = saveGameTransforms ? GetModelConfigurations(tGames) : arcadeDatabase.Get(Id).Games,
        //        Props               = GetModelConfigurations(tProps)
        //    };

        //    if (fpsCameraSettings != null)
        //        cfg.FpsArcadeProperties.CameraSettings = fpsCameraSettings;

        //    if (cylCameraSettings != null)
        //        cfg.CylArcadeProperties.CameraSettings = cylCameraSettings;

        //    return arcadeDatabase.Save(cfg);
        //}

        //public bool SaveModelsOnly(MultiFileDatabase<ArcadeConfiguration> arcadeDatabase, ArcadeConfiguration cfg)
        //{
        //    GetGamesAndProps(out cfg.Games, out cfg.Props);
        //    return arcadeDatabase.Save(cfg);
        //}

        public void FromArcadeConfiguration(ArcadeConfiguration cfg)
        {
            Id                  = cfg.Id;
            Description         = cfg.Description;
            FpsArcadeProperties = cfg.FpsArcadeProperties;
            CylArcadeProperties = cfg.CylArcadeProperties;
        }

        //public void SetGamesAndPropsTransforms(ArcadeConfiguration cfg) => SetGamesAndPropsTransforms(cfg.Games, cfg.Props);

        //public void SetGamesAndPropsTransforms(ModelConfiguration[] games, ModelConfiguration[] props)
        //{
        //    GetChildNodes(out Transform tGames, out Transform tProps);
        //    SetModelTransforms(tGames, games);
        //    SetModelTransforms(tProps, props);
        //}

        //private void GetGamesAndProps(out ModelConfiguration[] games, out ModelConfiguration[] props)
        //{
        //    GetChildNodes(out Transform tGames, out Transform tProps);
        //    games = GetModelConfigurations(tGames);
        //    props = GetModelConfigurations(tProps);
        //}

        //private void GetChildNodes(out Transform tGames, out Transform tProps)
        //{
        //    tGames = transform.GetComponentInChildren<GamesNodeTag>().transform;
        //    tProps = transform.GetComponentInChildren<PropsNodeTag>().transform;
        //}

        //private static ModelConfiguration[] GetModelConfigurations(Transform node)
        //{
        //    ModelConfiguration[] result = new ModelConfiguration[node.childCount];

        //    for (int i = 0; i < result.Length; ++i)
        //    {
        //        Transform child = node.GetChild(i);
        //        ModelConfigurationComponent modelSetup = child.GetComponent<ModelConfigurationComponent>();
        //        result[i] = modelSetup.ToModelConfiguration();
        //    }

        //    return result;
        //}

        //private static void SetModelTransforms(Transform node, ModelConfiguration[] modelConfigurations)
        //{
        //    for (int i = 0; i < node.childCount; ++i)
        //    {
        //        Transform child = node.GetChild(i);
        //        ModelConfiguration modelConfiguration = modelConfigurations[i];
        //        child.SetPositionAndRotation(modelConfiguration.Position, Quaternion.Euler(modelConfiguration.Rotation));
        //        child.localScale = modelConfiguration.Scale;
        //    }
        //}
    }
}
