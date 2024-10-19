#if !UNITY_EDITOR
using EFT.UI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFT.Communications;
using EFT.InventoryLogic;
using UnityEngine;
using EFT.AssetsManager;
using Comfort.Common;
using EFT;
using System.Collections;
using GameBoyEmulator.Utils;

namespace GameBoyEmulator.Managers
{
    public class GameBoyModItemManager : MonoBehaviour
    {
        DefaultEmulatorManager emulatorManager;
        AnimationManager animationManager;

        public async Task UnloadCartridgeAsync(CustomUsableItem gameBoy, LootItemClass[] gclass2644_0, InventoryControllerClass inventoryControllerClass, ItemUiContext itemUiContext, AnimationManager _animationManager)
        {
            GameBoyCartridge currentCartridge = gameBoy.GetCurrentCartridge();
            Slot cartridgeSlot = gameBoy.GetCartridgeSlot();
            animationManager = _animationManager;
            if (currentCartridge != null)
            {
                EquipmentClass equipment = inventoryControllerClass.Inventory.Equipment;
                bool inStorage = equipment.Contains(currentCartridge);

                if (!inStorage && gclass2644_0 == null)
                {
                    NotificationManagerClass.DisplaySingletonWarningNotification("Error: equipment panel is null while cartridge is not in storage.".Localized(null));
                }
                else
                {
                    IEnumerable<LootItemClass> targets = gclass2644_0 != null
                        ? equipment.ToEnumerable().Concat(gclass2644_0)
                        : equipment.ToEnumerable();

                    GStruct414<GInterface339> value = InteractionsHandlerClass.QuickFindAppropriatePlace(
                        currentCartridge, inventoryControllerClass, targets,
                        InteractionsHandlerClass.EMoveItemOrder.UnloadWeapon, true);

                    if (value.Succeeded)
                    {
                        bool success = (await ItemUiContext.RunWithSound(inventoryControllerClass, currentCartridge, value, null)).Succeed;
                        if (!success && inventoryControllerClass.CanThrow(currentCartridge))
                        {
                            inventoryControllerClass.ThrowItem(currentCartridge, null, null, true);
                        }
                    }
                    else
                    {
                        NotificationManagerClass.DisplayWarningNotification("Can't find a place for item".Localized(null), ENotificationDurationType.Default);
                    }
                }
            }
        }

