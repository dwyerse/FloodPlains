using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Manager : MonoBehaviour
{
    public string currentLevel = "1";

    public GameObject levelComplete;
    public LevelGenerator levelGen;
    public Picker picker;
    public River river;
    public GameObject cameraTarget;
    public GameObject menuTarget;
    public GameObject menu;

    public bool allowEscape = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && allowEscape)
        {
            PausedFinished();
            allowEscape = false;
        }
    }

    public void PausedFinished()
    {
        picker.Clear();
        river.Clear();
        LeanTween.move(cameraTarget, menuTarget.transform.position, 1.5f).setEaseInOutCubic().setOnComplete(() =>
                {
                    menu.GetComponent<Menu>().enabled = true;
                    menu.GetComponent<Menu>().UpdateColors();
                });
    }

    public void levelFinished()
    {
        picker.Clear();
        river.Clear();
        levelComplete.GetComponent<CanvasGroup>().alpha = 0;
        levelComplete.SetActive(true);
        LeanTween.alphaCanvas(levelComplete.GetComponent<CanvasGroup>(), 1, 2f).setEaseInOutCubic().setOnComplete(returnToMenu);

        PlayerPrefs.SetString("Complete " + currentLevel, "true");

        GameObject level = GameObject.Find(currentLevel);
        LevelState levelState = level.GetComponent<LevelState>();
        if (levelState.complete != true)
        {
            levelState.complete = true;
            levelState.recentlyUpdated = true;
            foreach (GameObject child in levelState.children)
            {
                PlayerPrefs.SetString("Available " + child.name, "true");
                child.GetComponent<LevelState>().available = true;
                child.GetComponent<LevelState>().recentlyUpdated = true;
            }
        }

    }

    public void returnToMenu()
    {
        LeanTween.alphaCanvas(levelComplete.GetComponent<CanvasGroup>(), 0, 1f).setOnComplete(() =>
        {
            levelComplete.SetActive(false);
        });
        LeanTween.move(cameraTarget, menuTarget.transform.position, 1.5f).setEaseInOutCubic().setOnComplete(() =>
        {
            menu.GetComponent<Menu>().enabled = true;
            menu.GetComponent<Menu>().UpdateColors();
        });
    }

    public void StartLevel()
    {
        allowEscape = true;
        picker.Init();
        river.Init();
    }

    public void PrepareLevel(string level)
    {
        currentLevel = level;
        picker.Clear();
        river.Clear();
        levelGen.GenerateLevel(currentLevel);
    }

}