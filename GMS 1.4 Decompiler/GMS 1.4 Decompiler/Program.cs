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

public class GmxBetterString
{
    [XmlText]
    public string text { get; set; }

    public GmxBetterString()
    {

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
                //if (File.Exists(sourcePath))
                //{
                    externalSoundNames.Add(filename);
                    if (!getExternalSoundNamesOnly)
                    {
                        File.Copy(sourcePath, String.Concat(audioFilesPath, filename), true);
                    }
                //}
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

            if (vertex.Contains("#define _YY_GLSLES_ 1"))
            {
                string[] arr = vertex.Split("#define _YY_GLSLES_ 1");
                if (arr.Length > 1)
                {
                    vertex = arr[1];
                } else
                {
                    vertex = "";
                }
            }

            if (fragment.Contains("#define _YY_GLSLES_ 1"))
            {
                string[] arr = fragment.Split("#define _YY_GLSLES_ 1");
                if (arr.Length > 1)
                {
                    fragment = arr[1];
                }
                else
                {
                    fragment = "";
                }
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
