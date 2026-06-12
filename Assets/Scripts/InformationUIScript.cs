/*
* Author: Zhi hng
* Date: 12 June 2026
* Description: Changes the UI which acts as a tutorial and messages to show "this door is locked" for example.
*/

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationUIScript : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI informationUI;
    [SerializeField]
    GameObject player;

    void Start()
    {
        informationUI.transform.parent.gameObject.SetActive(false);
    }
    public new void BroadcastMessage(string message)
    {
        StartCoroutine(ShowText(2f, message));
    }
    IEnumerator ShowText(float duration, string message)
    {
        informationUI.text = message;
        informationUI.transform.parent.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(informationUI.rectTransform);
        yield return new WaitForSeconds(duration);
        informationUI.transform.parent.gameObject.SetActive(false);
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
                if (!isInTutorialZone)
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
