using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MonoBehaviour
{
    Vector2 source = new Vector2(2, 0);
    Vector2 initialDirection = new Vector2(0, 1);
    public List<GameObject> waterTiles = new List<GameObject>();
    public List<Vector3> waterPositions = new List<Vector3>();
    public List<Vector3> prevWaterPositions = new List<Vector3>();

    List<GameObject> houseObjects = new List<GameObject>();
    List<GameObject> fieldObjects = new List<GameObject>();

    List<GameObject> fillers = new List<GameObject>();
    public List<Vector3> houses;
    public List<Vector3> fields;
    public GameObject housePrefab;
    public GameObject fieldPrefab;
    public Bend[,] bends;
    public GameObject waterPrefab;
    public GameObject fillerAcross;
    public GameObject fillerDown;

    public Picker picker;
    public Manager manager;
    LevelGenerator levelGen;

    float WATER_TILE_RADIUS = 4.75f;
    float WATER_TILE_HEIGHT = 0.01f;

    public void Init()
    {
        levelGen = manager.levelGen;

        bends = new Bend[levelGen.map.width, levelGen.map.height];
        houses = levelGen.houses;
        fields = levelGen.fields;

        float delay = 0;

        foreach (Vector3 housePos in houses)
        {
            GameObject house = Instantiate(housePrefab);
            house.transform.localScale = new Vector3(0, 0, 0);
            houseObjects.Add(house);
            house.transform.position = housePos;
            LeanTween.scale(house, new Vector3(1, 1, 1), 0.15f).setDelay(delay);
            delay += 0.1f;
        }

        foreach (Vector3 fieldPos in fields)
        {
            GameObject field = Instantiate(fieldPrefab);
            field.transform.localScale = new Vector3(0, 0, 0);
            fieldObjects.Add(field);
            field.transform.position = fieldPos;
            LeanTween.scale(field, new Vector3(1, 1, 1), 0.15f).setDelay(delay);
            delay += 0.1f;
        }

        List<Vector2> positions = new List<Vector2>();
        GenerateRiver(source, initialDirection, positions, 0);
    }

    public void Clear()
    {
        foreach (GameObject house in houseObjects)
        {
            if (house != null)
            {
                LeanTween.scale(house, new Vector3(0, 0, 0), 1.5f).setEase(LeanTweenType.easeOutBounce).setOnComplete(DestroyObj).setOnCompleteParam(house);
            }
        }

        foreach (GameObject field in fieldObjects)
        {
            if (field != null)
            {
                LeanTween.scale(field, new Vector3(0, 0, 0), 1.5f).setEase(LeanTweenType.easeOutBounce).setOnComplete(DestroyObj).setOnCompleteParam(field);
            }

        }

        foreach (GameObject waterTile in waterTiles)
        {
            if (waterTile != null)
            {
                LeanTween.scale(waterTile, new Vector3(0, 0, 0), 1.5f).setEase(LeanTweenType.easeOutBounce).setOnComplete(DestroyObj).setOnCompleteParam(waterTile);
            }
        }
        waterPositions.Clear();
        bends = null;

    }

    void DestroyObj(object obj)
    {
        Destroy(obj as GameObject);
    }

    public void PlaceBend(int i, int j, int x, int y)
    {
        if (i >= 0 && j >= 0 && i < bends.GetLength(0) && j < bends.GetLength(1) && bends[i, j] == null)
        {
            if (houses.Contains(new Vector3(i * 10, 0.01f, j * 10))) { return; }
            if (fields.Contains(new Vector3(i * 10, 0.01f, j * 10))) { return; }

            bends[i, j] = new Bend();
            bends[i, j].direction = new Vector2(x, y);
            UpdateRiver();
            print("Update River");
        }
    }

    public void RemoveBend(int i, int j)
    {
        if (i >= 0 && j >= 0 && i < bends.GetLength(0) && j < bends.GetLength(1))
        {
            if (houses.Contains(new Vector3(i * 10, 0.01f, j * 10))) { return; }
            if (fields.Contains(new Vector3(i * 10, 0.01f, j * 10))) { return; }

            bends[i, j] = null;
            UpdateRiver();
        }
    }

    public void ChangeBend(int i, int j, int x, int y)
    {
        if (i >= 0 && j >= 0 && i < bends.GetLength(0) && j < bends.GetLength(1) && bends[i, j] != null)
        {
            if (houses.Contains(new Vector3(i * 10, 0.01f, j * 10))) { return; }
            if (fields.Contains(new Vector3(i * 10, 0.01f, j * 10))) { return; }

            bends[i, j].direction = new Vector2(x, y);
            UpdateRiver();
        }
    }

    void CheckIfWin()
    {
        bool houseFlooded = false;
        bool fieldDry = false;

        foreach (Vector3 house in houses)
        {
            if (waterPositions.Contains(house))
            {
                houseFlooded = true;
            }
        }
        foreach (Vector3 field in fields)
        {
            if (!waterPositions.Contains(field))
            {
                fieldDry = true;
            }
        }

        if (!houseFlooded && !fieldDry)
        {
            manager.levelFinished();
        }
    }

    void ClearRiver()
    {
        foreach (GameObject waterTile in waterTiles)
        {
            Destroy(waterTile);
        }
        waterPositions.Clear();

        Bridge[] bridges = GameObject.FindObjectsOfType<Bridge>();

        foreach (Bridge bridge in bridges)
        {
            bridge.over = false;
            bridge.under = false;
        }

        foreach (GameObject filler in fillers)
        {
            Destroy(filler);
        }

    }

    int animatingCount;

    void UpdateRiver()
    {
        ClearRiver();

        List<Vector2> positions = new List<Vector2>();
        animatingCount = 0;

        GenerateRiver(source, initialDirection, positions, 0);
    }
    void GenerateRiver(Vector2 currentPos, Vector2 direction, List<Vector2> positions, float delay)
    {
        bool animate = false;
        bool loop = false;
        int maxWATER = 1000;

        Vector3 previousDirection = direction;

        while (!loop && currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x < levelGen.map.width && currentPos.y < levelGen.map.height)
        {
            if (positions.Contains(currentPos))
            {
                if (maxWATER-- > 0)
                {
                    print("Loop!");
                    loop = true;
                    if (bends[(int)currentPos.x, (int)currentPos.y] != null)
                    {
                        if (bends[(int)currentPos.x, (int)currentPos.y].direction.x == 3)
                        {

                            Bridge bridge = picker.getBridge(currentPos.x, currentPos.y);
                            print("Second Bridge " + direction + " : " + bridge.underDir);

                            if (Mathf.Abs(direction.x) == bridge.underDir.x)
                            {
                                if (!bridge.under)
                                {
                                    loop = false;
                                }
                            }
                            else
                            {
                                if (!bridge.over)
                                {
                                    loop = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    print("Infinite Loop");
                    loop = true;
                }
            }

            if (!loop)
            {
                positions.Add(currentPos);
                GameObject waterObj = Instantiate(waterPrefab);
                Vector3 waterObjPos = new Vector3(currentPos.x * 10, WATER_TILE_HEIGHT, currentPos.y * 10);
                waterObj.transform.position = new Vector3(currentPos.x * 10, WATER_TILE_HEIGHT, currentPos.y * 10);
                if (!animate && !prevWaterPositions.Contains(waterObj.transform.position))
                {
                    animate = true;
                }

                waterPositions.Add(new Vector3(currentPos.x * 10, WATER_TILE_HEIGHT, currentPos.y * 10));
                bool normalBend = false;
                if (currentPos.x >= 0 && currentPos.y >= 0 && currentPos.x < levelGen.map.width && currentPos.y < levelGen.map.height && bends[(int)currentPos.x, (int)currentPos.y] != null)
                {
                    if (bends[(int)currentPos.x, (int)currentPos.y].direction.x == 2)
                    {

                        if (direction.x == 0)
                        {
                            picker.RotateSplit(currentPos.x, currentPos.y, true);
                            direction = new Vector2(-1, 0);
                            GenerateRiver(new Vector2(currentPos.x + 1, currentPos.y + 0), new Vector2(1, 0), positions, delay);
                        }
                        else
                        {
                            picker.RotateSplit(currentPos.x, currentPos.y, false);

                            direction = new Vector2(0, -1);
                            GenerateRiver(new Vector2(currentPos.x + 0, currentPos.y + 1), new Vector2(0, 1), positions, delay);
                        }
                    }
                    else if (bends[(int)currentPos.x, (int)currentPos.y].direction.x == 3)
                    {

                        print("First Bridge");
                        Bridge bridge = picker.getBridge(currentPos.x, currentPos.y);
                        waterObj.SetActive(false);

                        if (direction.x == bridge.underDir.x)
                        {
                            bridge.under = true;
                        }
                        else
                        {
                            bridge.over = true;
                        }

                        if (bridge.over && bridge.under)
                        {
                            waterObj.SetActive(true);
                        }

                    }
                    else
                    {
                        normalBend = true;
                        previousDirection = direction;
                        direction = bends[(int)currentPos.x, (int)currentPos.y].direction;
                    }

                }

                if (animate)
                {
                    waterObj.transform.localScale = new Vector3(0, 0, 0);
                    animatingCount++;
                    if (!normalBend)
                    {
                        LeanTween.scale(waterObj, new Vector3(10 - Mathf.Abs(direction.y), 10 - Mathf.Abs(direction.x), 10), 0.3f).setEase(LeanTweenType.easeOutBounce).setDelay(delay).setOnComplete(OnAnimationComplete);
                    }
                    else
                    {
                        if (Mathf.Abs(direction.x) != Mathf.Abs(previousDirection.x))
                        {
                            PlaceFillers(direction, previousDirection, waterObj);
                            LeanTween.scale(waterObj, new Vector3(9, 9, 10), 0.3f).setEase(LeanTweenType.easeOutBounce).setDelay(delay).setOnComplete(OnAnimationComplete);
                        }
                        else
                        {
                            LeanTween.scale(waterObj, new Vector3(10 - Mathf.Abs(direction.y), 10 - Mathf.Abs(direction.x), 10), 0.3f).setEase(LeanTweenType.easeOutBounce).setDelay(delay).setOnComplete(OnAnimationComplete);
                        }
                    }
                    delay += 0.1f;
                }
                else
                {
                    if (!normalBend)
                    {
                        waterObj.transform.localScale = new Vector3(10 - Mathf.Abs(direction.y), 10 - Mathf.Abs(direction.x), 10);
                    }
                    else
                    {
                        if (Mathf.Abs(direction.x) != Mathf.Abs(previousDirection.x))
                        {
                            PlaceFillers(direction, previousDirection, waterObj);
                            waterObj.transform.localScale = new Vector3(9, 9, 10);
                        }
                        else
                        {
                            waterObj.transform.localScale = new Vector3(10 - Mathf.Abs(direction.y), 10 - Mathf.Abs(direction.x), 10);
                        }
                    }
                }

                currentPos = new Vector2(currentPos.x + direction.x, currentPos.y + direction.y);
                waterTiles.Add(waterObj);
            }
        }
        if (animatingCount == 0)
        {
            CheckIfWin();
        }
        prevWaterPositions.Clear();
        prevWaterPositions.AddRange(waterPositions);
    }

    void PlaceFillers(Vector3 direction, Vector3 previousDirection, GameObject waterObj)
    {
        GameObject across = Instantiate(fillerAcross);
        GameObject down = Instantiate(fillerDown);

        if (direction.x != 0)
        {
            across.transform.position = new Vector3(waterObj.transform.position.x + (WATER_TILE_RADIUS * direction.x), WATER_TILE_HEIGHT, waterObj.transform.position.z);
            down.transform.position = new Vector3(waterObj.transform.position.x, WATER_TILE_HEIGHT, waterObj.transform.position.z - (WATER_TILE_RADIUS * previousDirection.y));
        }
        else
        {
            down.transform.position = new Vector3(waterObj.transform.position.x, WATER_TILE_HEIGHT, waterObj.transform.position.z + (WATER_TILE_RADIUS * direction.y));
            across.transform.position = new Vector3(waterObj.transform.position.x - (WATER_TILE_RADIUS * previousDirection.x), WATER_TILE_HEIGHT, waterObj.transform.position.z);
        }
        fillers.Add(across);
        fillers.Add(down);
    }

    void OnAnimationComplete()
    {
        animatingCount--;
        if (animatingCount == 0)
        {
            CheckIfWin();
        }
    }


}
