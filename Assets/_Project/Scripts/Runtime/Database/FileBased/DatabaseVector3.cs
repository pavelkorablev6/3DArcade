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
    public sealed class DatabaseVector3
    {
        public static DatabaseVector3 Zero => new DatabaseVector3();
        public static DatabaseVector3 One => new DatabaseVector3(1f, 1f, 1f);

        [XmlAttribute("x")] public float X = 0f;
        [XmlAttribute("y")] public float Y = 0f;
        [XmlAttribute("z")] public float Z = 0f;

        public DatabaseVector3()
        {
        }

        public DatabaseVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public DatabaseVector3(Vector3 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public void RoundValues()
        {
            X = Mathf.Round(X * 100f) / 100f;
            Y = Mathf.Round(Y * 100f) / 100f;
            Z = Mathf.Round(Z * 100f) / 100f;
        }

        public static implicit operator DatabaseVector3(Vector3 v) => new DatabaseVector3(v);

        public static implicit operator Vector3(DatabaseVector3 v) => new Vector3(v.X, v.Y, v.Z);
    }
}
