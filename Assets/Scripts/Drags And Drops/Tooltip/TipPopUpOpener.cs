﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipPopUpOpener : PopUpOpener
{
    public void OpenTipsPopUp(TipSectionData[] tipData, string title)
    {
        popUpOpener.OpenTooltipPopUp(tipData, title);
    }
}
