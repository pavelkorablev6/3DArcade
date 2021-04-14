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

namespace Arcade
{
    [System.Serializable]
    public sealed class ModelConfigurationOverrides
    {
        [System.Serializable]
        public sealed class Files
        {
            [XmlArray("marquee_image_files"), XmlArrayItem("file")]
            public string[] MarqueeImageFiles = new string[0];

            [XmlArray("marquee_video_files"), XmlArrayItem("file")]
            public string[] MarqueeVideoFiles = new string[0];

            [XmlArray("screen_snap_files"), XmlArrayItem("file")]
            public string[] ScreenSnapFiles = new string[0];

            [XmlArray("screen_title_files"), XmlArrayItem("file")]
            public string[] ScreenTitleFiles = new string[0];

            [XmlArray("screen_video_files"), XmlArrayItem("file")]
            public string[] ScreenVideoFiles = new string[0];

            [XmlArray("generic_image_files"), XmlArrayItem("file")]
            public string[] GenericImageFiles = new string[0];

            [XmlArray("generic_video_files"), XmlArrayItem("file")]
            public string[] GenericVideoFiles = new string[0];

            [XmlArray("info_files"), XmlArrayItem("file")]
            public string[] InfoFiles = new string[0];
        }

        [System.Serializable]
        public sealed class Directories
        {
            [XmlArray("marquee_image_directories"), XmlArrayItem("directory")]
            public string[] MarqueeImageDirectories = new string[0];

            [XmlArray("marquee_video_directories"), XmlArrayItem("directory")]
            public string[] MarqueeVideoDirectories = new string[0];

            [XmlArray("screen_snap_directories"), XmlArrayItem("directory")]
            public string[] ScreenSnapDirectories = new string[0];

            [XmlArray("screen_title_directories"), XmlArrayItem("directory")]
            public string[] ScreenTitleDirectories = new string[0];

            [XmlArray("screen_video_directories"), XmlArrayItem("directory")]
            public string[] ScreenVideoDirectories = new string[0];

            [XmlArray("generic_image_directories"), XmlArrayItem("directory")]
            public string[] GenericImageDirectories = new string[0];

            [XmlArray("generic_video_directories"), XmlArrayItem("directory")]
            public string[] GenericVideoDirectories = new string[0];

            [XmlArray("info_directories"), XmlArrayItem("directory")]
            public string[] InfoDirectories = new string[0];
        }

        [XmlAttribute("description")]
        public string Description = "";

        [XmlElement("model")]
        public string Model = "";

        [XmlElement("emulator")]
        public string Emulator = "";

        [XmlElement("artwork_files")]
        public Files ArtworkFiles = new Files();

        [XmlElement("artwork_directories")]
        public Directories ArtworkDirectories = new Directories();

        [XmlElement("game")]
        public GameConfiguration Game = new GameConfiguration();
    }

    [System.Serializable]
    public sealed class ModelConfiguration
    {
        [XmlAttribute("id")]
        public string Id = "";

        [XmlElement("interaction_type")]
        public InteractionType InteractionType = InteractionType.Default;

        [XmlAttribute("platform")]
        public string Platform = "";

        [XmlElement("grabbable")]
        public bool Grabbable = true;

        [XmlElement("movecab_movable")]
        public bool MoveCabMovable = true;

        [XmlElement("movecab_grabbable")]
        public bool MoveCabGrabbable = true;

        [XmlElement("overrides")]
        public ModelConfigurationOverrides Overrides = new ModelConfigurationOverrides();

        [XmlElement("position")]
        [UnityEngine.HideInInspector]
        public DatabaseVector3 Position = DatabaseVector3.Zero;

        [XmlElement("rotation")]
        [UnityEngine.HideInInspector]
        public DatabaseVector3 Rotation = DatabaseVector3.Zero;

        [XmlElement("scale")]
        [UnityEngine.HideInInspector]
        public DatabaseVector3 Scale = DatabaseVector3.One;

        [XmlIgnore]
        [UnityEngine.HideInInspector]
        public PlatformConfiguration PlatformConfiguration { get; set; }

        [XmlIgnore]
        [UnityEngine.HideInInspector]
        public GameConfiguration GameConfiguration { get; set; }
    }
}
