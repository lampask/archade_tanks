using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float waitTime = .2f;
    private float _snapshot;
    public int unitSize = 20;
    [Header("Resources")]
    public GameObject projectilePrefab;
    public int fieldsBetweenShoot = 6;
    private Grid playBoardData;
    private Vector2 directions;
    private int counter = 0;


    // Start is called before the first frame update
    void Start()
    {
        playBoardData = GameObject.Find("Manager").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_snapshot + waitTime < Time.time)
        {
            _snapshot = Time.time;
            MoveOrChangeDirection();
            if (counter >= fieldsBetweenShoot)
            {
                EnemyFire();
            }
            counter++;
        }
    }

    private void MoveOrChangeDirection()
    {
        int directionChanger = Random.Range(0, 16);

        switch (directionChanger)
        {
            case 0:
                directions = Vector2.left;
                break;
            case 1:
                directions = Vector2.right;
                break;
            case 2:
                directions = Vector2.up;
                break;
            case 3:
                directions = Vector2.down;
                break;
            default:
                break;
        }
        transform.position += new Vector3(directions.x, directions.y, 0) * unitSize;
        transform.position = new Vector3(transform.position.x < 0 ? (playBoardData.dimensions.x - 1) * unitSize : transform.position.x % (playBoardData.dimensions.x * unitSize),
        transform.position.y < 0 ? (playBoardData.dimensions.y - 1) * unitSize : transform.position.y % (playBoardData.dimensions.y * unitSize), 0);
    }

    private void EnemyFire() {
        counter = 0;
        var proj = Instantiate(projectilePrefab, transform.position + new Vector3(directions.x * unitSize, directions.y * unitSize, 0),
                            Quaternion.identity).GetComponent<Projectile>();
        proj.direction = directions;
        proj.origin = gameObject;
    }

    public void TakeHit()
    {
        Destroy(gameObject);
    }

}
