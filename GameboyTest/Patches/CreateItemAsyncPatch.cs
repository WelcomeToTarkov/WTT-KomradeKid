#if !UNITY_EDITOR
using EFT.AssetsManager;
using EFT.CameraControl;
using EFT.InventoryLogic;
using EFT;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using SPT.Reflection.Patching;
using Comfort.Common;
using GameBoyEmulator.Utils;
using GameBoyEmulator.CustomEFTTypes;
using EFT.UI;

namespace GameBoyEmulator.Patches
{
    internal class CreateItemAsyncPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(PoolManager).GetMethod("CreateItemAsync", BindingFlags.Instance | BindingFlags.Public);
        }

        [PatchPrefix]
        public static bool PatchPrefix(PoolManager __instance, Item item, ECameraType cameraType, [CanBeNull] IPlayer player, bool isAnimated, GDelegate78 yield, ref Task<GameObject> __result, CancellationToken ct = default(CancellationToken))
        {
            if (item == null)
            {
                return true;
            }

            if (item is CustomUsableItem || item is GameBoyCartridge || item is GameBoyAccessory)
            {
                __result = CustomCreateItemAsync(__instance, item, cameraType, player, isAnimated, yield, ct);
                return false;
            }
            return true;
        }

        public static async Task<GameObject> CustomCreateItemAsync(PoolManager __instance, Item item, ECameraType cameraType, [CanBeNull] IPlayer player, bool isAnimated, GDelegate78 yield, CancellationToken ct)
        {
            PoolManager.Class1216 @class = new PoolManager.Class1216();
            PoolManager.PoolsCategory poolCategory = PoolManager.PoolsCategory.Raid;

            CancellationToken token;

            var dictionaryField = typeof(PoolManager).GetField("dictionary_2", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dictionaryField != null)
            {
                var dictionary = (Dictionary<PoolManager.PoolsCategory, CancellationToken>)dictionaryField.GetValue(__instance);
                if (dictionary.TryGetValue(PoolManager.PoolsCategory.Raid, out token))
                {
                    ct = CancellationTokenSource.CreateLinkedTokenSource(token, ct).Token;
                }
            }

            if (ct.IsCancellationRequested)
            {
                return null;
            }

            @class.itemGameObject = __instance.method_2(item.Prefab, poolCategory);
            if (@class.itemGameObject == null)
            {
                PoolManager.Logger.LogError($"Failed to create GameObject for item: {item}", Array.Empty<object>());
                return null;
            }

            var cancellationTokenRegistration = ct.Register(() => UnityEngine.Object.Destroy(@class.itemGameObject));

            if (@class.itemGameObject.scene.buildIndex != -1 && @class.itemGameObject.transform.parent == null && Application.isPlaying)
            {
                UnityEngine.Object.DontDestroyOnLoad(@class.itemGameObject);
            }
            @class.itemGameObject.transform.localScale = Vector3.one;

            await yield(null);

            if (ct.IsCancellationRequested)
            {
                return null;
            }

            AssetPoolObject component = @class.itemGameObject.GetComponent<AssetPoolObject>();

            if (component == null)
            {
                PoolManager.Logger.LogError($"No AssetPoolObject found for item: {item}", Array.Empty<object>());
                return null;
            }

            Transform weaponHierarchy = null;
            bool flag = item is CustomUsableItem;
            if (flag)
            {
                weaponHierarchy = (component as WeaponPrefab).Hierarchy.transform;
            }

            await yield(null);

            if (ct.IsCancellationRequested)
            {
                return null;
            }
            ContainerCollection collection = item as ContainerCollection;
            bool flag2 = false;
            if (collection != null)
            {
                flag2 = await __instance.method_4(collection, flag, cameraType, player, isAnimated, @class.itemGameObject, component, weaponHierarchy, ct, yield);
            }
            if (ct.IsCancellationRequested)
            {
                return null;
            }
            if (flag)
            {
                WeaponPrefab weaponPrefab = component as WeaponPrefab;
                weaponPrefab.Init(player, player != null);
                if (flag2 && player != null)
                {
                    weaponPrefab.RebindAnimator(player);
                }
            }
            else
            {
                GameBoyCartridge gameboyCartridge = item as GameBoyCartridge;
                if (gameboyCartridge != null)
                {
                    gameboyCartridge.ApplyStickerTexture(@class.itemGameObject);
                    @class.itemGameObject.name = @class.itemGameObject.name.Replace("(Clone)", string.Empty);
                }
                GameBoyAccessory gameboyAccessory = item as GameBoyAccessory;
                if (gameboyAccessory != null)
                {
                    @class.itemGameObject.name = @class.itemGameObject.name.Replace("(Clone)", string.Empty);

                }
            }

            if (ct.IsCancellationRequested)
            {
                return null;
            }

            var components = @class.itemGameObject.GetComponents<GInterface168>();
            foreach (var comp in components)
            {
                comp.Init(item, isAnimated);
            }
            Mod mod;
            if ((mod = (item as Mod)) != null && mod.IsAnimated)
            {
                @class.itemGameObject.name = @class.itemGameObject.name.Replace("(Clone)", string.Empty);
            }
            cancellationTokenRegistration.Dispose();
            return @class.itemGameObject;
        }



    }
}
#endif