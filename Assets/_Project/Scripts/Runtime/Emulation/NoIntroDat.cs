using System.Xml.Serialization;

namespace Arcade.Emulation.NoIntro
{
    [XmlRoot("datafile")]
    public sealed class Datafile
    {
        [XmlElement("header")] public Header Header;
        [XmlElement("game")] public Game[] Games;
    }

    [XmlRoot("header")]
    public sealed class Header
    {
        [XmlElement("name")] public string Name;
        [XmlElement("description")] public string Description;
        [XmlElement("version")] public string Version;
        [XmlElement("date")] public string Date;
    }

    [XmlRoot("game")]
    public sealed class Game
    {
        [XmlAttribute("name")] public string Name;
        [XmlElement("description")] public string Description;
        [XmlElement("release")] public Release[] Releases;
        [XmlElement("rom")] public Rom[] Roms;
    }

    [XmlRoot("release")]
    public sealed class Release
    {
        [XmlAttribute("name")] public string Name;
        [XmlAttribute("region")] public string Region;
    }

    [XmlRoot("rom")]
    public sealed class Rom
    {
        [XmlAttribute("name")] public string Name;
        [XmlAttribute("size")] public string Size;
        [XmlAttribute("crc")] public string Crc;
        [XmlAttribute("md5")] public string Md5;
        [XmlAttribute("sha1")] public string Sha1;
        [XmlAttribute("status")] public string Status;
    }
}
