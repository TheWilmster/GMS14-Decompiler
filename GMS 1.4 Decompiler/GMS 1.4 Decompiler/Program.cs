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

public class DecompilerConfig
{
    public string dataFilePath { get; set; }
    public bool exportSounds { get; set; }
    public bool exportSprites { get; set; }
    public bool exportScripts { get; set; }
    public bool exportBackgrounds { get; set; }
    public bool exportPaths { get; set; }
    public bool exportShaders { get; set; }
    public string? bypassShaderType { get; set; }
    public bool exportFonts { get; set; }
    public bool exportObjects { get; set; }
    public bool exportRooms { get; set; }
    public bool exportProjectAndIncludedFiles { get; set; }
    public DecompilerConfig()
    {
        dataFilePath = "C:\\Users\\wcoop\\Documents\\pizzatowerdemobuildspublic\\Public\\pizzatowerearlybuildtest_v211\\data.win";
        exportSounds = true;
        exportSprites = true;
        exportScripts = true;
        exportBackgrounds = true;
        exportPaths = true;
        exportShaders = true;
        bypassShaderType = null;
        exportFonts = true;
        exportObjects = true;
        exportRooms = true;
        exportProjectAndIncludedFiles = true;
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
        if (config.exportBackgrounds) decompiler.DumpBackgrounds();
        if (config.exportPaths) decompiler.DumpPaths();
        if (config.exportShaders) decompiler.DumpShaders();
        if (config.exportFonts) decompiler.DumpFonts();
        if (config.exportObjects) decompiler.DumpObjects();
        if (config.exportRooms) decompiler.DumpRooms();
        if (config.exportProjectAndIncludedFiles)
        {
            decompiler.DumpIncludedFiles();
            decompiler.DumpProjectFile();
        }
    }
}

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

    public GmxProjectDatafiles() {
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

    public GmxProjectDatafileConfigOptions() {
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

[XmlRoot("room")]
public class GmxAssetRoom
{
    [XmlElement("caption")]
    public GmxBetterString caption { get; set; }

    [XmlElement("width")]
    public uint width { get; set; }

    [XmlElement("height")]
    public uint height { get; set; }

    [XmlElement("vsnap")]
    public uint verticalSnap { get; set; }

    [XmlElement("hsnap")]
    public uint horizontalSnap { get; set; }

    [XmlElement("isometric")]
    public int isometric { get; set; }

    [XmlElement("speed")]
    public float speed { get; set; }

    [XmlElement("persistent")]
    public int persistent { get; set; }

    [XmlElement("colour")]
    public uint colour { get; set; }

    [XmlElement("showcolour")]
    public int showColour { get; set; }

    [XmlElement("code")]
    public GmxBetterString code { get; set; }

    [XmlElement("enableViews")]
    public int enableViews { get; set; }

    [XmlElement("clearViewBackground")]
    public int clearViewBackground { get; set; }

    [XmlElement("clearDisplayBuffer")]
    public int clearDisplayBuffer { get; set; }

    [XmlElement("makerSettings")]
    public GmxRoomMakerSettings makerSettings { get; set; }

    [XmlElement("backgrounds")]
    public GmxRoomBackgrounds backgrounds { get; set; }

    [XmlElement("views")]
    public GmxRoomViewports viewports { get; set; }

    [XmlElement("instances")]
    public GmxRoomInstances instances { get; set; }

    [XmlElement("tiles")]
    public GmxRoomTiles tiles { get; set; }

    [XmlElement("PhysicsWorld")]
    public int physicsWorld { get; set; }

    [XmlElement("PhysicsWorldTop")]
    public uint physicsWorldTop { get; set; }

    [XmlElement("PhysicsWorldLeft")]
    public uint physicsWorldLeft { get; set; }

    [XmlElement("PhysicsWorldRight")]
    public uint physicsWorldRight { get; set; }

    [XmlElement("PhysicsWorldBottom")]
    public uint physicsWorldBottom { get; set; }

    [XmlElement("PhysicsWorldGravityX")]
    public float physicsWorldGravityX { get; set; }

    [XmlElement("PhysicsWorldGravityY")]
    public float physicsWorldGravityY { get; set; }

    [XmlElement("PhysicsWorldPixToMeters")]
    public float physicsWorldPixToMeters { get; set; }

    public GmxAssetRoom() {}

    public GmxAssetRoom(UndertaleRoom resource, Decompiler decompiler)
    {
        caption ??= new GmxBetterString();
        caption.text = resource.Caption.Content ?? "";
        width = resource.Width;
        height = resource.Height;
        verticalSnap = 16;
        horizontalSnap = 16;
        isometric = 0;
        speed = resource.Speed;
        persistent = -Convert.ToInt32(resource.Persistent);

        // convert from android color code (gms2) to decimal color code (gms1)
        string androidHexColor = resource.BackgroundColor.ToString("X");

        //double alpha = double.Parse(androidHexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        uint red = uint.Parse(androidHexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        uint green = uint.Parse(androidHexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        uint blue = uint.Parse(androidHexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        double decimalCol = ((double)red * System.Math.Pow(256, 2)) + ((double)green * System.Math.Pow(256, 1)) + (double)blue;
        colour = (uint)decimalCol;
        showColour = -Convert.ToInt32(resource.DrawBackgroundColor);

        GlobalDecompileContext globalDecompileContext = new(decompiler.GameData);
        Underanalyzer.Decompiler.IDecompileSettings decompilerSettings = decompiler.GameData.ToolInfo.DecompilerSettings;
        code ??= new GmxBetterString();
        code.text = resource.CreationCodeId != null ?
                        new Underanalyzer.Decompiler.DecompileContext(globalDecompileContext, resource.CreationCodeId, decompilerSettings).DecompileToString() :
                        "";
        enableViews = -Convert.ToInt32(resource.Flags.HasFlag(UndertaleRoom.RoomEntryFlags.EnableViews));
        clearViewBackground = -Convert.ToInt32(resource.Flags.HasFlag(UndertaleRoom.RoomEntryFlags.ClearViewBackground));
        clearDisplayBuffer = -Convert.ToInt32(!resource.Flags.HasFlag(UndertaleRoom.RoomEntryFlags.DoNotClearDisplayBuffer));

        // temporarily used by the ide
        makerSettings = new GmxRoomMakerSettings();

        backgrounds = new GmxRoomBackgrounds();
        backgrounds.backgrounds = new List<GmxRoomBackground>();
        foreach (UndertaleRoom.Background background in resource.Backgrounds)
        {
            GmxRoomBackground xmlBackground = new GmxRoomBackground();
            xmlBackground.visible = -Convert.ToInt32(background.Enabled);
            xmlBackground.isForeground = -Convert.ToInt32(background.Foreground);
            xmlBackground.name = background.BackgroundDefinition?.Name?.Content ?? String.Empty;
            xmlBackground.x = background.X;
            xmlBackground.y = background.Y;
            xmlBackground.horizontallyTiled = -Convert.ToInt32(background.TiledHorizontally);
            xmlBackground.verticallyTiled = -Convert.ToInt32(background.TiledVertically);
            xmlBackground.horizontalSpeed = background.SpeedX;
            xmlBackground.verticalSpeed = background.SpeedY;
            xmlBackground.stretch = -Convert.ToInt32(background.Stretch);
            backgrounds.backgrounds.Add(xmlBackground);
        }

        viewports = new GmxRoomViewports();
        viewports.viewports = new List<GmxRoomViewport>();
        foreach (UndertaleRoom.View viewport in resource.Views)
        {
            GmxRoomViewport xmlViewport = new GmxRoomViewport();
            xmlViewport.visible = -Convert.ToInt32(viewport.Enabled);
            xmlViewport.objName = viewport.ObjectId?.Name?.Content ?? "<undefined>";
            xmlViewport.xView = viewport.ViewX;
            xmlViewport.yView = viewport.ViewY;
            xmlViewport.wView = viewport.ViewWidth;
            xmlViewport.hView = viewport.ViewHeight;
            xmlViewport.xPort = viewport.PortX;
            xmlViewport.yPort = viewport.PortY;
            xmlViewport.wPort = viewport.PortWidth;
            xmlViewport.hPort = viewport.PortHeight;
            xmlViewport.hBorder = viewport.BorderX;
            xmlViewport.vBorder = viewport.BorderY;
            xmlViewport.hSpeed = viewport.SpeedX;
            xmlViewport.vSpeed = viewport.SpeedY;
            viewports.viewports.Add(xmlViewport);
        }

        instances = new GmxRoomInstances();
        instances.instances = new List<GmxRoomInstance>();
        foreach (UndertaleRoom.GameObject gameObject in resource.GameObjects)
        {
            GmxRoomInstance xmlInstance = new GmxRoomInstance();
            xmlInstance.objName = gameObject.ObjectDefinition.Name.Content;
            xmlInstance.x = gameObject.X;
            xmlInstance.y = gameObject.Y;
            xmlInstance.instanceName = String.Concat("inst_", gameObject.InstanceID);
            xmlInstance.isLocked = 0;
            xmlInstance.creationCode = gameObject.CreationCode != null ?
                        new Underanalyzer.Decompiler.DecompileContext(globalDecompileContext, gameObject.CreationCode, decompilerSettings).DecompileToString() :
                        "";
            xmlInstance.xscale = gameObject.ScaleX;
            xmlInstance.yscale = gameObject.ScaleY;
            xmlInstance.colour = gameObject.Color;
            xmlInstance.rotation = gameObject.Rotation;
            instances.instances.Add(xmlInstance);
        }

        tiles = new GmxRoomTiles();
        tiles.tiles = new List<GmxRoomTile>();
        foreach (UndertaleRoom.Tile tile in resource.Tiles)
        {
            GmxRoomTile xmlTile = new GmxRoomTile();
            xmlTile.backgroundName = tile.BackgroundDefinition?.Name?.Content ?? String.Empty;
            xmlTile.x = tile.X;
            xmlTile.y = tile.Y;
            xmlTile.width = tile.Width;
            xmlTile.height = tile.Height;
            xmlTile.tileCoordX = tile.SourceX;
            xmlTile.tileCoordY = tile.SourceY;
            xmlTile.id = tile.InstanceID;
            xmlTile.instanceName = String.Concat("inst_", tile.InstanceID);
            xmlTile.depth = tile.TileDepth;
            xmlTile.isLocked = 0;
            xmlTile.colour = tile.Color;
            xmlTile.xscale = tile.ScaleX;
            xmlTile.yscale = tile.ScaleY;
            tiles.tiles.Add(xmlTile);
        }

        physicsWorld = -Convert.ToInt32(resource.World);
        physicsWorldTop = resource.Top;
        physicsWorldLeft = resource.Left;
        physicsWorldRight = resource.Right;
        physicsWorldBottom = resource.Bottom;
        physicsWorldGravityX = resource.GravityX;
        physicsWorldGravityY = resource.GravityY;
        physicsWorldPixToMeters = resource.MetersPerPixel;
    }
}

public class GmxBetterString
{
    [XmlText]
    public string text { get; set; }

    public GmxBetterString()
    {

    }
}

public class GmxRoomMakerSettings
{
    [XmlElement("isSet")]
    public int isSet { get; set; }

    [XmlElement("w")]
    public uint width { get; set; }

    [XmlElement("h")]
    public uint height { get; set; }

    [XmlElement("showGrid")]
    public int showGrid { get; set; }

    [XmlElement("showObjects")]
    public int showObjects { get; set; }

    [XmlElement("showTiles")]
    public int showTiles { get; set; }

    [XmlElement("showBackgrounds")]
    public int showBackgrounds { get; set; }

    [XmlElement("showForegrounds")]
    public int showForegrounds { get; set; }

    [XmlElement("showViews")]
    public int showViews { get; set; }

    [XmlElement("deleteUnderlyingObj")]
    public int deleteUnderlyingObj { get; set; }

    [XmlElement("deleteUnderlyingTiles")]
    public int deleteUnderlyingTiles { get; set; }

    [XmlElement("page")]
    public uint page { get; set; }

    [XmlElement("xoffset")]
    public int xOffset { get; set; }

    [XmlElement("yoffset")]
    public int yOffset { get; set; }

    public GmxRoomMakerSettings()
    {
        isSet = 0;
        width = 0;
        height = 0;
        showGrid = 0;
        showObjects = 0;
        showTiles = 0;
        showBackgrounds = 0;
        showForegrounds = 0;
        showViews = 0;
        deleteUnderlyingObj = 0;
        deleteUnderlyingTiles = 0;
        page = 0;
        xOffset = 0;
        yOffset = 0;
    }
}

public class GmxRoomBackgrounds
{
    [XmlElement("background")]
    public List<GmxRoomBackground> backgrounds { get; set; }
}

public class GmxRoomBackground
{
    [XmlAttribute("visible")]
    public int visible { get; set; }

    [XmlAttribute("foreground")]
    public int isForeground { get; set; }

    [XmlAttribute("name")]
    public string name { get; set; }

    [XmlAttribute("x")]
    public float x { get; set; }

    [XmlAttribute("y")]
    public float y { get; set; }

    [XmlAttribute("htiled")]
    public int horizontallyTiled { get; set; }

    [XmlAttribute("vtiled")]
    public int verticallyTiled { get; set; }

    [XmlAttribute("hspeed")]
    public int horizontalSpeed { get; set; }

    [XmlAttribute("vspeed")]
    public int verticalSpeed { get; set; }

    [XmlAttribute("stretch")]
    public int stretch { get; set; }
}

public class GmxRoomViewports
{
    [XmlElement("view")]
    public List<GmxRoomViewport> viewports { get; set; }
}

public class GmxRoomViewport
{
    [XmlAttribute("visible")]
    public int visible { get; set; }

    [XmlAttribute("objName")]
    public string objName { get; set; }

    [XmlAttribute("xview")]
    public int xView { get; set; }

    [XmlAttribute("yview")]
    public int yView { get; set; }

    [XmlAttribute("wview")]
    public int wView { get; set; }

    [XmlAttribute("hview")]
    public int hView { get; set; }

    [XmlAttribute("xport")]
    public int xPort { get; set; }

    [XmlAttribute("yport")]
    public int yPort { get; set; }

    [XmlAttribute("wport")]
    public int wPort { get; set; }

    [XmlAttribute("hport")]
    public int hPort { get; set; }

    [XmlAttribute("hborder")]
    public uint hBorder { get; set; }

    [XmlAttribute("vborder")]
    public uint vBorder { get; set; }

    [XmlAttribute("hspeed")]
    public int hSpeed { get; set; }

    [XmlAttribute("vspeed")]
    public int vSpeed { get; set; }
}

public class GmxRoomInstances
{
    [XmlElement("instance")]
    public List<GmxRoomInstance> instances { get; set; }
}

public class GmxRoomInstance
{
    [XmlAttribute("objName")]
    public string objName { get; set; }

    [XmlAttribute("x")]
    public int x { get; set; }

    [XmlAttribute("y")]
    public int y { get; set; }

    [XmlAttribute("name")]
    public string instanceName { get; set; }

    [XmlAttribute("locked")]
    public int isLocked { get; set; }

    [XmlAttribute("code")]
    public string creationCode { get; set; }

    [XmlAttribute("scaleX")]
    public float xscale { get; set; }

    [XmlAttribute("scaleY")]
    public float yscale { get; set; }

    [XmlAttribute("colour")]
    public uint colour { get; set; }

    [XmlAttribute("rotation")]
    public float rotation { get; set; }
}

public class GmxRoomTiles
{
    [XmlElement("tile")]
    public List<GmxRoomTile> tiles { get; set; }
}

public class GmxRoomTile
{
    [XmlAttribute("bgName")]
    public string backgroundName { get; set; }

    [XmlAttribute("x")]
    public int x { get; set; }

    [XmlAttribute("y")]
    public int y { get; set; }

    [XmlAttribute("w")]
    public uint width { get; set; }

    [XmlAttribute("h")]
    public uint height { get; set; }

    [XmlAttribute("xo")]
    public int tileCoordX { get; set; }

    [XmlAttribute("yo")]
    public int tileCoordY { get; set; }

    [XmlAttribute("id")]
    public uint id { get; set; }

    [XmlAttribute("name")]
    public string instanceName { get; set; }

    [XmlAttribute("depth")]
    public int depth { get; set; }

    [XmlAttribute("locked")]
    public int isLocked { get; set; }

    [XmlAttribute("colour")]
    public uint colour { get; set; }

    [XmlAttribute("scaleX")]
    public float xscale { get; set; }

    [XmlAttribute("scaleY")]
    public float yscale { get; set; }
}

[XmlRoot("object")]
public class GmxAssetObject
{
    [XmlElement("spriteName")]
    public string spriteName { get; set; }

    [XmlElement("solid")]
    public int solid { get; set; }

    [XmlElement("visible")]
    public int visible { get; set; }

    [XmlElement("depth")]
    public int depth { get; set; }

    [XmlElement("persistent")]
    public int persistent { get; set; }

    [XmlElement("parentName")]
    public string parentName { get; set; }

    [XmlElement("maskName")]
    public string maskName { get; set; }

    [XmlElement("events")]
    public GmxObjectEvents events { get; set; }

    [XmlElement("PhysicsObject")]
    public int physicsObject { get; set; }

    [XmlElement("PhysicsObjectSensor")]
    public int physicsObjectSensor { get; set; }

    [XmlElement("PhysicsObjectShape")]
    public int physicsObjectShape { get; set; }

    [XmlElement("PhysicsObjectDensity")]
    public float physicsObjectDensity { get; set; }

    [XmlElement("PhysicsObjectRestitution")]
    public float physicsObjectRestitution { get; set; }

    [XmlElement("PhysicsObjectGroup")]
    public uint physicsObjectGroup { get; set; }

    [XmlElement("PhysicsObjectLinearDamping")]
    public float physicsObjectLinearDamping { get; set; }

    [XmlElement("PhysicsObjectAngularDamping")]
    public float physicsObjectAngularDamping { get; set; }

    [XmlElement("PhysicsObjectFriction")]
    public float physicsObjectFriction { get; set; }

    [XmlElement("PhysicsObjectAwake")]
    public int physicsObjectAwake { get; set; }

    [XmlElement("PhysicsObjectKinematic")]
    public int physicsObjectKinematic { get; set; }

    [XmlElement("PhysicsShapePoints")]
    public GmxPhysicsShapePoints physicsShapePoints { get; set; }

    public GmxAssetObject() { }

    public GmxAssetObject(UndertaleGameObject resource, Decompiler decompiler)
    {
        spriteName = resource.Sprite?.Name?.Content ?? "<undefined>";
        solid = -Convert.ToInt32(resource.Solid);
        visible = -Convert.ToInt32(resource.Visible);
        depth = resource.Depth;
        persistent = -Convert.ToInt32(resource.Persistent);
        parentName = resource.ParentId?.Name?.Content ?? "<undefined>";
        maskName = resource.TextureMaskId?.Name?.Content ?? "<undefined>";
        events = new GmxObjectEvents();
        events.events = new List<GmxObjectEvent>();

        GlobalDecompileContext globalDecompileContext = new(decompiler.GameData);
        Underanalyzer.Decompiler.IDecompileSettings decompilerSettings = decompiler.GameData.ToolInfo.DecompilerSettings;

        for (int eventType = 0; eventType < resource.Events.Count; eventType++) {
            if (eventType == (int)EventType.PreCreate)
            {
                continue;
            }

            foreach (var ev in resource.Events[eventType]) // i usually would be the actual type here but .net is being fucky
            {
                List<GmxObjectAction> actions = new List<GmxObjectAction>();
                foreach (UndertaleGameObject.EventAction action in ev.Actions)
                {
                    GmxObjectAction xmlAction = new GmxObjectAction();
                    xmlAction.libid = action.LibID;
                    xmlAction.id = action.ID;
                    xmlAction.kind = action.Kind;
                    xmlAction.useRelative = -Convert.ToInt32(action.UseRelative);
                    xmlAction.isQuestion = -Convert.ToInt32(action.IsQuestion);
                    xmlAction.useApplyTo = -Convert.ToInt32(action.UseApplyTo);
                    xmlAction.exeType = action.ExeType;
                    xmlAction.actionName = action.ActionName.Content;
                    xmlAction.codeString = "";
                    xmlAction.whoName = action.Who switch
                    {
                        -1 => "self",
                        -2 => "other",
                        _ => decompiler.GameData.GameObjects[action.Who].Name.Content
                    };
                    xmlAction.relative = -Convert.ToInt32(action.Relative);
                    xmlAction.isNot = -Convert.ToInt32(action.IsNot);
                    xmlAction.arguments = new GmxObjectActionArguments();
                    xmlAction.arguments.arguments = new List<GmxObjectActionArgument>();
                    for (int i = 0; i < action.ArgumentCount; i++)
                    {
                        GmxObjectActionArgument argument = new GmxObjectActionArgument();
                        argument.kind = 1;
                        argument.str = action.CodeId != null ?
                        new Underanalyzer.Decompiler.DecompileContext(globalDecompileContext, action.CodeId, decompilerSettings).DecompileToString() :
                        "";
                        xmlAction.arguments.arguments.Add(argument);
                    }
                    actions.Add(xmlAction);
                }
                events.events.Add(new GmxObjectEvent((EventType)eventType, (int)ev.EventSubtype, actions));
            }
        }

        physicsObject = -Convert.ToInt32(resource.UsesPhysics);
        physicsObjectSensor = -Convert.ToInt32(resource.IsSensor);
        physicsObjectShape = (int)resource.CollisionShape;
        physicsObjectDensity = resource.Density;
        physicsObjectRestitution = resource.Restitution;
        physicsObjectGroup = resource.Group;
        physicsObjectLinearDamping = resource.LinearDamping;
        physicsObjectAngularDamping = resource.AngularDamping;
        physicsObjectFriction = resource.Friction;
        physicsObjectAwake = -Convert.ToInt32(resource.Awake);
        physicsObjectKinematic = -Convert.ToInt32(resource.Kinematic);
    }
}

public class GmxPhysicsShapePoints
{

}

public class GmxObjectEvents
{
    [XmlElement("event")]
    public List<GmxObjectEvent> events { get; set; }

    public GmxObjectEvents() { }
}

public class GmxObjectEvent
{
    [XmlAttribute("eventtype")]
    public int eventType { get; set; }

    [XmlAttribute("enumb")]
    public int eventNumber { get; set; }

    [XmlElement("action")]
    public List<GmxObjectAction> actions { get; set; }

    public GmxObjectEvent() { }

    public GmxObjectEvent(EventType evType, int evNum, List<GmxObjectAction> actionsList)
    {
        eventType = (int)evType;
        eventNumber = evNum;
        actions = actionsList;
    }
}

public class GmxObjectAction
{
    [XmlElement("libid")]
    public uint libid { get; set; }

    [XmlElement("id")]
    public uint id { get; set; }

    [XmlElement("kind")]
    public uint kind { get; set; }

    [XmlElement("userelative")]
    public int useRelative { get; set; }

    [XmlElement("isquestion")]
    public int isQuestion { get; set; }

    [XmlElement("useapplyto")]
    public int useApplyTo { get; set; }

    [XmlElement("exetype")]
    public uint exeType { get; set; }

    [XmlElement("functionname")]
    public string actionName { get; set; }

    [XmlElement("codestring")]
    public string codeString { get; set; }

    [XmlElement("whoName")]
    public string whoName { get; set; }

    [XmlElement("relative")]
    public int relative { get; set; }

    [XmlElement("isnot")]
    public int isNot { get; set; } // what the fuck does "is not" even mean

    [XmlElement("arguments")]
    public GmxObjectActionArguments arguments { get; set; }
}

public class GmxObjectActionArguments
{
    [XmlElement("argument")]
    public List<GmxObjectActionArgument> arguments { get; set; }

    public GmxObjectActionArguments() { }
}

public class GmxObjectActionArgument
{
    [XmlElement("kind")]
    public uint kind { get; set; }

    [XmlElement("string")]
    public string str { get; set; }

    public GmxObjectActionArgument() { }
}

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
    public GmxFontTextureGroups textureGroups {  get; set; }

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
    public GmxFontRanges () {}
}

public class GmxFontTextureGroups
{
    [XmlElement("texgroup0")]
    public int texgroup;

    public GmxFontTextureGroups() {}

    public GmxFontTextureGroups(int index)
    {
        texgroup = index;
    }
}

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
        foreach (UndertaleSprite.TextureEntry Texture in resource.Textures) {
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
            DecompilePath = String.Concat(AppDomain.CurrentDomain.BaseDirectory, "DecompiledProjects\\", GameData.GeneralInfo.Name.Content, "\\");

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
            Console.SetCursorPosition(0, row);

            DumpSprite(sprite);
            i++;
        }
        Console.WriteLine("Sprites done!                                                                                                                    ");
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
            Console.SetCursorPosition(0, row);

            DumpScript(script);
            i++;
        }
        Console.WriteLine("Scripts done!                                                                                                                    ");
    }

    public void DumpSounds(bool getExternalSoundNamesOnly = false)
    {
        if (!getExternalSoundNamesOnly) Console.WriteLine("Dumping Sounds...");
        int i = 0;
        foreach (UndertaleSound sound in GameData.Sounds)
        {
            if (!getExternalSoundNamesOnly)
            {
                var (_, row) = Console.GetCursorPosition();
                Console.WriteLine("                                                                                                                             ");
                Console.SetCursorPosition(0, row);

                (_, row) = Console.GetCursorPosition();
                Console.WriteLine(String.Concat(
                    "(", i + 1, "/", GameData.Sounds.Count, ") Dumping ", sound.Name.Content, "..."
                ));
                Console.SetCursorPosition(0, row);
            }
            DumpSound(sound);
            i++;
        }
        if (!getExternalSoundNamesOnly)
        Console.WriteLine("Sounds done!                                                                                                                     ");
    }

    public void DumpBackgrounds()
    {
        Console.WriteLine("Dumping Backgrounds...");
        int i = 0;
        foreach (UndertaleBackground background in GameData.Backgrounds)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Backgrounds.Count, ") Dumping ", background.Name.Content, "..."
            ));
            Console.SetCursorPosition(0, row);

            DumpBackground(background);
            i++;
        }
        Console.WriteLine("Backgrounds done!                                                                                                                ");
    }

    public void DumpPaths()
    {
        Console.WriteLine("Dumping Paths...");
        int i = 0;
        foreach (UndertalePath path in GameData.Paths)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Paths.Count, ") Dumping ", path.Name.Content, "..."
            ));
            Console.SetCursorPosition(0, row);

            DumpPath(path);
            i++;
        }
        Console.WriteLine("Paths done!                                                                                                                      ");
    }

    public void DumpShaders()
    {
        Console.WriteLine("Dumping Shaders...");
        int i = 0;
        foreach (UndertaleShader shader in GameData.Shaders)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Shaders.Count, ") Dumping ", shader.Name.Content, "..."
            ));
            Console.SetCursorPosition(0, row);

            DumpShader(shader);
            i++;
        }
        Console.WriteLine("Shaders done!                                                                                                                    ");
    }

    public void DumpFonts()
    {
        Console.WriteLine("Dumping Fonts...");
        int i = 0;
        foreach (UndertaleFont font in GameData.Fonts)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Fonts.Count, ") Dumping ", font.Name.Content, "..."
            ));
            Console.SetCursorPosition(0, row);

            DumpFont(font);
            i++;
        }
        Console.WriteLine("Fonts done!                                                                                                                      ");
    }

    public void DumpObjects()
    {
        Console.WriteLine("Dumping Objects...");
        int i = 0;
        foreach (UndertaleGameObject gameObject in GameData.GameObjects)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.GameObjects.Count, ") Dumping ", gameObject.Name.Content, "..."
            ));
            Console.SetCursorPosition(0, row);

            DumpObject(gameObject);
            i++;
        }
        Console.WriteLine("Objects done!                                                                                                                    ");
    }
    public void DumpRooms()
    {
        Console.WriteLine("Dumping Rooms...");
        int i = 0;
        foreach (UndertaleRoom room in GameData.Rooms)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", GameData.Rooms.Count, ") Dumping ", room.Name.Content, "..."
            ));
            Console.SetCursorPosition(0, row);

            DumpRoom(room);
            i++;
        }
        Console.WriteLine("Rooms done!                                                                                                                      ");
    }

    public void DumpIncludedFiles()
    {
        Console.WriteLine("Copying over included files...");
        List<string> filteredFiles = GetIncludedFiles();

        string datafilesPath = String.Concat(DecompilePath, "datafiles\\");
        if (!Directory.Exists(datafilesPath))
        {
            Directory.CreateDirectory(datafilesPath);
        }

        int i = 0;
        foreach (string file in filteredFiles)
        {
            var (_, row) = Console.GetCursorPosition();
            Console.WriteLine("                                                                                                                             ");
            Console.SetCursorPosition(0, row);

            (_, row) = Console.GetCursorPosition();
            Console.WriteLine(String.Concat(
                "(", i + 1, "/", filteredFiles.Count, ") Copying ", file, "..."
            ));
            Console.SetCursorPosition(0, row);

            string sourcePath = Path.Combine(Path.GetDirectoryName(GamePath), file);
            string destPath = Path.Combine(datafilesPath, file);
            File.Copy(sourcePath, destPath, true);
            i++;
        }
        Console.WriteLine("Included files done!                                                                                                             ");
    }

    public List<string> GetIncludedFiles()
    {
        if (externalSoundNames == null)
        {
            DumpSounds(true);
        }

        List<string> filteredFiles = new List<string>();
        foreach (string file in Directory.GetFiles(Path.GetDirectoryName(GamePath)))
        {
            string trimmed_file = Path.GetFileName(file);
            if ((externalSoundNames == null || !externalSoundNames.Contains(trimmed_file)) && !String.Equals(trimmed_file, "D3DX9_43.dll") && !String.Equals(trimmed_file, "data.win") && !String.Equals(trimmed_file, "options.ini") && !String.Equals(trimmed_file, GameData.GeneralInfo.FileName))
            {
                filteredFiles.Add(trimmed_file);
            }
        }
        return filteredFiles;
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

    public void DumpSound(UndertaleSound resource, bool getExternalSoundNamesOnly = false)
    {
        if (resource != null)
        {
            GmxAssetSound sound = new GmxAssetSound(resource, this);

            string soundsPath = String.Concat(DecompilePath, "sound\\");

            string audioFilesPath = String.Concat(soundsPath, "audio\\");
            if (!Directory.Exists(soundsPath) && !getExternalSoundNamesOnly)
            {
                Directory.CreateDirectory(soundsPath);
            }
            if (!Directory.Exists(audioFilesPath) && !getExternalSoundNamesOnly)
            {
                Directory.CreateDirectory(audioFilesPath);
            }
            externalSoundNames ??= new List<string>();

            if (!resource.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsEmbedded))
            {
                string filename = resource.File.Content;
                if (!filename.Contains('.'))
                {
                    filename += sound.extension;
                }
                string sourcePath = Path.Combine(Path.GetDirectoryName(GamePath), filename);
                if (File.Exists(sourcePath))
                {
                    externalSoundNames.Add(filename);
                    if (!getExternalSoundNamesOnly)
                    {
                        File.Copy(sourcePath, String.Concat(audioFilesPath, filename), true);
                    }
                }
            }
            else if (!getExternalSoundNamesOnly)
            {
                File.WriteAllBytes(String.Concat(audioFilesPath, resource.Name.Content, sound.extension), GetAudioData(resource));
            }

            if (!getExternalSoundNamesOnly)
            {
                File.WriteAllText(String.Concat(soundsPath, resource.Name.Content, ".sound.gmx"), ToXML(sound));
            }
        }
    }

    public void DumpBackground(UndertaleBackground resource)
    {
        if (resource != null)
        {
            
            if (resource.Texture != null)
            {
                GmxAssetBackground background = new GmxAssetBackground(resource);

                TextureWorker worker = new TextureWorker();

                string imagesPath = String.Concat(DecompilePath, "background\\images\\");

                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                worker.ExportAsPNG(resource.Texture, String.Concat(imagesPath, resource.Name.Content, ".png"), null, true);

                File.WriteAllText(String.Concat(DecompilePath, "background\\", resource.Name.Content, ".background.gmx"), ToXML(background));
            }
        }
    }

    public void DumpPath(UndertalePath resource)
    {
        if (resource != null)
        {
            GmxAssetPath path = new GmxAssetPath(resource);

            string pathsPath = String.Concat(DecompilePath, "paths\\");

            if (!Directory.Exists(pathsPath))
            {
                Directory.CreateDirectory(pathsPath);
            }

            File.WriteAllText(String.Concat(pathsPath, resource.Name.Content, ".path.gmx"), ToXML(path));
        }
    }

    public void DumpShader(UndertaleShader resource, string? bypassShaderType = null)
    {
        if (resource != null)
        {
            string vertex;
            string fragment;
            string shaderType;
            shaderType = resource.Type switch
            {
                UndertaleShader.ShaderType.GLSL => "GLSL",
                UndertaleShader.ShaderType.GLSL_ES => "GLSL ES",
                UndertaleShader.ShaderType.HLSL9 => "HLSL9",
                UndertaleShader.ShaderType.HLSL11 => "HLSL11",
                UndertaleShader.ShaderType.PSSL => "PSSL",
                UndertaleShader.ShaderType.Cg_PSVita => "Cg_PSVita",
                UndertaleShader.ShaderType.Cg_PS3 => "Cg_PS3",
                _ => "{undefined shader type}"
            };
            if (bypassShaderType != null)
            {
                shaderType = bypassShaderType;
            }
            switch (shaderType)
            {
                case "GLSL":
                    vertex = resource.GLSL_Vertex.Content;
                    fragment = resource.GLSL_Fragment.Content;
                    break;
                case "GLSL ES":
                    vertex = resource.GLSL_ES_Vertex.Content;
                    fragment = resource.GLSL_ES_Fragment.Content;
                    break;
                case "HLSL9":
                    vertex = resource.HLSL9_Vertex.Content;
                    fragment = resource.HLSL9_Fragment.Content;
                    break;
                case "HLSL11":
                case "PSSL":
                case "Cg_PSVita":
                case "Cg_PS3":
                    vertex = String.Concat("//Either this decompiler does not yet support ", shaderType, ", or your decompiler version is outdated.");
                    fragment = String.Concat("//Either this decompiler does not yet support ", shaderType, ", or your decompiler version is outdated.");
                    break;
                default:
                    vertex = String.Concat("//Shader type ", shaderType, " does not exist. Did you type in the name wrong?");
                    fragment = String.Concat("//Shader type ", shaderType, " does not exist. Did you type in the name wrong?");
                    break;
            }
            string shaderString = String.Concat(vertex, "\n", "//######################_==_YOYO_SHADER_MARKER_==_######################@~//\n", fragment);

            string shadersPath = String.Concat(DecompilePath, "shaders\\");
            if (!Directory.Exists(shadersPath))
            {
                Directory.CreateDirectory(shadersPath);
            }

            File.WriteAllText(String.Concat(shadersPath, resource.Name.Content, ".shader"), shaderString);
        }
    }

    public void DumpFont(UndertaleFont resource)
    {
        if (resource != null)
        {
            GmxAssetFont font = new GmxAssetFont(resource);

            string fontsPath = String.Concat(DecompilePath, "fonts\\");
            if (!Directory.Exists(fontsPath))
            {
                Directory.CreateDirectory(fontsPath);
            }

            TextureWorker worker = new TextureWorker();
            worker.ExportAsPNG(resource.Texture, String.Concat(fontsPath, resource.Name.Content, ".png"));

            File.WriteAllText(String.Concat(fontsPath, resource.Name.Content, ".font.gmx"), ToXML(font));
        }
    }

    public void DumpObject(UndertaleGameObject resource)
    {
        if (resource != null)
        {
            GmxAssetObject gameObject = new GmxAssetObject(resource, this);

            string objectsPath = String.Concat(DecompilePath, "objects\\");
            if (!Directory.Exists(objectsPath))
            {
                Directory.CreateDirectory(objectsPath);
            }

            File.WriteAllText(String.Concat(objectsPath, resource.Name.Content, ".object.gmx"), ToXML(gameObject));
        }
    }
    
    public void DumpRoom(UndertaleRoom resource)
    {
        if (resource != null)
        {
            GmxAssetRoom room = new GmxAssetRoom(resource, this);

            string roomsPath = String.Concat(DecompilePath, "rooms\\");
            if (!Directory.Exists(roomsPath))
            {
                Directory.CreateDirectory(roomsPath);
            }

            File.WriteAllText(String.Concat(roomsPath, resource.Name.Content, ".room.gmx"), ToXML(room));
        }
    }

    public void DumpProjectFile()
    {
        string rtfHelp = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Arial;}}\r\n{\\colortbl ;\\red221\\green221\\blue221;}\r\n\\viewkind4\\uc1\\pard\\cf1\\fs24\\par\r\n}\r\n�";

        GmxAssetProject projectFile = new GmxAssetProject(this);

        File.WriteAllText(String.Concat(DecompilePath, GameData.GeneralInfo.Name.Content, ".project.gmx"), ToXML(projectFile));
        File.WriteAllText(String.Concat(DecompilePath, "help.rtf"), rtfHelp);
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
        if (resource.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsEmbedded))
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
    private List<string> externalSoundNames;
}
