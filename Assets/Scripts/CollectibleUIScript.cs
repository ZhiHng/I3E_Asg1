/*
* Author: Zhi hng
* Date: 12 June 2026
* Description: Detects the collectibles in each room and changes UI based on the room the player is in.
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUIScript : MonoBehaviour
{
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
        if (keycard != 0)
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
        roomCollectibleText.text = text.TrimEnd('\n');
        LayoutRebuilder.ForceRebuildLayoutImmediate(roomCollectibleText.rectTransform);
        if (text.Length == 0)
        {
            roomCollectibleText.transform.parent.gameObject.SetActive(false);
        }
    }
}
