using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityGB;
using UnityEngine.UI;
using System;
using EFT;
using EFT.Visual;
using BepInEx.Configuration;
using GameBoyEmulator.Utils;
using GameBoyEmulator.Managers;


#if !UNITY_EDITOR
using EFT.UI;
#endif
internal class MenuSettings
{
    public static ConfigEntry<KeyboardShortcut> UpKey;
    public static ConfigEntry<KeyboardShortcut> DownKey;
    public static ConfigEntry<KeyboardShortcut> LeftKey;
    public static ConfigEntry<KeyboardShortcut> RightKey;
    public static ConfigEntry<KeyboardShortcut> AKey;
    public static ConfigEntry<KeyboardShortcut> BKey;
    public static ConfigEntry<KeyboardShortcut> StartKey;
    public static ConfigEntry<KeyboardShortcut> SelectKey;
    public static ConfigEntry<KeyboardShortcut> PowerKey;
    public static ConfigEntry<KeyboardShortcut> UnloadCartridgeKey;
    public static ConfigEntry<KeyboardShortcut> LoadCartridgeKey;
    public static ConfigEntry<KeyboardShortcut> UpdateKeybindings;

    public static Dictionary<KeyboardShortcut, EmulatorBase.Button> _keyMapping;
    public static void Init(ConfigFile config)
    {
        UpKey = config.Bind(
            "Keybindings",
            "Up",
            new KeyboardShortcut(KeyCode.UpArrow),
            new ConfigDescription("Key to move Up", null,
                new ConfigurationManagerAttributes { Order = 12 })
        );

        DownKey = config.Bind(
            "Keybindings",
            "Down",
            new KeyboardShortcut(KeyCode.DownArrow),
            new ConfigDescription("Key to move Down", null,
                new ConfigurationManagerAttributes { Order = 11 })
        );

        LeftKey = config.Bind(
            "Keybindings",
            "Left",
            new KeyboardShortcut(KeyCode.LeftArrow),
            new ConfigDescription("Key to move Left", null,
                new ConfigurationManagerAttributes { Order = 10 })
        );

        RightKey = config.Bind(
            "Keybindings",
            "Right",
            new KeyboardShortcut(KeyCode.RightArrow),
            new ConfigDescription("Key to move Right", null,
                new ConfigurationManagerAttributes { Order = 9 })
        );

        AKey = config.Bind(
            "Keybindings",
            "A Button",
            new KeyboardShortcut(KeyCode.Comma),
            new ConfigDescription("Key for A button", null,
                new ConfigurationManagerAttributes { Order = 8 })
        );

        BKey = config.Bind(
            "Keybindings",
            "B Button",
            new KeyboardShortcut(KeyCode.Period),
            new ConfigDescription("Key for B button", null,
                new ConfigurationManagerAttributes { Order = 7 })
        );

        StartKey = config.Bind(
            "Keybindings",
            "Start",
            new KeyboardShortcut(KeyCode.Quote),
            new ConfigDescription("Key for Start button", null,
                new ConfigurationManagerAttributes { Order = 6 })
        );

        SelectKey = config.Bind(
            "Keybindings",
            "Select",
            new KeyboardShortcut(KeyCode.Colon),
            new ConfigDescription("Key for Select button", null,
                new ConfigurationManagerAttributes { Order = 5 })
        );

        PowerKey = config.Bind(
            "Keybindings",
            "Power",
            new KeyboardShortcut(KeyCode.P),
            new ConfigDescription("Key to toggle power on/off", null,
                new ConfigurationManagerAttributes { Order = 4 })
        );

        UnloadCartridgeKey = config.Bind(
            "Keybindings",
            "Unload Cartridge",
            new KeyboardShortcut(KeyCode.U),
            new ConfigDescription("Key to unload the cartridge", null,
                new ConfigurationManagerAttributes { Order = 3 })
        );

        LoadCartridgeKey = config.Bind(
            "Keybindings",
            "Load Cartridge",
            new KeyboardShortcut(KeyCode.K),
            new ConfigDescription("Key to load a new cartridge", null,
                new ConfigurationManagerAttributes { Order = 2 })
        );

        UpdateKeybindings = config.Bind(
            "Keybindings",
            "Update Keybindings",
            new KeyboardShortcut(KeyCode.Backslash),
            new ConfigDescription("Key to refresh any kebinding changes to current GameBoy", null,
                new ConfigurationManagerAttributes { Order = 1 })
        );

    }
    public static Dictionary<KeyboardShortcut, EmulatorBase.Button> GetButtonMappings()
    { 
        return new Dictionary<KeyboardShortcut, EmulatorBase.Button>
        {
            { AKey.Value, EmulatorBase.Button.A },
            { BKey.Value, EmulatorBase.Button.B },
            { StartKey.Value, EmulatorBase.Button.Start },
            { SelectKey.Value, EmulatorBase.Button.Select }
        };
    }
    public static Dictionary<KeyboardShortcut, EmulatorBase.Button> GetDPadMappings()
    {
        return new Dictionary<KeyboardShortcut, EmulatorBase.Button>
        {
            { UpKey.Value, EmulatorBase.Button.Up },
            { DownKey.Value, EmulatorBase.Button.Down },
            { LeftKey.Value, EmulatorBase.Button.Left },
            { RightKey.Value, EmulatorBase.Button.Right },
        };
    }

