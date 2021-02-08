﻿/* MIT License

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
    [XmlRoot("arcade")]
    public sealed class ArcadeConfiguration : XMLDatabaseEntry
    {
        [XmlElement("scene_override")]
        public string Scene;

        [XmlElement("render_settings")]
        public RenderSettings RenderSettings;

        [XmlElement("fpsarcade_properties")]
        public FpsArcadeProperties FpsArcadeProperties;

        [XmlElement("cylarcade_properties")]
        public CylArcadeProperties CylArcadeProperties;

        [XmlArray("games"), XmlArrayItem("game")]
        public ModelConfiguration[] Games;

        [XmlArray("props"), XmlArrayItem("prop")]
        public ModelConfiguration[] Props;
    }
}
