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
    public enum WheelVariant
    {
        CameraInsideHorizontal,
        CameraOutsideHorizontal,
        LineHorizontal,
        CameraInsideVertical,
        CameraOutsideVertical,
        LineVertical,
        LineCustom
    }

    [System.Serializable]
    public sealed class CylArcadeProperties
    {
        [XmlElement("camera_settings")]
        public CameraSettings CameraSettings;

        [XmlElement("wheel_variant")]
        public WheelVariant WheelVariant;

        [XmlAttribute("inverse_navigation")]
        public bool InverseNavigation;

        [XmlAttribute("inverse_list")]
        public bool InverseList;

        [XmlElement("wheel_radius")]
        public float WheelRadius;

        [XmlElement("sprockets")]
        public int Sprockets;

        [XmlElement("selected_sprocket")]
        public int SelectedSprocket;

        [XmlElement("selected_position_z")]
        public float SelectedPositionZ;

        [XmlElement("model_spacing")]
        public float ModelSpacing;

        [XmlAttribute("mouse_look")]
        public bool MouseLook;

        [XmlAttribute("mouselook_reverse")]
        public bool MouseLookReverse;

        [XmlElement("sprocket_rotation")]
        public XMLVector3 SprocketRotation;

        [XmlElement("line_angle")]
        public float LineAngle;

        [XmlAttribute("horizontal_navigation")]
        public bool HorizontalNavigation;
    }
}
