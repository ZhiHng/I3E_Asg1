/*
* Author: Zhi Hng
* Date: 11 June 2026
* Description: Plays animation
*/

using System; // Import standard .NET system types (not strictly needed here but common in C# files)
using UnityEngine; // Import Unity-specific classes like MonoBehaviour, GameObject, Collider, and print

public class DoorScript : MonoBehaviour
{
    [SerializeField]
    AudioClip doorAudio;
    Animator doorAnimator, batteryAnimator;
    [SerializeField]
    GameObject player;
    public int batteries;

    bool isOpen = false;

    void Start()
    {
        doorAnimator = transform.Find("doorSlide").gameObject.GetComponent<Animator>();
        batteryAnimator = transform.Find("powerUnitGroup").gameObject.GetComponent<Animator>();
        batteryAnimator.SetInteger("batteries", batteries);
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > 3f)
        {
            if(isOpen)
            {
                doorAnimator.SetTrigger("doorClose");
                isOpen = !isOpen;
            }
        }
    }

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

    public void AddBattery(int numberOfBattery)
    {
        print("adding battery");
        batteries += numberOfBattery;
        batteryAnimator.SetInteger("batteries", batteries);
    }
}
