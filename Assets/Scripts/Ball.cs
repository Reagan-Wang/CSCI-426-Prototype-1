using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    AudioSource ballAudio;
    Rigidbody2D ballBody;

    public GameObject player;
    public PlayerScript playerScript;
    public TrailRenderer trailRenderer;

    public float ballMaxVolumeVelocityCutoff = 15f;

    // Start is called before the first frame update
    void Start()
    {
        ballAudio = GetComponent<AudioSource>();
        ballBody = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerScript.trailsOn)
        {
            trailRenderer.enabled = false;
        }
        else
        {
            trailRenderer.enabled = true;
        }
        if (playerScript.foreverTrails)
        {
            trailRenderer.time = float.PositiveInfinity;
        }
        else
        {
            trailRenderer.time = 1f;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player")
        {
            ballAudio.volume = ballBody.velocity.magnitude / ballMaxVolumeVelocityCutoff;
            ballAudio.pitch = ballMaxVolumeVelocityCutoff / ballBody.velocity.magnitude;
            if (playerScript.audioOn)
            {
                ballAudio.Play();
            }
        }
    }
}
