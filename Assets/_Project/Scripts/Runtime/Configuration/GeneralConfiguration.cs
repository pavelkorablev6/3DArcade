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

using System.Xml.Serialization;
using UnityEngine;
using Zenject;

namespace Arcade
{
    [XmlRoot("general")]
    public sealed class GeneralConfiguration
    {
        [XmlElement("starting_arcade")]
        public string StartingArcade;

        [XmlElement("starting_arcade_type")]
        public ArcadeType StartingArcadeType;

        [XmlElement("enable_vr")]
        public bool EnableVR;

        private readonly string _filePath;

        public GeneralConfiguration()
        {
        }

        [Inject]
        public GeneralConfiguration(IVirtualFileSystem virtualFileSystem) => _filePath = virtualFileSystem.GetFile("general_cfg");

        public bool Load()
        {
            try
            {
                if (!FileSystem.FileExists(_filePath))
                    return CreateDefaultConfiguration();

                GeneralConfiguration cfg = XMLUtils.Deserialize<GeneralConfiguration>(_filePath);
                if (cfg == null)
                    return CreateDefaultConfiguration();

                Debug.Log($"[{GetType().Name}] Loaded general configuration.");
                Initialize(cfg);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public bool Save()
        {
            try
            {
                return XMLUtils.Serialize(_filePath, this);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private bool CreateDefaultConfiguration()
        {
            Initialize();
            bool result = Save();
            if (result)
                Debug.Log($"[{GetType().Name}] Created default general configuration.");
            return result;
        }

        private void Initialize(GeneralConfiguration cfg = null)
        {
            if (cfg != null)
            {
                StartingArcade     = cfg.StartingArcade;
                StartingArcadeType = cfg.StartingArcadeType;
                EnableVR           = cfg.EnableVR;
            }
            else
            {
                StartingArcade     = "internal_default";
                StartingArcadeType = ArcadeType.Fps;
                EnableVR           = false;
            }
        }
    }
}