        public async Task LoadCartridgeAsync(CustomUsableItem gameBoy, LootItemClass[] gclass2644_0, InventoryControllerClass inventoryControllerClass, ItemUiContext itemUiContext, AnimationManager _animationManager)
        {
            animationManager = _animationManager;


            if (gameBoy.GetCurrentCartridge() == null)
            {
                Slot cartridgeSlot = gameBoy.GetCartridgeSlot();
                GameBoyCartridge gameboyCartridge = FindCartridge(cartridgeSlot, gclass2644_0, inventoryControllerClass);

                if (gameboyCartridge == null)
                {
                    NotificationManagerClass.DisplayWarningNotification("Can't find any appropriate cartridge".Localized(null), ENotificationDurationType.Default);
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

        public GameBoyCartridge FindCartridge(Slot cartridgeSlot, IEnumerable<LootItemClass> gclass2644_0, InventoryControllerClass inventoryControllerClass)
        {
            return (GClass1864.InRaid
                ? inventoryControllerClass.GetReachableItemsOfType<GameBoyCartridge>(null)
                : gclass2644_0.GetTopLevelItems().OfType<GameBoyCartridge>())
                .FirstOrDefault();
        }



        public void OnCartridgeAppeared(Slot cartridgeSlot, GameBoyCartridge cartridge, bool animated)
        {
            if (cartridgeSlot == null || cartridge == null)
            {
                return;
            }

            Player player = GameBoyEmulator.player;

            InsertCartridge(Singleton<PoolManager>.Instance.CreateItem(cartridge, Player.GetVisibleToCamera(player), player, true), animated);
        }

        public void InsertCartridge(GameObject cartridgeObject, bool animated)
        {
            cartridgeObject.SetActive(false);

            if (animated)
            {
                TriggerCartridgeLoadAnimation(cartridgeObject);
            }
            else
            {
                cartridgeObject.SetActive(true);

            }

            InsertCartridgeIntoBone(cartridgeObject);
        }

        private void TriggerCartridgeLoadAnimation(GameObject cartridgeObject)
        {
            GameObject emulatorGameobject = CommonUtils.GetGameBoyEmulatorObject();

            if (emulatorGameobject != gameObject)
            {
                return;
            }

            emulatorManager = emulatorGameobject.GetComponent<DefaultEmulatorManager>();
            if (emulatorManager == null)
            {
                return;
            }

            animationManager = emulatorGameobject.GetComponent<AnimationManager>();
            if (animationManager == null)
            {
                return;
            }

            AudioSource animationAudioSource = animationManager.animationManagerAudioSource;
            if (animationAudioSource == null)
            {
                return;
            }

            GameObjectEnableTrigger enableCartridgeGameObject = new(0.45f, cartridgeObject, true);
            animationManager.AddAnimationTrigger("GameboyLoad", enableCartridgeGameObject);

            SoundTrigger blowAirSFX = new(0.75f, animationAudioSource, emulatorManager.blowAirSFX);
            animationManager.AddAnimationTrigger("GameboyLoad", blowAirSFX);


            animationManager.PlayAnimation("GameboyLoad", 3);
        }

        private void InsertCartridgeIntoBone(GameObject cartridgeObject)
        {
            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;

            Transform cartridgeBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cartridge");

            if (cartridgeBone == null)
            {
                return;
            }

            if (cartridgeBone.childCount > 0)
            {
                if (!cartridgeBone.GetChild(0).gameObject.activeSelf)
                {
                    cartridgeBone.GetChild(0).gameObject.SetActive(true);
                }
                return;
            }

            cartridgeObject.transform.SetParent(cartridgeBone, false);

            cartridgeObject.transform.localPosition = UnityEngine.Vector3.zero;

            cartridgeObject.transform.localRotation = UnityEngine.Quaternion.Euler(90.0f, 0.0f, 0.0f);

            GClass746.SetLayersRecursively(cartridgeObject, LayerMask.NameToLayer("Player"));
        }

        public void OnAccessoryAppeared(Slot accessorySlot, GameBoyAccessory accessory)
        {
            if (accessorySlot == null || accessory == null)
            {
                return;
            }

            Player player = GameBoyEmulator.player;

            string accessoryType = accessory.AccessoryType;

            InsertAccessoryIntoBone(Singleton<PoolManager>.Instance.CreateItem(accessory, Player.GetVisibleToCamera(player), player, true), accessoryType);
        }

        public void InsertAccessoryIntoBone(GameObject accessoryObject, string accessoryType)
        {
            accessoryObject.SetActive(false);

            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;

            Transform accessoryBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cable");

            if (accessoryBone == null)
            {
                return;
            }

            if (accessoryBone.childCount > 0)
            {
                var existingAccessory = accessoryBone.GetChild(0).gameObject;
                if (!existingAccessory.activeSelf)
                {
                    existingAccessory.SetActive(true);
                }
                return;
            }

            accessoryObject.transform.SetParent(accessoryBone, false);
            accessoryObject.transform.localPosition = Vector3.zero;
            accessoryObject.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            GClass746.SetLayersRecursively(accessoryObject, LayerMask.NameToLayer("Player"));

            accessoryObject.SetActive(true);

            if (emulatorManager._emulatorOn && accessoryType == "Light")
            {
                emulatorManager.ToggleLightAndCover(true);
            }
        }

        public void RemoveCartridge(bool animated)
        {
            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;
            WeaponPrefab weaponPrefab = controllerObject?.GetComponent<WeaponPrefab>();


            Transform cartridgeBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cartridge");
            if (cartridgeBone == null)
            {
                return;
            }

            if (cartridgeBone.childCount <= 0)
            {
                return;
            }

            GameObject cartridgeObject = cartridgeBone.GetChild(0).gameObject;

            GameObject emulatorGameObject = CommonUtils.GetGameBoyEmulatorObject();

            if (animated)
            {
                animationManager = emulatorGameObject.GetComponent<AnimationManager>();
                animationManager.AddAnimationTrigger("GameboyUnload", new GameObjectDestroyTrigger(1.6f, cartridgeObject));
                animationManager.PlayAnimation("GameboyUnload", 3);
            }
            else
            {
                AssetPoolObject assetPoolObject = cartridgeObject.GetComponent<AssetPoolObject>();
                if (assetPoolObject != null)
                {
                    ConsoleScreen.Log("Found asset pool component! returning to pool");
                    AssetPoolObject.ReturnToPool(cartridgeObject, true);
                }
            }
        }

        public void RemoveAccessory()
        {
            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;
            WeaponPrefab weaponPrefab = controllerObject?.GetComponent<WeaponPrefab>();

            Transform accessoryBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cable");
            if (accessoryBone == null)
            {
                return;
            }

            if (accessoryBone.childCount <= 0)
            {
                return;
            }

            GameObject accessoryObject = accessoryBone.GetChild(0).gameObject;

            AssetPoolObject assetPoolObject = accessoryObject.GetComponent<AssetPoolObject>();
            if (assetPoolObject != null)
            {
                ConsoleScreen.Log("Found asset pool component! returning to pool");
                AssetPoolObject.ReturnToPool(accessoryObject, true);
            }

        }
        public Light FindLightComponent()
        {
            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;
            WeaponPrefab weaponPrefab = controllerObject?.GetComponent<WeaponPrefab>();

            Transform accessoryBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cable");
            if (accessoryBone == null)
            {
                return null;
            }

            if (accessoryBone.childCount <= 0)
            {
                return null;
            }

            GameObject accessoryObject = accessoryBone.GetChild(0).gameObject;
            Light light = accessoryObject.GetComponentsInChildren<Light>().FirstOrDefault();
            if (light != null)
            {
                return light;
            }
            return null;
        }
        public GameObject FindLightCoverOffGameObject()
        {
            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;
            if (controllerObject == null)
            {
                return null;
            }

            Transform accessoryBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cable");
            if (accessoryBone == null || accessoryBone.childCount == 0)
            {
                return null;
            }

            GameObject accessoryObject = accessoryBone.GetChild(0).gameObject;
            if (accessoryObject == null)
            {
                return null;
            }

            Transform lightCover = accessoryObject.transform.Find("Light_Cover_Off");
            return lightCover?.gameObject;
        }
        public GameObject FindLightCoverOnGameObject()
        {
            Player player = GameBoyEmulator.player;
            GameObject controllerObject = player?.HandsController?.ControllerGameObject;
            if (controllerObject == null)
            {
                return null;
            }

            Transform accessoryBone = CommonUtils.FindChildRecursive(controllerObject.transform, "Cable");
            if (accessoryBone == null || accessoryBone.childCount == 0)
            {
                return null;
            }

            GameObject accessoryObject = accessoryBone.GetChild(0).gameObject;
            if (accessoryObject == null)
            {
                return null;
            }

            Transform lightCover = accessoryObject.transform.Find("Light_Cover_On");
            return lightCover?.gameObject;
        }
    }
}
#endif