using BepInEx;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Events;
using FallGuys.Player.Protocol.Client.Cosmetics;
using FG.Common;
#if !APR_27
using FG.Common.CatapultServices;
using FG.Common.Character;

#endif
using FG.Common.CMS;
using FGClient;
using FGClient.CatapultServices;
#if APR_27
using FGCommon.CatapultServices;
#endif
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using Mediatonic.Tools.ParsingUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if !APR_27
using FGLegacyTools.Cosmetics;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using static COMMON_PrefabSpawner;
using static FG.Common.GameSession;

namespace FGLegacyTools.HarmonyPathces
{
    public static partial class HarmonyPatches
    {
        static Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> ParsedCMS;
        static FallGuysCMSData TheNether;

        //classes that should think they're running on the server side
        static readonly List<string> IsGameServerList = 
        [
            "COMMON_Pendulum",
            "COMMON_Bumper",
            "COMMON_MovingPlatform",
            "COMMON_ScrollingSceneRandomiser",
            "COMMON_PlayerEliminationVolume",
            "COMMON_FakeDoorRandomiser",
            "WallGuysSegmentGenerator",
            "CheckpointZoneRadius",
            "CheckpointManager",
            "ObjectInAreaTrigger",
            "ScoreAwarder",
            "ScoreValue",
#if !APR_27 //causes crash on that build
            "COMMON_ObjectiveReachEndZone",
            "COMMON_ObjectiveBase",
#endif
            "COMMON_TeleportTarget",
            "COMMON_TeamQualificationObject",
            "COMMON_Bumper",
            "COMMON_PendulumBumper",
            "COMMON_PrefabSpawner",
            "COMMON_RoundProgressValueScaler",
            "COMMON_GrabToQualify",
            "TeamQualificationObjectsScoreTracker",
            "UITrackedObjectiveTransform",
            "RolloutManager"

        ];

        [HarmonyPatch(typeof(CMSLoader), nameof(CMSLoader.Awake)), HarmonyPostfix]
        static void Awake(CMSLoader __instance)
        {
            Plugin.Log.LogInfo($"Starting CMS Parsing ");

#if !APR_27
            var data = MiniJSON.Json.Parser.Parse(Encoding.UTF8.GetString(FLZ_Extensions.GetEmbeddedRes("AddotionalContent.cms_data_FULL")));
#else
            var data = MiniJSON.Json.Parser.Parse(Encoding.UTF8.GetString(FLZ_Extensions.GetEmbeddedRes("AddotionalContent.cms_data_APR_27")));
#endif

            Plugin.Log.LogInfo($"CMS Loaded - Success={data != null}");

            if (data != null)
            {
                ParsedCMS = data.Cast<Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object>>();

#if !APR_27
                TheNether = new FallGuysCMSData(ParsedCMS, CMSLoader.Instance._logger, CMSLoader.Instance._assert, null);
#else
                TheNether = new FallGuysCMSData(ParsedCMS, CMSLoader._logger, CMSLoader._assert, null);
#endif
            }

            if (TheNether != null)
                Plugin.Log.LogInfo($"CMS Parsed - Success={TheNether != null && TheNether.Rounds != null}");
            else
                Plugin.Log.LogInfo($"CMS Parsed - Still null");

        }

#if APR_27
        [HarmonyPatch(typeof(CatapultServicesManager), nameof(CatapultServicesManager.HandleGaveUpTryingToReconnect)), HarmonyPrefix]
        static bool HandleGaveUpTryingToReconnect(CatapultServicesManager __instance)
#else
        [HarmonyPatch(typeof(CatapultServicesManager), nameof(CatapultServicesManager.HandleGaveUpTryingToReconnect)), HarmonyPrefix]
        static bool HandleGaveUpTryingToReconnect(CatapultServicesManager __instance, Il2CppSystem.Exception error)
#endif
        {
            CMSLoader.Instance.CMSData = GetCMS();
            return false;
        }

        [HarmonyPatch(typeof(MainMenuViewModel), nameof(MainMenuViewModel.Connect)), HarmonyPrefix]
        static bool Connect(MainMenuViewModel __instance)
        {
            Utility.Instance.BootGame(null);
            return false;
        }

        [HarmonyPatch(typeof(GameSession), nameof(GameSession.SetSessionState)), HarmonyPostfix]
        static void SetSessionState(GameSession __instance, SessionState newState)
        {
            switch (newState)
            {
                case SessionState.Results:
#if APR_27
                    Utility.Instance.RequestRandomRound();
#else
                    if (!Utility.Instance.Won)
                        Utility.Instance.RequestRandomRound();
                    else
                        GlobalGameStateClient.Instance.SwitchToVictoryScreen(GlobalGameStateClient.Instance.CustomisationSelections, System.Environment.UserName, true);
                 
#endif
                    break;
            }
        }

        [HarmonyPatch(typeof(COMMON_PlayerEliminationVolume), nameof(COMMON_PlayerEliminationVolume.ConsiderEliminating)), HarmonyPrefix]
        static bool ConsiderEliminating(COMMON_PlayerEliminationVolume __instance, GameObject other)
        {
            Utility.Instance.DoElimination();
            return false;
        }

