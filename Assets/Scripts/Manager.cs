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

    public GameObject winGame;

    public bool allowEscape = false;

    public string FINAL_LEVEL = "23";

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



        allowEscape = false;
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
                print("asdf");

                LevelState childLevelState = child.GetComponent<LevelState>();
                if (childLevelState.parents.Count > 0)
                {
                    print("HERE");
                    bool all = true;
                    foreach (GameObject parent in childLevelState.parents)
                    {
                        if (!
                        parent.GetComponent<LevelState>().complete)
                        {
                            all = false;
                        }

                    }
                    if (all)
                    {
                        PlayerPrefs.SetString("Available " + child.name, "true");
                        childLevelState.available = true;
                        childLevelState.recentlyUpdated = true;
                    }
                }
                else
                {
                    PlayerPrefs.SetString("Available " + child.name, "true");
                    childLevelState.available = true;
                    childLevelState.recentlyUpdated = true;
                }
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

            if (currentLevel.Equals(FINAL_LEVEL))
            {
                winGame.GetComponent<CanvasGroup>().alpha = 0;
                winGame.SetActive(true);
                LeanTween.alphaCanvas(winGame.GetComponent<CanvasGroup>(), 1, 2f).setEaseInOutCubic().setOnComplete(() =>
                {
                    LeanTween.alphaCanvas(winGame.GetComponent<CanvasGroup>(), 0, 2f).setDelay(5).setEaseInOutCubic().setOnComplete(() =>
                        {
                            winGame.SetActive(false);
                        });

                });
            }
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