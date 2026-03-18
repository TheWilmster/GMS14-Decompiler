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

[XmlRoot("font")]
public class GmxAssetFont
{
    [XmlElement("name")]
    public string name { get; set; }

    [XmlElement("size")]
    public float size { get; set; }

    [XmlElement("bold")]
    public int bold { get; set; }

    [XmlElement("renderhq")]
    public int renderInHighQuality { get; set; }

    [XmlElement("italic")]
    public int italic { get; set; }

    [XmlElement("charset")]
    public uint characterSet { get; set; }

    [XmlElement("aa")]
    public uint antiAliasing { get; set; }

    [XmlElement("includeTTF")]
    public int includeTTF { get; set; }

    [XmlElement("TTFName")]
    public string ttfName { get; set; }

    [XmlElement("texgroups")]
    public GmxFontTextureGroups textureGroups { get; set; }

    [XmlElement("ranges")]
    public GmxFontRanges asciiRanges { get; set; }

    [XmlElement("glyphs")]
    public GmxFontGlyphs glyphs { get; set; }

    [XmlElement("kerningPairs")]
    public GmxFontKerningPairs kerningPairs { get; set; }

    [XmlElement("image")]
    public string imageName { get; set; }

    public GmxAssetFont() { }

    public GmxAssetFont(UndertaleFont resource)
    {
        name = resource.DisplayName.Content;
        size = resource.EmSize;
        bold = -Convert.ToInt32(resource.Bold);
        renderInHighQuality = 0;
        italic = -Convert.ToInt32(resource.Italic);
        characterSet = 1;
        antiAliasing = resource.AntiAliasing;
        includeTTF = 0;
        ttfName = "";
        textureGroups = new GmxFontTextureGroups(0);
        asciiRanges = new GmxFontRanges();
        asciiRanges.ranges = new List<string>();
        asciiRanges.ranges.Add(String.Concat(resource.RangeStart, resource.RangeEnd));
        glyphs = new GmxFontGlyphs();
        glyphs.glyphs = new List<GmxFontGlyph>();
        foreach (UndertaleFont.Glyph glyph in resource.Glyphs)
        {
            GmxFontGlyph xmlGlyph = new GmxFontGlyph();
            xmlGlyph.asciiCharacter = glyph.Character;
            xmlGlyph.x = glyph.SourceX;
            xmlGlyph.y = glyph.SourceY;
            xmlGlyph.width = glyph.SourceWidth;
            xmlGlyph.height = glyph.SourceHeight;
            xmlGlyph.shift = glyph.Shift;
            xmlGlyph.offset = glyph.Offset;
            glyphs.glyphs.Add(xmlGlyph);
        }
        kerningPairs = new GmxFontKerningPairs();
        imageName = String.Concat(resource.Name.Content, ".png");
    }
}

public class GmxFontKerningPairs
{
}

public class GmxFontGlyphs
{
    [XmlElement("glyph")]
    public List<GmxFontGlyph> glyphs { get; set; }
}

public class GmxFontGlyph
{
    [XmlAttribute("character")]
    public int asciiCharacter;

    [XmlAttribute("x")]
    public int x;

    [XmlAttribute("y")]
    public int y;

    [XmlAttribute("w")]
    public int width;

    [XmlAttribute("h")]
    public int height;

    [XmlAttribute("shift")]
    public int shift;

    [XmlAttribute("offset")]
    public int offset;
}

public class GmxFontRanges
{
    [XmlElement("range")]
    public List<string> ranges { get; set; }
    public GmxFontRanges() { }
}

public class GmxFontTextureGroups
{
    [XmlElement("texgroup0")]
    public int texgroup;

    public GmxFontTextureGroups() { }

    public GmxFontTextureGroups(int index)
    {
        texgroup = index;
    }
}
