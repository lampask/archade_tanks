using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject origin;
    public Vector2 direction;
    public int unitSize = 20;
    public float waitTime = .1f;
    public int lifespan = 10;

    private float _snapshot;
    private int _lifetime;
    
    private void Start()
    {
        _snapshot = Time.time;
    }

    private void Update()
    {
        if (_snapshot + waitTime < Time.time)
        {
            _snapshot = Time.time;
            transform.position += new Vector3(direction.x, direction.y, 0) * unitSize;
            _lifetime++;
        }
        if (_lifetime == lifespan) Remove();
    }

    public void Remove()
    {
        // R. i. p.
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger occurred");
        if (other.CompareTag("Player"))
        {
            var pb = other.gameObject.GetComponent<PlayerBehaviour>();
            if (other.gameObject != origin) pb.TakeHit();
        }
        else if (other.CompareTag("Enemy"))
        {
            var pb = other.gameObject.GetComponent<EnemyBehavior>();
            if (other.gameObject != origin) pb.TakeHit();
        }

    }
}