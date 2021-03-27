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

using System.Linq;
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

        public bool Save(MultiFileDatabase<ArcadeConfiguration> arcadeDatabase, CameraSettings fpsCameraSettings, CameraSettings cylCameraSettings, bool saveGameTransforms)
        {
            GetParentNodes(out Transform games, out Transform props);

            ArcadeConfiguration cfg = new ArcadeConfiguration
            {
                Id                  = Id,
                Description         = Description,
                FpsArcadeProperties = FpsArcadeProperties,
                CylArcadeProperties = CylArcadeProperties,
                Games               = saveGameTransforms ? GetModelConfigurations(games) : arcadeDatabase.Get(Id).Games,
                Props               = GetModelConfigurations(props)
            };

            if (fpsCameraSettings != null)
                cfg.FpsArcadeProperties.CameraSettings = fpsCameraSettings;

            if (cylCameraSettings != null)
                cfg.CylArcadeProperties.CameraSettings = cylCameraSettings;

            return arcadeDatabase.Save(cfg);
        }

        public bool SaveModelsOnly(MultiFileDatabase<ArcadeConfiguration> arcadeDatabase, ArcadeConfiguration cfg)
        {
            GetGamesAndProps(out cfg.Games, out cfg.Props);
            return arcadeDatabase.Save(cfg);
        }

        public void FromArcadeConfiguration(ArcadeConfiguration cfg)
        {
            Id                  = cfg.Id;
            Description         = cfg.Description;
            FpsArcadeProperties = cfg.FpsArcadeProperties;
            CylArcadeProperties = cfg.CylArcadeProperties;
        }

        public ArcadeConfiguration ToArcadeConfiguration()
        {
            ArcadeConfiguration cfg = new ArcadeConfiguration
            {
                Id                  = Id,
                Description         = Description,
                FpsArcadeProperties = FpsArcadeProperties,
                CylArcadeProperties = CylArcadeProperties
            };

            GamesNodeTag gamesRoot = FindObjectOfType<GamesNodeTag>();
            if (gamesRoot != null)
            {
                ModelConfigurationComponent[] components = gamesRoot.GetComponentsInChildren<ModelConfigurationComponent>();
                cfg.Games = components.Select(x => x.ToModelConfiguration()).ToArray();
            }

            PropsNodeTag propsRoot = FindObjectOfType<PropsNodeTag>();
            if (propsRoot != null)
            {
                ModelConfigurationComponent[] components = propsRoot.GetComponentsInChildren<ModelConfigurationComponent>();
                cfg.Props = components.Select(x => x.ToModelConfiguration()).ToArray();
            }

            return cfg;
        }

        //public void SetGamesAndPropsTransforms(ArcadeConfiguration cfg) => SetGamesAndPropsTransforms(cfg.Games, cfg.Props);

        //public void SetGamesAndPropsTransforms(ModelConfiguration[] games, ModelConfiguration[] props)
        //{
        //    GetChildNodes(out Transform tGames, out Transform tProps);
        //    SetModelTransforms(tGames, games);
        //    SetModelTransforms(tProps, props);
        //}

        private void GetGamesAndProps(out ModelConfiguration[] outGames, out ModelConfiguration[] outProps)
        {
            GetParentNodes(out Transform gamesNode, out Transform propsNode);
            outGames = gamesNode != null ? GetModelConfigurations(gamesNode) : null;
            outProps = propsNode != null ? GetModelConfigurations(propsNode) : null;
        }

        private void GetParentNodes(out Transform outGamesNode, out Transform outPropsNode)
        {
            GamesNodeTag gamesNode = FindObjectOfType<GamesNodeTag>();
            outGamesNode = gamesNode != null ? gamesNode.transform : null;

            PropsNodeTag propsNode = FindObjectOfType<PropsNodeTag>();
            outPropsNode = propsNode != null ? propsNode.transform : null;
        }

        private static ModelConfiguration[] GetModelConfigurations(Transform node)
        {
            ModelConfigurationComponent[] cfgComponents = node.GetComponentsInChildren<ModelConfigurationComponent>();
            return cfgComponents.Select(x => x.ToModelConfiguration()).ToArray();;
        }

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
