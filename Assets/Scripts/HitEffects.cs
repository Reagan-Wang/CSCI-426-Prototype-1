using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HitEffects : MonoBehaviour
{
    public float hitStopMultiplier = 0.1f;
    public float hitStopDuration = 0.5f ;

    private PlayerScript playerScript;
    private Rigidbody2D playerBody;

    public Volume volume;
    
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerScript = GetComponent<PlayerScript>();
    }

    public ParticleSystem enemyParticleSystem;
    public ParticleSystem wallParticleSystem;
    public ParticleSystem enemyParticleSystem2;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && playerBody.velocity.magnitude > playerScript.hitStopCutoff)
        {
            Vector2 hitPoint = collision.contacts[0].point;

            if (playerScript.particlesOn)
            {
                enemyParticleSystem.transform.position = hitPoint;
                enemyParticleSystem.Play();
                enemyParticleSystem2.transform.position = hitPoint;
                enemyParticleSystem2.Play();
            }

            if (playerScript.hitstopOn)
            {
                StartCoroutine(AdjustPostExposure(3f, 0.1f));
                StartCoroutine(HitStop());
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 hitPoint = collision.contacts[0].point;
            if (playerScript.particlesOn)
            {
                wallParticleSystem.transform.position = hitPoint;
                wallParticleSystem.Play();
            }

            if (playerScript.hitstopOn && playerBody.velocity.magnitude > playerScript.hitStopCutoff)
            {
                StartCoroutine(HitStop());
            }
        }
    }
    
    private IEnumerator AdjustPostExposure(float targetValue, float duration)
    {
        if (volume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            float originalValue = 0;
            colorAdjustments.postExposure.value = targetValue;

            yield return new WaitForSeconds(duration);

            colorAdjustments.postExposure.value = originalValue;
        }
    }
    
    
    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f; 
    }
}
