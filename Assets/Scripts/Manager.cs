using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Manager : MonoBehaviour
{
    public string currentLevel = "1";
    public bool levelActive = false;
    public bool paused = false;

    public GameObject levelComplete;
    public LevelGenerator levelGen;
    public Picker picker;
    public River river;
    public GameObject cameraTarget;
    public GameObject menuTarget;
    public GameObject menu;

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
        levelState.complete = true;
        foreach (GameObject child in levelState.children)
        {
            PlayerPrefs.SetString("Available " + child.name, "true");
            child.GetComponent<LevelState>().available = true;
        }
        menu.GetComponent<Menu>().UpdateColors();

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
        });
    }

    public void StartLevel()
    {
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