﻿using UnityEngine;
using System.Collections;

public class PreMadeAudioFactory : MonoBehaviour
{
    [SerializeField]
    private AudioRequisitor audioRequisitor = null;

    [SerializeField]
    private AudioHolder audioHolder = null;

    public PreMadeAudioRequest CreateStopAllSFXRequest(GameObject assignor)
    {
        return PreMadeAudioRequest.CreateSTOP_SFXAudioRequest(audioRequisitor, assignor);
    }

    #region Created Using AudioClips
    public PreMadeAudioRequest CreateRandomPlaceCardAudioRequest(GameObject assignor)
    {
        string[] AUDIO_NAMES = { "0 Place Card SFX", "1 Place Card SFX",
            "2 Place Card SFX", "3 Place Card SFX", "4 Place Card SFX" };
        AudioClip placeCardSFX = audioHolder.GetAleatoryClipAmong(AUDIO_NAMES);

        return CreateSFXRequestUsingTheClip(new AudioClip[1] { placeCardSFX }, assignor);
    }
    public PreMadeAudioRequest CreateCryingAudioRequest(GameObject assignor)
    {
        AudioClip crying = audioHolder.GetAudioByName("Sit And Cry");
        AudioClip crying2 = audioHolder.GetAudioByName("Sit And Cry 2");
        AudioClip crying3 = audioHolder.GetAudioByName("Sit And Cry 3");
        AudioClip crying4 = audioHolder.GetAudioByName("Sit And Cry 4");

        return CreateSFXRequestUsingTheClip(new AudioClip[4] { crying, crying2, crying3, crying4 }, assignor);
    }
    private PreMadeAudioRequest CreateSFXRequestUsingTheClip(AudioClip[] clips, GameObject assignor)
    {
        return PreMadeAudioRequest.CreateSFXSoundRequest(clips, audioRequisitor, assignor);
    }
    #endregion

    #region Created Using Name
    public PreMadeAudioRequest CreateDrinkAudioRequest(GameObject assignor)
    {
        return CreateSFXRequestUsingTheName(assignor, "Drink");
    }
    public PreMadeAudioRequest CreateFacepalmAudioRequest(GameObject assignor)
    {
        return CreateSFXRequestUsingTheName(assignor, "Facepalm");
    }
    public PreMadeAudioRequest CreateOffendAudioRequest(GameObject assignor)
    {
        return CreateSFXRequestUsingTheName(assignor, "Fuck You");
    }
    private PreMadeAudioRequest CreateSFXRequestUsingTheName(GameObject assignor, string audioName)
    {
        return PreMadeAudioRequest.CreateSFXSoundRequest
            (audioHolder.GetAudioByName(audioName), audioRequisitor, assignor);
    }
    #endregion

    #region Stop BGM
    public PreMadeAudioRequest CreateCoolAudioRequest(GameObject assignor)
    {
        return CreateSFX_AND_STOP_BGMSoundRequest(assignor, "Cool");
    }
    public PreMadeAudioRequest CreateBoringAudioRequest(GameObject assignor)
    {
        return CreateSFX_AND_STOP_BGMSoundRequest(assignor, "Bored");
    }
    public PreMadeAudioRequest CreateVictoryAudioRequest(GameObject assignor)
    {
        return CreateSFX_AND_STOP_BGMSoundRequest(assignor, "VICTORY");
    }
    public PreMadeAudioRequest CreateDefeatAudioRequest(GameObject assignor)
    {
        return CreateSFX_AND_STOP_BGMSoundRequest(assignor, "DEFEAT");
    }
    private PreMadeAudioRequest CreateSFX_AND_STOP_BGMSoundRequest(GameObject assignor, string audioName)
    {
        AudioClip defeatBGM = audioHolder.GetAudioByName(audioName);
        PreMadeAudioRequest preMadeAudioRequest =
            PreMadeAudioRequest.CreateSFX_AND_STOP_BGMSoundRequest(defeatBGM, audioRequisitor, assignor);
        return preMadeAudioRequest;
    }
    #endregion
}
