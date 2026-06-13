/*
* Author: Zhi hng
* Date: 13 June 2026
* Description: Changes the UI which acts as a tutorial and messages to show "this door is locked" for example.
*/

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationUIScript : MonoBehaviour
{
    /// <summary>
    /// Reference the TextMeshProUGUI used to show the helpful information
    /// </summary>
    [SerializeField]
    TextMeshProUGUI informationUI;
    /// <summary>
    /// Reference the player gameobject which has the PlayerScript
    /// </summary>
    [SerializeField]
    GameObject player;

    void Start()
    {
        informationUI.transform.parent.gameObject.SetActive(false); //disables the information TextMeshProUGUI to hide from the player's view
    }
    /// <summary>
    /// Enables the UI to show a particular message
    /// </summary>
    /// <param name="message">Message to be shown</param>
    public new void BroadcastMessage(string message)
    {
        StartCoroutine(ShowText(2f, message));
    }
    /// <summary>
    /// Enables the TextMeshProUGUI for a duration to show a message and disables it again
    /// </summary>
    /// <param name="duration">Duration for the message to stay on the screen</param>
    /// <param name="message">Message to be shown</param>
    /// <returns></returns>
    IEnumerator ShowText(float duration, string message)
    {
        informationUI.text = message;
        informationUI.transform.parent.gameObject.SetActive(true); // Show the text
        LayoutRebuilder.ForceRebuildLayoutImmediate(informationUI.rectTransform); // Forces Unity to redraw the black background behind the text
        yield return new WaitForSeconds(duration);
        informationUI.transform.parent.gameObject.SetActive(false); //Hide the text
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Find which child collider was triggered
            Collider[] childColliders = GetComponentsInChildren<Collider>();
            bool isInTutorialZone = false;
            foreach (var child in childColliders)
            {
                // Check if the player is inside this child collider
                if (child.bounds.Contains(other.transform.position))
                {
                    if (child.gameObject.name.Contains("move"))
                    {
                        informationUI.text = "Keys W A S D to move\nShift key to Sprint";
                        informationUI.transform.parent.gameObject.SetActive(true);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(informationUI.rectTransform);
                        isInTutorialZone = true;
                    }
                    else if (child.gameObject.name.Contains("jump"))
                    {
                        informationUI.text = "Space bar to jump";
                        informationUI.transform.parent.gameObject.SetActive(true);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(informationUI.rectTransform);
                        isInTutorialZone = true;
                    }
                    else if (child.gameObject.name.Contains("interact"))
                    {
                        informationUI.text = "Key E to interact with interactable objects";
                        informationUI.transform.parent.gameObject.SetActive(true);
                        LayoutRebuilder.ForceRebuildLayoutImmediate(informationUI.rectTransform);
                        isInTutorialZone = true;
                    }
                }
                if (!isInTutorialZone) // Ensures that if the player is not in all 3 collider's bounds, the text UI will hide
                {
                    informationUI.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        informationUI.transform.parent.gameObject.SetActive(false);
    }
}
