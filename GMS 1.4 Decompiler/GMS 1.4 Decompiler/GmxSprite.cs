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
using System.Drawing;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using static System.Net.Mime.MediaTypeNames;

[XmlRoot("sprite")]
public class GmxAssetSprite
{
    [XmlElement("type")]
    public uint type { get; set; }

    [XmlElement("xorig")]
    public int xOrigin { get; set; }

    [XmlElement("yorigin")]
    public int yOrigin { get; set; }

    [XmlElement("colkind")]
    public int collisionKind { get; set; }

    [XmlElement("coltolerance")]
    public int collisionTolerance { get; set; }

    [XmlElement("sepmasks")]
    public int collisionMaskType { get; set; }

    [XmlElement("bboxmode")]
    public uint bboxMode { get; set; }

    [XmlElement("bbox_left")]
    public int bboxLeft { get; set; }

    [XmlElement("bbox_right")]
    public int bboxRight { get; set; }

    [XmlElement("bbox_top")]
    public int bboxTop { get; set; }

    [XmlElement("bbox_bottom")]
    public int bboxBottom { get; set; }

    [XmlElement("HTile")]
    public int horizontallyTiled { get; set; }

    [XmlElement("VTile")]
    public int verticallyTiled { get; set; }

    [XmlElement("TextureGroups")]
    public GmxSpriteTextureGroups textureGroups { get; set; }

    [XmlElement("For3D")]
    public int for3D { get; set; }

    [XmlElement("width")]
    public uint width { get; set; }

    [XmlElement("height")]
    public uint height { get; set; }

    [XmlElement("frames")]
    public GmxSpriteFrames frames { get; set; }

    public GmxAssetSprite() { }

    public GmxAssetSprite(UndertaleSprite resource)
    {
        type = (uint)resource.SSpriteType;
        xOrigin = resource.OriginX;
        yOrigin = resource.OriginY;
        collisionKind = 0; // fuck you collision kind
        collisionTolerance = 0;
        collisionMaskType = resource.SepMasks switch
        {
            UndertaleSprite.SepMaskType.AxisAlignedRect => -1,
            UndertaleSprite.SepMaskType.Precise => 0,
        };
        bboxMode = resource.BBoxMode;
        bboxLeft = resource.MarginLeft;
        bboxRight = resource.MarginRight;
        bboxBottom = resource.MarginBottom;
        bboxTop = resource.MarginTop;
        horizontallyTiled = 0;
        verticallyTiled = 0;
        textureGroups = new GmxSpriteTextureGroups(0);
        for3D = 0;
        if (resource.Textures.Count > 0 && resource.Textures[0]?.Texture?.TexturePage?.TextureData?.Width == resource.Width &&
            resource.Textures[0]?.Texture?.TexturePage?.TextureData?.Height == resource.Height)
        {
            for3D = -1; // hopefully
        }
        width = resource.Width;
        height = resource.Height;
        frames = new GmxSpriteFrames();
        frames.frames = new List<GmxSpriteFrame>();
        int i = 0;
        foreach (UndertaleSprite.TextureEntry Texture in resource.Textures)
        {
            string name = String.Concat("images\\", resource.Name.Content, "_", i, ".png");

            frames.frames.Add(new GmxSpriteFrame(i, name));
            i++;
        }
    }
}

public class GmxSpriteTextureGroups
{
    [XmlElement("TextureGroup0")]
    public int index { get; set; }

    public GmxSpriteTextureGroups() { }
    public GmxSpriteTextureGroups(int index)
    {
        this.index = index;
    }
}

/// xml serialization is so fucking weird that i have to give this a name inconsistent from the rest
public class GmxSpriteFrame
{
    [XmlAttribute(AttributeName = "index")]
    public int index { get; set; }

    [XmlText]
    public string path { get; set; }

    public GmxSpriteFrame() { }

    public GmxSpriteFrame(int _index, string _path)
    {
        index = _index;
        path = _path;
    }
}

public class GmxSpriteFrames
{
    [XmlElement("frame")]
    public List<GmxSpriteFrame> frames { get; set; }
    public GmxSpriteFrames() { }
}