#if !UNITY_EDITOR
using EFT.InventoryLogic;
using System.Collections.Generic;
using System.Reflection;
using System;

public class NewTemplateIdToObjectClass
{
    public static List<NewTemplateIdToObjectClass.TemplateIdToObjectType> customMappings =
[
            // Add CustomUsableItem Item
            new(
                "66e42bd851fa456a1ee37885", // Template ID
                typeof(CustomUsableItem),   // Item type
                typeof(GClass2547),         // Template type
                (id, template) => new CustomUsableItem(id, (GClass2547)template) // Constructor
            ),
            // Add GameBoyCartridge Template
            new(
                "66f16b85ed966fb78f5563d8", // Template ID
                null,   // Item type
                typeof(GameBoyModTemplateType),         // Template type
                null // Constructor
            ),
            // Add GameBoyCartridge Item
            new(
                "66f17b4cb59dbccbf12990e6", // Template ID
                typeof(GameBoyCartridge),   // Item type
                typeof(GameBoyModTemplateType),         // Template type
                (id, template) => new GameBoyCartridge(id, (GameBoyModTemplateType)template) // Constructor
            ),
                    new(
                "6704271a4cc9e20c610eb120", // Template ID
                typeof(GameBoyAccessory),   // Item type
                typeof(GameBoyModTemplateType),         // Template type
                (id, template) => new GameBoyAccessory(id, (GameBoyModTemplateType)template) // Constructor
            )
];
    public static void AddNewTemplateIdToObjectMapping(List<TemplateIdToObjectType> mappings)
    {
        Type templateIdToObjectMappingsClass = typeof(TemplateIdToObjectMappingsClass);

        foreach (var mapping in mappings)
        {
            // Add to TypeTable
            FieldInfo typeTableField = templateIdToObjectMappingsClass.GetField("TypeTable", BindingFlags.Public | BindingFlags.Static);
            if (typeTableField != null)
            {
                var typeTable = (Dictionary<string, Type>)typeTableField.GetValue(null);
                if (!typeTable.ContainsKey(mapping.TemplateId) && mapping.ItemType != null)
                {
                    typeTable.Add(mapping.TemplateId, mapping.ItemType);
                    Console.WriteLine($"Added {mapping.ItemType.Name} to TypeTable.");
                }
            }

            // Add to TemplateTypeTable
            FieldInfo templateTypeTableField = templateIdToObjectMappingsClass.GetField("TemplateTypeTable", BindingFlags.Public | BindingFlags.Static);
            if (templateTypeTableField != null)
            {
                var templateTypeTable = (Dictionary<string, Type>)templateTypeTableField.GetValue(null);
                if (!templateTypeTable.ContainsKey(mapping.TemplateId))
                {
                    templateTypeTable.Add(mapping.TemplateId, mapping.TemplateType);
                    Console.WriteLine($"Added {mapping.TemplateType.Name} to TemplateTypeTable.");
                }
            }

            // Add to ItemConstructors only if ItemType is not null
            if (mapping.ItemType != null)
            {
                FieldInfo itemConstructorsField = templateIdToObjectMappingsClass.GetField("ItemConstructors", BindingFlags.Public | BindingFlags.Static);
                if (itemConstructorsField != null)
                {
                    var itemConstructors = (Dictionary<string, Func<string, object, Item>>)itemConstructorsField.GetValue(null);
                    if (!itemConstructors.ContainsKey(mapping.TemplateId))
                    {
                        itemConstructors.Add(mapping.TemplateId, mapping.Constructor);
                        Console.WriteLine($"Added {mapping.ItemType.Name} constructor to ItemConstructors.");
                    }
                }
            }
        }
    }

    public class TemplateIdToObjectType
    {
        public string TemplateId { get; set; }
        public Type ItemType { get; set; }
        public Type TemplateType { get; set; }
        public Func<string, object, Item> Constructor { get; set; }

        public TemplateIdToObjectType(string templateId, Type itemType, Type templateType, Func<string, object, Item> constructor)
        {
            TemplateId = templateId;
            ItemType = itemType; // Can be null for templates only
            TemplateType = templateType;
            Constructor = constructor; // Can also be null for templates only
        }
    }
}
#endif