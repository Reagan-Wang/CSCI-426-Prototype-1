using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public float flingStrength = 0.01f;

    //Text

    public TextMeshProUGUI toggles;

    //Player controls
    [SerializeField]
    private InputActionReference screenshakeToggle, audioToggle, screenFlashToggle, hitstopToggle, bulletTimeToggle, particlesToggle, impactFlashToggle, trailsToggle, foreverTrailsToggle;

    //Allows access to the camera's variables
    public GameObject CameraGet;
    Rigidbody2D playerBody;

    //Allows access to trail renderer component
    public TrailRenderer trailRenderer;

    //Makes the mouse work
    float mouseButtonHoldTime = 0.0f;
    bool mouseButtonHeld = false;

    //Gamefeel variables
    public float screenShakeIntensity = 0.1f;
    public float minimumStrength = 0.5f;
    public float chargeSpeed = 2f;
    public float hitStopCutoff = 1f;
    public float screenShakeMinimumTime = 0.5f;

    //Audio
    AudioSource playerAudioSource;
    bool m_Play;
    public float velocityMaxVolumeCutoff = 20f;

    //Toggle gamefeel elements

    public bool screenshakeOn = true;
    public bool audioOn = true;
    public bool screenFlashOn = true;
    public bool hitstopOn = true;
    public bool bulletTimeOn = true;
    public bool particlesOn = true;
    public bool impactFlashOn = true;
    public bool trailsOn = true;
    public bool foreverTrails = false;

    //When Player Press Down MB0
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private Coroutine colorCoroutine;
    public float changeSpeed = 500f;
    public GameObject pointer;
    private float originalHue;
    private Coroutine hueShiftCoroutine;
    public float hueShiftAmount = 40f;
    public float hueShiftSpeed = 200f;
    
    //Bullet Time
    public AudioClip bulletTimeAudio;

    void Awake()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerAudioSource = GetComponent<AudioSource>();
        trailRenderer = GetComponent<TrailRenderer>();

        CameraGet = GameObject.FindGameObjectWithTag("MainCamera");

        //When Player Press Down MB0
        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            if (bulletTimeOn) {
            colorAdjustments.saturation.value = 100f;
            originalHue = colorAdjustments.hueShift.value;
            }
        }
    }

    void Update()
    {
        if (audioToggle.action.WasPressedThisFrame())
        {
            audioOn = !audioOn;
        }

        if (screenFlashToggle.action.WasPressedThisFrame())
        {
            screenFlashOn = !screenFlashOn;
        }

        if (screenshakeToggle.action.WasPressedThisFrame())
        {
            screenshakeOn = !screenshakeOn;
        }

        if (hitstopToggle.action.WasPressedThisFrame())
        {
            hitstopOn = !hitstopOn;
        }

        if (bulletTimeToggle.action.WasPressedThisFrame())
        {
            bulletTimeOn = !bulletTimeOn;
        }

        if (particlesToggle.action.WasPressedThisFrame())
        {
            particlesOn = !particlesOn;
        }

        if (impactFlashToggle.action.WasPressedThisFrame())
        {
            impactFlashOn = !impactFlashOn;
        }

        if (trailsToggle.action.WasPressedThisFrame()){
            trailsOn = !trailsOn;
        }

        if (foreverTrailsToggle.action.WasPressedThisFrame())
        {
            foreverTrails = !foreverTrails;
        }

        if (!trailsOn)
        {
            trailRenderer.enabled = false;
        }
        else
        {
            trailRenderer.enabled = true;
        }

        if (foreverTrails)
        {
            trailRenderer.time = float.PositiveInfinity;
        }
        else
        {
            trailRenderer.time = 1f;
        }

        toggles.text = "Screenshake: " + screenshakeOn + "\n" + 
            "Audio: " + audioOn + "\n" +
            "Screen Flash: " + screenFlashOn + "\n" +
            "Hitstop: " + hitstopOn + "\n" +
            "Bullet Time: " + bulletTimeOn + "\n" +
            "Particles: " + particlesOn + "\n" +
            "Impact Flash: " + impactFlashOn + "\n" +
            "Trails On: " + trailsOn + "\n" +
            "Forever Trails: " + foreverTrails;


        if (Input.GetMouseButtonDown(0))
        {
            pointer.SetActive(true);
            if (bulletTimeOn)
            {
                ApplyBlackAndWhiteEffect(true);
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }

            if (audioOn)
            {
                playerAudioSource.PlayOneShot(bulletTimeAudio);
            }

            if (bulletTimeOn)
            {
                if (hueShiftCoroutine != null)
                    StopCoroutine(hueShiftCoroutine);
                hueShiftCoroutine = StartCoroutine(AdjustHueShift(true));
            }
            
            mouseButtonHoldTime = 0.0f;
        }

        if (Input.GetMouseButton(0))
        {
            PointToMouse();
            ScalePointer();
            mouseButtonHoldTime += Time.deltaTime;
            
            // float timeScale = Mathf.Clamp(1.0f - mouseButtonHoldTime / 3.0f, 0.1f, 1.0f);
            // Time.timeScale = timeScale;
            // Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        
    if (Input.GetMouseButtonUp(0))
        {
            ApplyBlackAndWhiteEffect(false);
            ApplyHueShift(false);
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            pointer.SetActive(false);
            
            if (hueShiftCoroutine != null)
                StopCoroutine(hueShiftCoroutine);
            hueShiftCoroutine = StartCoroutine(AdjustHueShift(false));
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
            mouseButtonHoldTime += Time.deltaTime * chargeSpeed;

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

                playerBody.AddForce(playerToMouse.normalized * flingStrength * (mouseButtonHoldTime + minimumStrength),
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
    
    void ApplyHueShift(bool apply)
    {
        if (apply)
        {
            colorAdjustments.hueShift.value = hueShiftAmount;
        }
        else
        {
            colorAdjustments.hueShift.value = originalHue;
        }
    }

    IEnumerator AdjustSaturation(bool toBlackAndWhite)
    {
        float targetSaturation = toBlackAndWhite ? -100 : 0;
        float currentSaturation = colorAdjustments.saturation.value;

        while (toBlackAndWhite ? currentSaturation > targetSaturation : currentSaturation < targetSaturation)
        {
            currentSaturation = Mathf.MoveTowards(currentSaturation, targetSaturation, Time.deltaTime * changeSpeed);
            colorAdjustments.saturation.value = currentSaturation;
            yield return null;
        }
    }
    
    IEnumerator AdjustHueShift(bool toShift)
    {
        float targetHue = toShift ? hueShiftAmount : 0;
        float currentHue = colorAdjustments.hueShift.value;

        while (!Mathf.Approximately(currentHue, targetHue))
        {
            currentHue = Mathf.MoveTowards(currentHue, targetHue, Time.deltaTime * hueShiftSpeed);
            colorAdjustments.hueShift.value = currentHue;
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
        playerAudioSource.volume = playerBody.velocity.magnitude / velocityMaxVolumeCutoff;
        //playerAudioSource.pitch = velocityMaxVolumeCutoff / playerBody.velocity.magnitude;

        if (audioOn)
        {
            playerAudioSource.Play();
        }

        if (playerBody.velocity.magnitude > hitStopCutoff && screenshakeOn)
        {
            CameraGet.GetComponent<ShakeBehaviour>().shakeDuration = (0.2f * playerBody.velocity.magnitude * screenShakeIntensity + screenShakeMinimumTime);
        }
    }
}