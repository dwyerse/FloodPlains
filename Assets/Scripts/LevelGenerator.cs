using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D map;
    public List<Vector3> houses = new List<Vector3>();
    public List<Vector3> fields = new List<Vector3>();
    public PipeData pipeData = new PipeData();
    public string level;

    public void GenerateLevel(string l)
    {
        level = l;
        houses.Clear();
        fields.Clear();
        print("Levels/level-" + level);
        map = Resources.Load<Texture2D>("Levels/level-" + level);
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                Color pixelColor = map.GetPixel(x, y);
                if (pixelColor.Equals(Color.red))
                {
                    houses.Add(new Vector3(x * 10, 0.01f, y * 10));
                }
                else if (pixelColor.Equals(Color.green))
                {
                    fields.Add(new Vector3(x * 10, 0.01f, y * 10));
                }
            }
        }
    }

}
