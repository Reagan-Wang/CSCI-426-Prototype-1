using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour
{
    public float hitStopMultiplier = 0.1f;
    private float hitStopDuration;

    private PlayerScript playerScript;
    private Rigidbody2D playerBody;
    
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerScript = GetComponent<PlayerScript>();
    }

    public ParticleSystem enemyParticleSystem;
    public ParticleSystem wallParticleSystem;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && playerBody.velocity.magnitude > playerScript.hitStopCutoff)
        {
            Vector2 hitPoint = collision.contacts[0].point;
            enemyParticleSystem.transform.position = hitPoint;
            enemyParticleSystem.Play();
            
            StartCoroutine(HitStop());
        }
        
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 hitPoint = collision.contacts[0].point;
            wallParticleSystem.transform.position = hitPoint;
            wallParticleSystem.Play();
            
            StartCoroutine(HitStop());
        }
        
    }
    
    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f; 
    }
}
