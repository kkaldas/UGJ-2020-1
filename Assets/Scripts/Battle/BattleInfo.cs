﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleInfo
{
    protected static Classes masterClass;
    protected static Color backgroundColor;
    protected static AudioClip bgm;

    public static void PrepareSimpleBattle()
    {
        masterClass = Classes.NOT_A_CLASS;
        backgroundColor = Color.gray;
    }

    public static void PrepareToughBattle()
    {
        masterClass = Classes.NOT_A_CLASS;
        backgroundColor = Color.red;// new Color(0xEE, 0x91, 0x91);
    }

    public static void PrepareBossBattle(AudioClip bgmParam)
    {
        masterClass = Classes.NOT_A_CLASS;
        backgroundColor = Color.black;
        bgm = bgmParam;
    }

    public static void PrepareMasterBattle(Classes vmasterClass, AudioClip bgmParam)
    {
        masterClass = vmasterClass;
        backgroundColor = ClassInfo.GetColorOfClass(vmasterClass);
        bgm = bgmParam;
    }

    public static bool IsMasterBattle()
    {
        return masterClass != Classes.NOT_A_CLASS;
    }
}
