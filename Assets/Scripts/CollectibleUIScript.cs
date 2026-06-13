/*
* Author: Zhi hng
* Date: 13 June 2026
* Description: Detects the collectibles in each room and changes UI based on the room the player is in.
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUIScript : MonoBehaviour
{
    /// <summary>
    /// Reference the TextMeshProUGUI the script will edit when player steps into each room
    /// </summary>
    public TextMeshProUGUI roomCollectibleText;

    void Start()
    {
        roomCollectibleText.transform.parent.gameObject.SetActive(false); // Hides the UI unless player steps in a room
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Only runs when the player steps in the room. This script is on multiple rooms so only one room will edit the UI at a time.
        {
            roomCollectibleText.transform.parent.gameObject.SetActive(true);
            ScanRoom();
        }
    }
    void OnTriggerExit(Collider other)
    {
        roomCollectibleText.transform.parent.gameObject.SetActive(false); // Hides the UI when player leaves the room
    }
    /// <summary>
    /// Checks for the number of collectibles in the current room's collider
    /// </summary>
    void ScanRoom()
    {
        int keycard = 0;
        int battery = 0;
        int document = 0;
        int testTubeR = 0;
        int testTubeG = 0;
        int testTubeB = 0;
        // Get all colliders on this room object
        Collider[] roomColliders = gameObject.GetComponents<Collider>();

        foreach (var roomCollider in roomColliders)
        {
            // Check every collider in each room if there are collectibles where their transform position lies in the bounds of the room colliders
            Collider[] hits = Physics.OverlapBox(roomCollider.bounds.center, roomCollider.bounds.extents);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Collectible"))
                {
                    if (hit.name.Contains("keycard")) keycard++;
                    if (hit.name.Contains("battery")) battery++;
                    if (hit.name.Contains("folder")) document++;
                    if (hit.name.Contains("testtubeRed")) testTubeR++;
                    if (hit.name.Contains("testtubeBlue")) testTubeB++;
                    if (hit.name.Contains("testtubeGreen")) testTubeG++;
                }
            }
        }
        string text = "";
        if (keycard != 0) // Only shows in UI if there is at least one item of its type
        {
            text += "Keycards: " + keycard + "\n";
        }
        if (battery != 0)
        {
            text += "Battery: " + battery + "\n";
        }
        if (document != 0)
        {
            text += "Documents: " + document + "\n";
        }
        if (testTubeR != 0)
        {
            text += "Test tube R: " + testTubeR + "\n";
        }
        if (testTubeG != 0)
        {
            text += "Test tube G: " + testTubeG + "\n";
        }
        if (testTubeB != 0)
        {
            text += "Test tube B: " + testTubeB + "\n";
        }
        roomCollectibleText.text = text.TrimEnd('\n'); // Removes the extra line at the bottom
        LayoutRebuilder.ForceRebuildLayoutImmediate(roomCollectibleText.rectTransform); // Forces Unity to redraw the black background behind the text
        if (text.Length == 0) // If no other collectibles in the room after collecting all, do not show UI
        {
            roomCollectibleText.transform.parent.gameObject.SetActive(false);
        }
    }
}
