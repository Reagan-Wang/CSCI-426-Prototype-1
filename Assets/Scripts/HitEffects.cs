using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour
{
    public float hitStopDuration = 0.1f;

    public ParticleSystem enemyParticleSystem;
    public ParticleSystem wallParticleSystem;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
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
