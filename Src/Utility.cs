using Events;
using FG.Common;
using FG.Common.Character;
using FG.Common.CMS;
using FGClient;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ThatOneRandom3AMProject.GameStateView;
using ThatOneRandom3AMProject.HarmonyPathces;
using ThatOneRandom3AMProject.ServerGameStateView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using static FG.Common.COMMON_ObjectiveBase;
using static MenuInputHandler;

namespace ThatOneRandom3AMProject
{
    internal class Utility : MonoBehaviour
    {
        internal static Utility Instance;
        string RoundToPlay = "round_";
        StateGameLoading GameLoading;
        internal IGameStateView ServerGameStateView;
        internal Round ActiveRound;
        internal FallGuysCharacterController LocalPlayer;
        bool UsingFreeFly;
        bool UIVisible = true;
        internal bool Won;
        void Awake()
        {
            if (Instance != null)
                Destroy(Instance);

            Instance = this;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                UIVisible = !UIVisible;
            }

            if (LocalPlayer == null || !GlobalGameStateClient.Instance.GameStateView.IsGamePlaying)
                return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                var pos = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager().GameRules.PickRespawnPosition(-1);
                LocalPlayer.transform.SetPositionAndRotation(pos.transform.position, pos.transform.rotation);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                UsingFreeFly = !UsingFreeFly;
                LocalPlayer.RigidBody.isKinematic = UsingFreeFly;
            }

