
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AutoUpdatePrefs : MonoBehaviour
{

    public enum Direction { NoAction, Clear, AllButLast, AllComplete, AllAvailable };
    public Direction mode;
    void Awake()
    {
        if (mode == Direction.Clear)
        {
            for (int i = 0; i < Int32.Parse(Manager.FINAL_LEVEL) + 1; i++)
            {
                PlayerPrefs.SetString("Complete " + i, "");
                PlayerPrefs.SetString("Available " + i, "");
            }
        }
        else if (mode == Direction.AllButLast)
        {
            for (int i = 0; i < Int32.Parse(Manager.FINAL_LEVEL); i++)
            {
                PlayerPrefs.SetString("Complete " + i, "true");
                PlayerPrefs.SetString("Available " + i, "true");
            }
            PlayerPrefs.SetString("Complete " + Manager.FINAL_LEVEL, "");
            PlayerPrefs.SetString("Available " + Manager.FINAL_LEVEL, "true");
        }
    }
}
