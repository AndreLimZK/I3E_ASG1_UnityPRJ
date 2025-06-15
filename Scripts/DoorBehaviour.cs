using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    private bool isOpen = false; // controls the state of the door
    public bool isDoorLocked = true; // Set door locked status
    private AudioSource doorAudio; // Reference to the AudioSource component for sound effects

    void Awake()
    {
        doorAudio = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (isDoorLocked)
        {
            Debug.Log("Door is locked! Find a key to unlock it.");
            return; // Exit if the door is locked
        }
        if (!isOpen)
        {
            OpenDoor();
        }
        else
        {
            Close();
        }
    }

    public void OpenDoor()
    {
        Vector3 doorRotation = transform.eulerAngles;
        doorRotation.y -= 90f; // Rotate the door by 90 degrees
        transform.eulerAngles = doorRotation;
        isOpen = true;
        Debug.Log("Door opened!");

        if (doorAudio != null)
        {
            doorAudio.Play(); // Play door opening sound
        }
    }

    public void Close()
    {
        if (isOpen)
        {
            Vector3 doorRotation = transform.eulerAngles;
            doorRotation.y += 90f; // Rotate the door back to closed position
            transform.eulerAngles = doorRotation;
            isOpen = false;
            Debug.Log("Door closed!");
        }
    }

    public void DoorUnlock()
    {
        isDoorLocked = false; // Set door unlocked status
        Debug.Log("Door is unlocked.");
    }
}
