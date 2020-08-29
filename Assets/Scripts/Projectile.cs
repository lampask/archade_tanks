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
        if (other.CompareTag("Player1")  || other.CompareTag("Player2"))
        {
            var pb = other.gameObject.GetComponent<PlayerBehaviour>();
            if (other.gameObject != origin) pb.TakeHit();
        }
        else if (other.CompareTag("Enemy") && !origin.CompareTag("Enemy"))
        {
            var pb = other.gameObject.GetComponent<EnemyBehavior>();
            if (other.gameObject != origin) pb.TakeHit();

            if (origin.CompareTag("Player1"))
            {
                scoreHandler.addScoreToPlayerOne(20);
            }
            else if (origin.CompareTag("Player2")) {
                scoreHandler.addScoreToPlayerTwo(20);
            }
        }
        _lifetime = lifespan;
    }
}