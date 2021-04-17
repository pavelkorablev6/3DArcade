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
    public sealed class CylArcadeProperties : ArcadeProperties
    {
        [XmlAttribute("wheel_variant")]
        public CylArcadeWheelVariant WheelVariant = CylArcadeWheelVariant.LineHorizontal;

        [XmlAttribute("wheel_radius")]
        public float WheelRadius = 5f;

        [XmlAttribute("sprockets")]
        public int Sprockets = 5;

        [XmlElement("mouse_look")]
        public bool MouseLook = false;

        [XmlElement("selected_sprocket")]
        public int SelectedSprocket = -1;

        [XmlElement("selected_position_z")]
        public float SelectedPositionZ = 5f;

        [XmlElement("sprocket_rotation")]
        public DatabaseVector3 SprocketRotation = new DatabaseVector3(0f, 180f, 0f);

        [XmlElement("model_spacing")]
        public float ModelSpacing = 0.2f;

        [XmlElement("horizontal_navigation")]
        public bool HorizontalNavigation = true;

        [XmlElement("inverse_navigation")]
        public bool InverseNavigation = false;

        [XmlElement("inverse_list")]
        public bool InverseList = false;

        [XmlElement("line_angle")]
        public float LineAngle = -5f;

        public CylArcadeProperties() => Scene = "_empty";
    }
}
