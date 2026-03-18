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

[XmlRoot("sound")]
public class GmxAssetSound
{
    [XmlElement("kind")]
    public uint kind { get; set; }

    [XmlElement("extension")]
    public string extension { get; set; }

    [XmlElement("origname")]
    public string path { get; set; }

    [XmlElement("effects")]
    public uint effects { get; set; }

    [XmlElement("volume")]
    public GmxSoundVolume volume;

    [XmlElement("pan")]
    public float pan { get; set; }

    [XmlElement("bitRates")]
    public GmxSoundBitRates bitRates { get; set; }

    [XmlElement("sampleRates")]
    public GmxSoundSampleRates sampleRates { get; set; }

    [XmlElement("types")]
    public GmxSoundTypes types { get; set; }

    [XmlElement("bitDepths")]
    public GmxSoundBitDepths bitDepths { get; set; }

    [XmlElement("preload")]
    public int preload { get; set; }

    [XmlElement("data")]
    public string data { get; set; }

    [XmlElement("compressed")]
    public int compressed { get; set; }

    [XmlElement("streamed")]
    public int streamed { get; set; }

    [XmlElement("uncompressOnLoad")]
    public int uncompressOnLoad { get; set; }

    [XmlElement("audioGroup")]
    public int audioGroup { get; set; }

    public GmxAssetSound() { }

    public GmxAssetSound(UndertaleSound resource, Decompiler decompiler)
    {
        kind = 0;
        extension = resource.Type.Content;
        path = String.Concat("sound\\audio\\", resource.Name.Content, extension);
        effects = resource.Effects;
        volume = new GmxSoundVolume()
        {
            volume = resource.Volume
        };
        pan = 0;
        bitRates = new GmxSoundBitRates();
        preload = -Convert.ToInt32(resource.Preload);
        data = String.Concat(resource.Name.Content, extension);
        compressed = -Convert.ToInt32(resource.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsCompressed));
        streamed = -Convert.ToInt32(!resource.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsEmbedded));
        uncompressOnLoad = -Convert.ToInt32(resource.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsDecompressedOnLoad));

        byte[] soundData = decompiler.GetAudioData(resource);

        MemoryStream stream = new MemoryStream(soundData);
        IMediaAnalysis info = FFProbe.Analyse(stream);

        bitRates.bitRate = (long?)info.PrimaryAudioStream.BitRate ?? 320000;
        bitRates.bitRate /= 1000;
        sampleRates = new();
        sampleRates.sampleRate = info.PrimaryAudioStream.SampleRateHz;
        types = new();
        types.type = 0;
        bitDepths = new();
        bitDepths.bitDepth = info.PrimaryAudioStream.BitDepth ?? 16;
        audioGroup = resource.GroupID;
    }
}

public class GmxSoundVolume
{
    [XmlElement("volume")]
    public float volume { get; set; }

    public GmxSoundVolume() { }
}

public class GmxSoundBitRates
{
    [XmlElement("bitRate")]
    public long bitRate { get; set; }

    public GmxSoundBitRates() { }
}
public class GmxSoundBitDepths
{
    [XmlElement("bitDepth")]
    public int bitDepth { get; set; }

    public GmxSoundBitDepths() { }
}
public class GmxSoundSampleRates
{
    [XmlElement("sampleRate")]
    public int sampleRate { get; set; }

    public GmxSoundSampleRates() { }
}

public class GmxSoundTypes
{
    [XmlElement("type")]
    public int type { get; set; }

    public GmxSoundTypes() { }
}
