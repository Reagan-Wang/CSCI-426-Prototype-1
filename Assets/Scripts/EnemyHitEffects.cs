using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitEffects : MonoBehaviour
{
    public ParticleSystem ball;
    public ParticleSystem wallParticleSystem;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 hitPoint = collision.contacts[0].point;
            wallParticleSystem.transform.position = hitPoint;
            wallParticleSystem.Play();
        }
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 hitPoint = collision.contacts[0].point;
            ball.transform.position = hitPoint;
            ball.Play();
        }
    }
}
