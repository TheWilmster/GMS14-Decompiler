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

[XmlRoot("assets")]
public class GmxAssetProject
{
    [XmlElement("Configs")]
    public GmxProjectConfigs configs { get; set; }

    [XmlElement("datafiles")]
    public GmxProjectDatafiles datafiles { get; set; }

    [XmlElement("NewExtensions")]
    public string newExtensions { get; set; }

    [XmlElement("sounds")]
    public GmxProjectSounds sounds { get; set; }

    [XmlElement("sprites")]
    public GmxProjectSprites sprites { get; set; }

    [XmlElement("backgrounds")]
    public GmxProjectBackgrounds backgrounds { get; set; }

    [XmlElement("paths")]
    public GmxProjectPaths paths { get; set; }
    [XmlElement("scripts")]
    public GmxProjectScripts scripts { get; set; }

    [XmlElement("shaders")]
    public GmxProjectShaders shaders { get; set; }

    [XmlElement("fonts")]
    public GmxProjectFonts fonts { get; set; }
    [XmlElement("objects")]
    public GmxProjectObjects objects { get; set; }
    [XmlElement("rooms")]
    public GmxProjectRooms rooms { get; set; }
    [XmlElement("help")]
    public GmxProjectHelp help { get; set; }

    [XmlElement("TutorialState")]
    public GmxProjectTutorialState tutorialState { get; set; }

    public GmxAssetProject() { }

    public GmxAssetProject(Decompiler decompiler)
    {
        configs = new GmxProjectConfigs();
        datafiles = new GmxProjectDatafiles();
        newExtensions = String.Empty; // extensions not supported yet
        sounds = new GmxProjectSounds();
        sprites = new GmxProjectSprites();
        backgrounds = new GmxProjectBackgrounds();
        paths = new GmxProjectPaths();
        scripts = new GmxProjectScripts();
        shaders = new GmxProjectShaders();
        fonts = new GmxProjectFonts();
        objects = new GmxProjectObjects();
        rooms = new GmxProjectRooms();
        help = new GmxProjectHelp();
        tutorialState = new GmxProjectTutorialState();

        List<string> includedFiles = decompiler.GetIncludedFiles();

        foreach (string file in includedFiles)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(decompiler.GamePath), file);
            FileInfo fileInfo = new FileInfo(filePath);
            GmxProjectDatafile xmlDatafile = new GmxProjectDatafile(file, fileInfo.Length);
            datafiles.datafiles.Add(xmlDatafile);
        }
        datafiles.number = datafiles.datafiles.Count + 1;

        foreach (UndertaleSound sound in decompiler.GameData.Sounds)
        {
            sounds.sounds.Add(String.Concat("sound\\", sound.Name.Content));
        }

        foreach (UndertaleSprite sprite in decompiler.GameData.Sprites)
        {
            sprites.sprites.Add(String.Concat("sprites\\", sprite.Name.Content));
        }

        foreach (UndertaleBackground background in decompiler.GameData.Backgrounds)
        {
            backgrounds.backgrounds.Add(String.Concat("background\\", background.Name.Content));
        }

        foreach (UndertalePath path in decompiler.GameData.Paths)
        {
            paths.paths.Add(String.Concat("paths\\", path.Name.Content));
        }

        foreach (UndertaleScript script in decompiler.GameData.Scripts)
        {
            scripts.scripts.Add(String.Concat("scripts\\", script.Name.Content, ".gml"));
        }

        foreach (UndertaleShader shader in decompiler.GameData.Shaders)
        {
            GmxProjectShader xmlShader = new GmxProjectShader();
            xmlShader.path = String.Concat("shaders\\", shader.Name.Content, ".shader");
            xmlShader.type = shader.Type switch
            {
                UndertaleShader.ShaderType.GLSL_ES => "GLSLES",
                UndertaleShader.ShaderType.GLSL => "GLSL",
                UndertaleShader.ShaderType.HLSL9 => "HLSL9",
                _ => "GLSLES"
            };
            shaders.shaders.Add(xmlShader);
        }

        foreach (UndertaleFont font in decompiler.GameData.Fonts)
        {
            fonts.fonts.Add(String.Concat("fonts\\", font.Name.Content));
        }

        foreach (UndertaleGameObject obj in decompiler.GameData.GameObjects)
        {
            objects.objects.Add(String.Concat("objects\\", obj.Name.Content));
        }

        foreach (UndertaleRoom room in decompiler.GameData.Rooms)
        {
            rooms.rooms.Add(String.Concat("rooms\\", room.Name.Content));
        }
    }
}

public class GmxProjectConfigs
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("Config")]
    public List<string> configs { get; set; }

    public GmxProjectConfigs()
    {
        name = "configs";
        configs = new List<string>();
        configs.Add("Configs\\Default");
    }
}

