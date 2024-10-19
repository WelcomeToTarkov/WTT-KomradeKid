#if !UNITY_EDITOR
using EFT.InputSystem;
using EFT.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Comfort.Common;
using EFT;
using System.ComponentModel;
using EFT.Game.Spawning;
using EFT.InventoryLogic;
using System.Collections.Generic;

namespace GameBoyEmulator.Utils
{
#if DEBUG
    public class GameBoyEmulatorUI : MonoBehaviour
    {
        public static GameBoyEmulatorUI Instance;
        public GameObject emulatorCanvasGO;

        private static readonly string BundleDirectory = System.IO.Path.Combine(GameBoyEmulator.pluginPath + "/bundles"); 

        private AssetBundle loadedBundle;
        private bool isUIBundleLoaded = false;
        public bool showingEmulatorUI = false;


        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void InitializeUI()
        {
            if (isUIBundleLoaded)
            {
                Console.WriteLine("GameBoy Emulator UI is already loaded.");
                return;
            }

            string bundleFile = System.IO.Path.Combine(BundleDirectory, "gameboyemulator.bundle");
            loadedBundle = AssetBundle.LoadFromFile(bundleFile);

            if (loadedBundle == null)
            {
                Console.WriteLine("Failed to load AssetBundle: " + bundleFile);
                return;
            }

            GameObject canvasPrefab = loadedBundle.LoadAsset<GameObject>("GameBoyEmulatorPrefab");

            if (canvasPrefab == null)
            {
                Console.WriteLine("Failed to load GameBoy Emulator prefab from bundle: " + bundleFile);
                loadedBundle.Unload(false);
                return;
            }

            emulatorCanvasGO = GameObject.Instantiate(canvasPrefab);
            emulatorCanvasGO.name = "GameBoyEmulatorCanvas";
            emulatorCanvasGO.transform.SetParent(this.gameObject.transform, false);

            emulatorCanvasGO.SetActive(false);
            isUIBundleLoaded = true;
            loadedBundle.Unload(false);

#if DEBUG
            Console.WriteLine("Successfully created GameBoyEmulatorUI");
#endif
        }

        public void Show()
        {
            if (!isUIBundleLoaded)
            {
                InitializeUI();
            }

            if (emulatorCanvasGO == null)
            {
                Console.WriteLine("emulatorCanvasGO is null.");
                return;
            }

            emulatorCanvasGO.SetActive(true);
            showingEmulatorUI = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CursorSettings.SetCursor(ECursorType.Idle);
            UIEventSystem.Instance.SetTemporaryStatus(true);
            GamePlayerOwner.IgnoreInputWithKeepResetLook = true;
            GamePlayerOwner.IgnoreInputInNPCDialog = true;
        }
        public void Update()
        {
            if (showingEmulatorUI)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CursorSettings.SetCursor(ECursorType.Idle);
                UIEventSystem.Instance.SetTemporaryStatus(true);
                GamePlayerOwner.IgnoreInputWithKeepResetLook = true;
                GamePlayerOwner.IgnoreInputInNPCDialog = true;
            }
        }
        public void Hide()
        {
            if (emulatorCanvasGO != null)
            {
                Canvas canvas = emulatorCanvasGO.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.enabled = false;
                }

                showingEmulatorUI = false;

                UIEventSystem.Instance.SetTemporaryStatus(false);
                GamePlayerOwner.IgnoreInputWithKeepResetLook = false;
                GamePlayerOwner.IgnoreInputInNPCDialog = false;
                GameBoyEmulator.player.UpdateInteractionCast();
            }
        }

        public void DestroyUIElements()
        {
            if (emulatorCanvasGO != null)
            {
                Destroy(emulatorCanvasGO);
                emulatorCanvasGO = null;
            }

            if (loadedBundle != null)
            {
                loadedBundle.Unload(true);
                loadedBundle = null;
            }
            UIEventSystem.Instance.SetTemporaryStatus(false);
            GamePlayerOwner.IgnoreInputWithKeepResetLook = false;
            GamePlayerOwner.IgnoreInputInNPCDialog = false;
            GameBoyEmulator.player.UpdateInteractionCast();

            showingEmulatorUI = false;
            isUIBundleLoaded = false;
        }
    }
#endif
}
#endif
