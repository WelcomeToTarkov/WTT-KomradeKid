#if !UNITY_EDITOR
using EFT.InventoryLogic;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using EFT.UI;

public class GameBoyModTemplateType : GClass2547
{
    public string RomName;
    public string CartridgeImage;
    public string AccessoryType;
}

public class GameBoyModItemType(string id, GameBoyModTemplateType template) : LootItemClass(id, template)
{
}

public class GameBoyAccessory : GameBoyModItemType
{
    public GameBoyAccessory(string id, GameBoyModTemplateType template) : base(id, template)
    {
        AccessoryType = template.AccessoryType;
        this.Components.Add(this.Tag = new TagComponent(this));

    }

    public string AccessoryType { get; }


    public override IEnumerable<EItemInfoButton> ItemInteractionButtons
    {
        get
        {
            foreach (var eitemInfoButton in method_41())
            {
                yield return eitemInfoButton;
            }
            yield return EItemInfoButton.Install;
            yield return EItemInfoButton.Uninstall;
            if (!string.IsNullOrEmpty(this.Tag?.Name))
            {
                yield return EItemInfoButton.ResetTag;
            }
        }
    }



    public IEnumerable<EItemInfoButton> method_41()
    {
        return base.ItemInteractionButtons;
    }

    [GAttribute23]
    public readonly TagComponent Tag;

    [CanBeNull]
    public GameBoyAccessory GetCurrentAccessory()
    {
        return this;
    }

    public IEnumerable<CustomUsableItem> GetSuitableGameBoy(LootItemClass[] collections)
    {
        CustomUsableItem[] array = collections.GetAllItemsFromCollections()
                                              .Concat(collections)
                                              .OfType<CustomUsableItem>()
                                              .Distinct()
                                              .ToArray();

        List<CustomUsableItem> list = new List<CustomUsableItem>();
        foreach (CustomUsableItem gameboy in array)
        {
            if (gameboy.IsAccessorySuitable(this))
            {
                list.Add(gameboy);
            }
        }
        return list;
    }
}


public class GameBoyCartridge : GameBoyModItemType
{
    public GameBoyCartridge(string id, GameBoyModTemplateType template) : base(id, template)
    {
        RomName = template.RomName;
        CartridgeImage = template.CartridgeImage;
        this.Components.Add(this.Tag = new TagComponent(this));
    }

    public string RomName { get; }
    public string CartridgeImage { get; }

    public override IEnumerable<EItemInfoButton> ItemInteractionButtons
    {
        get
        {
            foreach (var eitemInfoButton in method_41())
            {
                yield return eitemInfoButton;
            }
            yield return EItemInfoButton.Install;
            yield return EItemInfoButton.Uninstall;
            if (!string.IsNullOrEmpty(this.Tag?.Name))
            {
                yield return EItemInfoButton.ResetTag;
            }
        }
    }

    public IEnumerable<EItemInfoButton> method_41()
    {
        return base.ItemInteractionButtons;
    }

    [GAttribute23]
    public readonly TagComponent Tag;

    [CanBeNull]
    public GameBoyCartridge GetCurrentCartridge()
    {
        return this;
    }

    public IEnumerable<CustomUsableItem> GetSuitableGameBoy(LootItemClass[] collections)
    {
        CustomUsableItem[] array = collections.GetAllItemsFromCollections()
                                              .Concat(collections)
                                              .OfType<CustomUsableItem>()
                                              .Distinct()
                                              .ToArray();

        List<CustomUsableItem> list = new List<CustomUsableItem>();
        foreach (CustomUsableItem gameboy in array)
        {
            if (gameboy.IsCartridgeSuitable(this))
            {
                list.Add(gameboy);
            }
        }
        return list;
    }
    public void ApplyStickerTexture(GameObject cartridgeObject)
    {
        if (!string.IsNullOrEmpty(CartridgeImage))
        {
            var stickerPlaneRenderer = GetStickerRenderer(cartridgeObject);

            if (stickerPlaneRenderer != null)
            {
                Texture2D stickerTexture = LoadTextureFromFile(CartridgeImage);

                if (stickerTexture != null)
                {
                    stickerPlaneRenderer.material.mainTexture = stickerTexture;
                    stickerPlaneRenderer.enabled = true; // Enable rendering once the texture is applied.
                }
            }
        }
    }

    private Renderer GetStickerRenderer(GameObject cartridgeObject)
    {
        var stickerPlaneObject = cartridgeObject.transform.Find("StickerPlane");

        if (stickerPlaneObject != null)
        {
            return stickerPlaneObject.GetComponent<Renderer>();
        }

        return null;
    }

    private Texture2D LoadTextureFromFile(string fileName)
    {

        string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string path = Path.Combine(pluginPath, "Images", fileName);

        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);

            if (texture.LoadImage(fileData))
            {
                return texture;
            }
        }
        return null;
    }

}
#endif