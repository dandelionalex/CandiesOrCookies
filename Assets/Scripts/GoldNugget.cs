using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using PickMaster.DI.Signals;
using PickMaster.Logic;
using PickMaster.Enums;

namespace PickMaster.Game.View
{
    public class GoldNugget : MonoBehaviour
    {

        private SignalBus signalBus;
        private GameLogic gameLogic;


        private void Init(GameLogic gameLogic, SignalBus signalBus)
        {
            this.signalBus = signalBus;
            this.gameLogic = gameLogic;
        }

        private void OnEnable()
        {
            signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
        }
        private void OnDisable()
        {
            signalBus.Unsubscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
        }

        private void OnGameStateChangedSignal(GameStateChangedSignal signal)
        {
            if (signal.GameState == GameState.Gold)
            {
                print("me, golden brick, is on the road");
            }
        }
    }

}
