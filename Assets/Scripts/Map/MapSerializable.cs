﻿using System.Collections.Generic;
using System;

[Serializable]
public class MapSerializable
{
    private int rootIndex;
    private List<string> GONames = new List<string>();
    private List<bool> Cleareds = new List<bool>();
    private List<int> Levels = new List<int>();
    private List<int> PlayLvlBtnIndexes = new List<int>();
    private List<List<int>> AntecessorsIndexes = new List<List<int>>();

    public MapSerializable( List<SpotInfo> spotsInfo, int rootIndex )
    {
        this.rootIndex = rootIndex;
        for (int i = 0; i < spotsInfo.Count; i++)
        {
            SpotInfo spt = spotsInfo[i];
            GONames.Add(spt.GOName);
            Cleareds.Add(spt.Cleared);
            Levels.Add(spt.Level);
            PlayLvlBtnIndexes.Add(spt.PlayLvlBtnIndex);
            AntecessorsIndexes.Add(spt.AntecessorsIndexes);
        }
    }

    public List<SpotInfo> Recover(out int rootIndex)
    {
        rootIndex = this.rootIndex;
        List<SpotInfo> spotsInfo = new List<SpotInfo>();
        for (int i = 0; i < GONames.Count; i++)
        {
            spotsInfo.Add(
                new SpotInfo(
                    goName: GONames[i],
                    cleared: Cleareds[i],
                    level: Levels[i],
                    playLvlBtnIndex: PlayLvlBtnIndexes[i],
                    antecessorsIndexes: AntecessorsIndexes[i]
            ));
        }
        return spotsInfo;
    }
}
