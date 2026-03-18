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

[XmlRoot("path")]
public class GmxAssetPath
{
    [XmlElement("kind")]
    public uint kind { get; set; }

    [XmlElement("closed")]
    public int closed { get; set; }

    [XmlElement("precision")]
    public uint precision { get; set; }

    [XmlElement("backroom")]
    public int backroom { get; set; }

    [XmlElement("hsnap")]
    public uint horizontalSnap { get; set; }

    [XmlElement("vsnap")]
    public uint verticalSnap { get; set; }

    [XmlElement("points")]
    public GmxPathPoints points { get; set; }

    public GmxAssetPath() { }

    public GmxAssetPath(UndertalePath resource)
    {
        kind = Convert.ToUInt32(resource.IsSmooth);
        closed = -Convert.ToInt32(resource.IsClosed);
        precision = resource.Precision;
        backroom = -1;
        horizontalSnap = 16;
        verticalSnap = 16;
        points = new GmxPathPoints();
        points.points = new List<string>();
        foreach (UndertalePath.PathPoint? pathPoint in resource.Points)
        {
            points.points.Add(String.Concat(pathPoint.X, ",", pathPoint.Y, ",", pathPoint.Speed));
        }
    }
}

public class GmxPathPoints
{
    [XmlElement("point")]
    public List<string> points;

    public GmxPathPoints()
    {

    }
}
