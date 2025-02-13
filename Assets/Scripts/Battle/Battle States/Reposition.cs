﻿using UnityEngine;
using UnityEngine.UI;

public class Reposition : BattleState
{
    private Battlefield attackerBattlefield;
    private Battlefield opponentBattlefield;
    private UICustomBtn endRepositioningBtn;
    private GameObject youCanRepositionNowGameObject;

    private int oldIndex;
    private int currentIndex;

    private bool repositioningBtnClicked = false;

    // If player tries to attack during the reposition state, let's finish reposition and attack.
    private bool triedToAttack;
    private BattleState nextState;
    private int abfIndex;
    private int obfIndex;
    private static bool alreadyAskedTip = false;

    public Reposition(Battlefield attackerBattlefield, Battlefield opponentBattlefield, UICustomBtn endRepositioningBtn, GameObject youCanRepositionNowText, GameObject btnsBackground)
    {
        this.attackerBattlefield = attackerBattlefield;
        this.opponentBattlefield = opponentBattlefield;
        this.endRepositioningBtn = endRepositioningBtn;
        this.youCanRepositionNowGameObject = youCanRepositionNowText;

        ClearSelection();

        youCanRepositionNowText.SetActive(true);
        btnsBackground.SetActive(currentBattleStatesFactory == playerBattleStatesFactory);

        if (!alreadyAskedTip && currentBattleStatesFactory == playerBattleStatesFactory)
        {
            TipDragAndDrop.AskToUseTips();
            alreadyAskedTip = true;
        }

        endRepositioningBtn.onClicked = OnEndRepositioningBtnClicked;

        if (currentBattleStatesFactory == enemyBattleStatesFactory)
        {
            new EnemyAI().Reposition(attackerBattlefield, endRepositioningBtn);
        }
        else
        {
            endRepositioningBtn.gameObject.SetActive(true);
        }
    }

    private void OnEndRepositioningBtnClicked()
    {
        repositioningBtnClicked = true;
    }

    public override void ExecuteAction()
    {
        if (opponentBattlefield.GetSelectedIndex() != -1)
        {
            triedToAttack = true;
        }

        if (repositioningBtnClicked || triedToAttack)
        {
            endRepositioningBtn.gameObject.SetActive(false);
            return;
        }

        // Stay here until player select a card from his battlefield
        currentIndex = attackerBattlefield.GetSelectedIndex();

        if (currentIndex == -1)
        {
            ClearSelection();
            return;
        }

        // Cache the index
        if (oldIndex == -1)
        {
            oldIndex = currentIndex;
            return;
        }

        if (oldIndex != currentIndex)
        {
            Card oldSelectedCard = attackerBattlefield.GetReferenceToCardAtOrGetNull(oldIndex);
            Card currentSelectedCard = attackerBattlefield.GetReferenceToCardAtOrGetNull(currentIndex);

            if (oldSelectedCard == null && currentSelectedCard != null)
            {
                if (!currentSelectedCard.IsFreezing)
                {
                    SwapCards();
                }
            }
            else if (oldSelectedCard != null && currentSelectedCard == null)
            {
                if (!oldSelectedCard.IsFreezing)
                {
                    SwapCards();
                }
            }
            else if (oldSelectedCard != null && currentSelectedCard != null)
            {
                if (!oldSelectedCard.IsFreezing && !currentSelectedCard.IsFreezing)
                {
                    SwapCards();
                }
            }

            ClearSelection();
        }
    }

    private void SwapCards()
    {
        Card firstCard = attackerBattlefield.RemoveCardOrGetNull(oldIndex);
        Card secondCard = attackerBattlefield.RemoveCardOrGetNull(currentIndex);

        bool smooth = currentBattleStatesFactory == enemyBattleStatesFactory;
        if (firstCard != null)
        {
            PutCardInIndex(firstCard, currentIndex, smooth);
        }
        if (secondCard != null)
        {
            PutCardInIndex(secondCard, oldIndex, smooth);
        }
    }

    private void PutCardInIndex(Card card, int index, bool smooth)
    {
        if (smooth)
        {
            attackerBattlefield.PutCardInIndexWithSmoothMovement(card, index);
        }
        else
        {
            attackerBattlefield.PutCardInIndexThenTeleportToSlot(card, index);
        }
    }

    private void ClearSelection()
    {
        oldIndex = -1;
        currentIndex = -1;
        attackerBattlefield.ClearSelection();
        opponentBattlefield.ClearSelection();
    }

    public override BattleState GetNextState()
    {
        nextState = this;
        if (repositioningBtnClicked || triedToAttack)
        {
            endRepositioningBtn.onClicked = null;

            BeforeCreateAttackState();
            nextState = currentBattleStatesFactory.CreateAttackState();
            AfterCreateAttackState();
        }
        return nextState;
    }

    // By default, Attack State clear it's selections on the constructor, but if the player tried to
    // attack during the Reposition State, we want the attack to happen.
    private void BeforeCreateAttackState()
    {
        if (triedToAttack)
        {
            abfIndex = attackerBattlefield.GetSelectedIndex();
            obfIndex = opponentBattlefield.GetSelectedIndex();
        }
    }

    private void AfterCreateAttackState()
    {
        if (triedToAttack)
        {
            ((Attack)nextState).SetAttackerSelectedIndex(abfIndex);
            ((Attack)nextState).SetOpponentSelectedIndex(obfIndex);
        }

        endRepositioningBtn.ChangeText("To Attack ->");
        youCanRepositionNowGameObject.SetActive(false);
    }
}
