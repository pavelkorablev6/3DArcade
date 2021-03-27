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

using SK.Utilities.Unity;
using UnityEngine;

namespace Arcade
{

    [DisallowMultipleComponent, SelectionBase]
    public sealed class ModelConfigurationComponent : MonoBehaviour
    {
        [System.Serializable]
        public sealed class ModelGeneralData
        {
            public string Id;
            public string Platform;
            public bool Grabbable = true;
            public bool MoveCabMovable = true;
            public bool MoveCabGrabbable = true;
        }

        [System.Serializable]
        public sealed class ModelVisualOverrides
        {
            public string Description;
            public string Model;
        }

        [System.Serializable]
        public sealed class ModelLaunchOverrides
        {
            public InteractionType InteractionType;
            public string Emulator;
        }

        [System.Serializable]
        public sealed class ModelArtworkFilesOverrides
        {
            public string[] MarqueeImageFiles;
            public string[] MarqueeVideoFiles;
            public string[] ScreenSnapFiles;
            public string[] ScreenTitleFiles;
            public string[] ScreenVideoFiles;
            public string[] GenericImageFiles;
            public string[] GenericVideoFiles;
            public string[] InfoFiles;
        }

        [System.Serializable]
        public sealed class ModelArtworkDirectoriesOverrides
        {
            public string[] MarqueeImageDirectories;
            public string[] MarqueeVideoDirectories;
            public string[] ScreenSnapDirectories;
            public string[] ScreenTitleDirectories;
            public string[] ScreenVideoDirectories;
            public string[] GenericImageDirectories;
            public string[] GenericVideoDirectories;
            public string[] InfoDirectories;
        }

        [System.Serializable]
        public sealed class ModelGameDataOverrides
        {
            public string CloneOf;
            public string RomOf;
            public string Genre;
            public string Year;
            public string Manufacturer;
            public GameScreenType ScreenType;
            public GameScreenOrientation ScreenOrientation;
            public bool Mature;
        }

        public ModelGeneralData GeneralData;
        public ModelVisualOverrides VisualOverrides;
        public ModelLaunchOverrides LaunchOverrides;
        public ModelArtworkFilesOverrides ArtworkFilesOverrides;
        public ModelArtworkDirectoriesOverrides ArtworkDirectoriesOverrides;
        public ModelGameDataOverrides GameDataOverrides;

        public void FromModelConfiguration(ModelConfiguration cfg)
        {
            GeneralData = new ModelGeneralData
            {
                Id               = cfg.Id,
                Platform         = cfg.Platform,
                Grabbable        = cfg.Grabbable,
                MoveCabMovable   = cfg.MoveCabMovable,
                MoveCabGrabbable = cfg.MoveCabGrabbable
            };

            VisualOverrides = new ModelVisualOverrides
            {
                Description = cfg.Description,
                Model       = cfg.Model
            };

            LaunchOverrides = new ModelLaunchOverrides
            {
                InteractionType = cfg.InteractionType,
                Emulator        = cfg.Emulator
            };

            ArtworkFilesOverrides = new ModelArtworkFilesOverrides
            {
                MarqueeImageFiles = cfg.MarqueeImageFiles,
                MarqueeVideoFiles = cfg.MarqueeVideoFiles,
                ScreenSnapFiles   = cfg.ScreenSnapFiles,
                ScreenTitleFiles  = cfg.ScreenTitleFiles,
                ScreenVideoFiles  = cfg.ScreenVideoFiles,
                GenericImageFiles = cfg.GenericImageFiles,
                GenericVideoFiles = cfg.GenericVideoFiles,
                InfoFiles         = cfg.InfoFiles
            };

            ArtworkDirectoriesOverrides = new ModelArtworkDirectoriesOverrides
            {
                MarqueeImageDirectories = cfg.MarqueeImageDirectories,
                MarqueeVideoDirectories = cfg.MarqueeVideoDirectories,
                ScreenSnapDirectories   = cfg.ScreenSnapDirectories,
                ScreenTitleDirectories  = cfg.ScreenTitleDirectories,
                ScreenVideoDirectories  = cfg.ScreenVideoDirectories,
                GenericImageDirectories = cfg.GenericImageDirectories,
                GenericVideoDirectories = cfg.GenericVideoDirectories,
                InfoDirectories         = cfg.InfoDirectories
            };

            GameDataOverrides = new ModelGameDataOverrides
            {
                CloneOf           = cfg.CloneOf,
                RomOf             = cfg.RomOf,
                Genre             = cfg.Genre,
                Year              = cfg.Year,
                Manufacturer      = cfg.Manufacturer,
                ScreenType        = cfg.ScreenType,
                ScreenOrientation = cfg.ScreenOrientation,
                Mature            = cfg.Mature
            };
        }

        public ModelConfiguration ToModelConfiguration() => new ModelConfiguration
        {
            Id                      = GeneralData.Id,
            Platform                = GeneralData.Platform,
            Grabbable               = GeneralData.Grabbable,
            MoveCabMovable          = GeneralData.MoveCabMovable,
            MoveCabGrabbable        = GeneralData.MoveCabGrabbable,
            Description             = VisualOverrides.Description,
            Model                   = VisualOverrides.Model,
            InteractionType         = LaunchOverrides.InteractionType,
            Emulator                = LaunchOverrides.Emulator,
            MarqueeImageFiles       = ArtworkFilesOverrides.MarqueeImageFiles,
            MarqueeVideoFiles       = ArtworkFilesOverrides.MarqueeVideoFiles,
            ScreenSnapFiles         = ArtworkFilesOverrides.ScreenSnapFiles,
            ScreenTitleFiles        = ArtworkFilesOverrides.ScreenTitleFiles,
            ScreenVideoFiles        = ArtworkFilesOverrides.ScreenVideoFiles,
            GenericImageFiles       = ArtworkFilesOverrides.GenericImageFiles,
            GenericVideoFiles       = ArtworkFilesOverrides.GenericVideoFiles,
            InfoFiles               = ArtworkFilesOverrides.InfoFiles,
            MarqueeImageDirectories = ArtworkDirectoriesOverrides.MarqueeImageDirectories,
            MarqueeVideoDirectories = ArtworkDirectoriesOverrides.MarqueeVideoDirectories,
            ScreenSnapDirectories   = ArtworkDirectoriesOverrides.ScreenSnapDirectories,
            ScreenTitleDirectories  = ArtworkDirectoriesOverrides.ScreenTitleDirectories,
            ScreenVideoDirectories  = ArtworkDirectoriesOverrides.ScreenVideoDirectories,
            GenericImageDirectories = ArtworkDirectoriesOverrides.GenericImageDirectories,
            GenericVideoDirectories = ArtworkDirectoriesOverrides.GenericVideoDirectories,
            InfoDirectories         = ArtworkDirectoriesOverrides.InfoDirectories,
            CloneOf                 = GameDataOverrides.CloneOf,
            RomOf                   = GameDataOverrides.RomOf,
            Genre                   = GameDataOverrides.Genre,
            Year                    = GameDataOverrides.Year,
            Manufacturer            = GameDataOverrides.Manufacturer,
            ScreenType              = GameDataOverrides.ScreenType,
            ScreenOrientation       = GameDataOverrides.ScreenOrientation,
            Mature                  = GameDataOverrides.Mature,

            Position = transform.localPosition,
            Rotation = MathUtils.ClampEulerAngles(transform.localEulerAngles),
            Scale    = transform.localScale
        };
    }
}
