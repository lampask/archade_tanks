using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 dimensions;
    public int startLevel;
    public int bottomOffset = 40;
    [Header("Resources")] public GameObject data;
    public Camera camRef;

    private float _scale = 150f;
    
    
    public List<LevelData> levels = new List<LevelData>();
    public List<GameObject> blocks = new List<GameObject>();
    
    private void Start()
    {
        camRef.transform.position = dimensions / 2 * _scale / 7.5f - Vector2.one * _scale / 7.5f / 2f;
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
                a.transform.localPosition = new Vector3(x * _scale/7.5f, y * _scale/7.5f, 0);
                a.transform.localScale = Vector3.one * _scale;
            }
        }     
    }
}