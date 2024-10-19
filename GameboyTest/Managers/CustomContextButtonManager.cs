using Comfort.Common;
using EFT.Communications;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static GClass2438;
using GameBoyEmulator.Utils;

namespace GameBoyEmulator.Managers
{

    internal class CustomContextButtonManager
    {
        public static CustomUsableItem FindCustomUsableItem(InventoryControllerClass inventoryControllerClass, IEnumerable<LootItemClass> collections)
        {
            return (GClass1864.InRaid
                ? inventoryControllerClass.GetReachableItemsOfType<CustomUsableItem>(null)
                : collections.GetTopLevelItems().OfType<CustomUsableItem>())
                .FirstOrDefault();
        }

        public static GameBoyCartridge FindGameBoyCartridge(InventoryControllerClass inventoryControllerClass, IEnumerable<LootItemClass> collections)
        {
            return (GClass1864.InRaid
                ? inventoryControllerClass.GetReachableItemsOfType<GameBoyCartridge>(null)
                : collections.GetTopLevelItems().OfType<GameBoyCartridge>())
                .FirstOrDefault();
        }

        public static async Task InstallCartridge(ItemUiContext _itemUiContext, ItemContextAbstractClass itemContext, LootItemClass[] collections)
        {
            if (_itemUiContext == null)
            {
                return;
            }

            if (itemContext == null)
            {
                return;
            }

            if (collections == null || collections.Length == 0)
            {
                return;
            }

            InventoryControllerClass inventoryControllerClass = InventoryControllerAccessor.GetInventoryControllerClass(_itemUiContext);
            if (inventoryControllerClass == null)
            {
                return;
            }

            CustomUsableItem gameBoy = FindCustomUsableItem(inventoryControllerClass, collections);

            if (gameBoy.GetCurrentCartridge() == null)
            {
                Slot cartridgeSlot = gameBoy.GetCartridgeSlot();

                GameBoyCartridge gameboyCartridge = itemContext.Item as GameBoyCartridge;

                if (gameboyCartridge == null)
                {
                    NotificationManagerClass.DisplaySingletonWarningNotification("Can't find any appropriate cartridge".Localized(null));
                }
                else
                {
                    GClass2783 to = new(cartridgeSlot);

                    var result = await ItemUiContext.RunWithSound(
                        inventoryControllerClass,
                        gameboyCartridge,
                        InteractionsHandlerClass.Move(gameboyCartridge, to, inventoryControllerClass, true),
                        null
                    );

                    if (!result.Succeed && inventoryControllerClass.CanThrow(gameboyCartridge))
                    {
                        inventoryControllerClass.ThrowItem(gameboyCartridge, null, null, true);
                    }
                }
            }
            else
            {
                NotificationManagerClass.DisplaySingletonWarningNotification("A cartridge is already loaded in the GameBoy.".Localized(null));
            }
        }
        public static async Task InstallAccessory(ItemUiContext _itemUiContext, ItemContextAbstractClass itemContext, LootItemClass[] collections)
        {
            if (_itemUiContext == null)
            {
                return;
            }

            if (itemContext == null)
            {
                return;
            }

            if (collections == null || collections.Length == 0)
            {
                return;
            }

            InventoryControllerClass inventoryControllerClass = InventoryControllerAccessor.GetInventoryControllerClass(_itemUiContext);
            if (inventoryControllerClass == null)
            {
                return;
            }

            CustomUsableItem gameBoy = FindCustomUsableItem(inventoryControllerClass, collections);

            if (gameBoy.GetCurrentAccessory() == null)
            {
                Slot accessorySlot = gameBoy.GetAccessorySlot();

                GameBoyAccessory gameboyAccessory = itemContext.Item as GameBoyAccessory;

                if (gameboyAccessory == null)
                {
                    NotificationManagerClass.DisplaySingletonWarningNotification("Can't find any appropriate accessory".Localized(null));
                }
                else
                {
                    GClass2783 to = new(accessorySlot);

                    var result = await ItemUiContext.RunWithSound(
                        inventoryControllerClass,
                        gameboyAccessory,
                        InteractionsHandlerClass.Move(gameboyAccessory, to, inventoryControllerClass, true),
                        null
                    );

                    if (!result.Succeed && inventoryControllerClass.CanThrow(gameboyAccessory))
                    {
                        inventoryControllerClass.ThrowItem(gameboyAccessory, null, null, true);
                    }
                }
            }
            else
            {
                NotificationManagerClass.DisplaySingletonWarningNotification("Accessory is already loaded in the GameBoy.".Localized(null));
            }

        }

