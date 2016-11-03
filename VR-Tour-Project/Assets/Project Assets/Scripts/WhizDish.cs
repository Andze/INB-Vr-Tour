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

    private int iStartTimeout;
    private const int frequency = 2000;
    private float[] samples;
    private int deviceIndex;
    private AudioSource audioSrc;
    private float directionMultiplier;

    void Start()
    {
        iStartTimeout = 25;
        samples = new float[frequency];
        deviceIndex = 0;
        audioSrc = gameObject.GetComponent<AudioSource>();

        if (orientationFocus == null)
            orientationFocus = gameObject;

        directionMultiplier = inverseDirection ? -1f : 1f;

        StartMicListener();
    }
    
    void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying)
            StartMicListener();

        if (iStartTimeout > 0)
            iStartTimeout = iStartTimeout - 1;

        AnalyzeSound();
        UpdateMovement();
    }
    
    private void StartMicListener()
    {
        Debug.Log(string.Format("StartMicListener called devices_count: {0}", Microphone.devices.Length));

        // Disable Vive Teleportation if WizDish is plugged in.
        if (Microphone.devices.Length >= 2)
            GetComponentInChildren<TeleportVive>().enabled = false;
        else enabled = false;

        if (Microphone.devices.Length > 0)
        {
            audioSrc.clip = Microphone.Start(Microphone.devices[deviceIndex], true, 1, frequency);
            audioSrc.loop = true;
            audioSrc.mute = false; // true; // Mute the sound, we don’t want the player to hear it
            
            while (!(Microphone.GetPosition(Microphone.devices[deviceIndex]) > 0))
                Debug.Log(string.Format("StartMicListener called position {0}", Microphone.GetPosition(Microphone.devices[deviceIndex])));

            Debug.Log(string.Format("StartMicListener before play is called."));
            audioSrc.Play();
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
        Debug.Log(wizAudio);

        for (int i = 0; i < 2000; i++)
            samples[i] = 0;
    }

    private void UpdateMovement()
    {
        bool moveForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        float micSpeed = wizAudio;
        if (micSpeed > .1)
            moveForward = true;
        else
        {
            moveForward = false;
            micSpeed = 0;
        }

        if (moveForward && orientationFocus != null)
            transform.position += MultiplyVector(orientationFocus.transform.up, new Vector3(1, 0, 1))
                * directionMultiplier * Time.deltaTime * movementSpeed;
    }

    private Vector3 MultiplyVector(Vector3 a, Vector3 b)
    {
        float x = a.x * b.x;
        float y = a.y * b.y;
        float z = a.z * b.z;

        return new Vector3(x, y, z);
    }
}
