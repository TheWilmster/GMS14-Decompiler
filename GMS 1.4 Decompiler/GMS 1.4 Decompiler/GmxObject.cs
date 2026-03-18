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

        for (int eventType = 0; eventType < resource.Events.Count; eventType++)
        {
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
