/*
* Author: Zhi hng
* Date: 13 June 2026
* Description: Plays collect audio and deletes collectible
*/

using UnityEngine;

public class CollectibleScript : MonoBehaviour
{
    /// <summary>
    /// Score that this collectible will provide to the player
    /// </summary>
    public int collectibleScore = 0; // Store the score value of this collectible, editable from the Unity Inspector. (this allows different collectibles to be worth different amounts of points)
    /// <summary>
    /// Volume of the audio which this collectible will play when collected
    /// </summary>
    [SerializeField]
    float volume;
    /// <summary>
    /// Audio that this collectible will play when collected
    /// </summary>
    [SerializeField]
    AudioClip collectibleAudioClip;
    /// <summary>
    /// Plays the audio linked to the collectible and deletes the collectible
    /// </summary>
    public void Collect()
    {
        AudioSource.PlayClipAtPoint(collectibleAudioClip, transform.position, volume);
        Destroy(gameObject); // Immediately destroy this GameObject to remove it from the scene, allowing the sound to play independently
    }
}
