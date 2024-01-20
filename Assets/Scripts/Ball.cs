using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    AudioSource ballAudio;
    Rigidbody2D ballBody;
    public float ballMaxVolumeVelocityCutoff = 15f;

    // Start is called before the first frame update
    void Start()
    {
        ballAudio = GetComponent<AudioSource>();
        ballBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag != "Player")
        {
            ballAudio.volume = ballBody.velocity.magnitude / ballMaxVolumeVelocityCutoff;
            ballAudio.pitch = ballMaxVolumeVelocityCutoff / ballBody.velocity.magnitude;
            ballAudio.Play();
        }
    }
}
