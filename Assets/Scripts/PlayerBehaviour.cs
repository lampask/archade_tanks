using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 directions;
    public int unitSize = 20;
    public float waitTime = .2f;
    public bool bordered;

    [Header("Game Stats")] 
    public int score;
    public int lives = 10;
    public int ammo = 5;
    
    [Header("Resources")] 
    public Grid playBoardData;
    public GameObject projectilePrefab;
    public Transform livesContainer;
    public GameObject hearthImage;
    public AudioClip bounceSound;
    public AudioClip shootSound;
    public AudioClip explosionSound;
    public float volume = 0.1f;


    private Color _cHolder;
    private float _snapshot;
    private bool _firing = false;

    private void Start()
    {
        _snapshot = Time.time;
        _cHolder = GetComponent<SpriteRenderer>().color;
        for (var i = 0; i < lives; i++)
        {
            Instantiate(hearthImage, livesContainer);
        }
    }

    private void Update()
    {
        if (_snapshot + waitTime < Time.time)
        {
            _snapshot = Time.time;
            if (lives > 0)
            {
                if (bordered)
                {
                    if (playBoardData.dimensions.x * unitSize <= transform.position.x + directions.x * unitSize ||
                        playBoardData.dimensions.y * unitSize <= transform.position.y + directions.y * unitSize) return;
                    if (transform.position.x + directions.x * unitSize < 0 ||
                        transform.position.y + directions.y * unitSize < 0) return;
                }

                if (directions != Vector2.zero)
                {
                    var hit = Physics2D.Raycast(
                        transform.position + new Vector3(directions.x, directions.y, 0) * unitSize,
                        Vector3.back, 20);
                    
                    // MORE CHECKS
                    var leftHit = Physics2D.Raycast(
                        transform.position + new Vector3(directions.x, directions.y, 0) * unitSize,
                        Vector3.back, 20);
                    var rightHit = Physics2D.Raycast(
                        transform.position + new Vector3(directions.x, directions.y, 0) * unitSize,
                        Vector3.back, 20);
                    
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Obstacle"))
                        {
                            TakeHit();
                            AudioSource.PlayClipAtPoint(bounceSound, transform.position, volume);
                            directions = -directions;
                        }

                        if (hit.collider.CompareTag("Player1") || hit.collider.CompareTag("Player2") ||
                            hit.collider.CompareTag("Enemy"))
                        {
                            directions = -directions;
                        }
                    }
                }

                transform.position += new Vector3(directions.x, directions.y, 0) * unitSize;
                transform.rotation = Quaternion.Euler(0, 0,
                    (directions.x != 0 ? -directions.x : 1) * Vector2.Angle(Vector2.up, directions));
                transform.position = new Vector3(
                    transform.position.x < 0
                        ? (playBoardData.dimensions.x - 1) * unitSize
                        : transform.position.x % (playBoardData.dimensions.x * unitSize),
                    transform.position.y < 0
                        ? (playBoardData.dimensions.y - 1) * unitSize
                        : transform.position.y % (playBoardData.dimensions.y * unitSize), 0);
                if (_firing)
                {
                    Fire();
                }

                _firing = false;
            }
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
        if (!_firing)
        {
            _firing = true;
        }
        Debug.Log($"{gameObject.name} Fire");
    }

    private void OnMenu(InputValue _)
    {
        Debug.Log("sfgs");
    }

    private void Fire()
    {
            var proj = Instantiate(projectilePrefab, transform.position + new Vector3(directions.x * unitSize, directions.y * unitSize, 0),
                            Quaternion.identity).GetComponent<Projectile>();
            proj.direction = directions;
            proj.origin = gameObject;
        AudioSource.PlayClipAtPoint(shootSound, transform.position, volume);
    }

    public void TakeHit()
    {
        if (lives-- <= 0)
        {
            // TODO: Die
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, volume);
            Debug.Log("Die");
            return;  
        }

        var currentLive = livesContainer.GetChild(0);
        Destroy(currentLive.gameObject);
        StartCoroutine(HitEffect());
    }
    
    private IEnumerator HitEffect()
    {
        var sr = GetComponent<SpriteRenderer>();
        for (var i = 0; i < 3; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(.1f);
            sr.color = _cHolder;
            yield return new WaitForSeconds(.15f);
        }
    }
}
