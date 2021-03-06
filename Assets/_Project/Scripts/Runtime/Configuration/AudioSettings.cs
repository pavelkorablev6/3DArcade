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

namespace Arcade
{
    [System.Serializable]
    public sealed class AudioSettings
    {
        [XmlElement("main_volume"), Range(0f, 1f)]    public float MainVolume      = 1f;
        [XmlElement("music_volume"), Range(0f, 1f)]   public float MusicVolume     = 0.6f;
        [XmlElement("ambient_volume"), Range(0f, 1f)] public float AmbientVolume   = 0.4f;
        [XmlElement("minimum_distance")]              public float MinimumDistance = 1f;
        [XmlElement("maximum_distance")]              public float MaximumDistance = 3f;
    }
}
