﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarnCard : IncidentAction
{
    [SerializeField]
    private AudioClip earnCardAudio = null;
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    private Card[] cards;

    public override void Execute()
    {
        cards = new RandomDeckBuilder(2).GetDeck();
        cards = DeckPrototypeFactory.ReplaceTheRandomCards(cards);

        customPopUpOpener.OpenDisplayingCards
            (
                title: "Earn a Card",
                warningMessage: "Choose a card to add to your collection, I mean, to become your friend.",
                confirmBtnMessage: "<<< The Leftmost",
                cancelBtnMessage: "The rightmost >>>",
                onConfirm: () => { AddCardClosePopUpClearSpot(0); },
                onCancel: () => { AddCardClosePopUpClearSpot(1); },
                PreMadeAudioRequest.CreateSFX_AND_STOP_BGMAudioRequest(earnCardAudio, audioRequisitor, assignor: gameObject),
                cards
            );
    }

    private void AddCardClosePopUpClearSpot(int cardIndex)
    {
        DeckPrototypeFactory.AddCardToPlayerCardsCollection(cards[cardIndex]);
        openerOfPopUpsMadeInEditor.CloseAllPopUpsExceptLoading();
        sceneOpener.OpenMapScene();
    }
}
