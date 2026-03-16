using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using FFMpegCore;

public class DecompilerConfig
{
    public string dataFilePath { get; set; }
    public bool exportSounds { get; set; }
    public bool exportSprites { get; set; }
    public bool exportScripts { get; set; }

    public DecompilerConfig()
    {
        dataFilePath = "C:\\Users\\wcoop\\Documents\\pizzatowerdemobuildspublic\\Public\\pizzatowerearlybuildtest_v211\\data.win";
        exportSounds = true;
        exportSprites = true;
        exportScripts = true;
    }
}

static class Program
{
    static void Main(string[] args)
    {
        string configPath = String.Concat(AppDomain.CurrentDomain.BaseDirectory, "DecompilerConfiguration.json");
        DecompilerConfig? config;
        Console.WriteLine("   ____    __  __    ____            _      _  _         ____  U _____ u   ____   U  ___ u  __  __    ____              _     U _____ u   ____     \r\nU /\"___|uU|' \\/ '|u / __\"| u        /\"|    | ||\"|       |  _\"\\ \\| ___\"|/U /\"___|   \\/\"_ \\/U|' \\/ '|uU|  _\"\\ u  ___     |\"|    \\| ___\"|/U |  _\"\\ u  \r\n\\| |  _ /\\| |\\/| |/<\\___ \\/       u | |u   | || |_     /| | | | |  _|\"  \\| | u     | | | |\\| |\\/| |/\\| |_) |/ |_\"_|  U | | u   |  _|\"   \\| |_) |/  \r\n | |_| |  | |  | |  u___) |        \\| |/   |__   _|    U| |_| |\\| |___   | |/__.-,_| |_| | | |  | |  |  __/    | |    \\| |/__  | |___    |  _ <    \r\n  \\____|  |_|  |_|  |____/>>        |_|   _  /|_|\\      |____/ u|_____|   \\____|\\_)-\\___/  |_|  |_|  |_|     U/| |\\u   |_____| |_____|   |_| \\_\\   \r\n  _)(|_  <<,-,,-.    )(  (__)     _//<,-,(\")u_|||_u      |||_   <<   >>  _// \\\\      \\\\   <<,-,,-.   ||>>_.-,_|___|_,-.//  \\\\  <<   >>   //   \\\\_  \r\n (__)__)  (./  \\.)  (__)         (__)(_/  \" (__)__)     (__)_) (__) (__)(__)(__)    (__)   (./  \\.) (__)__)\\_)-' '-(_/(_\")(\"_)(__) (__) (__)  (__) ");
        if (File.Exists(configPath))
        {
            Console.WriteLine("Reading config file...");
            config = JsonSerializer.Deserialize<DecompilerConfig>(File.ReadAllText(configPath));
            if (config == null)
            {
                config = new DecompilerConfig();
            }
        }
        else
        {
            Console.WriteLine("Generating config file...");
            config = new DecompilerConfig();
            File.WriteAllText(configPath, JsonSerializer.Serialize(config, new JsonSerializerOptions()
            {
                WriteIndented = true
            }));
            Console.WriteLine("Restart the program after setting your configurations!");
            System.Environment.Exit(0);
        }

        //GlobalFFOptions.Configure(options => options.BinaryFolder = String.Concat(config.ffmpegPath + "bin"));

        Decompiler decompiler = new Decompiler();
        decompiler.LoadDataFile(config.dataFilePath);

        if (config.exportSounds) decompiler.DumpSounds();
        if (config.exportSprites) decompiler.DumpSprites();
        if (config.exportScripts) decompiler.DumpScripts();
    }
}

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
    public uint tileHorziontalSep { get; set; }

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
    }
}

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
    public GmxVolume volume;

    [XmlElement("pan")]
    public float pan { get; set; }

    [XmlElement("bitRates")]
    public GmxBitRates bitRates { get; set; }

    [XmlElement("sampleRates")]
    public GmxSampleRates sampleRates { get; set; }

    [XmlElement("types")]
    public GmxSoundTypes types { get; set; }

    [XmlElement("bitDepths")]
    public GmxBitDepths bitDepths { get; set; }

    [XmlElement("preload")]
    public int preload { get; set; }

    [XmlElement("data")]
    public string data { get; set; }

    [XmlElement("compressed")]
    public int compressed { get; set; }

    [XmlElement("streamed")]
    public int streamed { get; set;  }

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
        volume = new GmxVolume()
        {
            volume = resource.Volume
        };
        pan = 0;
        bitRates = new GmxBitRates();
        preload = -Convert.ToInt32(resource.Preload);
        data = String.Concat(resource.Name.Content, extension);
        compressed = -(int)(resource.Flags & UndertaleSound.AudioEntryFlags.IsCompressed);
        streamed = (int)~(resource.Flags & UndertaleSound.AudioEntryFlags.IsEmbedded);
        uncompressOnLoad = -(int)(resource.Flags & UndertaleSound.AudioEntryFlags.IsDecompressedOnLoad);

        byte[] soundData = decompiler.GetAudioData(resource);

        MemoryStream stream = new MemoryStream(soundData);
        IMediaAnalysis info = FFProbe.Analyse(stream);

        bitRates.bitRate = info.PrimaryAudioStream.BitRate;
        sampleRates = new();
        sampleRates.sampleRate = info.PrimaryAudioStream.SampleRateHz;
        types = new();
        types.type = 0;
        bitDepths = new();
        bitDepths.bitDepth = (int)info.PrimaryAudioStream.BitDepth;
        audioGroup = resource.GroupID;
    }
}