        [HarmonyPatch(typeof(FGBehaviour), nameof(FGBehaviour.GameState), MethodType.Getter), HarmonyPostfix]
        static void GameState(FGBehaviour __instance, ref IGameStateView __result)
        {
            if (IsGameServerList.Contains(__instance.GetIl2CppType().Name) && Utility.Instance != null && Utility.Instance.ServerGameStateView != null)
                __result = Utility.Instance.ServerGameStateView;
        }


        [HarmonyPatch(typeof(CatapultServicesManager), nameof(CatapultServicesManager.BeginLogin)), HarmonyPostfix]
        static void BeginLogin(CatapultServicesManager __instance, Il2CppSystem.Action onSucceeded, Il2CppSystem.Action<Il2CppSystem.Exception> onFailed, ServicesEnvironment environment, Il2CppSystem.Action onContentUpdateBegin = null)
        {
            CMSLoader.Instance.CMSData = GetCMS();

#if !APR_27
            CatapultServices.Instance.PlayerCosmeticsService.Cast<PlayerCosmeticsService>().CosmeticsCollection = CosmeticsExtensions.GetCosmetics();
#endif

            try { onSucceeded.Invoke(); } catch { }

            if (Utility.Instance != null)
                return;

            var player = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            player.AddComponent<Utility>();
        }

        [HarmonyPatch(typeof(SplashScreenViewModel), nameof(SplashScreenViewModel.BeginLogin)), HarmonyPostfix]
        static void BeginLogin(SplashScreenViewModel __instance)
        {
            CMSLoader.Instance.CMSData = GetCMS();

            __instance.OnLoginSucceeded();
            __instance._mainMenuManager.OnSplashScreenComplete();

            if (Utility.Instance != null)
                return;

            var player = new GameObject
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            player.AddComponent<Utility>();
        }

        [HarmonyPatch(typeof(StateGameLoading), nameof(StateGameLoading.OnIntroCamerasComplete)), HarmonyPrefix]
        static bool OnIntroCamerasComplete(StateGameLoading __instance)
        {
            try
            {
                Broadcaster.Instance.Broadcast(new GameStateEvents.IntroCameraSequenceEndedEvent());
                CGMDespatcher.process(new GameMessageServerStartGame()
                {
                    CountdownRemaining = 5,
                    GameTimeRemaining = Utility.Instance.ActiveRound.Duration + 5,
                    InitialTeamScores = new(),
                    NumTeams = Utility.Instance.ActiveRound.TeamCount,
                    TeamAssignments = new(),
#if !APR_27
                    InitialPlayerCount = 1,
#endif
                });
            }
            catch
            {
                Utility.Leave("Something went wrong...\nloading this round again may fix the issue");
            }
            return false;
        }

        [HarmonyPatch(typeof(CarryObject), nameof(CarryObject.Start)), HarmonyPrefix]
        static bool Start(CarryObject __instance)
        {
            var netObj = __instance.gameObject.AddComponent<MPGNetObject>();
            netObj.netID_ = GlobalGameStateClient.Instance.NetObjectManager.getNextNetID();
            netObj.spawnObjectType_ = EnumSpawnObjectType.POSSESS;
            netObj.CreateTransform();
            return true;
        }

        [HarmonyPatch(typeof(COMMON_Rotator), nameof(COMMON_Rotator.FixedUpdate)), HarmonyPrefix]
        static bool FixedUpdate(COMMON_Rotator __instance)
        {
            return GlobalGameStateClient.Instance.GameStateView.IsGamePlaying;
        }

#if !APR_27
        [HarmonyPatch(typeof(COMMON_GrabToQualify), nameof(COMMON_GrabToQualify.Start)), HarmonyPrefix]
        static bool Start(COMMON_GrabToQualify __instance)
        {
            var netObj = __instance.gameObject.AddComponent<MPGNetObject>();
            netObj.netID_ = GlobalGameStateClient.Instance.NetObjectManager.getNextNetID();
            netObj.spawnObjectType_ = EnumSpawnObjectType.POSSESS;
            netObj.CreateTransform();
            return true;
        }

        [HarmonyPatch(typeof(MotorFunctionGrab), nameof(MotorFunctionGrab.IsValidTarget)), HarmonyPostfix]
        static void Start(MotorFunctionGrab __instance, GrabTarget grabTarget, ref bool __result)
        {
            __result = grabTarget != null && grabTarget.IsValid;

            if (grabTarget != null && grabTarget.TargetGameObject != null && grabTarget.TargetGameObject.TryGetComponent<COMMON_GrabToQualify>(out var targ))
                targ.OnGrabbed(__instance.MotorAgent.Character.NetObject);
        }
#endif


