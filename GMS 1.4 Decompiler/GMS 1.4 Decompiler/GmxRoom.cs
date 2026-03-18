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

    public GmxAssetRoom() { }

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
