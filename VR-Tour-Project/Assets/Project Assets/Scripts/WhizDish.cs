using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WhizDish : MonoBehaviour
{
    public float wizAudio = 0.0f;
    public float volume;
    public float movementSpeed = 2.5f;
    public float amplify = 25;
    public GameObject orientationFocus;
    public bool inverseDirection = false;
    public float dampening = 0.5f;

    private int iStartTimeout;
    private const int frequency = 2000;
    private float[] samples;
    private AudioSource audioSrc;
    private float directionMultiplier;
    private Vector3 velocity;

    void Start()
    {
        iStartTimeout = 25;
        samples = new float[frequency];
        audioSrc = gameObject.GetComponent<AudioSource>();

        if (orientationFocus == null)
            orientationFocus = gameObject;

        directionMultiplier = inverseDirection ? -1f : 1f;

        StartMicListener();
    }

    //float elapsedTime = 0.0f;
    void Update()
    {
        //elapsedTime += Time.deltaTime;

        if (!GetComponent<AudioSource>().isPlaying)
            StartMicListener();

        if (iStartTimeout > 0)
            iStartTimeout = iStartTimeout - 1;

        AnalyzeSound();
        //CalculateStepSpeed();
        UpdateMovement();
    }
    
    private void StartMicListener()
    {
        Debug.Log(string.Format("StartMicListener called devices_count: {0}", Microphone.devices.Length));

        // Check if a valid Microphone is connected.
        bool wizConnected = false;
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            if (Microphone.devices[i] == "Microphone (Realtek High Definition Audio)")
            {
                wizConnected = true;
                break;
            }
        }

        // Disable Vive Teleportation if WizDish is plugged in.
        if (wizConnected)
        {
            Debug.Log("WizDish connected.");

            TeleportVive tV = GetComponentInChildren<TeleportVive>();
            if (tV != null) tV.enabled = false;

            audioSrc.clip = Microphone.Start(null, true, 1, frequency);
            audioSrc.loop = true;
            audioSrc.mute = false; // true; // Mute the sound, we don’t want the player to hear it

            while (!(Microphone.GetPosition(null) > 0))
                Debug.Log(string.Format("StartMicListener called position {0}", Microphone.GetPosition(null)));
            
            audioSrc.Play();
        }
        else
        {
            Debug.Log("No WizDish connected.");
            enabled = false;
        }
    }

    private void AnalyzeSound()
    {
        if (audioSrc.isPlaying)
        {
            audioSrc.GetOutputData(samples, 0);
            Debug.Log(string.Format("AnalyzeSound samples_count: {0}", samples.Length));

            float sum = 0.0f;
            if (iStartTimeout < 1)
            {
                for (int i = 0; i < frequency; i++)
                {
                    if (samples[i] < 0)
                        sum -= samples[i];
                    else sum += samples[i];
                }
                wizAudio = sum / (float)frequency;
            }
        }

        wizAudio *= amplify;

        for (int i = 0; i < 2000; i++)
            samples[i] = 0;
    }
    

    //bool isStepping = false;
    //float stepStart = 0.0f;
    //float stepTime = 0.0f;
    //private void CalculateStepSpeed()
    //{
    //    if (!isStepping)
    //    {
    //        if (wizAudio > 0.1f)
    //        {
    //            isStepping = true;
    //            stepStart = elapsedTime;
    //        }
    //    }
    //    else
    //    {
    //        if (wizAudio < 0.1f)
    //        {
    //            isStepping = false;
    //            stepTime = elapsedTime - stepStart;
    //        }
    //    }
    //}

    private void UpdateMovement()
    {
        bool moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        float micSpeed = wizAudio;
        if (micSpeed > 0.35f)
            moveForward = true;
        else
        {
            moveForward = false;
            micSpeed = 0;
        }

        float speedMod = micSpeed > 1f ? micSpeed * 2f : 1f;

        if (moveForward && orientationFocus != null)
        {
            velocity = MultiplyVector(orientationFocus.transform.up, new Vector3(1, 0, 1))
                * directionMultiplier * Time.deltaTime * movementSpeed * speedMod;
        }
        
        transform.position += velocity;

        if (!VectorLessThan(velocity, 0.01f))
            velocity *= dampening;
        else velocity = Vector3.zero;


        //Debug.Log(stepTime);
        //if (orientationFocus != null && wizAudio > 0.1f)
        //{
        //    float speedMod = 1f - stepTime;

        //    if (speedMod > 0f)
        //        transform.position += MultiplyVector(orientationFocus.transform.up, new Vector3(1, 0, 1))
        //            * directionMultiplier * Time.deltaTime * movementSpeed * speedMod;
        //}
    }

    private Vector3 MultiplyVector(Vector3 a, Vector3 b)
    {
        float x = a.x * b.x;
        float y = a.y * b.y;
        float z = a.z * b.z;

        return new Vector3(x, y, z);
    }

    private bool VectorLessThan(Vector3 a, float b)
    {
        bool cX = Mathf.Abs(a.x) < b;
        bool cY = Mathf.Abs(a.y) < b;
        bool cZ = Mathf.Abs(a.z) < b;

        if (cX && cY && cZ)
            return true;
        else return false;
    }
}
