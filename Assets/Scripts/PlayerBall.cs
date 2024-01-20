using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerScript : MonoBehaviour
{
    public float flingStrength = 0.01f;
    Rigidbody2D playerBody;

    float mouseButtonHoldTime = 0.0f;
    bool mouseButtonHeld = false;

    //When Player Press Down MB0
    private float originalTimeScale = 1f;

    void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ApplyBlackAndWhiteEffect(true);
            Time.timeScale = 0.5f; // 减缓时间
        }

        if (Input.GetMouseButtonUp(0))
        {
            ApplyBlackAndWhiteEffect(false);
            Time.timeScale = originalTimeScale; // 恢复时间
        }
    }


    void FixedUpdate()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos.z = 10f;

        //Old: jumps directly to mouse

        //transform.position = mousePos;


        //New: tracks to mouse

        Vector3 playerPosition = transform.position;
        Vector3 playerToMouse;

        playerToMouse = mousePos - playerPosition;

        
        //Caps
        if (Input.GetMouseButton(0))
        {

            //Debug.Log("mouse button held down");
            mouseButtonHoldTime += Time.deltaTime;

            if (mouseButtonHoldTime > 3f) //Caps the strength so we don't get insanely wacky shit with forces of like, a bajillion.
            {
                mouseButtonHoldTime = 3f;
            }

            mouseButtonHeld = true;
        }
        else
        {
            if (mouseButtonHeld)
            {
                mouseButtonHeld = false;
                playerBody.AddForce(playerToMouse.normalized * flingStrength * mouseButtonHoldTime, ForceMode2D.Impulse);
                mouseButtonHoldTime = 0f;
            }
        }
    }

    void ApplyBlackAndWhiteEffect(bool apply)
    {
        if (apply)
        {
            
        }
        else
        {

        }
    }
}