    public static Dictionary<KeyboardShortcut, string> GetAnimatorKeyBindings()
    {
        return new Dictionary<KeyboardShortcut, string>
        {
            { UpKey.Value, "DPadUpPress" },
            { DownKey.Value, "DPadDownPress" },
            { LeftKey.Value, "DPadLeftPress" },
            { RightKey.Value, "DPadRightPress" },
            { AKey.Value, "APress" },
            { BKey.Value, "BPress" },
            { StartKey.Value, "StartPress" },
            { SelectKey.Value, "SelectPress" }
        };
    }

    public static Dictionary<KeyboardShortcut, string> GetPowerKeyBindings()
    {
        return new Dictionary<KeyboardShortcut, string>
        {
            { PowerKey.Value, "PowerOn" }
        };
    }

    public static Dictionary<KeyboardShortcut, string> GetCartridgeKeyBindings()
    {
        return new Dictionary<KeyboardShortcut, string>
        {
            { UnloadCartridgeKey.Value, "UnloadCartridge" },
            { LoadCartridgeKey.Value, "LoadCartridge" }
        };
    }
}

public class DefaultEmulatorManager : MonoBehaviour
{
    public ISaveMemory _saveMemory;

    public string Filename;
    public RawImage ScreenUI;
    public GameObject batteryIndicator;
    public AudioClip blowAirSFX;

    public DefaultAudioOutput defaultAudioOutput;
    public AudioSource emulatorAudioSource;

    public Emulator Emulator { get; set; }
    private bool _emulatorInitialized;
    public bool _emulatorOn;

    private Dictionary<KeyboardShortcut, EmulatorBase.Button> _buttonMapping;
    private Dictionary<KeyboardShortcut, EmulatorBase.Button> _dPadMapping;
    private Dictionary<KeyboardShortcut, string> animatorKeyBindings;
    private Dictionary<KeyboardShortcut, string> powerKeyBindings;
    private Dictionary<KeyboardShortcut, string> cartridgeKeyBindings;

#if !UNITY_EDITOR
    private CustomUsableItemController controller;
    private CustomUsableItem item;

    private LootItemClass[] gclass2644_0;
    private InventoryControllerClass inventoryControllerClass;
    private ItemUiContext itemUiContext;

    private GameBoyModItemManager modItemManager;
    AnimationManager animationManager;

    WeaponPrefab weaponPrefab;
    GameObject weaponGameObject;
    GameObject gameboyEmulatorGameObject;
    SlotManager slotManager;

    private Dictionary<EmulatorBase.Button, bool> _keyStates = new Dictionary<EmulatorBase.Button, bool>();
    private IAnimator animator;

#endif
    void Start()
    {
        InitializeKeyBindings();
        StartCoroutine(Init());
    }

    private void OnDisable()
    {
        if (Emulator != null)
        {
            Emulator.Save();
        }
    }
    private void InitializeKeyBindings()
    {
        _buttonMapping = MenuSettings.GetButtonMappings();
        _dPadMapping = MenuSettings.GetDPadMappings();
        animatorKeyBindings = MenuSettings.GetAnimatorKeyBindings();
        powerKeyBindings = MenuSettings.GetPowerKeyBindings();
        cartridgeKeyBindings = MenuSettings.GetCartridgeKeyBindings();
    }

    private IEnumerator Init()
    {
        yield return new WaitForSeconds(.2f);
        InitializeControllers();
        InitializeEmulator();
    }

    private bool IsAnimatorPlaying()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(1);

