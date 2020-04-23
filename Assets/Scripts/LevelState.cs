using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelState : MonoBehaviour
{

    public bool complete = false;
    public bool available = false;
    public bool recentlyUpdated = false;

    public List<GameObject> children;


    void Start()
    {
        if (PlayerPrefs.GetString("Complete " + gameObject.name) != "")
        {
            complete = true;
        }

        if (PlayerPrefs.GetString("Available " + gameObject.name) != "")
        {
            available = true;
        }

    }

    public void UnlockChildren()
    {
        foreach (GameObject child in children)
        {
            PlayerPrefs.SetString("Available " + child.name, "true");
        }
    }

}