        public static async Task LoadCartridge(ItemUiContext _itemUiContext, CustomUsableItem gameboy, LootItemClass[] collections)
        {
            InventoryControllerClass inventoryControllerClass = InventoryControllerAccessor.GetInventoryControllerClass(_itemUiContext);
            if (inventoryControllerClass == null)
            {
                return;
            }

            if (gameboy.GetCurrentMagazine() == null)
            {
                Slot cartridgeSlot = gameboy.GetCartridgeSlot();
                GameBoyCartridge gameboyCartridge = FindGameBoyCartridge(inventoryControllerClass, collections);
                if (gameboyCartridge == null)
                {
                    NotificationManagerClass.DisplaySingletonWarningNotification("Can't find any appropriate cartridge".Localized(null));
                }
                else
                {
                    GClass2783 to = new GClass2783(cartridgeSlot);

                    var result = await ItemUiContext.RunWithSound(
                                                                    inventoryControllerClass,
                                                                    gameboyCartridge,
                                                                    InteractionsHandlerClass.Move(gameboyCartridge, to, inventoryControllerClass, true),
                                                                    null
                                                                );

                    if (!result.Succeed && inventoryControllerClass.CanThrow(gameboyCartridge))
                    {
                        inventoryControllerClass.ThrowItem(gameboyCartridge, null, null, true);
                    }
                }
            }
        }

        public static async Task UnloadCartridge(ItemUiContext _itemUiContext, CustomUsableItem gameboy, LootItemClass[] gclass2644_0)
        {
                GameBoyCartridge currentCartridge = gameboy.GetCurrentCartridge();
                if (currentCartridge != null)
                {
                InventoryControllerClass inventoryControllerClass = InventoryControllerAccessor.GetInventoryControllerClass(_itemUiContext);

                    EquipmentClass equipment = inventoryControllerClass.Inventory.Equipment;
                    bool flag;
                    if (!(flag = equipment.Contains(currentCartridge)) && gclass2644_0 == null)
                    {
                        ConsoleScreen.LogError("Something went wrong. Right panel is null while mag is not from equipment.");
                    }
                    else
                    {
                        IEnumerable<LootItemClass> enumerable;
                        if (gclass2644_0 != null)
                        {
                            enumerable = (flag ? equipment.ToEnumerable<EquipmentClass>().Concat(gclass2644_0) : gclass2644_0.Concat(equipment.ToEnumerable<EquipmentClass>()));
                        }
                        else
                        {
                            IEnumerable<LootItemClass> enumerable2 = equipment.ToEnumerable<EquipmentClass>();
                            enumerable = enumerable2;
                        }
                        IEnumerable<LootItemClass> targets = enumerable;
                        GStruct414<GInterface339> value = InteractionsHandlerClass.QuickFindAppropriatePlace(currentCartridge, inventoryControllerClass, targets, InteractionsHandlerClass.EMoveItemOrder.UnloadWeapon, true);
                        bool flag2;
                        if (flag2 = value.Succeeded)
                        {
                            flag2 = (await ItemUiContext.RunWithSound(inventoryControllerClass, currentCartridge, value, null)).Succeed;
                        }
                        if (!flag2)
                        {
                            if (!GClass1864.InRaid)
                            {
                                NotificationManagerClass.DisplaySingletonWarningNotification("Can't find a place for item".Localized(null));
                            }
                            else if (inventoryControllerClass.CanThrow(currentCartridge))
                            {
                                inventoryControllerClass.ThrowItem(currentCartridge, null, null, true);
                            }
                        }
                    }
                }
        }
    
    }
}
