using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Powerup : MonoBehaviour
{
    public AudioClip powerUpSound;
    public float volume = .4f;
    private Camera camRef;
    List<Action<PlayerBehaviour>> powerups = new List<Action<PlayerBehaviour>>();

    public Sprite a;
    public Sprite b;
    public Sprite c;
    
    private Sprite _selectedOne;

    private void Start()
    {
        camRef = Camera.main;
        powerups.Add((id) =>
        {
            _selectedOne = a;
            id.lives++;
            Instantiate(id.hearthImage, id.livesContainer);
        });

        powerups.Add((id) =>
        {
            _selectedOne = b;
            id.addAmmo();
        });

        powerups.Add((id) =>
        {
            _selectedOne = c;
            foreach(var enemy in FindObjectsOfType<EnemyBehavior>())
            {
                if (enemy.slowTimeCounter == 0) {
                    enemy.SlowTime();
                }
                else
                {
                    enemy.slowTimeCounter = 10;
                }
            }
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            PlayerBehaviour otherPlayerBehavior = other.GetComponent<PlayerBehaviour>();
            otherPlayerBehavior.powerUpsCollected++;
            AudioSource.PlayClipAtPoint(powerUpSound, camRef.transform.position,volume);
            powerups[UnityEngine.Random.Range(0, powerups.Count)].Invoke(otherPlayerBehavior);
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(Pop());
        }
    }

    private IEnumerator Pop()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = _selectedOne;
        while (transform.localScale.x < 450f)
        {
            transform.localScale += Vector3.one*3;
            sr.color -= new Color(0, 0, 0, 0.03f);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }
}