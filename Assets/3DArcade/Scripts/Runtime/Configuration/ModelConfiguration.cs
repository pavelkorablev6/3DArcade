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
    public sealed class ModelConfiguration : XMLDatabaseEntry
    {
        [XmlElement("interaction_type")]
        public InteractionType InteractionType;

        [XmlElement("platform")]
        public string Platform;

        [XmlElement("emulator")]
        public string Emulator;

        [XmlElement("model")]
        public string Model;

        [XmlElement("grabbable")]
        public bool Grabbable;

        [XmlElement("movecab_movable")]
        public bool MoveCabMovable;

        [XmlElement("movecab_grabbable")]
        public bool MoveCabGrabbable;

        [XmlArray("marquee_image_directories"), XmlArrayItem("directory")]
        public string[] MarqueeImageDirectories;

        [XmlArray("marquee_video_directories"), XmlArrayItem("directory")]
        public string[] MarqueeVideoDirectories;

        [XmlArray("screen_image_directories"), XmlArrayItem("directory")]
        public string[] ScreenImageDirectories;

        [XmlArray("screen_video_directories"), XmlArrayItem("directory")]
        public string[] ScreenVideoDirectories;

        [XmlArray("title_image_directories"), XmlArrayItem("directory")]
        public string[] TitleImageDirectories;

        [XmlArray("generic_image_directories"), XmlArrayItem("directory")]
        public string[] GenericImageDirectories;

        [XmlArray("generic_video_directories"), XmlArrayItem("directory")]
        public string[] GenericVideoDirectories;

        [XmlArray("info_directories"), XmlArrayItem("directory")]
        public string[] InfoDirectories;

        [XmlElement("position")]
        public XMLVector3 Position;

        [XmlElement("rotation")]
        public XMLVector3 Rotation;

        [XmlElement("scale")]
        public XMLVector3 Scale;
    }
}
