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
    public sealed class DatabaseRect
    {
        [XmlAttribute("x")]
        public float X = 0f;

        [XmlAttribute("y")]
        public float Y = 0f;

        [XmlAttribute("width")]
        public float Width = 1f;

        [XmlAttribute("height")]
        public float Height = 1f;

        public DatabaseRect()
        {
        }

        public DatabaseRect(float x, float y, float width, float height)
        {
            X      = x;
            Y      = y;
            Width  = width;
            Height = height;
        }

        public DatabaseRect(Rect r)
        {
            X      = r.x;
            Y      = r.y;
            Width  = r.width;
            Height = r.height;
        }

        public static implicit operator DatabaseRect(Rect r) => new DatabaseRect(r);

        public static implicit operator Rect(DatabaseRect r) => new Rect(r.X, r.Y, r.Width, r.Height);
    }
}
