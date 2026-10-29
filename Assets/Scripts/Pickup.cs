using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{   
    [SerializeField, Header("How many points this pickup is worth")]
    private int value = 10;

    [SerializeField, Header("Optional sound effect")]
    private AudioClip pickupSound;
    [Header("More or other effects? Connect EffectTrigger here.")]
    public UnityEvent OnPickup;

    public SoundManager soundManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))       
        {
            if (pickupSound != null)
            {
                //soundManager.PlaySFX(pickupSound);
                
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
