using FG.Common;
using FG.Common.Messages;
using FG.Common.Network;
using FGClient;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using System;
using UnityEngine;

namespace ThatOneRandom3AMProject.ServerGameStateView
{

    [Il2CppImplements(typeof(IGameStateServerActions))]
    internal class FLZ_ServerGameStateActions : Il2CppSystem.Object
    {
        public FLZ_ServerGameStateActions() : base(ClassInjector.DerivedConstructorPointer<FLZ_ServerGameStateActions>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }

        void MarkPlayerAsSuccessful(MPGNetObject playerNetObject, bool shouldDespawn)
        {
            throw new NotImplementedException();
        }

        void MarkTeamAsSuccessful(int teamId, bool shouldDespawn)
        {
            throw new NotImplementedException();
        }

        void EliminateParticipant(MPGNetObject playerNetObject)
        {
            throw new NotImplementedException();
        }

        void RequestDestroy(MPGNetObject go)
        {
            throw new NotImplementedException();
        }

        void BootstrapNetworkObject(MPGNetObjectBootstrapper bootstrapper, Il2CppSystem.Action<GameObject> fixupNewObject)
        {
            throw new NotImplementedException();
        }

        void BootstrapNetworkObject(MPGNetObjectBootstrapper bootstrapper, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 spawnScale, Il2CppSystem.Action<GameObject> fixupNewObject)
        {
            throw new NotImplementedException();
        }

        void PossessNetworkObject(MPGNetObjectPossessable possessable)
        {
            throw new NotImplementedException();
        }

        void PlayNetworkedEvent(string eventName)
        {
            throw new NotImplementedException();
        }

        void PlayNamedActionOnObject(ISceneHashedComponent component, int actionIndex)
        {
            throw new NotImplementedException();
        }

        void PlayNamedActionOnObject(MPGNetObject mpgNetObject, int actionIndex)
        {
            throw new NotImplementedException();
        }

        void AttachRopeToTargets(COMMON_Rope rope)
        {
            throw new NotImplementedException();
        }

        void AwardTeamPoints(int teamId, int amount)
        {
            var cgm = GlobalGameStateClient.Instance._gameStateMachine._currentState.Cast<StateGameInProgress>()._clientGameManager;
            cgm.UpdateTeamScore(teamId, cgm.CurrentTeamScore(teamId) + amount);
        }

        void AwardAllTeamsPoints(int amount)
        {
            throw new NotImplementedException();
        }

        void SetTeamScore(int teamId, int newScore)
        {
            var cgm = GlobalGameStateClient.Instance._gameStateMachine._currentState.Cast<StateGameInProgress>()._clientGameManager;
            cgm.UpdateTeamScore(teamId, newScore);
        }

        void TeleportNetObject(MPGNetObject netObject, Vector3 targetPosition)
        {
            throw new NotImplementedException();
        }

        void IncreasePlayingTime(float amountInSeconds)
        {
            throw new NotImplementedException();
        }

        void DecreasePlayingTime(float amountInSeconds)
        {
            throw new NotImplementedException();
        }

        void SetPlayingTimeRemaining(float newTimeRemaining)
        {
            throw new NotImplementedException();
        }

        void SetJumbotronDisplay(JumbotronDisplayNetworkData displaydata)
        {
            throw new NotImplementedException();
        }

        void StartSubRound(int subRoundIndex)
        {
            throw new NotImplementedException();
        }

        void SendClientUIAlert(UIOverlayAlertMessageNetworkData networkData)
        {
            throw new NotImplementedException();
        }
    }
}
