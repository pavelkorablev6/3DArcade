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
    public sealed class ModelConfiguration : ArcadeObject
    {
        [XmlAttribute("id")]
        public new string Id = "";

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
        public PlatformConfiguration PlatformConfiguration { get; set; }

        [XmlIgnore]
        public EmulatorConfiguration EmulatorConfiguration { get; set; }

        [XmlIgnore]
        public GameConfiguration GameConfiguration { get; set; }
    }
}
