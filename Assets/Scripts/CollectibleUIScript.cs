/*
* Author: Zhi hng
* Date: 11 June 2026
* Description: Detects the collectibles in each room and changes UI based on the room the player is in.
*/

using TMPro;
using UnityEngine;

public class CollectibleUIScript : MonoBehaviour
{
    int keycard;
    int battery;
    public TextMeshProUGUI roomCollectibleText;

    void Start()
    {
        roomCollectibleText.transform.parent.gameObject.SetActive(false);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomCollectibleText.transform.parent.gameObject.SetActive(true);
            ScanRoom();
        }
    }
    void OnTriggerExit(Collider other)
    {
        roomCollectibleText.transform.parent.gameObject.SetActive(false);
    }
    void ScanRoom()
    {
        keycard = 0;
        battery = 0;
        // Get all colliders on this room object
        Collider[] roomColliders = gameObject.GetComponents<Collider>();

        foreach (var roomCollider in roomColliders)
        {
            Collider[] hits = Physics.OverlapBox(roomCollider.bounds.center, roomCollider.bounds.extents);

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Collectible"))
                {
                    if (hit.name.Contains("keycard")) keycard++;
                    if (hit.name.Contains("battery")) battery++;
                }
            }
        }
        roomCollectibleText.text = "Keycards: " + keycard + "\nBattery: " + battery;
    }
}
