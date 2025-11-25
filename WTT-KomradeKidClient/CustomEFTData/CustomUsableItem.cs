#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Comfort.Common;
using Diz.LanguageExtensions;
using EFT.InventoryLogic;
using JetBrains.Annotations;

namespace GameBoyEmulator.CustomEFTData;

public sealed class CustomUsableItem : CompoundItem
{
    private Slot _cartridgeSlotCache;

    private Slot _accessorySlotCache;

    public CustomUsableItem(string id, CompoundItemTemplateClass template) : base(id, template)
    {
        Slots = Array.ConvertAll(template.Slots, method_7);
    }
    [StructLayout(LayoutKind.Auto)]
    private struct Struct769
    {
        public TraderControllerClass itemController;
    }
    public override IEnumerable<EItemInfoButton> ItemInteractionButtons
    {
        get
        {
            foreach (var itemInfoButton in method_41())
            {
                yield return itemInfoButton;
            }
        }
    }

    private IEnumerable<EItemInfoButton> method_41()
    {
        return base.ItemInteractionButtons;
    }

    public override GStruct153 Apply([NotNull] TraderControllerClass itemController, [NotNull] Item item, int count, bool simulate)
    {
        Struct769 @struct;
        @struct.itemController = itemController;
        if (!@struct.itemController.Examined(item))
        {
            return new GClass1551(item);
        }
        if (!@struct.itemController.Examined(this))
        {
            return new GClass1551(this);
        }
        Slot cartridgeSlot = GetCartridgeSlot();
        Slot accessorySlot = GetAccessorySlot();
        GameBoyCartridge cartridge;
        GameBoyAccessory accessory;
        Error error;
        if ((cartridge = (item as GameBoyCartridge)) != null && cartridgeSlot != null)
        {
            if (!cartridgeSlot.CanAccept(cartridge))
            {
                return new Slot.GClass1579(cartridge, cartridgeSlot);
            }
            ItemAddress itemAddress = cartridgeSlot.CreateItemAddress();
            IResult result = smethod_1(itemAddress, ref @struct);
            if (result.Failed)
            {
                return new GClass1522(result.Error);
            }
            GStruct154<GClass3411> value = InteractionsHandlerClass.Move(cartridge, itemAddress, @struct.itemController, simulate);
            if (value.Succeeded)
            {
                return value;
            }
            Item containedItem = cartridgeSlot.ContainedItem;
            if (!GClass842.DisabledForNow && containedItem != null && GClass3396.CanSwap(cartridge, cartridgeSlot))
            {
                return new GStruct153((Error)null);
            }
        }
        else
        {
            GStruct153 result3 = base.Apply(@struct.itemController, item, count, simulate);
            if (result3.Succeeded)
            {
                return result3;
            }
            IResult result4 = smethod_1(cartridgeSlot?.CreateItemAddress(), ref @struct);
            if (result4.Failed)
            {
                return new GClass1522(result4.Error);
            }
        }
        if ((accessory = (item as GameBoyAccessory)) != null && accessorySlot != null)
        {
            if (!accessorySlot.CanAccept(accessory))
            {
                return new Slot.GClass1579(accessory, accessorySlot);
            }
            ItemAddress itemAddress = accessorySlot.CreateItemAddress();
            IResult result = smethod_1(itemAddress, ref @struct);
            if (result.Failed)
            {
                return new GClass1522(result.Error);
            }
            GStruct154<GClass3411> value = InteractionsHandlerClass.Move(accessory, itemAddress, @struct.itemController, simulate);
            if (value.Succeeded)
            {
                return value;
            }
            error = value.Error;
            Item containedItem = accessorySlot.ContainedItem;
            if (!GClass842.DisabledForNow && containedItem != null && GClass3396.CanSwap(accessory, accessorySlot))
            {
                return new GStruct153((Error)null);
            }
        }
        else
        {
            GStruct153 result3 = base.Apply(@struct.itemController, item, count, simulate);
            if (result3.Succeeded)
            {
                return result3;
            }
            error = result3.Error;
            IResult result4 = smethod_1(accessorySlot?.CreateItemAddress(), ref @struct);
            if (result4.Failed)
            {
                return new GClass1522(result4.Error);
            }
        }
        return error;
    }


    private static IResult smethod_1(ItemAddress slotItemAddress, ref Struct769 struct769)
    {
        InventoryController inventoryControllerClass;
        if ((inventoryControllerClass = (struct769.itemController as InventoryController)) != null && inventoryControllerClass.Inventory.Equipment.ContainerSlots.Contains(slotItemAddress.Container) && struct769.itemController.SelectEvents(null).Any())
        {
            return new FailedResult("Inventory/PlayerIsBusy");
        }
        return SuccessfulResult.New;
    }

    private bool FindCartridgeSlot(Slot x)
    {
        return x.ID.Contains("Cartridge");
    }

    private bool FindAccessorySlot(Slot x)
    {
        return x.ID.Contains("Cable");
    }

    [CanBeNull]
    public Slot GetCartridgeSlot()
    {
        if (_cartridgeSlotCache != null)
        {
            return _cartridgeSlotCache;
        }


        Slot foundSlot = Slots.FirstOrDefault(FindCartridgeSlot);

        if (foundSlot != null)
        {
            _cartridgeSlotCache = foundSlot;
        }

        return foundSlot;
    }
    [CanBeNull]
    public Slot GetAccessorySlot()
    {
        if (_accessorySlotCache != null)
        {
            return _accessorySlotCache;
        }


        Slot foundSlot = Slots.FirstOrDefault(FindAccessorySlot);

        if (foundSlot != null)
        {
            _accessorySlotCache = foundSlot;
        }

        return foundSlot;
    }

    [CanBeNull]
    public GameBoyCartridge GetCurrentCartridge()
    {
        Slot cartridgeSlot = GetCartridgeSlot();

        if (cartridgeSlot is { ContainedItem: GameBoyCartridge cartridge })
        {
            return cartridge;
        }
        else
        {
            return null;
        }
    }
    [CanBeNull]
    public GameBoyAccessory GetCurrentAccessory()
    {
        Slot accessorySlot = GetAccessorySlot();

        if (accessorySlot is { ContainedItem: GameBoyAccessory accessory })
        {
            return accessory;
        }
        else
        {
            return null;
        }
    }
    public bool CanStartCartridgeReload()
    {

        GameBoyCartridge currentCartridge = GetCurrentCartridge();
        if (currentCartridge != null && !KomradeClient.Player.InventoryController.Examined(currentCartridge))
        {
            NotificationManagerClass.DisplaySingletonWarningNotification("Attached cartridge is not examined.".Localized());
            return false;
        }
        return true;
    }
    public bool IsAccessorySuitable(GameBoyAccessory accessory)
    {
        if (accessory == null)
        {
            return false;
        }

        Slot[] suitableSlots = AllSlots.Where(FindAccessorySlot).ToArray();

        if (suitableSlots.Length == 0)
        {
            return false;
        }

        foreach (var slot in suitableSlots)
        {
            if (slot.CanAccept(accessory))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCartridgeSuitable(GameBoyCartridge cartridge)
    {
        if (cartridge == null)
        {
            return false;
        }

        Slot[] suitableSlots = AllSlots.Where(FindCartridgeSlot).ToArray();

        if (suitableSlots.Length == 0)
        {
            return false;
        }

        // Check if any slot can accept the cartridge
        foreach (var slot in suitableSlots)
        {
            if (slot.CanAccept(cartridge))
            {
                return true;
            }
        }

        return false;
    }

}
#endif