#if !APR_27
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

namespace FGLegacyTools.GameStateView
{
    [Il2CppImplements(typeof(IGameStateView))]
    internal class FLZ_ClientGameStateView : Il2CppSystem.Object
    {
        public FLZ_ClientGameStateView() : base(ClassInjector.DerivedConstructorPointer<FLZ_ClientGameStateView>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }
        public bool IsPlayer(GameObject potentialPlayer) => GlobalGameStateClient.Instance.GameStateView.IsPlayer(potentialPlayer);
        public MPGNetObject GetNetObjectByID(MPGNetID mpgNetID) => GlobalGameStateClient.Instance.GameStateView.GetNetObjectByID(mpgNetID);
        public bool CanCompleteObjectives(MPGNetObject otherNetObject) => GlobalGameStateClient.Instance.GameStateView.CanCompleteObjectives(otherNetObject);
        public int CurrentTeamScore(int teamId) => GlobalGameStateClient.Instance.GameStateView.CurrentTeamScore(teamId);
        public bool IsNetworkedGame => GlobalGameStateClient.Instance.GameStateView.IsNetworkedGame;
        public bool IsGameServer => true;
        public bool IsComboServer => GlobalGameStateClient.Instance.GameStateView.IsComboServer;
        public bool IsGameCountingDown => GlobalGameStateClient.Instance.GameStateView.IsGameCountingDown;
        public bool IsGamePlaying => GlobalGameStateClient.Instance.GameStateView.IsGamePlaying;
        public bool IsGameEnded => GlobalGameStateClient.Instance.GameStateView.IsGameEnded;
        public bool IsGameLevelLoaded => GlobalGameStateClient.Instance.GameStateView.IsGameLevelLoaded;
        public string GameLevelName => GlobalGameStateClient.Instance.GameStateView.GameLevelName;
        public float CountdownTimeRemaining => GlobalGameStateClient.Instance.GameStateView.CountdownTimeRemaining;
        public float GameplayTimeRemaining => GlobalGameStateClient.Instance.GameStateView.GameplayTimeRemaining;
        public bool ShouldDisplayTimeRemainingNow => GlobalGameStateClient.Instance.GameStateView.ShouldDisplayTimeRemainingNow;
        public float GameplayTimeElapsed => GlobalGameStateClient.Instance.GameStateView.GameplayTimeElapsed;
        public float RoundProportionElapsed => GlobalGameStateClient.Instance.GameStateView.RoundProportionElapsed;
        public float SimulationTime => GlobalGameStateClient.Instance.GameStateView.SimulationTime;
        public float PlayoutBufferLength => GlobalGameStateClient.Instance.GameStateView.PlayoutBufferLength;
        public float CurrentEstimatedLatency => GlobalGameStateClient.Instance.GameStateView.CurrentEstimatedLatency;
        public Canvas Canvas => GlobalGameStateClient.Instance.GameStateView.Canvas;
        public int RoundRandomSeed => GlobalGameStateClient.Instance.GameStateView.RoundRandomSeed;
    }
}
#endif