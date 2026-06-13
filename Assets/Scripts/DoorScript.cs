/*
* Author: Zhi Hng
* Date: 13 June 2026
* Description: Plays animation
*/

using System; // Import standard .NET system types (not strictly needed here but common in C# files)
using UnityEngine; // Import Unity-specific classes like MonoBehaviour, GameObject, Collider, and print

public class DoorScript : MonoBehaviour
{
    /// <summary>
    /// Audio to play when door opens
    /// </summary>
    [SerializeField]
    AudioClip doorAudio;
    Animator doorAnimator, batteryAnimator;
    /// <summary>
    /// Reference the player gameobject with the PlayerScript
    /// </summary>
    [SerializeField]
    GameObject player;
    /// <summary>
    /// Number of batteries the current interacted door has.
    /// </summary>
    public int batteries;

    bool isOpen = false;

    void Start()
    {
        doorAnimator = transform.Find("doorSlide").gameObject.GetComponent<Animator>();
        batteryAnimator = transform.Find("powerUnitGroup").gameObject.GetComponent<Animator>();
        batteryAnimator.SetInteger("batteries", batteries); // Integer parameter on the animator for each number of batteries. Sets the animation to the number of batteries in the door when the game starts
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > 3f) // Closes door if player is far away
        {
            if(isOpen)
            {
                doorAnimator.SetTrigger("doorClose");
                isOpen = !isOpen;
            }
        }
    }
    /// <summary>
    /// Open / Close the door and place in batteries for doors that are battery powered
    /// </summary>
    public void Interact()
    {
        if(isOpen)
        {
            doorAnimator.SetTrigger("doorClose");
        }
        else
        {
            doorAnimator.SetTrigger("doorOpen");
            AudioSource.PlayClipAtPoint(doorAudio, transform.position, 1);
        }
        isOpen = !isOpen;
    }
    /// <summary>
    /// Adds batteries to the battery powered door
    /// </summary>
    /// <param name="numberOfBattery">Number of batteries to add to the door</param>
    public void AddBattery(int numberOfBattery)
    {
        batteries += numberOfBattery;
        batteryAnimator.SetInteger("batteries", batteries);
    }
}
