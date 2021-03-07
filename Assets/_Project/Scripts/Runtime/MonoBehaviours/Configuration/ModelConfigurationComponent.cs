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
    [DisallowMultipleComponent, SelectionBase]
    public sealed class ModelConfigurationComponent : MonoBehaviour
    {
        public string Id                       => _id;
        public string Platform                 => _platform;
        public bool MoveCabMovable             => _moveCabMovable;
        public bool MoveCabGrabbable           => _moveCabGrabbable;
        public string Description              => !string.IsNullOrEmpty(_description) ? _description : _id;
        public InteractionType InteractionType => _interactionType;
        public string Emulator                 => _emulator;

        [Header("Configuration")]
        [SerializeField] private string _id;
        [SerializeField] private string _platform;
        [SerializeField] private bool _grabbable        = true;
        [SerializeField] private bool _moveCabMovable   = true;
        [SerializeField] private bool _moveCabGrabbable = true;

        [Header("Visual Overrides")]
        [SerializeField] private string _description;
        [SerializeField] private string _model;

        [Header("Launch Overrides")]
        [SerializeField] private InteractionType _interactionType;
        [SerializeField] private string _emulator;

        [Header("Artwork Overrides")]
        [SerializeField] private string[] _marqueeImageDirectories;
        [SerializeField] private string[] _marqueeVideoDirectories;
        [SerializeField] private string[] _screenSnapDirectories;
        [SerializeField] private string[] _screenTitleDirectories;
        [SerializeField] private string[] _screenVideoDirectories;
        [SerializeField] private string[] _genericImageDirectories;
        [SerializeField] private string[] _genericVideoDirectories;
        [SerializeField] private string[] _infoDirectories;

        [Header("Data Overrides")]
        [SerializeField] private string _cloneOf;
        [SerializeField] private string _romOf;
        [SerializeField] private string _genre;
        [SerializeField] private string _year;
        [SerializeField] private string _manufacturer;
        [SerializeField] private GameScreenType _screenType;
        [SerializeField] private GameScreenOrientation _screenOrientation;
        [SerializeField] private bool _mature;

        public void FromModelConfiguration(ModelConfiguration modelConfiguration)
        {
            _id                      = modelConfiguration.Id;
            _platform                = modelConfiguration.Platform;
            _grabbable               = modelConfiguration.Grabbable;
            _moveCabMovable          = modelConfiguration.MoveCabMovable;
            _moveCabGrabbable        = modelConfiguration.MoveCabGrabbable;
            _description             = modelConfiguration.Description;
            _model                   = modelConfiguration.Model;
            _interactionType         = modelConfiguration.InteractionType;
            _emulator                = modelConfiguration.Emulator;
            _marqueeImageDirectories = modelConfiguration.MarqueeImageDirectories;
            _marqueeVideoDirectories = modelConfiguration.MarqueeVideoDirectories;
            _screenSnapDirectories   = modelConfiguration.ScreenSnapDirectories;
            _screenTitleDirectories  = modelConfiguration.ScreenTitleDirectories;
            _screenVideoDirectories  = modelConfiguration.ScreenVideoDirectories;
            _genericImageDirectories = modelConfiguration.GenericImageDirectories;
            _genericVideoDirectories = modelConfiguration.GenericVideoDirectories;
            _infoDirectories         = modelConfiguration.InfoDirectories;
            _cloneOf                 = modelConfiguration.CloneOf;
            _romOf                   = modelConfiguration.RomOf;
            _genre                   = modelConfiguration.Genre;
            _year                    = modelConfiguration.Year;
            _manufacturer            = modelConfiguration.Manufacturer;
            _screenType              = modelConfiguration.ScreenType;
            _screenOrientation       = modelConfiguration.ScreenOrientation;
            _mature                  = modelConfiguration.Mature;
        }

        public ModelConfiguration ToModelConfiguration() => new ModelConfiguration
        {
            Id                      = _id,
            Platform                = _platform,
            Grabbable               = _grabbable,
            MoveCabMovable          = _moveCabMovable,
            MoveCabGrabbable        = _moveCabGrabbable,
            Description             = _description,
            Model                   = _model,
            InteractionType         = _interactionType,
            Emulator                = _emulator,
            MarqueeImageDirectories = _marqueeImageDirectories,
            MarqueeVideoDirectories = _marqueeVideoDirectories,
            ScreenSnapDirectories   = _screenSnapDirectories,
            ScreenTitleDirectories  = _screenTitleDirectories,
            ScreenVideoDirectories  = _screenVideoDirectories,
            GenericImageDirectories = _genericImageDirectories,
            GenericVideoDirectories = _genericVideoDirectories,
            InfoDirectories         = _infoDirectories,
            CloneOf                 = _cloneOf,
            RomOf                   = _romOf,
            Genre                   = _genre,
            Year                    = _year,
            Manufacturer            = _manufacturer,
            ScreenType              = _screenType,
            ScreenOrientation       = _screenOrientation,
            Mature                  = _mature,

            Position = transform.localPosition,
            Rotation = MathUtils.CorrectEulerAngles(transform.localEulerAngles),
            Scale    = transform.localScale
        };
    }
}
