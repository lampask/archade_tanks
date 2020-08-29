using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerBehaviour : MonoBehaviour
{
    [FormerlySerializedAs("ID")] public int id;
    [SerializeField] private Vector2 directions;
    public int unitSize = 20;
    public float waitTime = .2f;
    public bool bordered;

    [Header("Game Stats")] 
    public int score;
    public int lives = 3;
    public int ammo = 5;
    
    [Header("Resources")] 
    public Grid playBoardData;
    public GameObject projectilePrefab;
    
    private float _snapshot;
    private void Start()
    {
        _snapshot = Time.time;
    }

    private void Update()
    {
        if (_snapshot + waitTime < Time.time)
        {
            _snapshot = Time.time;
            if (bordered)
            {
                if (playBoardData.dimensions.x * unitSize <= transform.position.x + directions.x * unitSize ||
                    playBoardData.dimensions.y * unitSize <= transform.position.y + directions.y * unitSize) return;
                if (transform.position.x + directions.x * unitSize < 0 ||
                    transform.position.y + directions.y * unitSize < 0) return;
            }
            transform.position += new Vector3(directions.x, directions.y, 0) * unitSize;
            transform.position = new Vector3(transform.position.x < 0 ? (playBoardData.dimensions.x-1)*unitSize : transform.position.x % (playBoardData.dimensions.x*unitSize), 
                transform.position.y < 0 ? (playBoardData.dimensions.y-1)*unitSize : transform.position.y % (playBoardData.dimensions.y*unitSize), 0);
        }
    }
    
    // EVENT FUNCTIONS

    private void OnChangeMovement(InputValue _)
    {
        var val = _.Get<Vector2>();
        if (Math.Abs(val.x) > .5f && Math.Abs(val.y) > .5f) return;
        if (val != Vector2.zero && val != directions) directions = new Vector2(Mathf.Round(val.x), Mathf.Round(val.y));
        Debug.Log($"{gameObject.name} {_.Get<Vector2>().ToString()}");
    }

    private void OnFire(InputValue _)
    {
        var proj = Instantiate(projectilePrefab, transform.position + new Vector3(directions.x * unitSize, directions.y * unitSize, 0), 
                Quaternion.identity).GetComponent<Projectile>();
        proj.direction = directions;
        proj.origin = id;
        Debug.Log($"{gameObject.name} Fire");
    }

    public void TakeHit()
    {
        if (--lives <= 0)
        {
            // TODO: Die
            Debug.Log("Die");
            return;  
        }
        Debug.Log("Hit");
    }
}
