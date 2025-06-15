using UnityEngine;
using TMPro;

public class PlayerBehaviour : MonoBehaviour
{
    public int maxHealth = 100;
    public float currentHealth;
    public HealthBar healthbar;
    public AudioClip damageSound; // Assign in Inspector
    private AudioSource audioSource; // Reference to the AudioSource component for sound effects

    private float damageTimer = 0f;
    public float damageInterval = 1f; // seconds between damage ticks

    public Vector3 respawnPoint; // Assign this in the Inspector

    int points = 0;
    CoinBehaviour currentCoin;
    public TMP_Text scoreText; // Assign in Inspector
    public TMP_Text prizesLeftText; // Assign in Inspector
    public TMP_Text finalScoreText; // Assign in Inspector
    public TMP_Text keycardStatusText; // Assign in Inspector


    private DoorBehaviour currentDoor;
    bool canInteractDoor = false;
    private bool canCollectKeycard = false;
    private GameObject keycardInRange;
    private bool hasKeycard = false;
    private bool canCollectPrize = false;
    private GameObject prizeInRange;
    public int prizesCollected = 0;
    public int totalPrizes = 3;
    public GameObject victoryUI; // Assign in Inspector

    void Start()
    {
        currentHealth = maxHealth;  // Initialize health bar with max health
        healthbar.SetSlider(currentHealth);
        UpdatePrizesLeftUI(); // Initialize prizes left UI
        UpdateKeycardStatusUI(); // Initialize keycard status UI
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on PlayerBehaviour.");
        }
    }

    public void UpdateScoreUI()
    {
        scoreText.text = "Score: " + points;
    }

    public void UpdatePrizesLeftUI()
    {
        int prizesLeft = totalPrizes - prizesCollected;
        prizesLeftText.text = "Prizes left: " + prizesLeft + "/" + totalPrizes;
    }

    public void UpdateKeycardStatusUI()
    {
        if (hasKeycard)
            keycardStatusText.text = "Keycard: Collected";
        else
            keycardStatusText.text = "Keycard: Not Collected";
    }

    public void ModifyScore(CoinBehaviour currentCoin)
    {
        points += currentCoin.coinValue;
        UpdateScoreUI();
        Debug.Log("Score" + points);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            currentCoin = collision.gameObject.GetComponent<CoinBehaviour>();
            if (currentCoin != null)
            {
                currentCoin.Collect(this);
            }
        }

        else if (collision.gameObject.CompareTag("healArea"))
        {
            HealBox healArea = collision.gameObject.GetComponent<HealBox>();
            if (healArea != null)
            {
                currentHealth += healArea.healAmount;
                if (currentHealth > maxHealth)
                    currentHealth = maxHealth; // Ensure health does not exceed max health
                healthbar.SetSlider(currentHealth); // Update health bar
                Debug.Log("Player healed! Current health: " + currentHealth);
                healArea.PlayHealSound(); // Play healing sound
                Destroy(healArea.gameObject); // Destroy the heal box after use
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("damageArea"))
        {
            DamageArea damageArea = other.gameObject.GetComponent<DamageArea>();
            if (damageArea != null)
            {
                currentHealth -= damageArea.damageAmount;
                if (damageSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(damageSound);
                }
                if (currentHealth < 0)
                    currentHealth = 0; // Ensure health does not go below zero
                healthbar.SetSlider(currentHealth);
                Debug.Log("Player damaged! Current health: " + currentHealth);
                if (currentHealth == 0)
                {
                    Die();  // Call the Die method if health reaches zero
                }
            }
        }

        else if (other.gameObject.CompareTag("Door"))
        {
            DoorBehaviour door = other.gameObject.GetComponent<DoorBehaviour>();
            if (door != null)
            {
                currentDoor = other.GetComponent<DoorBehaviour>();
                canInteractDoor = true; // Allow interaction with the door
            }
        }

        else if (other.gameObject.CompareTag("Prize"))
        {
            canCollectPrize = true;
            prizeInRange = other.gameObject; // Store the prize GameObject
        }

        else if (other.gameObject.CompareTag("Keycard"))
        {
            canCollectKeycard = true;
            keycardInRange = other.gameObject; // Store the keycard GameObject
            Debug.Log("Entered keycard area, canCollectKeycard set to true.");
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            DoorBehaviour door = other.gameObject.GetComponent<DoorBehaviour>();
            if (door != null && currentDoor == door)
            {
                door.Close(); // Close the door when exiting
                currentDoor = null;
                canInteractDoor = false; // Disable interaction with the door
            }
        }

        else if (other.gameObject.CompareTag("damageArea"))
        {
            damageTimer = 0f; // Reset damage timer when exiting the damage area
        }

        else if (other.gameObject.CompareTag("Prize"))
        {
            canCollectPrize = false;
            prizeInRange = null; // Clear the stored prize GameObject
        }

        else if (other.gameObject.CompareTag("Keycard"))
        {
            canCollectKeycard = false;
            keycardInRange = null; // Clear the stored keycard GameObject
            Debug.Log("Exited keycard area, canCollectKeycard set to false.");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("damageArea"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                DamageArea damageArea = other.gameObject.GetComponent<DamageArea>();
                if (damageArea != null)
                {
                    currentHealth -= damageArea.damageAmount;
                    if (damageSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(damageSound);
                    }
                    if (currentHealth < 0)
                        currentHealth = 0;
                    healthbar.SetSlider(currentHealth);
                    Debug.Log("Player taking damage! Current health: " + currentHealth);
                    if (currentHealth == 0)
                    {
                        Die();
                    }
                }
                damageTimer = 0f; // Reset timer after applying damage
            }
        }
    }

    public void OnInteract()
    {
        if (canInteractDoor && currentDoor != null)
        {
            currentDoor.Interact();
        }
        else if (canCollectPrize && prizeInRange != null)
        {
            PrizeBehaviour prizeBehaviour = prizeInRange.GetComponent<PrizeBehaviour>();
            if (prizeBehaviour != null)
            {
                prizeBehaviour.PlayCollectEffect(); // Play collection effect
            }

            prizesCollected++;
            UpdatePrizesLeftUI(); // Update the UI to reflect the number of prizes left
            Debug.Log("Prize collected! Total prizes: " + prizesCollected);

            Destroy(prizeInRange); // Destroy the prize GameObject
            canCollectPrize = false; // Reset collection state
            prizeInRange = null; // Clear the stored prize GameObject     

            if (prizesCollected >= totalPrizes)
            {
                ShowVictoryUI(); // Show victory UI when all prizes are collected
            }
        }

        else if (canCollectKeycard && keycardInRange != null)
        {
            // Play the keycard's collect sound
            KeyBehaviour keyScript = keycardInRange.GetComponent<KeyBehaviour>();
            if (keyScript != null && keyScript.collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(keyScript.collectSound);
            }

            DoorBehaviour[] allDoors = FindObjectsByType<DoorBehaviour>(FindObjectsSortMode.None);
            foreach (DoorBehaviour door in allDoors)
            {
                door.DoorUnlock();
            }
            hasKeycard = true;
            UpdateKeycardStatusUI();
            Debug.Log("Keycard collected! All doors unlocked.");
            Destroy(keycardInRange);
            keycardInRange = null;
            canCollectKeycard = false;
        }
    }

    void Die()
    {
        Debug.Log("Player has died.");
        currentHealth = maxHealth; // Reset health to max
        healthbar.SetSlider(currentHealth); // Update health bar
        transform.position = respawnPoint; // Respawn at the set respawn point
    }

    void ShowVictoryUI()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + points; // Display final score
            }
            else
            {
                Debug.LogWarning("Final Score Text UI is not assigned in the Inspector.");
            }
        }
        else
        {
            Debug.LogWarning("Victory UI GameObject is not assigned in the Inspector.");
        }
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
        Debug.Log("Respawn point set to: " + respawnPoint);
    }

}