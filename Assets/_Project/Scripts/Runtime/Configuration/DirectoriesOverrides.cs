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
    public sealed class DirectoriesOverrides
    {
        [XmlArray("marquee_image_directories"), XmlArrayItem("directory")] public string[] MarqueeImageDirectories = null;
        [XmlArray("marquee_video_directories"), XmlArrayItem("directory")] public string[] MarqueeVideoDirectories = null;
        [XmlArray("screen_snap_directories"), XmlArrayItem("directory")]   public string[] ScreenSnapDirectories   = null;
        [XmlArray("screen_title_directories"), XmlArrayItem("directory")]  public string[] ScreenTitleDirectories  = null;
        [XmlArray("screen_video_directories"), XmlArrayItem("directory")]  public string[] ScreenVideoDirectories  = null;
        [XmlArray("generic_image_directories"), XmlArrayItem("directory")] public string[] GenericImageDirectories = null;
        [XmlArray("generic_video_directories"), XmlArrayItem("directory")] public string[] GenericVideoDirectories = null;
        [XmlArray("info_directories"), XmlArrayItem("directory")]          public string[] InfoDirectories         = null;
    }
}
