using FFMpegCore;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using static System.Net.Mime.MediaTypeNames;

[XmlRoot("background")]
public class GmxAssetBackground
{
    [XmlElement("istileset")]
    public int isTileset { get; set; }

    [XmlElement("tilewidth")]
    public uint tileWidth { get; set; }

    [XmlElement("tileheight")]
    public uint tileHeight { get; set; }

    [XmlElement("tilexoff")]
    public uint tileXoffset { get; set; }

    [XmlElement("tileyoff")]
    public uint tileYoffset { get; set; }

    [XmlElement("tilehsep")]
    public uint tileHorizontalSep { get; set; }

    [XmlElement("tilevsep")]
    public uint tileVerticalSep { get; set; }

    [XmlElement("HTile")]
    public int tileHorizontally { get; set; }

    [XmlElement("VTile")]
    public int tileVertically { get; set; }

    [XmlElement("TextureGroups")]
    public GmxSpriteTextureGroups textureGroups { get; set; }

    [XmlElement("For3D")]
    public int for3D { get; set; }

    [XmlElement("width")]
    public uint width { get; set; }

    [XmlElement("height")]
    public uint height { get; set; }

    [XmlElement("data")]
    public string path { get; set; }

    public GmxAssetBackground() { }

    public GmxAssetBackground(UndertaleBackground resource)
    {
        isTileset = 0;
        tileWidth = 32;
        tileHeight = 32;
        tileXoffset = 0;
        tileYoffset = 0;
        tileHorizontalSep = 0;
        tileVerticalSep = 0;
        tileHorizontally = 0;
        tileVertically = 0;
        textureGroups = new GmxSpriteTextureGroups(0);
        for3D = 0;
        /*if (resource.Texture.TexturePage?.TextureData?.Width == resource.Texture.BoundingWidth &&
            resource.Texture.TexturePage?.TextureData?.Height == resource.Texture.BoundingHeight)
        {
            for3D = -1; // hopefully
        }*/
        width = (uint?)resource.Texture.BoundingWidth ?? (uint)resource.Texture.TexturePage.TextureWidth;
        height = (uint?)resource.Texture.BoundingHeight ?? (uint)resource.Texture.TexturePage.TextureHeight;
        path = String.Concat("images\\", resource.Name.Content, ".png");
    }
}
