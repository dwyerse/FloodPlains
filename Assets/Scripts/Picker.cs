using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Picker : MonoBehaviour
{
    Plane _groundPlane = new Plane(Vector3.up, Vector3.zero);
    public River river;
    public Manager manager;
    GameObject currentMode = null;
    public GameObject[] indicatorPrefabs;
    float INDICATOR_HEIGHT = 0.02f;


    Map<Vector3, GameObject> indicators = new Map<Vector3, GameObject>();
    Dictionary<GameObject, Vector3> originalIndicatorPositions = new Dictionary<GameObject, Vector3>();

    LevelGenerator level;

    public Color32 invalidColor = new Color32(0x46, 0x46, 0x46, 0xFF);

    public void Clear()
    {
        foreach (KeyValuePair<GameObject, Vector3> indicator in originalIndicatorPositions)
        {
            Destroy(indicator.Key);
        }
        indicators.Clear();
        originalIndicatorPositions.Clear();
    }
    public void Init()
    {
        level = manager.levelGen;
        for (int i = 0; i < indicatorPrefabs.Length; i++)
        {
            string[] split = level.level.Split('-');
            int pipeNum = level.pipeData.pipes[Int32.Parse(split[0]), i];

            for (int p = 0; p < pipeNum; p++)
            {
                GameObject pipe = Instantiate(indicatorPrefabs[i]);
                pipe.name = "" + i;
                pipe.transform.position = new Vector3(50 + (p * 10), INDICATOR_HEIGHT, (i * 10));

                pipe.transform.localScale = new Vector3(0, 0, 0);
                LeanTween.scale(pipe, new Vector3(1f, 1f, 1f), 1f).setEaseOutBounce();

                indicators[pipe.transform.position] = pipe;
                originalIndicatorPositions.Add(pipe, pipe.transform.position);
            }

        }
    }

    public Bridge getBridge(float x, float y)
    {   
        print(indicators);
        return indicators[new Vector3(x * 10, INDICATOR_HEIGHT, y * 10)].GetComponent<Bridge>();
    }

    Vector3 worldPosition;

    void Update()
    {
        int x = 0;
        int z = 0;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (_groundPlane.Raycast(ray, out distance))
        {
            worldPosition = ray.GetPoint(distance);
            x = ((int)Mathf.Round(worldPosition.x / 10.0f)) * 10;
            z = ((int)Mathf.Round(worldPosition.z / 10.0f)) * 10;

            Debug.DrawLine(Camera.main.transform.position, worldPosition);
        }


        if (currentMode != null && Input.GetMouseButton(0))
        {

            if (river.houses.Contains(new Vector3(x, 0, z)) || river.fields.Contains(new Vector3(x, 0, z)))
            {
                currentMode.transform.position = new Vector3(x, 5, z);
                SetObjectColor(currentMode, new Color32(0x7b, 0x7b, 0x7b, 0xFF));
            }
            else
            {
                currentMode.transform.position = new Vector3(x, 1, z);
                SetObjectColor(currentMode, Color.white);
            }

            if ((x < 0 || z < 0 || x / 10 > river.bends.GetLength(0) - 1 || z / 10 > river.bends.GetLength(1) - 1) && new Vector3(x, 0.02f, z) != originalIndicatorPositions[currentMode])
            {
                SetObjectColor(currentMode, new Color32(0x7b, 0x7b, 0x7b, 0xFF));
            }

        }
        else if (currentMode != null && !Input.GetMouseButton(0))
        {
            if (x < 0 || z < 0 || x / 10 > river.bends.GetLength(0) - 1 || z / 10 > river.bends.GetLength(1) - 1)
            {
                indicators[currentMode] = originalIndicatorPositions[currentMode];
                LeanTween.move(currentMode, originalIndicatorPositions[currentMode], 0.15f);
                SetObjectColor(currentMode, invalidColor);
            }
            else if (indicators.Contains(new Vector3(x, 0.02f, z)))
            {
                GameObject occupier = indicators[(new Vector3(x, 0.02f, z))];
                indicators[occupier] = originalIndicatorPositions[occupier];
                LeanTween.move(occupier, originalIndicatorPositions[occupier], 0.15f);

                indicators[currentMode] = new Vector3(x, 0.02f, z);
                LeanTween.move(currentMode, new Vector3(x, 0.02f, z), 0.15f);


                SetObjectColor(currentMode, invalidColor);

                switch (currentMode.name)
                {
                    case "0":
                        river.ChangeBend(x / 10, z / 10, -1, 0);
                        break;
                    case "1":
                        river.ChangeBend(x / 10, z / 10, 1, 0);
                        break;
                    case "2":
                        river.ChangeBend(x / 10, z / 10, 0, 1);
                        break;
                    case "3":
                        river.ChangeBend(x / 10, z / 10, 0, -1);
                        break;
                    case "4":
                        river.ChangeBend(x / 10, z / 10, 2, 2);
                        break;
                    case "5":
                        river.ChangeBend(x / 10, z / 10, 3, 3);
                        break;
                }

            }
            else
            {
                if (river.houses.Contains(new Vector3(x, 0.01f, z)) || river.fields.Contains(new Vector3(x, 0.01f, z)))
                {
                    indicators[currentMode] = originalIndicatorPositions[currentMode];
                    LeanTween.move(currentMode, originalIndicatorPositions[currentMode], 0.15f);
                    SetObjectColor(currentMode, invalidColor);
                }
                else
                {
                    indicators[currentMode] = new Vector3(x, 0.02f, z);
                    LeanTween.move(currentMode, new Vector3(x, 0.02f, z), 0.15f);
                    SetObjectColor(currentMode, invalidColor);

                    switch (currentMode.name)
                    {
                        case "0":
                            river.PlaceBend(x / 10, z / 10, -1, 0);
                            break;
                        case "1":
                            river.PlaceBend(x / 10, z / 10, 1, 0);
                            break;
                        case "2":
                            river.PlaceBend(x / 10, z / 10, 0, 1);
                            break;
                        case "3":
                            river.PlaceBend(x / 10, z / 10, 0, -1);
                            break;
                        case "4":
                            river.PlaceBend(x / 10, z / 10, 2, 2);
                            break;
                        case "5":
                            river.PlaceBend(x / 10, z / 10, 3, 3);
                            break;
                    }
                }

            }

            currentMode = null;

        }
        else if (currentMode == null && Input.GetMouseButtonDown(0))
        {
            if (indicators.Contains(new Vector3(x, 0.02f, z)))
            {
                river.RemoveBend(x / 10, z / 10);
                currentMode = indicators[new Vector3(x, 0.02f, z)];
                indicators[currentMode] = new Vector3(x, 1, z);
                SetObjectColor(currentMode, Color.white);
                currentMode.transform.position = new Vector3(x, 0, z);
                LeanTween.move(currentMode, new Vector3(x, 1, z), 0.15f);

            }
        }
    }

    public void RotateSplit(float x, float z, bool original)
    {
        if (original)
        {
            LeanTween.rotate(indicators[new Vector3(x * 10, 0.02f, z * 10)], new Vector3(0, 0, 0), 0.5f).setEaseInCubic();
        }
        else
        {
            LeanTween.rotate(indicators[new Vector3(x * 10, 0.02f, z * 10)], new Vector3(0, 90, 0), 0.5f).setEaseInCubic();
        }
    }
    void SetObjectColor(GameObject obj, Color color)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", color);
        obj.GetComponent<Renderer>().SetPropertyBlock(block);
    }

}


