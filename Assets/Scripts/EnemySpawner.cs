using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int numberOfEnemies = 5;
    [SerializeField] GameObject enemy;
    [SerializeField] int unitSize = 20;
    [SerializeField] Grid playBoardData;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var xPosition = 0;
            var yPosition = 0;
            do
            {
                xPosition = Random.Range(0, 20);
                yPosition = Random.Range(0, 12);
            } while (playBoardData.levels[playBoardData.currentLevel].layout[xPosition, yPosition] != 0);
           

            Instantiate(enemy, new Vector3(xPosition * unitSize, yPosition * unitSize, 0),
                            Quaternion.identity);
        }
    }
}
