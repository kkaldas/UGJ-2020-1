﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleStatesFactory : OpenersSuperclass
{
    [SerializeField]
    private BattleStatesFactory otherBattleStatesFactory = null;

    [SerializeField]
    private bool isThePlayersFactory = false;

    [SerializeField]
    private Deck deck = null;

    [SerializeField]
    private Hand hand = null;

    [SerializeField]
    private Battlefield battlefield = null;

    [SerializeField]
    private UICustomBtn endRepositioningBtn = null;

    [SerializeField]
    private UICustomBtn repositionAgainBtn = null;

    [SerializeField]
    private UICustomBtn endTurnBtn = null;

    [SerializeField]
    private AudioHolder audioHolder = null;

    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private GameObject sceneCanvasGameObject = null;

    public BattleState CreateGameStartState()
    {
        if (isThePlayersFactory)
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: this, enemyStatesFactory: otherBattleStatesFactory, audioRequisitor);
        }
        else
        {
            return new GameStart(firstToPlayStatesFactory: this, playerStatesFactory: otherBattleStatesFactory, enemyStatesFactory: this, audioRequisitor);
        }
    }

    public BattleState CreateDrawCardState()
    {
        return new DrawCard(deck, battlefield, hand);
    }

    public BattleState CreatePlaceCardState()
    {
        string[] AUDIO_NAMES = { "0 Place Card SFX", "1 Place Card SFX", 
            "2 Place Card SFX", "3 Place Card SFX", "4 Place Card SFX" };
        AudioClip placeCardSFX = audioHolder.GetAleatoryClipAmong(AUDIO_NAMES);

        PreMadeSoundRequest placeCardSoundRequest = 
            PreMadeSoundRequest.CreateSFXSoundRequest(placeCardSFX, audioRequisitor, assignor: gameObject);

        return new PlaceCard(hand, battlefield, deck, placeCardSoundRequest);
    }

    public BattleState CreateBonusRepositionState()
    {
        string[] AUDIO_NAMES = { "0 Place Card SFX", "1 Place Card SFX",
            "2 Place Card SFX", "3 Place Card SFX", "4 Place Card SFX" };
        AudioClip placeCardSFX = audioHolder.GetAleatoryClipAmong(AUDIO_NAMES);

        PreMadeSoundRequest placeCardSoundRequest =
            PreMadeSoundRequest.CreateSFXSoundRequest(placeCardSFX, audioRequisitor, assignor: gameObject);

        return new BonusReposition(hand, battlefield, deck, placeCardSoundRequest, otherBattleStatesFactory.hand, otherBattleStatesFactory.deck);
    }

    public BattleState CreateRepositionState()
    {
        return new Reposition(battlefield, otherBattleStatesFactory.battlefield, endRepositioningBtn);
    }

    public BattleState CreateAttackState()
    {
        return new Attack(battlefield, otherBattleStatesFactory.battlefield, endTurnBtn, repositionAgainBtn, customPopUpOpener);
    }

    public BattleState CreateEndTurnState()
    {
        return new EndTurn(battlefield, deck, hand);
    }

    public BattleState CreateIsGameTiedState()
    {
        return new IsGameTied(customPopUpOpener, whateverBF: battlefield, theOtherBF: otherBattleStatesFactory.battlefield);
    }

    public BattleState CreateBeginTurnState()
    {
        return new BeginTurn(battlefield, deck, hand);
    }

    public BattleState CreateEndGameState(BattleStatesFactory winnerFactory)
    {
        
        AudioClip victoryBGM = audioHolder.GetAudioByName("Victory");
        AudioClip defeatBGM = audioHolder.GetAudioByName("Defeat");

        PreMadeSoundRequest victoryAudioRequest =
            PreMadeSoundRequest.CreateSFX_AND_STOP_BGMSoundRequest(victoryBGM, audioRequisitor, assignor: gameObject);

        PreMadeSoundRequest defeatAudioRequest =
            PreMadeSoundRequest.CreateSFX_AND_STOP_BGMSoundRequest(defeatBGM, audioRequisitor, assignor: gameObject);

        PreMadeSoundRequest stopAllSFXRequest =
            PreMadeSoundRequest.CreateSTOP_SFXSoundRequest(audioRequisitor, assignor: gameObject);

        return new EndGame(winnerFactory, sceneCanvasGameObject, openerOfPopUpsMadeInEditor, customPopUpOpener, sceneOpener, victoryAudioRequest, defeatAudioRequest, stopAllSFXRequest);
    }
}
