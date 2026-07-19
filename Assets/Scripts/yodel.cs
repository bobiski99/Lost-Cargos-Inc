using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class yodel : MonoBehaviour
{
    [SerializeField] private AudioClip yodelSource;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        audioSource.playOnAwake = false;
        audioSource.clip = yodelSource;
    }

    private void OnMouseDown()
    {
        Box bx = GetComponent<Box>();
        if (bx.isOnTable == true)
        {
            audioSource.Play();
        }
        
    }
}