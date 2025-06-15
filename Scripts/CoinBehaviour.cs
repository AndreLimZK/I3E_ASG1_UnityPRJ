using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public int coinValue = 1;
    public AudioClip collectSound; // Assign in Inspector

    public void Collect(PlayerBehaviour player)
    {
        player.ModifyScore(this);
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        Destroy(gameObject);
    }

}
