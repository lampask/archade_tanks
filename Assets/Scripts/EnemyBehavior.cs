using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBehavior : MonoBehaviour
{
    public float waitTime = .2f;
    private float _snapshot;
    public int unitSize = 20;
    [Header("Resources")]
    public GameObject projectilePrefab;
    public GameObject powerUpPrefab;
    public AudioClip explosionSound;
    public AudioClip shootSound;
    public int fieldsBetweenShoot = 6;
    public float volume = 0.05f;
    private Grid playBoardData;
    public Vector2 directions;
    private int counter = 0;


    // Start is called before the first frame update
    private void Start()
    {
        playBoardData = GameObject.Find("Manager").GetComponent<Grid>();
    }

    // Update is called once per frame
    private void Update()
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
        var directionChanger = Random.Range(0, 16);

        var hit = Physics2D.Raycast(transform.position + new Vector3(directions.x, directions.y, 0) * unitSize, Vector3.back, 20);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Player1") || hit.collider.CompareTag("Player2") || hit.collider.CompareTag("Enemy"))
            {
                directions = -directions;
            }
        }
        
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
        transform.rotation = Quaternion.Euler(0, 0,
            (directions.x != 0 ? -directions.x : 1) * Vector2.Angle(Vector2.up, directions));
        transform.position = new Vector3(transform.position.x < 0 ? (playBoardData.dimensions.x - 1) * unitSize : transform.position.x % (playBoardData.dimensions.x * unitSize),
        transform.position.y < 0 ? (playBoardData.dimensions.y - 1) * unitSize : transform.position.y % (playBoardData.dimensions.y * unitSize), 0);
    }

    private void EnemyFire() {
        counter = 0;
        var proj = Instantiate(projectilePrefab, transform.position + new Vector3(directions.x * unitSize, directions.y * unitSize, 0),
                            Quaternion.identity).GetComponent<Projectile>();
        AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, volume);
        proj.direction = directions;
        proj.origin = gameObject;
    }

    public void TakeHit()
    {
        //StartCoroutine(HitEffect());
        AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position,volume);
        if (--Grid.instance.aliveEnemies == 0)
        {
            Grid.instance.nextLevel.Invoke();
            Time.timeScale = 0;
        }

        if (Random.Range(0, 10) < 4)
        {
            // DROP
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
    
    private IEnumerator HitEffect()
    {
        var sr = GetComponent<SpriteRenderer>();
        var baseCol = sr.color;
        for (var i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(.1f);
            sr.color = baseCol;
            yield return new WaitForSeconds(.2f);
        }
    }
}
