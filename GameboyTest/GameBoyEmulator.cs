#if !UNITY_EDITOR
using SPT.Reflection.Utils;
using BepInEx;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using GameBoyEmulator.Utils;
using GameBoyEmulator.Patches;
using System.Collections.Generic;
using HarmonyLib;
using GameBoyEmulator.CustomEFTTypes;
using EFT.Hideout;
using SimpleCommandUtils.Patches;



namespace GameBoyEmulator
{
    [BepInPlugin("com.GameBoyEmulator.Core", "GameBoyEmulator", "1.0")]

    internal class GameBoyEmulator : BaseUnityPlugin
    {
        public static GameBoyEmulator instance;
        public static CommandProcessor commandProcessor;
        public static BackendConfigSettingsClass backendConfigInstance;
        public static GameWorld gameWorld;
        public static Player player;
        public static string playerNickname;
        public static PreloaderUI preloaderUI;
        public static MenuUI menuUI;
        public static CommonUI commonUI;
        public static GUISounds guiSounds;
        public static GameUI gameUI;
        public static Profile playerProfile;
        public static ISession backEndSession;
        public static TarkovApplication tarkovApplication;

        public static string sptDirectory = Environment.CurrentDirectory;
        public static string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        internal void Awake()
        {
            instance = this;
            NewTemplateIdToObjectClass.AddNewTemplateIdToObjectMapping(NewTemplateIdToObjectClass.customMappings);
            MenuSettings.Init(Config);
            new InstallModPatch().Enable();
#if DEBUG
            // these patches are when/if i want to enable gameboy load/unload from the item itself

            //new IsActivePatch().Enable();
            //new IsInteractivePatch().Enable();
            //new LoadWeaponPatch().Enable();
            //new UnLoadWeaponPatch().Enable();



            // This one is for if i want to be able to spawn with the gameboy in my hands
            
            //new ClientPlayerMethod147Patch().Enable();

#endif
            //new GameEndedPatch().Enable();
            new TranslateCommandHideoutPatch().Enable();
            new ClientUsableItemControllerPatch().Enable();
            new UsableAnimationsHandsControllerPatch().Enable();
            new HandsControllerClassPatch().Enable();
            new IsAtBindablePlacePatch().Enable();
            new IsAtReachablePlace().Enable();
            new GetWeaponAnimationTypePatch().Enable();
            new CreateItemAsyncPatch().Enable();
            new SetControllerInsteadRemovedOnePatch().Enable();
            new SetHandsUsableItemPatch().Enable();
            new TryProceedPatch().Enable();

        }
        internal void Start()
        {
            Init();
            CommonUtils.CreateSlotSprites();
        }

        internal void Update()
        {
            if (Singleton<GameWorld>.Instantiated && (gameWorld == null || gameUI == null || player == null))
            {
                gameWorld = Singleton<GameWorld>.Instance;
                gameUI = MonoBehaviourSingleton<GameUI>.Instance;
                player = Singleton<GameWorld>.Instance.MainPlayer;
                playerProfile = PatchConstants.BackEndSession.Profile;
                playerNickname = playerProfile.Nickname;
                backEndSession = PatchConstants.BackEndSession;
            }
        }

        internal void OnGUI()
        {
        }
        public void Init()
        {
            backendConfigInstance = Singleton<BackendConfigSettingsClass>.Instance;
            preloaderUI = MonoBehaviourSingleton<PreloaderUI>.Instance;
            menuUI = MonoBehaviourSingleton<MenuUI>.Instance;
            commonUI = MonoBehaviourSingleton<CommonUI>.Instance;
            guiSounds = Singleton<GUISounds>.Instance;
            tarkovApplication = Singleton<TarkovApplication>.Instance;

            if (commandProcessor == null)
            {
                commandProcessor = new CommandProcessor();
                commandProcessor.RegisterCommandProcessor();
            }
        }

    }
}
#endif