using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 dimensions;
    public int bottomOffset = 40;
    [Header("Resources")] public GameObject data;
    public Camera camRef;

    private float _scale = 150f;
    private void Start()
    {
        // Calculate grid placement & sizes
        //var minSide = Math.Min(dimensions.x, dimensions.y);
        //_scale = (Screen.height - bottomOffset) / minSide;
        camRef.transform.position = dimensions / 2 * _scale / 7.5f - Vector2.one * _scale / 7.5f / 2f;
        camRef.transform.position += Vector3.back * 10;
        
        // Generation
        for (var x = 0; x < dimensions.x; x++)
        {
            for (var y = 0; y < dimensions.y; y++)
            {
                var a = Instantiate(data, transform);
                a.transform.localPosition = new Vector3(x * _scale/7.5f, y * _scale/7.5f, 0);
                a.transform.localScale = Vector3.one * _scale;
            }
        }        
    }
}