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

using SK.Utilities;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Arcade
{
    [System.Serializable, XmlRoot("general")]
    public sealed class GeneralConfiguration
    {
        [XmlElement("starting_arcade")]      public string StartingArcade         = "_dummy";
        [XmlElement("starting_arcade_type")] public ArcadeType StartingArcadeType = ArcadeType.Fps;
        [XmlElement("mouselook_reverse")]    public bool MouseLookReverse         = false;
        [XmlElement("enable_vr")]            public bool EnableVR                 = false;

        private const string VFS_FILE_ALIAS = "general_cfg";

        private string _filePath;

        public void Initialize(VirtualFileSystem virtualFileSystem)
        {
            if (virtualFileSystem.TryGetFile(VFS_FILE_ALIAS, out _filePath))
                Load();
        }

        private void Load()
        {
            try
            {
                if (_filePath is null)
                    return;

                if (!File.Exists(_filePath))
                {
                    Debug.Log($"[{GetType().Name}.Load] File not found, creating one using default values");
                    CreateDefaultConfiguration();
                    return;
                }

                GeneralConfiguration cfg = XMLUtils.Deserialize<GeneralConfiguration>(_filePath);
                if (cfg is null)
                {
                    CreateDefaultConfiguration();
                    return;
                }

                StartingArcade     = cfg.StartingArcade;
                StartingArcadeType = cfg.StartingArcadeType;
                MouseLookReverse   = cfg.MouseLookReverse;
                EnableVR           = cfg.EnableVR;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        public bool Save()
        {
            try
            {
                if (_filePath is null)
                {
                    Debug.LogWarning($"[{GetType().Name}.Save] File not mapped in VirtualFileSystem, values will not be saved to disk");
                    return true;
                }

                return XMLUtils.Serialize(_filePath, this);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        private void CreateDefaultConfiguration()
        {
            if (Save())
                Debug.Log($"[{GetType().Name}.CreateDefaultConfiguration] Created default general configuration.");
            else
                Debug.LogError($"[{GetType().Name}.CreateDefaultConfiguration] Failed to create default general configuration.");
        }
    }
}
