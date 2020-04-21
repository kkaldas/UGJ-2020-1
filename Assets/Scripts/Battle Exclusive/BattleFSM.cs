﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFSM : MonoBehaviour
{
    [SerializeField]
    private BattleStatesFactory firstOneToPlayStatesFactory = null;

    private BattleState oldState;

    [SerializeField]
    private DynamicFeedback feedbackText = null;

    private bool safeToUpdate = false;

    void Start()
    {
        oldState = firstOneToPlayStatesFactory.CreateGameStartState();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        yield return null;
        safeToUpdate = true;
    }

    void Update()
    {
        if (safeToUpdate)
        {
            oldState.ExecuteAction();

            BattleState newState = oldState.GetNextState();
            
            oldState = newState;

            //string newStateName = ;

            /*
            string somebodiesTurn = "Enemy's Turn";

            if (currentState.IsPlayerTurn())
            {
                somebodiesTurn = "Your Turn";
            }
            */

            feedbackText.DisplayState( newState.GetType().Name ); //+ ". It's " +somebodiesTurn
            
        }
    }
}
