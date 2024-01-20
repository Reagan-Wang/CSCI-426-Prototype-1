using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float flingStrength = 0.01f;
    Rigidbody2D playerBody;

    float mouseButtonHoldTime = 0.0f;
    bool mouseButtonHeld = false;

    // Start is called before the first frame update
    void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
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
}
