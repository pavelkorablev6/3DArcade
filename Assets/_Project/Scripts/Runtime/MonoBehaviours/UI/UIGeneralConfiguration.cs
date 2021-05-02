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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arcade
{
    [DisallowMultipleComponent]
    public sealed class UIGeneralConfiguration : MonoBehaviour
    {
        [SerializeField] private GeneralConfigurationVariable _generalConfiguration;
        [SerializeField] private Databases _databases;
        [SerializeField] private TMP_Dropdown _startingArcadeDropdown;
        [SerializeField] private TMP_Dropdown _startingArcadeTypeDropdown;
        [SerializeField] private Toggle _mouseLookReverseToggle;
        [SerializeField] private Toggle _enableVRToggle;

        public void OnEnable()
        {
            _generalConfiguration.Initialize();

            _startingArcadeDropdown.options = _databases.Arcades.GetNames().Select(x => new TMP_Dropdown.OptionData(x)).ToList();
            _startingArcadeDropdown.value   = _startingArcadeDropdown.options.FindIndex(x => x.text == _generalConfiguration.Value.StartingArcade);

            _startingArcadeTypeDropdown.options = System.Enum.GetNames(typeof(ArcadeType)).Select(x => new TMP_Dropdown.OptionData(x)).ToList();
            _startingArcadeTypeDropdown.value   = _startingArcadeTypeDropdown.options.FindIndex(x => x.text == _generalConfiguration.Value.StartingArcadeType.ToString());

            _mouseLookReverseToggle.isOn = _generalConfiguration.Value.MouseLookReverse;

            _enableVRToggle.isOn = _generalConfiguration.Value.EnableVR;
        }

        public void Save()
        {
            _generalConfiguration.Value.StartingArcade = _startingArcadeDropdown.options[_startingArcadeDropdown.value].text;

            if (System.Enum.TryParse(_startingArcadeTypeDropdown.options[_startingArcadeTypeDropdown.value].text, out ArcadeType arcadeType))
                _generalConfiguration.Value.StartingArcadeType = arcadeType;

            _generalConfiguration.Value.MouseLookReverse = _mouseLookReverseToggle.isOn;

            _generalConfiguration.Value.EnableVR = _enableVRToggle.isOn;

            _generalConfiguration.Save();
            _generalConfiguration.Initialize();
        }
    }
}
