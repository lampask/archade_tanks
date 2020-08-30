using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject origin;
    public Vector2 direction;
    public int unitSize = 20;
    public float waitTime = .1f;
    public int lifespan = 10;
    private ScoreHandler scoreHandler;
    public Sprite brokenObstacle;

    private float _snapshot;
    private int _lifetime;
    
    private void Start()
    {
        _snapshot = Time.time;
        scoreHandler = FindObjectOfType<ScoreHandler>();
    }

    private void Update()
    {
        if (_lifetime == lifespan) Remove();
        if (_snapshot + waitTime < Time.time)
        {
            _snapshot = Time.time;
            transform.position += new Vector3(direction.x, direction.y, 0) * unitSize;
            var ts = transform.position;
            transform.position = new Vector3(transform.position.x < 0 ? (Grid.instance.dimensions.x - 1) * unitSize : transform.position.x % (Grid.instance.dimensions.x * unitSize),
                transform.position.y < 0 ? (Grid.instance.dimensions.y - 1) * unitSize : transform.position.y % (Grid.instance.dimensions.y * unitSize), 0);
            if (transform.position != ts)
            {
                GetComponent<TrailRenderer>().Clear();
            }
            _lifetime++;
        }
    }

    public void Remove()
    {
        // R. i. p.
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger occurred");
        if (other.CompareTag("Enemy") && !origin.CompareTag("Enemy"))
        {
            var pb = other.gameObject.GetComponent<EnemyBehavior>();
            if (other.gameObject != origin) pb.TakeHit();

            if(origin.TryGetComponent<PlayerBehaviour>(out PlayerBehaviour component))
            {
                component.enemiesKilled++;
            }

            if (origin.CompareTag("Player1"))
            {
                scoreHandler.addScoreToPlayerOne(20);
            }
            else if (origin.CompareTag("Player2")) {
                scoreHandler.addScoreToPlayerTwo(20);
            }
        }
        else if (other.CompareTag("Player1")  || other.CompareTag("Player2"))
        {
            var pb = other.gameObject.GetComponent<PlayerBehaviour>();
            if (other.gameObject != origin)
            {
                if (pb.lives > 0)
                {
                    if (other.CompareTag("Player1") && origin.CompareTag("Player2")) scoreHandler.addScoreToPlayerTwo(10);
                    else if (other.CompareTag("Player2") && origin.CompareTag("Player1")) scoreHandler.addScoreToPlayerOne(10);
                    pb.TakeHit();
                }
            }
        }
        else if (other.CompareTag("ObstacleDamagable"))
        {
            other.GetComponent<SpriteRenderer>().sprite = brokenObstacle;
            other.tag = "Untagged";
            other.enabled = false;
        }
        _lifetime = lifespan;
    }
}