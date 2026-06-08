/*
* Author: Zhi hng
* Date: 8 June 2026
* Description: Plays collect audio and deletes collectible
*/

using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    public int collectibleScore = 0; // Store the score value of this collectible, editable from the Unity Inspector. (this allows different collectibles to be worth different amounts of points)

    AudioSource collectibleAudio;

    [SerializeField]
    AudioClip collectibleAudioClip;

    void Start()
    {
        collectibleAudio = GetComponent<AudioSource>(); // Get the AudioSource component attached to this GameObject so we can play a sound when collected
    }

    public void Collect() // Custom method to handle the collection of this item, called from the PlayerScript when the player interacts with it
    {
        // if(collectibleAudio != null) // Check if the AudioSource component exists before trying to play a sound
        // {
        //     collectibleAudio.Play(); // Play the collection sound effect to provide feedback to the player
        // }
        // SOLUTION 1
        //Destroy(gameObject, collectibleAudio.clip.length); // Destroy this GameObject after the sound has finished playing to remove it from the scene
    
        // SOLUTION 2
        AudioSource.PlayClipAtPoint(collectibleAudioClip, transform.position);
        Destroy(gameObject); // Immediately destroy this GameObject to remove it from the scene, allowing the sound to play independently
    }
}