        [HarmonyPatch(typeof(ClientGameManager), nameof(ClientGameManager.SetReady)), HarmonyPostfix]
        static void SetReady(ClientGameManager __instance, PlayerReadinessState readinessState)
        {
            try
            {
                switch (readinessState)
                {
                    case PlayerReadinessState.LevelLoaded:

                        foreach (var doorset in Resources.FindObjectsOfTypeAll<COMMON_FakeDoorRandomiser>())
                        {
                            doorset.InitializeServerSideData();
                            doorset.CreateBreakableDoors();
                        }

                        foreach (var pathSet in Resources.FindObjectsOfTypeAll<COMMON_GridPathRandomiser>())
                        {
                            pathSet?.GeneratePath();
                            pathSet?.CreatePathFromData();
                        }

                        __instance.GameRules.PreparePlayerStartingPositions();

                        var team = Utility.Instance.GetTeamForPlayer();
                        var pos = __instance.GameRules.PickRespawnPosition(team);
                        var msg = new GameMessageServerSpawnObject()
                        {
                            _netObjectSpawnData = new()
                            {
                                _creationMode = NetObjectSpawnData.EnumCreationMode.Spawn,
                                _hash = -491682846,
                                _extrapolationPolicy = MPGNetObject.TransformExtrapolationPolicy.ApplyLocalPhysics,
                                _netID = GlobalGameStateClient.Instance.NetObjectManager.getNextNetID(),
                                _playoutBufferModifier = 0,
                                _position = pos.transform.position,
                                _rotation = pos.transform.rotation,
                                _spawnObjectType = EnumSpawnObjectType.PLAYER,
                                _scale = Vector3.one,
#if APR_27
                            _spawnData = NetworkAware_Player_NoEditor.NetworkAwarePlayer.encodeSpawnData(GlobalGameStateClient.Instance.GetLocalClientNetworkID(), 0, System.Environment.UserName, false, GlobalGameStateClient.Instance.CustomisationSelections),
#else
                                _spawnData = NetworkAware_Player_NoEditor.NetworkAwarePlayer.encodeSpawnData(GlobalGameStateClient.Instance.GetLocalClientNetworkID(), 0, System.Environment.UserName, team, false, GlobalGameStateClient.Instance._playerProfile.CustomisationSelections),
#endif
                            }
                        };
                        CGMDespatcher.process(msg);

                        __instance._eventInstanceLoadingMusic.value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        Utility.Instance.StartIntro();
                        break;
                    case PlayerReadinessState.ReadyToPlay:
                        var blocks = Resources.FindObjectsOfTypeAll<COMMON_ScrollingSceneRandomiser>().ToList().Find(x => x.gameObject.scene.name == SceneManager.GetActiveScene().name);
                        if (blocks != null)
                        {
                            blocks.enabled = true;
#if APR_27
                        blocks?.HandleServerInitialSetup();
#endif
                            blocks?.HandleSetupFromData();
                        }

                        foreach (var netObj in Resources.FindObjectsOfTypeAll<MPGNetObject>())
                            netObj.IsRemotelyControlledObject = false;

                        Utility.HandleState(Utility.State.Playing);
                        break;
                }
            }
            catch
            {
                Utility.Leave("Something went wrong...");
            }
        }

#if !APR_27
        [HarmonyPatch(typeof(MotorFunctionMantleStateGrab), nameof(MotorFunctionMantleStateGrab.Begin)), HarmonyPostfix]
        static void StartHang(MotorFunctionMantleStateGrab __instance, int prevState)
        {
            var mantleController = __instance.Character.MotorAgent.GetMotorFunction<MotorFunctionMantle>();
            Utility.Instance.ContinueMantle(mantleController.GetState<MotorFunctionMantleStateClimbUp>().ID);
        }

        [HarmonyPatch(typeof(StateGameLoading), nameof(StateGameLoading.StartIntroCameras)), HarmonyPrefix]
        static bool StartIntroCameras(StateGameLoading __instance)
        {
            __instance._clientGameManager._eventInstanceLoadingMusic.value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            return true;
        }
#endif
        [HarmonyPatch(typeof(COMMON_PrefabSpawner), nameof(COMMON_PrefabSpawner.Spawn)), HarmonyPrefix]
        static bool Spawn(COMMON_PrefabSpawner __instance)
        {
            var entry = __instance.GetRandomValidSpawnEntry();
            if (entry == null)
                return false;

#if APR_27
            var res = GameObject.Instantiate(entry.value, entry.value.transform.position, entry.value.transform.rotation);
            __instance.OnInstantiateObject(res);
#else
            var res = GameObject.Instantiate(entry.value, __instance.ChooseSpawnPosition(), entry.value.transform.rotation);
            __instance.OnInstantiateObject(res, entry);
#endif
            return false;
        }

        internal static FallGuysCMSData GetCMS()
        {
            var poop = CMSLoader.Instance;

            if (poop.State != CMSLoaderState.Ready)
            {
                poop.State = CMSLoaderState.Ready;
                poop._roundsSO.Init(TheNether.Rounds);
#if !APR_27
                poop._episodesSO.Init(TheNether.Episodes);
                poop._localisedStrings.Init(TheNether.LocalisedStrings);
#endif
            }

            return TheNether;
        }
    }
}