            FreeFlyController();
        }

        static void PushString(string key, string value)
        {
            if (CMSLoader.Instance._localisedStrings._localisedStrings.ContainsKey(key))
            {
                CMSLoader.Instance._localisedStrings._localisedStrings[key] = value;
                return;
            }

            CMSLoader.Instance._localisedStrings._localisedStrings.Add(key, value);
        }

        static FallGuysCMSData GetOrSetCMS()
        {
            var cms = HarmonyPatches.GetCMS();
            if (CMSLoader.Instance.CMSData == null)
                CMSLoader.Instance.CMSData = cms;

            return cms;
        }

        internal void SetPlayer(MPGNetObject player) => LocalPlayer = player.GetComponent<FallGuysCharacterController>();

        public void FreeFlyController()
        {
            if (!UsingFreeFly)
                return;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out RaycastHit hit))
              LocalPlayer.transform.rotation = Quaternion.Euler(0f, Quaternion.LookRotation(LocalPlayer.transform.position - hit.point, Vector3.up).eulerAngles.y, 0f);

            if (Input.GetKey(KeyCode.W))
                LocalPlayer.transform.Translate(Vector3.forward * 60 * Time.deltaTime);

            if (Input.GetKey(KeyCode.S))
                LocalPlayer.transform.Translate(Vector3.back * 60 * Time.deltaTime);

            if (Input.GetKey(KeyCode.A))
                LocalPlayer.transform.Translate(Vector3.left * 60 * Time.deltaTime);

            if (Input.GetKey(KeyCode.D))
                LocalPlayer.transform.Translate(Vector3.right * 60 * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftControl))
                LocalPlayer.transform.Translate(Vector3.down * 40 * Time.deltaTime);

            if (Input.GetKey(KeyCode.Space))
                LocalPlayer.transform.Translate(Vector3.up * 40 * Time.deltaTime);

        }

        internal void BootGame(string play)
        {
            
            if (string.IsNullOrEmpty(play))
                play = RoundToPlay;

            play = play.ToLower();

            var cms = GetOrSetCMS();

            PushString("generic_cms_not_loaded_title", "CMS NOT LOADED");
            PushString("generic_cms_not_loaded_desc", "CMS Failed to load, try restarting the game.");
            PushString("generic_round_not_found_title", "ROUND NOT FOUND");
            PushString("generic_round_not_found_desc", $"Round \"{play}\" you tried to load not found.");

            if (CMSLoader.Instance.CMSData == null)
            {
                Leave();

                Broadcaster.Instance.Broadcast(new ShowModalMessageEvent()
                {
                    Title = "generic_cms_not_loaded_title",
                    Message = "generic_cms_not_loaded_desc",
                    ModalType = UIModalMessage.ModalType.MT_OK,
                });
                return;
            }

            if (!cms.Rounds.ContainsKey(play))
            {
                Leave();

                Broadcaster.Instance.Broadcast(new ShowModalMessageEvent()
                {
                    Title = "generic_round_not_found_title",
                    Message = "generic_round_not_found_desc",
                    ModalType = UIModalMessage.ModalType.MT_OK,
                });
                return;
            }

#if !APR_27
            Broadcaster.Instance.Broadcast(new OnDisplayLobby());
#endif

            if (!ClassInjector.IsTypeRegisteredInIl2Cpp<FLZ_ClientGameStateView>())
                ClassInjector.RegisterTypeInIl2Cpp<FLZ_ClientGameStateView>();

            if (!ClassInjector.IsTypeRegisteredInIl2Cpp<FLZ_ServerGameStateActions>())
                ClassInjector.RegisterTypeInIl2Cpp<FLZ_ServerGameStateActions>();

            UsingFreeFly = false;
            Won = false;
            ServerGameStateView = new FLZ_ClientGameStateView().Cast<IGameStateView>();

            var actions = new FLZ_ServerGameStateActions();
            ServerGameStateActions.Instance = new IGameStateServerActions(actions.Pointer);

            var r = cms.Rounds[play];
            NetworkGameData.SetGameOptionsFromRoundData(r);

            GameLoading = new StateGameLoading(GlobalGameStateClient.Instance._gameStateMachine, GlobalGameStateClient.Instance.CreateClientGameStateData(), GamePermission.Player, false, false);
            GlobalGameStateClient.Instance._gameStateMachine.ReplaceCurrentState(GameLoading.Cast<GameStateMachine.IGameState>());
            
            ActiveRound = r;
            COMMON_ObjectiveReachEndZone.m_OnObjectiveSatisfied_SERVERONLY = null;
            COMMON_ObjectiveReachEndZone.m_OnObjectiveSatisfied_SERVERONLY += DelegateSupport.ConvertDelegate<HandleObjectiveSatisfied>(DoQualification);
        }

        internal int GetTeamForPlayer()
        {
            if (ActiveRound == null || ActiveRound.TeamCount == 0)
                return -1;

            return UnityEngine.Random.RandomRange(0, ActiveRound.TeamCount - 1);
        }

        void DoQualification(MPGNetID playerObjectNetID, COMMON_ObjectiveBase pObjective)
        {
            if (!GlobalGameStateClient.Instance.GameStateView.IsGamePlaying)
                return;

            if (pObjective.GetIl2CppType() == Il2CppType.Of<COMMON_GrabToQualify>())
                Won = true;

            CGMDespatcher.process(new GameMessageServerPlayerProgress()
            {
                isFinal = true,
                progressCause = GameMessageServerPlayerProgress.ProgressCause.Individual,
                succeeded = true,
#if APR_27
                playerNetObjectID = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager()._myPlayerNetID,
#else
                playerId = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager()._myPlayerNetID.m_NetworkID,
#endif
            });

            CGMDespatcher.process(new GameMessageServerEndRound()
            {
                episodeProgress = EpisodeProgressStatus.Complete,
                progressState = PlayerProgressState.Succeeded,
                shouldReconnect = false,
            });
        }

        internal void DoElimination()
        {
            if (!GlobalGameStateClient.Instance.GameStateView.IsGamePlaying)
                return;

            //for some reason it insta kills you on slime climb
            if (SceneManager.GetActiveScene().name.ToLower().Contains("lava"))
                return;

            CGMDespatcher.process(new GameMessageServerPlayerProgress()
            {
                isFinal = true,
                progressCause = GameMessageServerPlayerProgress.ProgressCause.Individual,
                succeeded = false,
#if APR_27
                playerNetObjectID = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager()._myPlayerNetID,
#else
                playerId = GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager()._myPlayerNetID.m_NetworkID,
#endif
            });

            CGMDespatcher.process(new GameMessageServerEndRound()
            {
                episodeProgress = EpisodeProgressStatus.Complete,
                progressState = PlayerProgressState.Playing,
                shouldReconnect = false,
            });
        }

        static void Leave()
        {
            GlobalGameStateClient.Instance._gameStateMachine.ReplaceCurrentState(new StateReloading(GlobalGameStateClient.Instance._gameStateMachine, false, GlobalGameStateClient.Instance.CreateClientGameStateData()).Cast<GameStateMachine.IGameState>());
        }

        internal void RequestRandomRound()
        {
            var cms = GetOrSetCMS();
            if (cms.Rounds?.Count == 0)
            {
                PushString("generic_no_rounds_title", "NO ROUNDS WERE FOUND");
                PushString("generic_no_rounds_desc", "So there nothing to play on...");
                Broadcaster.Instance.Broadcast(new ShowModalMessageEvent()
                {
                    Title = "generic_no_rounds_title",
                    Message = "generic_no_rounds_desc",
                    ModalType = UIModalMessage.ModalType.MT_OK,
                });
                return;
            }

            var i = UnityEngine.Random.Range(0, cms.Rounds.Count);
            foreach (var kv in cms.Rounds)
                if (i-- == 0) { BootGame(kv.key); break; }
        }


        void OnGUI()
        {
            if (!UIVisible)
                return;

            var isBuildKnown = Definitions.KnownBuilds.Contains(Application.version);

            var bottomContent = new StringBuilder();
            bottomContent.AppendLine($"{Plugin.BuildDetails} #{Plugin.BuildDetails.GetCommit(6)}");
            if (!isBuildKnown)
                bottomContent.AppendLine("<color=red>UNKNOWN BUILD!</color>");

            var bottomSize = GUI.skin.label.CalcSize(new(bottomContent.ToString()));
            var boxWidth = GUI.skin.box.CalcSize(new(DisplayName));
            var actualWidth = boxWidth.x < 102 ? 102 : boxWidth.x;

            GUI.Box(new(0, 0, actualWidth, 140 + bottomSize.y - 16), "");
            GUI.Box(new(0, 0, actualWidth, 140 + bottomSize.y - 15), "");
            GUI.Box(new(0, 0, actualWidth, 140 + bottomSize.y - 15), DisplayName);

            RoundToPlay = GUI.TextField(new(5, 20, actualWidth - 10, 20), RoundToPlay);

            if (GUI.Button(new(5, 40, actualWidth - 10, 20), "Play"))
                BootGame(RoundToPlay);

            if (GUI.Button(new(5, 60, actualWidth - 10, 20), "Random"))
                RequestRandomRound();

            if (GUI.Button(new(5, 80, actualWidth - 10, 20), "Round List"))
            {
                var cms = GetOrSetCMS();
                var scenes = Enumerable.Range(0, SceneManager.sceneCountInBuildSettings).Select(i => System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i))).ToList();
                var knownScenes = new List<string>();

                foreach (var round in cms.Rounds)
                {
                    if (scenes.Contains(round.value.SceneName))
                    {
                        Plugin.Log.LogInfo($"{round.key} - {round.value.SceneName}");
                        knownScenes.Add(round.value.SceneName);
                    }
                    else
                        Plugin.Log.LogWarning($"{round.key} - {round.value.SceneName} (MISSING SCENE!)");
                }

                var missing = scenes.Where(x => !knownScenes.Contains(x)).ToList();
                Plugin.Log.LogWarning($"{missing.Count} non playable scenes found\n{string.Join(", ", missing)}");
            }

            if (GUI.Button(new(5, 100, actualWidth - 10, 20), "Leave"))
                Leave();

            if (GUI.Button(new(5, 120, actualWidth - 10, 20), "About/Usage"))
            {
                PushString("generic_about", "ABOUT");

                var builder = new StringBuilder();
                if (!isBuildKnown)
                    builder.AppendLine("<color=red>THIS BUILD IS NOT OFFICIALLY SUPPORTED BY THE MOD\nTHINGS MAY NOT WORK AS INTENDED</color>");
                
                builder.AppendLine("HOTKEYS");
                builder.AppendLine($"{KeyCode.F2} - Toggle UI");
                builder.AppendLine($"{KeyCode.F5} - Toggle Free Fly");
                builder.AppendLine($"{KeyCode.R} - Respawn");
                builder.AppendLine("(Round List prints all rounds in BepInEx console!)");
                builder.AppendLine("");
                builder.AppendLine($"Made by Floyzi");
                builder.AppendLine($"{Plugin.BuildDetails} - #{Plugin.BuildDetails.GetCommit(12)}");
                builder.AppendLine($"Compiled at: {Plugin.BuildDetails.BuildDate}");
                builder.AppendLine($"Build for: {Plugin.BuildDetails.Config} running on {Application.version}");

                PushString("generic_about_desc", $"{builder}");

                Broadcaster.Instance.Broadcast(new ShowModalMessageEvent()
                {
                    Title = "generic_about",
                    Message = "generic_about_desc",
                    ModalType = UIModalMessage.ModalType.MT_OK,
                });
            }

            GUI.Label(new(5, 140, actualWidth, bottomSize.y), $"{bottomContent}", new(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
        }

        internal void StartIntro() => GameLoading.StartIntroCameras();

        //ass
        internal IEnumerator ContinueMantle(int newId)
        {
            yield return new WaitForSeconds(0.25f);
            LocalPlayer.MotorAgent.GetMotorFunction<MotorFunctionMantle>().SetState(newId);
        }
    }
}
