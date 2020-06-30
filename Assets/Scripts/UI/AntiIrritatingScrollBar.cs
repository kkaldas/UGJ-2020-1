﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntiIrritatingScrollBar : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollbar = null;

    [SerializeField]
    private int initialValue;

    private void OnEnable()
    {
        scrollbar.value = initialValue;
    }
}