        return stateInfo.normalizedTime < 1 && stateInfo.length > 0;
    }
    /// <summary>
    /// KeyboardShortcut default behavior is awful and doesn't allow other buttons to be pressed during
    /// </summary>
    public static bool BetterIsDown(KeyboardShortcut key)
    {
        if (!Input.GetKeyDown(key.MainKey))
        {
            return false;
        }

        foreach (var modifier in key.Modifiers)
        {
            if (!Input.GetKey(modifier))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// KeyboardShortcut default behavior is awful and doesn't allow other buttons to be pressed during
    /// </summary>
    public static bool BetterIsPressed(KeyboardShortcut key)
    {
        if (!Input.GetKey(key.MainKey))
        {
            return false;
        }

        foreach (var modifier in key.Modifiers)
        {
            if (!Input.GetKey(modifier))
            {
                return false;
            }
        }

        return true;
    }
    void Update()
    {
#if UNITY_EDITOR
        HandlePowerInput();
        HandleEmulatorInputs();
#endif
        if (MenuSettings.UpdateKeybindings.Value.IsDown())
        {
            InitializeKeyBindings();
            ConsoleScreen.Log("Keybindings updated.");
        }

        if (_emulatorOn)
        {
            Emulator.RunNextStep();
        }

#if !UNITY_EDITOR
        Player player = GameBoyEmulator.GameBoyEmulator.player;
        if (player == null)
        {
            return;
        }
        GameObject currentItemInHands = player?.HandsController?.HandsHierarchy?.gameObject;
        if (weaponGameObject != null && weaponGameObject == currentItemInHands && !player.IsInventoryOpened && !MonoBehaviourSingleton<PreloaderUI>.Instance.Console.IsConsoleVisible)
        {
            if (!IsAnimatorPlaying())
            {
                HandlePowerInput();
                HandleRomLoadingInput();

                if (_emulatorOn)
                {
                    HandleDPadInputs();
                    HandleButtonInputs();
                    HandleAnimatorInputs();
                }

            }
        }
#endif
    }

    private void HandlePowerInput()
    {
        foreach (var keyBinding in powerKeyBindings)
        {
            if (BetterIsDown(keyBinding.Key))
            {
                switch (keyBinding.Value)
                {
                    case "PowerOn":
                        if (_emulatorOn)
                        {
                            TurnOffEmulator();
                        }
                        else
#if !UNITY_EDITOR
                        if (IsCartridgeLoaded())
#endif
                        {
                            TurnOnEmulator();
                        }
                        else
                        {
                            NotificationManagerClass.DisplaySingletonWarningNotification("Cannot turn on the GameBoy without a loaded ROM.".Localized(null));
                        }
                        break;
                }
            }
        }
    }

#if !UNITY_EDITOR
    private async void HandleRomLoadingInput()
    {
        foreach (var keyBinding in cartridgeKeyBindings)
        {
            if (BetterIsDown(keyBinding.Key))
            {
                switch (keyBinding.Value)
                {
                    case "LoadCartridge":
                        if (!_emulatorOn)
                            await LoadCartridgeAsync();
                        break;
                    case "UnloadCartridge":
                        if (!_emulatorOn)
                            await UnloadRomAsync();
                        break;
                }
            }
        }
    }
#endif

    private void HandleDPadInputs()
    {
        foreach (var keyMapping in _dPadMapping)
        {
            if (BetterIsPressed(keyMapping.Key))
            {
                Emulator.SetInput(keyMapping.Value, true);
            }
            else
            {
                Emulator.SetInput(keyMapping.Value, false);
            }
        }
    }
    private void HandleButtonInputs()
    {
        foreach (var keyMapping in _buttonMapping)
        {
            if (keyMapping.Key.IsPressed())
            {
                Emulator.SetInput(keyMapping.Value, true);
            }
            else
            {
                Emulator.SetInput(keyMapping.Value, false);
            }
        }
    }

#if !UNITY_EDITOR
    private void HandleAnimatorInputs()
    {
        foreach (var keyBinding in animatorKeyBindings)
        {
            if (BetterIsPressed(keyBinding.Key))
            {
                animator.SetBool(keyBinding.Value, true);
            }
            else
            {
                animator.SetBool(keyBinding.Value, false);
            }
        }
    }
#endif

    private void InitializeEmulator()
    {
        if (_emulatorInitialized)
        {
            return;
        }

        IVideoOutput drawable = new DefaultVideoOutput();
        IAudioOutput audio = defaultAudioOutput;
        if (_saveMemory == null)
        {
            ISaveMemory saveMemory = new DefaultSaveMemory();
            _saveMemory = saveMemory;

        }
        Emulator = new Emulator(drawable, audio, _saveMemory);

        ScreenUI.texture = ((DefaultVideoOutput)Emulator.Video).Texture;

        _emulatorInitialized = true;
    }

#if !UNITY_EDITOR
    public void InitializeControllers()
    {

        Player player = GameBoyEmulator.GameBoyEmulator.player;
        if (player == null)
        {
          return;
        }

        controller = player.HandsController as CustomUsableItemController;

        item = controller.Item as CustomUsableItem;

        inventoryControllerClass = player.InventoryControllerClass;


        GameObject controllerObject = player?.HandsController?.ControllerGameObject;

        weaponPrefab = controllerObject?.GetComponent<WeaponPrefab>();

        weaponGameObject = weaponPrefab?._objectInstance;

        itemUiContext = ItemUiContext.Instance;

        gclass2644_0 = itemUiContext?.GClass2644_0;

        if (controller != null && controller.FirearmsAnimator != null)
        {
            animator = controller.FirearmsAnimator.Animator;

            gameboyEmulatorGameObject = GetGameBoyEmulatorObject();

            InitializeAnimationManager();

            InitializeModItemManager();

            InitializeSlotManager();

        }
    }

    private void InitializeAnimationManager()
    {
        animationManager = gameboyEmulatorGameObject.GetComponent<AnimationManager>();

        if (animationManager == null)
        {
            animationManager = gameboyEmulatorGameObject.AddComponent<AnimationManager>();
            animationManager.Init(animator, this);
        }
        else
        {
            animationManager.Init(animator, this);
        }
    }

    private void InitializeModItemManager()
    {
        modItemManager = gameboyEmulatorGameObject.GetComponent<GameBoyModItemManager>();

        if (modItemManager == null)
        {
            modItemManager = gameboyEmulatorGameObject.AddComponent<GameBoyModItemManager>();
        }
    }

    private void InitializeSlotManager()
    {
        if (slotManager != null && slotManager.isRegistered)
        {
            return;
        }

        slotManager = gameboyEmulatorGameObject.GetComponent<SlotManager>();

        if (slotManager == null)
        {
            slotManager = gameboyEmulatorGameObject.AddComponent<SlotManager>();
        }

        slotManager.Init();
    }

    public GameObject GetGameBoyEmulatorObject()
    {
        Player player = GameBoyEmulator.GameBoyEmulator.player;
        if (player == null)
        {
            return null;
        }

        CustomUsableItemController customUsableItemController = player.HandsController as CustomUsableItemController;

        if (customUsableItemController == null)
        {
            return null;
        }
        GameObject controllerObject = customUsableItemController.ControllerGameObject;
        if (controllerObject == null)
        {
            return null;
        }

        WeaponPrefab weaponPrefab = controllerObject?.GetComponent<WeaponPrefab>();
        if (weaponPrefab == null)
        {
            return null;
        }

        Transform emulatorTransform = CommonUtils.FindDeepChild(controllerObject.transform, "gameboy_emulator");
        if (emulatorTransform == null)
        {
            return null;
        }

        GameObject emulatorGameobject = emulatorTransform.gameObject;

        return emulatorGameobject;

    }

#endif
#if !UNITY_EDITOR
    public GameBoyCartridge GetCurrentCartridge()
    {
        CustomUsableItem item = (CustomUsableItem)controller.GetItem();

        if (item == null)
        {
            return null;
        }
        return item.GetCurrentCartridge();
    }
    public GameBoyAccessory GetCurrentAccessory()
    {
        CustomUsableItem item = (CustomUsableItem)controller.GetItem();

        if (item == null)
        {
            return null;
        }
        return item.GetCurrentAccessory();
    }
    public bool IsCartridgeLoaded()
    {
        return GetCurrentCartridge() != null;
    }

    public async Task LoadCartridgeAsync()
    {
        if (!_emulatorInitialized)
        {
           InitializeEmulator();
        }

        CustomUsableItem item = controller.GetItem() as CustomUsableItem;
        if (item == null)
        {
            return;
        }

        GameBoyCartridge currentCartridge = item.GetCurrentCartridge();
        if (currentCartridge != null)
        {
            NotificationManagerClass.DisplaySingletonWarningNotification("Cartridge is already slotted into the item".Localized(null));
            return;
        }


        if (modItemManager == null)
        {
            return;
        }

        if (gclass2644_0 == null)
        {
            return;
        }

        if (inventoryControllerClass == null)
        {
            return;
        }

        if (itemUiContext == null)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }

        await modItemManager.LoadCartridgeAsync(item, gclass2644_0, inventoryControllerClass, itemUiContext, animationManager);
    }
#endif


#if !UNITY_EDITOR
    public async Task UnloadRomAsync()
    {
        if (IsCartridgeLoaded())
        {
            CustomUsableItem item = controller.Item as CustomUsableItem;
            if (item != null && item.GetCurrentCartridge != null)
            {
                await modItemManager.UnloadCartridgeAsync(item, gclass2644_0, inventoryControllerClass, itemUiContext, animationManager);
            }
        }
        else
        {
            NotificationManagerClass.DisplaySingletonWarningNotification("No cartridge present in the slot to unload".Localized(null));
        }
    }
#endif
    public void TurnOnEmulator()
    {
        if (!_emulatorInitialized)
        {
            InitializeEmulator();
        }

        CustomUsableItem item = controller.GetItem() as CustomUsableItem;
        if (item == null)
        {
            return;
        }

        GameBoyCartridge currentCartridge = GetCurrentCartridge();
        if (currentCartridge == null)
        {
            NotificationManagerClass.DisplayWarningNotification("Cannot turn on the GameBoy without a cartridge loaded.".Localized(null));
            return;
        }

        string filename = currentCartridge.RomName;

        if (filename == string.Empty || filename == null)
        {
            NotificationManagerClass.DisplayWarningNotification("No RomName defined for the current cartridge..".Localized(null));
            return;
        }
#if UNITY_EDITOR
        string filename = Filename;
#endif
        string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string path = Path.Combine(pluginPath, "Roms", filename);

        if (!File.Exists(path))
        {
            NotificationManagerClass.DisplayWarningNotification("No Rom found for the current Cartridge".Localized(null));
            return;
        }

        byte[] data = File.ReadAllBytes(path);

        if (data != null)
        {
            Emulator.LoadRom(data);
        }

#if !UNITY_EDITOR
        animationManager.PlayAnimation("GameboyPower", 4);
#endif
        StartCoroutine(WaitAndRun());

    }
    private IEnumerator WaitAndRun()
    {
        yield return new WaitForSeconds(1.5f);
        Color currentColor = ScreenUI.color;
        ScreenUI.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.09f);
        ScreenUI.texture = ((DefaultVideoOutput)Emulator.Video).Texture;
        batteryIndicator.SetActive(true);
        GameBoyAccessory accessory = GetCurrentAccessory();
        if (accessory != null && accessory.AccessoryType == "Light")
        {
            ToggleLightAndCover(true);
        }

        Emulator.Start();

        _emulatorOn = true;
    }


    public void TurnOffEmulator()
    {
        if (!_emulatorOn)
        {
            return;
        }
        _emulatorOn = false;

#if !UNITY_EDITOR
        animationManager.PlayAnimation("GameboyPower", 4);
#endif
        _emulatorInitialized = false;
        StartCoroutine(FadeOutAndTurnOff());
    }
    private IEnumerator FadeOutAndTurnOff()
    {
        yield return new WaitForSeconds(.95f);
        batteryIndicator.SetActive(false);
        Emulator.Stop();
        GameBoyAccessory accessory = GetCurrentAccessory();
        if (accessory != null && accessory.AccessoryType == "Light")
        {
            ToggleLightAndCover(false);
        }
        float fadeDuration = .25f;
        Color originalColor = ScreenUI.color;

        for (float t = 0; t <= fadeDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0.0f, t / fadeDuration);
            ScreenUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        ScreenUI.texture = null;

        ScreenUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
    public void ToggleLightAndCover(bool enable)
    {
        Light lightComponent = modItemManager.FindLightComponent();
        if (lightComponent != null)
        {
            lightComponent.enabled = enable;
        }

        GameObject lightCoverOn = modItemManager.FindLightCoverOnGameObject();
        GameObject lightCoverOff = modItemManager.FindLightCoverOffGameObject();
        if (lightCoverOn != null && lightCoverOff != null)
        {
            Renderer lightCoverOnRenderer = lightCoverOn.GetComponent<Renderer>();
            Renderer lightCoverOffRenderer = lightCoverOff.GetComponent<Renderer>();

            if (lightCoverOnRenderer != null)
                lightCoverOnRenderer.enabled = enable;

            if (lightCoverOffRenderer != null)
                lightCoverOffRenderer.enabled = !enable;
        }
    }

}
