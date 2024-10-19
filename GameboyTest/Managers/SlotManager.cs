using System;
using System.Collections;
using System.Linq;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using GameBoyEmulator.Utils;
using UnityEngine;

namespace GameBoyEmulator.Managers
{
    public class SlotManager : MonoBehaviour, GInterface116, IOnItemAdded, IOnItemRemoved
    {
        private Slot cartridgeSlot;
        private Slot accessorySlot;
        private CustomUsableItem gameboy;
        private GameBoyModItemManager modItemManager;

        public bool isRegistered = false;

        public void Init()
        {
            InitModItemManager();
            GameObject emulatorGameObject = CommonUtils.GetGameBoyEmulatorObject();
            if (emulatorGameObject == null)
            {
                return;
            }
            if (emulatorGameObject != gameObject)
            {
                return;
            }

            if (isRegistered) return;

            Player player = GameBoyEmulator.player;
            if (player == null)
            {
                return;
            }

            CustomUsableItemController customUsableItemController = player.HandsController as CustomUsableItemController;
            if (customUsableItemController == null)
            {
                return;
            }

            gameboy = customUsableItemController.Item as CustomUsableItem;

            if (gameboy == null)
            {
                return;
            }

            cartridgeSlot = gameboy.GetCartridgeSlot();
            accessorySlot = gameboy.GetAccessorySlot();


            if (gameboy.CurrentAddress == null) return;

            var itemOwner = gameboy.Parent.GetOwnerOrNull();
            if (itemOwner == null || isRegistered) return;

            itemOwner.RegisterView(this);
            isRegistered = true;
        }


        public void InitModItemManager()
        {
            modItemManager = gameObject.GetComponent<GameBoyModItemManager>();
        }

        public void OnItemRemoved(GEventArgs3 obj)
        {
            if (obj.Status == CommandStatus.Succeed && obj.From is GClass2783)
            {
                OnItemAddedOrRemoved((Slot)obj.From.Container, false);
            }
        }

        public void OnItemAdded(GEventArgs2 eventArgs)
        {
            if (eventArgs.Status == CommandStatus.Succeed && eventArgs.To is GClass2783)
            {
                OnItemAddedOrRemoved((Slot)eventArgs.To.Container, true);
            }

        }

        public void OnItemAddedOrRemoved(Slot slot, bool isAdded)
        {
            switch (slot)
            {
                case var slotType when slotType == cartridgeSlot:
                    if (isAdded)
                        LoadCartridge();
                    else
                        UnloadCartridge();
                    break;

                case var slotType when slotType == accessorySlot:
                    if (isAdded)
                        ApplyAccessory();
                    else
                        RemoveAccessory();
                    break;

                default:
                    // Optionally handle other cases if needed
                    break;
            }
        }


        public void Deinit()
        {
            var itemOwner = gameboy.Parent.GetOwnerOrNull();
            if (itemOwner != null)
            {
                itemOwner.UnregisterView(this);
                isRegistered = false; 
            }
        }

        private void LoadCartridge()
        {
            Player player = GameBoyEmulator.player;
            GameBoyCartridge cartridge = (GameBoyCartridge)cartridgeSlot.ContainedItem;
            if (cartridge == null || player == null) 
                return;

            bool isUsingGameBoy = player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy) 
                return;

            bool inventoryOpened = player.HandsController.IsInventoryOpen();
            modItemManager.OnCartridgeAppeared(cartridgeSlot, cartridge, !inventoryOpened);

        }

        private void UnloadCartridge()
        {

            Player player = GameBoyEmulator.player;
            if (player == null) return;

            bool isUsingGameBoy = player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy) return;

            bool inventoryOpened = player.HandsController.IsInventoryOpen();
            modItemManager.RemoveCartridge(!inventoryOpened);
        }


        private void ApplyAccessory()
        {
            Player player = GameBoyEmulator.player;
            GameBoyAccessory accessory = accessorySlot.ContainedItem as GameBoyAccessory;
            if (accessory == null || player == null) 
                return;

            bool isUsingGameBoy = player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy)
            {
                return;
            }

            modItemManager.OnAccessoryAppeared(accessorySlot, accessory);
        }

        private void RemoveAccessory()
        {

            Player player = GameBoyEmulator.player;
            if (player == null) return;

            bool isUsingGameBoy = player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy) return;

            bool inventoryOpened = player.HandsController.IsInventoryOpen();
            modItemManager.RemoveAccessory();
        }

    }
}
