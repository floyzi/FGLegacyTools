#if APR_27
using FG.Common;
using FGClient;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ThatOneRandom3AMProject.GameStateView
{
    [Il2CppImplements(typeof(IGameStateView))]
    internal class FLZ_ClientGameStateView : Il2CppSystem.Object
    {
        public FLZ_ClientGameStateView() : base(ClassInjector.DerivedConstructorPointer<FLZ_ClientGameStateView>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }
        public bool IsGameServer => true;
        public Il2CppSystem.WeakReference<ClientGameManager> _CGM => GlobalGameStateClient.Instance.GameStateView._CGM;
        public Canvas Canvas => GlobalGameStateClient.Instance.GameStateView.Canvas;
        public float CountdownTimeRemaining => GlobalGameStateClient.Instance.GameStateView.CountdownTimeRemaining;
        public float CurrentEstimatedLatency => GlobalGameStateClient.Instance.GameStateView.CurrentEstimatedLatency;
        public string CurrentGameLevelName => GlobalGameStateClient.Instance.GameStateView.CurrentGameLevelName;
        public string GameLevelName => GlobalGameStateClient.Instance.GameStateView.GameLevelName;
        public float GameplayTimeElapsed => GlobalGameStateClient.Instance.GameStateView.GameplayTimeElapsed;
        public float GameplayTimeRemaining => GlobalGameStateClient.Instance.GameStateView.GameplayTimeRemaining;
        public bool IsComboServer => GlobalGameStateClient.Instance.GameStateView.IsComboServer;
        public bool IsGameCountingDown => GlobalGameStateClient.Instance.GameStateView.IsGameCountingDown;
        public bool IsGameEnded => GlobalGameStateClient.Instance.GameStateView.IsGameEnded;
        public bool IsGameLevelLoaded => GlobalGameStateClient.Instance.GameStateView.IsGameLevelLoaded;
        public bool IsGamePlaying => GlobalGameStateClient.Instance.GameStateView.IsGamePlaying;
        public bool IsNetworkedGame => GlobalGameStateClient.Instance.GameStateView.IsNetworkedGame;
        public float PlayoutBufferLength => GlobalGameStateClient.Instance.GameStateView.PlayoutBufferLength;
        public float RoundProportionElapsed => GlobalGameStateClient.Instance.GameStateView.RoundProportionElapsed;
        public bool ShouldDisplayTimeRemainingNow => GlobalGameStateClient.Instance.GameStateView.ShouldDisplayTimeRemainingNow;
        public float SimulationTime => GlobalGameStateClient.Instance.GameStateView.SimulationTime;
        public void CanCompleteObjectivies(MPGNetObject otherNetObject) => GlobalGameStateClient.Instance.GameStateView.CanCompleteObjectives(otherNetObject);
        public void ClearClientGameManager() => GlobalGameStateClient.Instance.GameStateView.ClearClientGameManager();
        public void CurrentTeamScore(int teamId) => GlobalGameStateClient.Instance.GameStateView.CurrentTeamScore(teamId);
        public ClientGameManager GetLiveClientGameManager() => GlobalGameStateClient.Instance.GameStateView.GetLiveClientGameManager();
        public MPGNetObject GetNetObjectByID(MPGNetID mpgNetID) => GlobalGameStateClient.Instance.GameStateView.GetNetObjectByID(mpgNetID);
        public bool IsPlayer(GameObject potentialPlayer) => GlobalGameStateClient.Instance.GameStateView.IsPlayer(potentialPlayer);
        public void SetClientGameManager(ClientGameManager cgm) => GlobalGameStateClient.Instance.GameStateView.SetClientGameManager(cgm);
    }
}
#endif