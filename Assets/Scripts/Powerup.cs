﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Powerup : MonoBehaviour
{
    public AudioClip powerUpSound;
    public float volume = .4f;
    private Camera camRef;
    List<Action<PlayerBehaviour>> powerups = new List<Action<PlayerBehaviour>>();

    private void Start()
    {
        camRef = Camera.main;
        powerups.Add((id) =>
        {
            id.lives++;
            Instantiate(id.hearthImage, id.livesContainer);
        });

        powerups.Add((id) =>
        {
            id.addAmmo();
        });

        powerups.Add((id) =>
        {
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
            Destroy(gameObject);
        }
    }
}