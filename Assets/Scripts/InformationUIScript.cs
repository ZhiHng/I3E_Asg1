/*
* Author: Zhi hng
* Date: 11 June 2026
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
    public void BroadcastMessage(string message)
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
}
