using Comfort.Common;
using Diz.LanguageExtensions;
using EFT.InventoryLogic;
using EFT.UI;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class CustomUsableItem : LootItemClass
{
    private Slot _cartridgeSlotCache;

    private Slot _accessorySlotCache;

    public CustomUsableItem(string id, GClass2547 template) : base(id, template)
    {
        this.Slots = Array.ConvertAll<Slot, Slot>(template.Slots, new Converter<Slot, Slot>(this.method_7));
    }
    [StructLayout(LayoutKind.Auto)]
    public struct Struct769
    {
        public TraderControllerClass itemController;
    }
    public override IEnumerable<EItemInfoButton> ItemInteractionButtons
    {
        get
        {
            foreach (var eitemInfoButton in method_41())
            {
                yield return eitemInfoButton;
            }
        }
    }

    public IEnumerable<EItemInfoButton> method_41()
    {
        return base.ItemInteractionButtons;
    }

    public override GStruct413 Apply([NotNull] TraderControllerClass itemController, [NotNull] Item item, int count, bool simulate)
    {
        Struct769 @struct;
        @struct.itemController = itemController;
        if (!@struct.itemController.Examined(item))
        {
            return new GClass3318(item);
        }
        if (!@struct.itemController.Examined(this))
        {
            return new GClass3318(this);
        }
        Slot cartridgeSlot = this.GetCartridgeSlot();
        Slot accessorySlot = this.GetAccessorySlot();
        GameBoyCartridge cartridge;
        GameBoyAccessory accessory;
        Error error;
        if ((cartridge = (item as GameBoyCartridge)) != null && cartridgeSlot != null)
        {
            if (!cartridgeSlot.CanAccept(cartridge))
            {
                return new Slot.GClass3340(cartridge, cartridgeSlot);
            }
            GClass2783 gclass = new(cartridgeSlot);
            IResult result = smethod_1(gclass, ref @struct);
            if (result.Failed)
            {
                return new GClass3370(result.Error);
            }
            GStruct414<GClass2802> value = InteractionsHandlerClass.Move(cartridge, gclass, @struct.itemController, simulate);
            if (value.Succeeded)
            {
                return value;
            }
            error = value.Error;
            Item containedItem = cartridgeSlot.ContainedItem;
            if (!GClass748.DisabledForNow && containedItem != null && GClass2791.CanSwap(cartridge, cartridgeSlot))
            {
                return new GStruct413((Error)null);
            }
        }
        else
        {
            GStruct413 result3 = base.Apply(@struct.itemController, item, count, simulate);
            if (result3.Succeeded)
            {
                return result3;
            }
            error = result3.Error;
            IResult result4 = smethod_1(new GClass2783(cartridgeSlot), ref @struct);
            if (result4.Failed)
            {
                return new GClass3370(result4.Error);
            }
        }
        if ((accessory = (item as GameBoyAccessory)) != null && accessorySlot != null)
        {
            if (!accessorySlot.CanAccept(accessory))
            {
                return new Slot.GClass3340(accessory, accessorySlot);
            }
            GClass2783 gclass = new(accessorySlot);
            IResult result = smethod_1(gclass, ref @struct);
            if (result.Failed)
            {
                return new GClass3370(result.Error);
            }
            GStruct414<GClass2802> value = InteractionsHandlerClass.Move(accessory, gclass, @struct.itemController, simulate);
            if (value.Succeeded)
            {
                return value;
            }
            error = value.Error;
            Item containedItem = accessorySlot.ContainedItem;
            if (!GClass748.DisabledForNow && containedItem != null && GClass2791.CanSwap(accessory, accessorySlot))
            {
                return new GStruct413((Error)null);
            }
        }
        else
        {
            GStruct413 result3 = base.Apply(@struct.itemController, item, count, simulate);
            if (result3.Succeeded)
            {
                return result3;
            }
            error = result3.Error;
            IResult result4 = smethod_1(new GClass2783(accessorySlot), ref @struct);
            if (result4.Failed)
            {
                return new GClass3370(result4.Error);
            }
        }
        return error;
    }


    public static IResult smethod_1(GClass2783 slotItemAddress, ref Struct769 struct769_0)
    {
        InventoryControllerClass inventoryControllerClass;
        if ((inventoryControllerClass = (struct769_0.itemController as InventoryControllerClass)) != null && inventoryControllerClass.Inventory.Equipment.ContainerSlots.Contains(slotItemAddress.Slot) && struct769_0.itemController.SelectEvents(null).Any<GEventArgs1>())
        {
            return new FailedResult("Inventory/PlayerIsBusy", 0);
        }
        return SuccessfulResult.New;
    }
    public bool FindCartridgeSlot(Slot x)
    {
        return x.ID.Contains("Cartridge");
    }
    public bool FindAccessorySlot(Slot x)
    {
        return x.ID.Contains("Cable");
    }

    [CanBeNull]
    public Slot GetCartridgeSlot()
    {
        if (this._cartridgeSlotCache != null)
        {
            return this._cartridgeSlotCache;
        }


        Slot foundSlot = this.Slots.FirstOrDefault(new Func<Slot, bool>(FindCartridgeSlot));

        if (foundSlot != null)
        {
            this._cartridgeSlotCache = foundSlot;
        }

        return foundSlot;
    }
    [CanBeNull]
    public Slot GetAccessorySlot()
    {
        if (this._accessorySlotCache != null)
        {
            return this._accessorySlotCache;
        }


        Slot foundSlot = this.Slots.FirstOrDefault(new Func<Slot, bool>(FindAccessorySlot));

        if (foundSlot != null)
        {
            this._accessorySlotCache = foundSlot;
        }

        return foundSlot;
    }

    [CanBeNull]
    public GameBoyCartridge GetCurrentCartridge()
    {
        Slot cartridgeSlot = this.GetCartridgeSlot();

        if (cartridgeSlot != null && cartridgeSlot.ContainedItem is GameBoyCartridge cartridge)
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
        Slot accessorySlot = this.GetAccessorySlot();

        if (accessorySlot != null && accessorySlot.ContainedItem is GameBoyAccessory accessory)
        {
            return accessory;
        }
        else
        {
            return null;
        }
    }
    public virtual bool CanStartCartridgeReload()
    {

        GameBoyCartridge currentCartridge = GetCurrentCartridge();
        if (currentCartridge != null && !GameBoyEmulator.GameBoyEmulator.player.InventoryControllerClass.Examined(currentCartridge))
        {
            NotificationManagerClass.DisplaySingletonWarningNotification("Attached cartridge is not examined.".Localized(null));
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

        if (this.AllSlots == null)
        {
            return false;
        }


        Slot[] suitableSlots = this.AllSlots.Where(new Func<Slot, bool>(FindAccessorySlot)).ToArray();

        if (suitableSlots.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < suitableSlots.Length; i++)
        {
            if (suitableSlots[i].CanAccept(accessory))
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

        if (this.AllSlots == null)
        {
            return false;
        }

        Slot[] suitableSlots = this.AllSlots.Where(new Func<Slot, bool>(FindCartridgeSlot)).ToArray();

        if (suitableSlots.Length == 0)
        {
            return false;
        }

        // Check if any slot can accept the cartridge
        for (int i = 0; i < suitableSlots.Length; i++)
        {
            if (suitableSlots[i].CanAccept(cartridge))
            {
                return true;
            }
        }

        return false;
    }

}

