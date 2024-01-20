using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerScript : MonoBehaviour
{
    public float flingStrength = 0.01f;

    //Allows access to the camera's variables
    public GameObject CameraGet;
    Rigidbody2D playerBody;

    float mouseButtonHoldTime = 0.0f;
    bool mouseButtonHeld = false;
    public float screenShakeIntensity = 0.1f;

    //When Player Press Down MB0
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private Coroutine colorCoroutine;
    public float changeSpeed = 500f;
    public GameObject pointer;


    void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();

        CameraGet = GameObject.FindGameObjectWithTag("MainCamera");

        //When Player Press Down MB0
        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.saturation.value = 100f;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointer.SetActive(true);
            ApplyBlackAndWhiteEffect(true);
            Time.timeScale = 0.2f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            mouseButtonHoldTime = 0.0f;
        }

        if (Input.GetMouseButton(0))
        {
            PointToMouse();
            ScalePointer();
            mouseButtonHoldTime += Time.deltaTime;
        }
        
    if (Input.GetMouseButtonUp(0))
        {
            ApplyBlackAndWhiteEffect(false);
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            pointer.SetActive(false);
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


        if (Input.GetMouseButton(0))
        {

            //Debug.Log("mouse button held down");
            mouseButtonHoldTime += Time.deltaTime;

            /*if (mouseButtonHoldTime > 3f) //Caps the strength so we don't get insanely wacky shit with forces of like, a bajillion.
            {
                mouseButtonHoldTime = 3f;
            }*/

            mouseButtonHeld = true;
        }
        else
        {
            if (mouseButtonHeld)
            {
                mouseButtonHeld = false;


                Debug.Log("Ball still exists");
                Debug.Log("mouseButtonHoldTime: " + mouseButtonHoldTime);

                playerBody.AddForce(playerToMouse.normalized * flingStrength * mouseButtonHoldTime,
                    ForceMode2D.Impulse);
                mouseButtonHoldTime = 0f;
            }
        }
    }

    void ApplyBlackAndWhiteEffect(bool apply)
    {
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }

        colorCoroutine = StartCoroutine(AdjustSaturation(apply));
    }

    IEnumerator AdjustSaturation(bool toBlackAndWhite)
    {
        float targetSaturation = toBlackAndWhite ? -80 : 0;
        float currentSaturation = colorAdjustments.saturation.value;

        while (toBlackAndWhite ? currentSaturation > targetSaturation : currentSaturation < targetSaturation)
        {
            currentSaturation = Mathf.MoveTowards(currentSaturation, targetSaturation, Time.deltaTime * changeSpeed);
            colorAdjustments.saturation.value = currentSaturation;
            yield return null;
        }
    }

    void PointToMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;

        pointer.transform.position = transform.position + direction * 0.25f;
        pointer.transform.up = direction;
    }

    void ScalePointer()
    {
        float maxScaleY = 1.25f;
        float scaleY = Mathf.Clamp(mouseButtonHoldTime, 0.1f, maxScaleY);
        pointer.transform.localScale = new Vector3(pointer.transform.localScale.x, scaleY, pointer.transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            CameraGet.GetComponent<ShakeBehaviour>().shakeDuration = 0.2f * playerBody.velocity.magnitude * screenShakeIntensity;
        }
    }
}