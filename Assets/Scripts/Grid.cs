﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public static Grid instance;
    public Vector2 dimensions;
    public int currentLevel;
    public int bottomOffset = 40;
    [Header("Resources")] public GameObject data;
    public Camera camRef;
    public TMP_Text countdown;
    public TMP_Text starter;
    public TMP_Text timer;
    public AudioClip winSound;
    public AudioClip gameMusic;
    public AudioClip menuMusic;
    public AudioClip tickSound;
    public AudioClip confirmSound;

    public PlayerBehaviour player1;
    public PlayerBehaviour player2;
    public Transform gameOver;
    
    private float _gameTime;
    public float GameTime
    {
        get => _gameTime;
        set
        {
            _gameTime = value;
            gameTimeChanged.Invoke();
        }
    }

    public UnityEvent gameTimeChanged;
    
    private float _scale = 125f;

    public int maxEnemies = 2;
    [HideInInspector] public int aliveEnemies;

    public List<Sprite> baseTiles = new List<Sprite>();
    public List<LevelData> levels = new List<LevelData>();
    public List<GameObject> blocks = new List<GameObject>();

    public bool gameStarted;
    public UnityEvent gameStart;
    public UnityEvent nextLevel;

    private AudioSource _cas;

    private void Awake()
    {
        if (instance == null || instance.Equals(null))
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Time.timeScale = 0;
    }

    private void Start()
    {
        starter.text = GameTime == 0 ? "- INSERT COIN TO PLAY -" : "- PRESS START TO PLAY -";
        gameTimeChanged.AddListener(() =>
        {
            timer.text = $"{Mathf.Floor(GameTime/60).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')}:{(GameTime%60).ToString(CultureInfo.InvariantCulture).PadLeft(2, '0')}";
            starter.text = GameTime == 0 ? "- INSERT COIN TO PLAY -" : "- PRESS START TO PLAY -";
            });
        _cas = camRef.GetComponent<AudioSource>();
        StartCoroutine(Flash(0.5f));
        gameStart.AddListener(() =>
        {
            if (!gameStarted)
            {
                ScoreHandler.Instance.player1Score = 0;
                ScoreHandler.Instance.player2Score = 0;
                ScoreHandler.Instance.player1ScoreText.text = "0";
                ScoreHandler.Instance.player2ScoreText.text = "0";
                maxEnemies = 2;
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                foreach (var projectile in FindObjectsOfType<Projectile>())
                {
                    Destroy(projectile.gameObject);
                }
            }
            _cas.Pause();
            gameOver.gameObject.SetActive(false);
            camRef.transform.position = dimensions / 2 * _scale / 6.25f - Vector2.one * _scale / 6.25f / 2f;
            camRef.transform.position += Vector3.back * 10;
            starter.gameObject.SetActive(false);
            StartCoroutine(Countdown(3));
            Generate(Random.Range(0, levels.Count));
        });
        nextLevel.AddListener((() =>
        {
            AudioSource.PlayClipAtPoint(winSound, camRef.transform.position, 0.05f);
            for (var i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            if (player1.lives == 0 && gameStarted)
            {
                player1.lives++;
                player1.GetComponent<SpriteRenderer>().color = Color.white;
                Instantiate(player1.hearthImage, player1.livesContainer);
            }
            
            if (player2.lives == 0 && gameStarted)
            {
                player2.lives++;
                player2.GetComponent<SpriteRenderer>().color = Color.white;
                Instantiate(player2.hearthImage, player2.livesContainer);
            }
            
            gameStart.Invoke();
        }));
    }

    public IEnumerator Tick()
    {
        while (gameStarted && GameTime > 0)
        {
            yield return new WaitForSeconds(1);
            GameTime -= 1;
        }
    }
    
    private IEnumerator Flash(float s)
    {
        while (!gameStarted)
        {
            starter.color = Color.clear;
            yield return new WaitForSecondsRealtime(s);
            starter.color = Color.white;
            yield return new WaitForSecondsRealtime(s);
        }
    }

    private IEnumerator Countdown(int s)
    {
        countdown.gameObject.SetActive(true);
        var i = 0;
        while (!i.Equals(s))
        {
            AudioSource.PlayClipAtPoint(tickSound, camRef.transform.position, 0.05f);
            countdown.text = (s - i).ToString();
            i++;
            yield return new WaitForSecondsRealtime(1);
        }
        AudioSource.PlayClipAtPoint(confirmSound, camRef.transform.position, 0.05f);
        countdown.text = "0";
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        countdown.gameObject.SetActive(false);
        _cas.clip = gameMusic;
        _cas.Play();
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
        gameOver.gameObject.SetActive(true);
        var f = gameOver.GetChild(1).GetComponentsInChildren<TMP_Text>();
        f[1].text = ScoreHandler.Instance.player1ScoreText.text;
        f[2].text = player1.enemiesKilled.ToString();
        f[3].text = player1.numberOfShoots.ToString();
        f[4].text = player1.powerUpsCollected.ToString();
        f[5].text = player1.numberOfDeaths.ToString();
        var s = gameOver.GetChild(2).GetComponentsInChildren<TMP_Text>();
        s[1].text = ScoreHandler.Instance.player2ScoreText.text;
        s[2].text = player2.enemiesKilled.ToString();
        s[3].text = player2.numberOfShoots.ToString();
        s[4].text = player2.powerUpsCollected.ToString();
        s[5].text = player2.numberOfDeaths.ToString();
        _cas.clip = menuMusic;
        gameStarted = false;
        starter.gameObject.SetActive(true);
        starter.text = GameTime == 0 ? "- INSERT COIN TO PLAY -" : "- PRESS START TO PLAY -";
        StartCoroutine(Flash(0.5f));
    }
    
    public void Generate(int level)
    {
        var l = levels[level];
        player1.transform.position = l.start1Pos * player1.unitSize;
        player2.transform.position = l.start2Pos * player2.unitSize;
        player1.transform.GetComponentsInChildren<TrailRenderer>().ToList().ForEach(x => x.Clear());
        player2.transform.GetComponentsInChildren<TrailRenderer>().ToList().ForEach(x => x.Clear());
        // Generation
        for (var x = 0; x < dimensions.x; x++)
        {
            for (var y = 0; y < dimensions.y; y++)
            {
                var a = Instantiate(blocks[l.layout[x, y]], transform);
                a.transform.localPosition = new Vector3(x * _scale/6.25f, y * _scale/6.25f, 0);
                a.transform.localScale = Vector3.one * _scale;

                if (l.layout[x, y] == 0)
                    a.GetComponent<SpriteRenderer>().sprite = baseTiles[Random.Range(0, baseTiles.Count - 1)];
            }
        }     
    }
}