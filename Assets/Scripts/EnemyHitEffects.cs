using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitEffects : MonoBehaviour
{
    public ParticleSystem ball;
    public ParticleSystem wallParticleSystem;
    public GameObject player;
    public PlayerScript playerScript;

    //Hit Flash
    public Color flashColor = Color.white;
    private Color originalColor;
    public float flashDuration = 0.05f;
    public SpriteRenderer enemySpriteRenderer;
    
    AudioSource playerAudioSource;
    public AudioClip wallAudio;

    private void Awake()
    {
        originalColor = enemySpriteRenderer.color;
        playerAudioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 hitPoint = collision.contacts[0].point;

            if (playerScript.particlesOn)
            {
                wallParticleSystem.transform.position = hitPoint;
                wallParticleSystem.Play();
            }
            if (playerScript.audioOn)
            {
                playerAudioSource.PlayOneShot(wallAudio);
            }
        }
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitPoint = collision.contacts[0].point;

            if (playerScript.particlesOn)
            {
                ball.transform.position = hitPoint;
                ball.Play();
            }
            if (playerScript.impactFlashOn)
            {
                StartCoroutine(FlashColor());
            }
        }
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (enemySpriteRenderer != null && playerScript.impactFlashOn)
            {
                StartCoroutine(FlashColor());
            }
        }
        
        
IEnumerator FlashColor()
{
    int flashTimes = 2;
            
    for (int i = 0; i < flashTimes; i++)
    {
        enemySpriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        enemySpriteRenderer.color = originalColor;
        yield return new WaitForSeconds(flashDuration);
    }
        }
    }
}
