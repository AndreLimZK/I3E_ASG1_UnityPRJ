using UnityEngine;

public class PrizeBehaviour : MonoBehaviour
{
    public AudioClip collectSound;

    public void PlayCollectEffect()
    {
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        // Add particle effects or animation here if desired
    }
}