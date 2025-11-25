#if !UNITY_EDITOR
using System;
using EFT;
using EFT.InventoryLogic;
using GameBoyEmulator.CustomEFTData;
using GameBoyEmulator.Utils;
using UnityEngine;

namespace GameBoyEmulator.Managers
{
    public class SlotManager : MonoBehaviour, GInterface179, IOnItemAdded, IOnItemRemoved
    {
        private Slot _cartridgeSlot;
        private Slot _accessorySlot;
        private CustomUsableItem _gameboy;
        public bool isRegistered;
        private GameBoyModItemManager _gameBoyModItemManager;
        private GameObject _emulatorGameObject;
        private CustomUsableItemController _customUsableItemController;
        private Player _player;
        private void Start()
        {
            Init();
        }

    public void Init()
    {
        #if DEBUG
        Console.WriteLine("Initializing SlotManager...");
        #endif

        _player = KomradeClient.Player;

        #if DEBUG
        Console.WriteLine($"Player object found: {_player != null}");
        #endif

        if (!_player)
        {
            #if DEBUG
            Console.WriteLine("Player object is null. Returning...");
            #endif
            return;
        }

        _emulatorGameObject = CommonUtils.GetGameBoyEmulatorObject();

        #if DEBUG
        Console.WriteLine($"Emulator GameObject retrieved: {_emulatorGameObject != null}");
        #endif

        if (!_emulatorGameObject)
        {
            #if DEBUG
            Console.WriteLine("Emulator GameObject is null. Returning...");
            #endif
            return;
        }

        if (_emulatorGameObject != gameObject)
        {
            #if DEBUG
            Console.WriteLine("Emulator GameObject does not match the current GameObject. Returning...");
            #endif
            return;
        }

        _gameBoyModItemManager = gameObject.GetComponent<GameBoyModItemManager>();

        #if DEBUG
        Console.WriteLine($"GameBoyModItemManager component found: {_gameBoyModItemManager != null}");
        #endif

        _customUsableItemController = _player.HandsController as CustomUsableItemController;

        #if DEBUG
        Console.WriteLine($"CustomUsableItemController cast success: {_customUsableItemController != null}");
        #endif

        if (!_customUsableItemController)
        {
            #if DEBUG
            Console.WriteLine("CustomUsableItemController is null. Returning...");
            #endif
            return;
        }

        _gameboy = _customUsableItemController.Item as CustomUsableItem;

        #if DEBUG
        Console.WriteLine($"CustomUsableItem retrieved: {_gameboy != null}");
        #endif

        if (_gameboy == null)
        {
            #if DEBUG
            Console.WriteLine("CustomUsableItem is null. Returning...");
            #endif
            return;
        }

        _cartridgeSlot = _gameboy.GetCartridgeSlot();

        #if DEBUG
        Console.WriteLine($"Cartridge slot initialized: {_cartridgeSlot != null}");
        #endif

        _accessorySlot = _gameboy.GetAccessorySlot();

        #if DEBUG
        Console.WriteLine($"Accessory slot initialized: {_accessorySlot != null}");
        #endif

        if (_gameboy.CurrentAddress == null)
        {
            #if DEBUG
            Console.WriteLine("GameBoy's current address is null. Returning...");
            #endif
            return;
        }

        var itemOwner = _gameboy.Parent.GetOwnerOrNull();

        #if DEBUG
        Console.WriteLine($"Item owner retrieved: {itemOwner != null}");
        Console.WriteLine($"SlotManager already registered: {isRegistered}");
        #endif

        if (itemOwner == null || isRegistered)
        {
            #if DEBUG
            Console.WriteLine("Item owner is null or SlotManager is already registered. Returning...");
            #endif
            return;
        }

        itemOwner.RegisterView(this);

        isRegistered = true;

        #if DEBUG
        Console.WriteLine("SlotManager successfully initialized and registered.");
        #endif
    }


        public void OnItemRemoved(GEventArgs3 obj)
        {
            if (obj.Status == CommandStatus.Succeed && obj.From is GClass3391)
            {
                OnItemAddedOrRemoved((Slot)obj.From.Container, false);
            }
        }

        public void OnItemAdded(GEventArgs2 eventArgs)
        {
            if (eventArgs.Status == CommandStatus.Succeed && eventArgs.To is GClass3391)
            {
                OnItemAddedOrRemoved((Slot)eventArgs.To.Container, true);
            }

        }

        public void OnItemAddedOrRemoved(Slot slot, bool isAdded)
        {
            switch (slot)
            {
                case var slotType when slotType == _cartridgeSlot:
                    if (isAdded)
                        LoadCartridge();
                    else
                        UnloadCartridge();
                    break;

                case var slotType when slotType == _accessorySlot:
                    if (isAdded)
                        ApplyAccessory();
                    else
                        RemoveAccessory();
                    break;
            }
        }


        public void Deinit()
        {
            var itemOwner = _gameboy.Parent.GetOwnerOrNull();
            if (itemOwner != null)
            {
                itemOwner.UnregisterView(this);
                isRegistered = false; 
            }
        }

        private void LoadCartridge()
        {
            GameBoyCartridge cartridge = (GameBoyCartridge)_cartridgeSlot.ContainedItem;
            if (cartridge == null || !_player) 
                return;

            bool isUsingGameBoy = _player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy) 
                return;

            bool inventoryOpened = _player.HandsController.IsInventoryOpen();
            _gameBoyModItemManager.OnCartridgeAppeared(_cartridgeSlot, cartridge, !inventoryOpened);

        }

        private void UnloadCartridge()
        {

            if (_player == null) return;

            bool isUsingGameBoy = _player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy) return;

            bool inventoryOpened = _player.HandsController.IsInventoryOpen();
            _gameBoyModItemManager.RemoveCartridge(!inventoryOpened);
        }


        private void ApplyAccessory()
        {
            GameBoyAccessory accessory = _accessorySlot.ContainedItem as GameBoyAccessory;
            if (accessory == null || !_player) 
                return;

            var isUsingGameBoy = _player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy)
            {
                return;
            }

            _gameBoyModItemManager.OnAccessoryAppeared(_accessorySlot, accessory);
        }

        private void RemoveAccessory()
        {

            if (!_player) return;

            bool isUsingGameBoy = _player.HandsController.Item is CustomUsableItem;
            if (!isUsingGameBoy) return;

            _gameBoyModItemManager.RemoveAccessory();
        }

    }
}
#endif