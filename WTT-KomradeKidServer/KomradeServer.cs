using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services;
using Range = SemanticVersioning.Range;
using Path = System.IO.Path;

namespace KomradeKidServer;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "com.wtt.komradekid";
    public override string Name { get; init; } = "WTT-KomradeKidServer";
    public override string Author { get; init; } = "GrooveypenguinX";
    public override List<string>? Contributors { get; init; } = null;
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public override Range SptVersion { get; init; } = new("~4.0.2");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; } = true;
    public override string License { get; init; } = "MIT";
}

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 20)]
public class KomradeServer(
    WTTServerCommonLib.WTTServerCommonLib wttCommon,
    DatabaseService databaseService) : IOnLoad
{

    public async Task OnLoad()
    {
        
        Assembly assembly = Assembly.GetExecutingAssembly();

        var itemsDb = databaseService.GetTables().Templates.Items;

        itemsDb["66e42bd851fa456a1ee37885"] = new TemplateItem()
        {
            Id = "66e42bd851fa456a1ee37885",
            Name = "CustomUsableItem",
            Parent = "566162e44bdc2d3f298b4573",
            Type = "Node",
            Properties = new TemplateItemProperties()
        };
        itemsDb["66f16b85ed966fb78f5563d8"] = new TemplateItem()
        {
            Id = "66f16b85ed966fb78f5563d8",
            Name = "GameBoyModTemplateType",
            Parent = "566162e44bdc2d3f298b4573",
            Type = "Node",
            Properties = new TemplateItemProperties()
        };
        itemsDb["66f17b4cb59dbccbf12990e6"] = new TemplateItem()
        {
            Id = "66f17b4cb59dbccbf12990e6",
            Name = "GameboyCartridge",
            Parent = "66f16b85ed966fb78f5563d8",
            Type = "Node",
            Properties = new TemplateItemProperties()
        };
        itemsDb["6704271a4cc9e20c610eb120"] = new TemplateItem()
        {
            Id = "6704271a4cc9e20c610eb120",
            Name = "GameboyAccessory",
            Parent = "66f16b85ed966fb78f5563d8",
            Type = "Node",
            Properties = new TemplateItemProperties()
        };
        
        wttCommon.CustomSlotImageService.CreateSlotImages(assembly);
        await wttCommon.CustomItemServiceExtended.CreateCustomItems(assembly);
        await wttCommon.CustomItemServiceExtended.CreateCustomItems(assembly, Path.Join("db", "CustomCartridges"));
        await Task.CompletedTask;
    }
}
