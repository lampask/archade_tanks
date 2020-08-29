using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int numberOfEnemies = 5;
    [SerializeField] GameObject enemy;
    [SerializeField] int unitSize = 20;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var xPosition = Random.Range(0, 20);
            var yPosition = Random.Range(0, 12);

            Instantiate(enemy, new Vector3(xPosition * unitSize, yPosition * unitSize, 0),
                            Quaternion.identity);
        }
    }
}
