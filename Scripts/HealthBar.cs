using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // Reference to the UI Slider component

    public void SetSlider(float currentHealth)
    {
        healthSlider.value = currentHealth;
    } 
}
