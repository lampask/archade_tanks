using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 directions;
    public int unitSize = 20;
    public float waitTime = .2f;
    public bool bordered;

    [Header("Game Stats")] 
    public int score;
    public int lives = 10;
    public int ammo = 0;

    [Header("Resources")] public Camera camRef;
    public Grid playBoardData;
    public GameObject projectilePrefab;
    public Transform livesContainer;
    public GameObject hearthImage;
    
    public AudioClip bounceSound;
    public AudioClip shootSound;
    public AudioClip explosionSound;
    public AudioClip menuSound;
    public AudioClip deathSound;
    public AudioClip coinSound;

    [Header("Game Score")]
    public int enemiesKilled = 0;
    public int numberOfShoots = 0;
    public int powerUpsCollected = 0;
    public int numberOfDeaths = 0;

    public float volume = 0.1f;
    public TMP_Text pauseText;
    private bool paused = false;
    public Sprite brokenObstacle;

    
    private Color _cHolder;
    private float _snapshot;
    private bool _firing = false;
    private static readonly int Explode = Animator.StringToHash("Explode");

    private void Start()
    {
        pauseText.gameObject.SetActive(false);
        Grid.instance.gameStart.AddListener(() =>
        {
            if (!Grid.instance.gameStarted)
            {
                lives = 10;
                GetComponent<SpriteRenderer>().color = Color.white;
                _cHolder = GetComponent<SpriteRenderer>().color;
                for (var i = 0; i < lives; i++)
                {
                    Instantiate(hearthImage, livesContainer);
                }
            }
            StartCoroutine(Snapshot(3));
        });
    }

    private IEnumerator Snapshot(int s)
    {
        yield return new WaitForSeconds(s);
        _snapshot = Time.time;
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
                    
                    var rightHit = Physics2D.Raycast(
                        transform.position + new Vector3(directions.x == 0 ? 1 : directions.x, directions.y == 0 ? 1 : directions.y, 0) * unitSize,
                        Vector3.back, 20);
                    if (rightHit.collider != null)
                    {
                        if (rightHit.collider.CompareTag("Enemy"))
                        {
                            var d = rightHit.collider.gameObject.GetComponent<EnemyBehavior>();
                            if (new Vector3(d.directions.x, d.directions.y, 0) +
                                rightHit.collider.gameObject.transform.position == transform.position + new Vector3(directions.x, directions.y, 0))
                            {
                                directions = -directions;
                            }
                        } 
                    }
                    
                    var leftHit = Physics2D.Raycast(
                        transform.position + new Vector3(directions.x == 0 ? -1 : directions.x, directions.y == 0 ? -1 : directions.y, 0) * unitSize,
                        Vector3.back, 20);
                    if (leftHit.collider != null)
                    {
                        if (leftHit.collider.CompareTag("Enemy"))
                        {
                            var d = leftHit.collider.gameObject.GetComponent<EnemyBehavior>();
                            if (new Vector3(d.directions.x, d.directions.y, 0) +
                                leftHit.collider.gameObject.transform.position == transform.position + new Vector3(directions.x, directions.y, 0))
                            {
                                directions = -directions;
                            }
                        } 
                    }
                    
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("ObstacleDamagable"))
                        {
                            TakeHit();
                            directions = -directions;
                            if (hit.collider.CompareTag("ObstacleDamagable"))
                            {
                                hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = brokenObstacle;
                                hit.collider.gameObject.tag = "Untagged";
                                hit.collider.enabled = false;
                            }
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
            _firing = true;
    }

    private void OnMenu(InputValue _)
    {
        if (Grid.instance.GameTime == 0) return;
        if (!Grid.instance.gameStarted)
        {
            Grid.instance.gameStart.Invoke();
            if (!Grid.instance.gameStarted)
            {
                Grid.instance.gameStarted = true;
                StartCoroutine( Grid.instance.Tick());
            }
            return;
        }
        AudioSource.PlayClipAtPoint(menuSound, camRef.transform.position, volume);
        paused = !paused;
        camRef.GetComponent<AudioSource>().pitch = paused ? 0.75f : 1;
        Time.timeScale = paused ? 0 : 1;
        pauseText.gameObject.SetActive(paused);
    }
    
    private void OnCoin(InputValue _)
    {
        Grid.instance.GameTime += 60*5;
        AudioSource.PlayClipAtPoint(coinSound, camRef.transform.position, volume);
    }
    
    private void Fire()
    {

        numberOfShoots++;
        var proj = Instantiate(projectilePrefab, transform.position + new Vector3(directions.x * unitSize, directions.y * unitSize, 0),
                            Quaternion.identity).GetComponent<Projectile>();
        if (ammo != 0)
        {
            proj.GetComponent<SpriteRenderer>().color = Color.green;
            proj.lifespan = 30;
            ammo--;
        }
        proj.direction = directions;
            proj.origin = gameObject;
        AudioSource.PlayClipAtPoint(shootSound, camRef.transform.position, volume);
    }

    public void TakeHit()
    {
        numberOfDeaths++;

        if (lives == 0) return;
        if (--lives == 0)
        {
            AudioSource.PlayClipAtPoint(deathSound, camRef.transform.position, volume);
            transform.GetChild(0).GetComponent<Animator>().SetTrigger(Explode);
            _cHolder = new Color(0.2f, 0.2f, 0.2f);
            if (Grid.instance.player1.lives <= 0 && 0 >= Grid.instance.player2.lives)
                StartCoroutine(Grid.instance.GameOver());
        }
        else
        {
            AudioSource.PlayClipAtPoint(bounceSound, camRef.transform.position, volume);
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

    public void addAmmo()
    {
        ammo += 5;
    }
}