public class GmxVolume
{
    [XmlElement("volume")]
    public float volume { get; set; }

    public GmxVolume() { }
}

public class GmxBitRates
{
    [XmlElement("bitRate")]
    public long bitRate { get; set; }

    public GmxBitRates() { }
}
public class GmxBitDepths
{
    [XmlElement("bitDepth")]
    public int bitDepth { get; set; }

    public GmxBitDepths() { }
}
public class GmxSampleRates
{
    [XmlElement("sampleRate")]
    public int sampleRate { get; set; }

    public GmxSampleRates() { }
}

public class GmxSoundTypes
{
    [XmlElement("type")]
    public int type { get; set; }

    public GmxSoundTypes() { }
}

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

    [XmlArray("frames")]
    public List<GmxSpriteFrame> frames { get; set; }

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
        frames = new List<frame>();
        int i = 0;
        foreach (UndertaleSprite.TextureEntry Texture in resource.Textures) {
            string name = String.Concat("images\\", resource.Name.Content, "_", i, ".png");

            frames.Add(new frame(i, name));
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

public class Decompiler
{
  
    public void LoadDataFile(string path)
    {
        FileInfo dataFile = new FileInfo(path);
        GamePath = path;
        try
        {
            using FileStream stream = dataFile.OpenRead();
            UndertaleData gmData = UndertaleIO.Read(stream);
            GameData = gmData;
            DecompilePath = String.Concat(AppDomain.CurrentDomain.BaseDirectory, "DecompiledProject\\");

            if (!Directory.Exists(DecompilePath))
            {
                Directory.CreateDirectory(DecompilePath);
            }
        }
        catch (FileNotFoundException except)
        {
            throw new FileNotFoundException($"Data file '{except.FileName}' does not exist.");
        };
    }

    public void DumpSprites()
    {
        Console.WriteLine("Dumping Sprites...");
        int i = 0;
        foreach (UndertaleSprite sprite in GameData.Sprites)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Sprites.Count, ") Dumping ", sprite.Name.Content, "..."
            ));
            if (i < (GameData.Sprites.Count - 1)) Console.SetCursorPosition(0, row);

            DumpSprite(sprite);
            i++;
        }
    }

    public void DumpScripts()
    {
        Console.WriteLine("Dumping Scripts...");
        int i = 0;
        foreach (UndertaleScript script in GameData.Scripts)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Scripts.Count, ") Dumping ", script.Name.Content, "..."
            ));
            if (i < (GameData.Sprites.Count - 1)) Console.SetCursorPosition(0, row);

            DumpScript(script);
            i++;
        }
    }

    public void DumpSounds()
    {
        Console.WriteLine("Dumping Sounds...");
        int i = 0;
        foreach (UndertaleSound sound in GameData.Sounds)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Sounds.Count, ") Dumping ", sound.Name.Content, "..."
            ));
            if (i < (GameData.Sounds.Count - 1)) Console.SetCursorPosition(0, row);

            DumpSound(sound);
            i++;
        }
    }

    public void DumpSprite(UndertaleSprite resource)
    {
        if (resource != null)
        {
            GmxAssetSprite sprite = new GmxAssetSprite(resource);

            TextureWorker worker = new TextureWorker();
            int i = 0;
            foreach (UndertaleSprite.TextureEntry texture in resource.Textures)
            {
                if (texture.Texture != null)
                {
                    string imagesPath = String.Concat(DecompilePath, "sprites\\images\\");

                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    worker.ExportAsPNG(texture.Texture, String.Concat(imagesPath, resource.Name.Content, "_", i, ".png"), null, true);
                }
                i++;
            }

            File.WriteAllText(String.Concat(DecompilePath, "sprites\\", resource.Name.Content, ".sprite.gmx"), ToXML(sprite));
        }
    }

    public void DumpScript(UndertaleScript resource)
    {
        if (resource != null)
        {
            string scriptsPath = String.Concat(DecompilePath, "scripts\\");

            GlobalDecompileContext globalDecompileContext = new (GameData);
            Underanalyzer.Decompiler.IDecompileSettings decompilerSettings = GameData.ToolInfo.DecompilerSettings;

            string decompiledCode = resource.Code != null ?
                new Underanalyzer.Decompiler.DecompileContext(globalDecompileContext, resource.Code, decompilerSettings).DecompileToString() :
                "";

            if (!Directory.Exists(scriptsPath))
            {
                Directory.CreateDirectory(scriptsPath);
            }

            File.WriteAllText(String.Concat(scriptsPath, resource.Name.Content, ".gml"), decompiledCode);
        }
    }

    public void DumpSound(UndertaleSound resource)
    {
        if (resource != null)
        {
            GmxAssetSound sound = new GmxAssetSound(resource, this);

            string soundsPath = String.Concat(DecompilePath, "sound\\");

            string audioFilesPath = String.Concat(soundsPath, "audio\\");
            if (!Directory.Exists(soundsPath))
            {
                Directory.CreateDirectory(soundsPath);
            }
            if (!Directory.Exists(audioFilesPath))
            {
                Directory.CreateDirectory(audioFilesPath);
            }
            File.WriteAllBytes(String.Concat(audioFilesPath, resource.Name.Content, sound.extension), GetAudioData(resource));

            File.WriteAllText(String.Concat(soundsPath, resource.Name.Content, ".sound.gmx"), ToXML(sound));
        }
    }

    //thank you setupwitch
    private string ToXML(object obj, XmlWriterSettings? settings = null)
    {
        settings ??= new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
            Encoding = Encoding.UTF8
        };

        XmlSerializer serializer = new(obj.GetType());

        using StringWriter sw = new();
        using XmlWriter writer = XmlWriter.Create(sw, settings);

        // to get rid of the namespaces, e.g:
        // <sound xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        XmlSerializerNamespaces ns = new();
        ns.Add(string.Empty, string.Empty);

        serializer.Serialize(writer, obj, ns);

        return sw.ToString();
    }

    public Dictionary<string, IList<UndertaleEmbeddedAudio>> loadedAudioGroups = null;
    public IList<UndertaleEmbeddedAudio>? GetAudioGroupData(UndertaleSound resource)
    {
        loadedAudioGroups ??= new();

        string groupName = resource.AudioGroup != null ? resource.AudioGroup.Name.Content : "audiogroup_default";
        if (loadedAudioGroups.ContainsKey(groupName))
        {
            return loadedAudioGroups[groupName];
        }

        string relativeGroupPath;
        if (resource.AudioGroup is UndertaleAudioGroup { Path.Content: string customRelativePath })
        {
            relativeGroupPath = customRelativePath;
        }
        else
        {
            relativeGroupPath = String.Concat("audiogroup", resource.GroupID, ".dat");
        }
        string groupFilePath = Path.Combine(GamePath, relativeGroupPath);
        if (!File.Exists(groupFilePath))
        {
            // damn guess you're fucked
            return null;
        }

        UndertaleData? data = null;
        using (var stream = new FileStream(groupFilePath, FileMode.Open, FileAccess.Read))
        {
            data = UndertaleIO.Read(stream);
        }

        loadedAudioGroups[groupName] = data.EmbeddedAudio;
        return data.EmbeddedAudio;
    }

    public byte[] GetAudioData(UndertaleSound resource)
    {
        byte[] soundData = System.Convert.FromBase64String("UklGRiQAAABXQVZFZm10IBAAAAABAAIAQB8AAAB9AAAEABAAZGF0YQAAAAA=");
        if ((int)~(resource.Flags & UndertaleSound.AudioEntryFlags.IsCompressed) == 1 && (int)~(resource.Flags & UndertaleSound.AudioEntryFlags.IsEmbedded) == 1)
        {
            string sourcePath = Path.Combine(GamePath, String.Concat(resource.Name.Content, resource.Type.Content));
            soundData = File.ReadAllBytes(sourcePath);
        }
        else
        {
            if (resource.AudioFile != null)
            {
                soundData = resource.AudioFile.Data;
            }
            else if (resource.GroupID > GameData.GetBuiltinSoundGroupID())
            {
                IList<UndertaleEmbeddedAudio> audioGroup = GetAudioGroupData(resource);
                if (audioGroup != null)
                {
                    soundData = audioGroup[resource.AudioID].Data;
                }
            }
        }

        return soundData;
    }

    public UndertaleData GameData;
    public string GamePath;
    public string DecompilePath;
}
