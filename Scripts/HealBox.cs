using UnityEngine;

public class HealBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int healAmount = 10;
    public AudioClip healSound; // Assign in Inspector

    public void PlayHealSound()
    {
        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position);
        }
    }

}
