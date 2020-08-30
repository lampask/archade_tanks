using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public Int2dArray layout = new Int2dArray(20, 12);
    public Vector2 start1Pos;
    public Vector2 start2Pos;
}