public class GmxProjectDatafiles
{
    [XmlAttribute("number")]
    public int number { get; set; }

    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("datafile")]
    public List<GmxProjectDatafile> datafiles { get; set; }

    public GmxProjectDatafiles()
    {
        number = 1;
        name = "datafiles";
        datafiles = new List<GmxProjectDatafile>();
    }
}

public class GmxProjectDatafile
{
    [XmlElement("name")]
    public string fileName { get; set; }

    [XmlElement("exists")]
    public int fileExists { get; set; }

    [XmlElement("size")]
    public long fileSize { get; set; }

    [XmlElement("exportAction")]
    public uint exportAction { get; set; }

    [XmlElement("exportDir")]
    public string exportDir { get; set; }

    [XmlElement("overwrite")]
    public int overwrite { get; set; }

    [XmlElement("freeData")]
    public int freeData { get; set; }

    [XmlElement("removeEnd")]
    public int removedEnd { get; set; }

    [XmlElement("store")]
    public int store { get; set; }

    [XmlElement("ConfigOptions")]
    public GmxProjectDatafileConfigOptions configOptions { get; set; }

    [XmlElement("filename")]
    public string fileName2 { get; set; }

    public GmxProjectDatafile() { }

    public GmxProjectDatafile(string _fileName, long _fileSize)
    {
        fileName = _fileName;
        fileExists = -1;
        fileSize = _fileSize;
        exportAction = 2;
        exportDir = String.Empty;
        overwrite = 0;
        freeData = -1;
        removedEnd = 0;
        store = 0;
        configOptions = new GmxProjectDatafileConfigOptions();
        fileName2 = _fileName;
        configOptions.configs.Add(new GmxProjectDatafileConfig());
    }
}

public class GmxProjectDatafileConfigOptions
{
    [XmlElement("Config")]
    public List<GmxProjectDatafileConfig> configs { get; set; }

    public GmxProjectDatafileConfigOptions()
    {
        configs = new List<GmxProjectDatafileConfig>();
    }
}

public class GmxProjectDatafileConfig
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("CopyToMask")]
    public long copyToMask { get; set; }

    public GmxProjectDatafileConfig()
    {
        name = "Default";
        copyToMask = 9223372036854775807;
    }
}

public class GmxProjectSounds
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("sound")]
    public List<string> sounds { get; set; }

    public GmxProjectSounds()
    {
        name = "sound";
        sounds = new List<string>();
    }
}

public class GmxProjectSprites
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("sprite")]
    public List<string> sprites { get; set; }

    public GmxProjectSprites()
    {
        name = "sprites";
        sprites = new List<string>();
    }
}

public class GmxProjectBackgrounds
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("background")]
    public List<string> backgrounds { get; set; }

    public GmxProjectBackgrounds()
    {
        name = "background";
        backgrounds = new List<string>();
    }
}

public class GmxProjectPaths
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("path")]
    public List<string> paths { get; set; }

    public GmxProjectPaths()
    {
        name = "paths";
        paths = new List<string>();
    }
}

public class GmxProjectScripts
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("script")]
    public List<string> scripts { get; set; }

    public GmxProjectScripts()
    {
        name = "scripts";
        scripts = new List<string>();
    }
}

public class GmxProjectShaders
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("shader")]
    public List<GmxProjectShader> shaders { get; set; }

    public GmxProjectShaders()
    {
        name = "shaders";
        shaders = new List<GmxProjectShader>();
    }
}

public class GmxProjectShader
{
    [XmlAttribute("type")]
    public string type { get; set; }

    [XmlText]
    public string path { get; set; }
}

public class GmxProjectFonts
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("font")]
    public List<string> fonts { get; set; }

    public GmxProjectFonts()
    {
        name = "fonts";
        fonts = new List<string>();
    }
}

public class GmxProjectObjects
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("object")]
    public List<string> objects { get; set; }

    public GmxProjectObjects()
    {
        name = "objects";
        objects = new List<string>();
    }
}

public class GmxProjectRooms
{
    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlElement("room")]
    public List<string> rooms { get; set; }

    public GmxProjectRooms()
    {
        name = "rooms";
        rooms = new List<string>();
    }
}

public class GmxProjectHelp
{
    [XmlElement("rtf")]
    public string rtf { get; set; }

    public GmxProjectHelp()
    {
        rtf = "help.rtf";
    }
}

public class GmxProjectTutorialState
{
    [XmlElement("IsTutorial")]
    public int isTutorial { get; set; }
    [XmlElement("TutorialName")]
    public GmxBetterString tutorialName { get; set; }
    [XmlElement("TutorialPage")]
    public int tutorialPage { get; set; }

    public GmxProjectTutorialState()
    {
        isTutorial = 0;
        tutorialName ??= new GmxBetterString();
        tutorialName.text = String.Empty;
        tutorialPage = 0;
    }
}
