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
    [System.Serializable, XmlRoot("platform")]
    public sealed class PlatformConfiguration : DatabaseEntry
    {
        [XmlElement("master_list")]                                         public string MasterList                 = "";
        [XmlElement("emulator")]                                            public string Emulator                   = "";
        [XmlElement("model")]                                               public string Model                      = "";
        [XmlArray("marquee_images_directories"), XmlArrayItem("directory")] public string[] MarqueeImagesDirectories = new string[0];
        [XmlArray("marquee_videos_directories"), XmlArrayItem("directory")] public string[] MarqueeVideosDirectories = new string[0];
        [XmlArray("screen_snaps_directories"), XmlArrayItem("directory")]   public string[] ScreenSnapsDirectories   = new string[0];
        [XmlArray("screen_titles_directories"), XmlArrayItem("directory")]  public string[] ScreenTitlesDirectories  = new string[0];
        [XmlArray("screen_videos_directories"), XmlArrayItem("directory")]  public string[] ScreenVideosDirectories  = new string[0];
        [XmlArray("generic_images_directories"), XmlArrayItem("directory")] public string[] GenericImagesDirectories = new string[0];
        [XmlArray("generic_videos_directories"), XmlArrayItem("directory")] public string[] GenericVideosDirectories = new string[0];
        [XmlArray("info_directories"), XmlArrayItem("directory")]           public string[] InfoDirectories          = new string[0];
    }
}
