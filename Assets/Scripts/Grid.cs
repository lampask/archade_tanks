using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public Vector2 dimensions;
    public int startLevel;
    public int bottomOffset = 40;
    [Header("Resources")] public GameObject data;
    public Camera camRef;

    private float _scale = 125f;

    public List<Sprite> baseTiles = new List<Sprite>();
    public List<LevelData> levels = new List<LevelData>();
    public List<GameObject> blocks = new List<GameObject>();
    
    private void Start()
    {
        camRef.transform.position = dimensions / 2 * _scale / 6.25f - Vector2.one * _scale / 6.25f / 2f;
        camRef.transform.position += Vector3.back * 10;
        
        Generate(startLevel);
    }

    public void Generate(int level)
    {
        var l = levels[level];
        // Generation
        for (var x = 0; x < dimensions.x; x++)
        {
            for (var y = 0; y < dimensions.y; y++)
            {
                var a = Instantiate(blocks[l.layout[x, y]], transform);
                a.transform.localPosition = new Vector3(x * _scale/6.25f, y * _scale/6.25f, 0);
                a.transform.localScale = Vector3.one * _scale;

                if (l.layout[x, y] == 0)
                    a.GetComponent<SpriteRenderer>().sprite = baseTiles[Random.Range(0, baseTiles.Count - 1)];
            }
        }     
    }
}