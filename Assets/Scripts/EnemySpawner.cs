using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] public GameObject enemy;
    [SerializeField] private int unitSize = 20;
    [SerializeField] public Grid playBoardData;
    // Start is called before the first frame update
    private void Start()
    {
        Grid.instance.gameStart.AddListener(() =>
        {
            Spawn(Grid.instance.maxEnemies);
            Grid.instance.maxEnemies += Random.Range(0, 2);
        });
        
    }

    public void Spawn(int num)
    {
        for (int i = 0; i < num; i++)
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

        Grid.instance.aliveEnemies = num;
        Grid.instance.maxEnemies = num;
    }
 }
