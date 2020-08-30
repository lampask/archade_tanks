using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Powerup : MonoBehaviour
{
    List<Action<PlayerBehaviour>> powerups = new List<Action<PlayerBehaviour>>();

    private void Start()
    {
        powerups.Add((id) => {
            id.lives++;
            Instantiate(id.hearthImage, id.livesContainer);
        });

        powerups.Add((id) =>
        {
            id.addAmmo();
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            powerups[UnityEngine.Random.Range(0, powerups.Count)].Invoke(other.GetComponent<PlayerBehaviour>());
            Destroy(gameObject);
        }
    }
}