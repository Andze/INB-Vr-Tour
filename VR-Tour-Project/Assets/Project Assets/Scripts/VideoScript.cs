using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class VideoScript : MonoBehaviour
{
    private MovieTexture movie;
    private AudioSource audioSource;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        movie = (MovieTexture)r.material.mainTexture;
        if (movie != null)
        {
            movie.Play();
            movie.loop = true;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
}
