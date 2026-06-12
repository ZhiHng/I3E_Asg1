/*
* Author: Zhi Hng
* Date: 12 June 2026
* Description: Handles interactions of collectibles, doors
*/

using System; // Import standard .NET system types (not strictly needed here but common in C# files)
using UnityEngine; // Import Unity-specific classes like MonoBehaviour, GameObject, Collider, and print
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    int score = 0;
    [SerializeField]
    InformationUIScript informationUI;
    CollectibleScript currentCollectible; // Store the collectible object the player is currently able to interact with
    DoorScript currentDoor;
    GameObject currentHighlighted;
    Material originalMaterial;
    [SerializeField]
    Material highlightMaterial;
    [SerializeField]
    LayerMask highlightable;
    [SerializeField]
    Volume globalVolume;
    Vignette vignette;
    ColorAdjustments colorAdjustments;
    [SerializeField]
    AudioClip damageSound;

    int documentCount, testTubeRCount, testTubeGCount, testTubeBCount = 0; // Keep track of how many documents the player has collected so far
    int totalDocument, totalTestTubeR, totalTestTubeG, totalTestTubeB;
    [SerializeField]
    int batteryCount = 0;
    [SerializeField]
    float hitpoints = 100f;
    float damageTimer = 0f; //Determines the time interval between damage ticks
    bool isBurn = false;
    float burnTimer = 0;
    [SerializeField]
    int keycardClearance = 1;
    [SerializeField]
    TextMeshProUGUI collectibleText, hitpointsText; // Reference to the UI text element that displays the player's score
    CharacterController controller;
    bool isDead, isDeadAnimation, isEscape = false;
    [SerializeField]
    GameObject restartButton, scoreText;
    [SerializeField]
    GameObject liftCheckZone;
    
    void Start()
    {
        hitpointsText.text = "HP: " + hitpoints; // Initialize the score display to show the starting score of 0 when the game begins
        controller = GetComponent<CharacterController>();
        globalVolume.profile.TryGet<Vignette>(out vignette);
        globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        vignette.intensity.value = 0.2f;
        vignette.smoothness.overrideState = false;
        colorAdjustments.active = false;
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject item in collectibles)
        {
            if (item.name.Contains("folder"))
            {
                totalDocument++;
            }
            else if (item.name.Contains("testtubeRed"))
            {
                totalTestTubeR++;
            }
            else if (item.name.Contains("testtubeGreen"))
            {
                totalTestTubeG++;
            }
            else if (item.name.Contains("testtubeBlue"))
            {
                totalTestTubeB++;
            }
        }
        collectibleText.text = "Documents: " + documentCount + " / " + totalDocument + 
                                "\nTest Tube R: " + testTubeRCount + " / " + totalTestTubeR +
                                "\nTest Tube G: " + testTubeGCount + " / " + totalTestTubeG +
                                "\nTest Tube B: " + testTubeBCount + " / " + totalTestTubeB +
                                "\nBatteries: " + batteryCount;
    }
    void Update()
    {
        if (isDead)
        {   
            if (!isDeadAnimation)
            {
                GetComponent<CharacterController>().enabled = false;
                isDeadAnimation = true;
                StartCoroutine(DeadVision());
            }
        }
        if (isEscape)
        {
            if (!isDeadAnimation)
            {
                GetComponent<CharacterController>().enabled = false;
                isDeadAnimation = true;
                StartCoroutine(Escaped());
            }
        }
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1.5f, highlightable)) {
            GameObject target = hit.collider.gameObject;

            if (currentHighlighted != target) {
                
                // Remove highlight from old
                if (currentHighlighted != null) {
                    Renderer[] childrenRenderer = currentHighlighted.GetComponentsInChildren<Renderer>();
                    foreach (Renderer child in childrenRenderer)
                    {
                        child.material = originalMaterial;
                    }
                }

                // Apply highlight to new
                currentHighlighted = target;
                originalMaterial = currentHighlighted.GetComponentInChildren<Renderer>().material;
                Renderer[] childrenRenderers = currentHighlighted.GetComponentsInChildren<Renderer>();
                foreach (Renderer child in childrenRenderers)
                    {
                        child.material = highlightMaterial;
                    }
            }
        } else {
            // No hit, remove highlight
            if (currentHighlighted != null) {
                Renderer[] childrenRenderer = currentHighlighted.GetComponentsInChildren<Renderer>();
                foreach (Renderer child in childrenRenderer)
                    {
                        child.material = originalMaterial;
                    }
                currentHighlighted = null;
            }
        }
        
        if (isBurn)
        {
            if (damageTimer > 1)
            {
                hitpoints -= 15;
                burnTimer--;
                damageTimer = 0;
                hitpointsText.text = "HP: " + hitpoints;
                PlayerDamaged();
            }
            damageTimer += Time.deltaTime;
            if (burnTimer <= 0)
            {
                isBurn = false;
            }
        }

        if (hitpoints <= 0)
        {
            isDead = true;
        }
    }
    void OnInteract() // Custom interaction method called when the player performs an interact action
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1.5f, highlightable)) {
            if(hit.collider.gameObject.CompareTag("DoorButton")) // Check if the object entering the trigger is tagged as a door
            {
                currentDoor = hit.collider.gameObject.GetComponentInParent<DoorScript>(); // Get the DoorScript component from the parent of the collider
                if ((currentDoor.gameObject.name.Contains("Lvl2") && keycardClearance < 2) || (currentDoor.gameObject.name.Contains("Lvl3") && keycardClearance < 3) || (currentDoor.gameObject.name.Contains("Lvl4") && keycardClearance < 4)) //Checks level for clearance for door
                {
                    currentDoor = null;
                    informationUI.BroadcastMessage("Not enough clearance on your keycard for access");
                }
                if (currentDoor.gameObject.name.Contains("Battery") && currentDoor.batteries != 4) //Checks level for clearance for door
                {
                    currentDoor = null;
                    informationUI.BroadcastMessage("Not enough batteries to power the door");
                }
            }
            if (hit.collider.gameObject.CompareTag("LiftButton"))
            {
                if (liftCheckZone.GetComponent<Collider>().bounds.Contains(gameObject.transform.position))
                {
                    Animator liftAnimator = hit.collider.transform.parent.gameObject.GetComponent<Animator>();
                    gameObject.transform.SetParent(liftAnimator.transform);
                    liftAnimator.SetBool("startLift", true);
                    isEscape = true;
                }
            }
            if(hit.collider.gameObject.CompareTag("PowerUnit")) 
            {
                currentDoor = hit.collider.gameObject.GetComponentInParent<DoorScript>(); // Get the DoorScript component from the parent of the collider
                if (currentDoor.batteries != 4 && batteryCount != 0)
                {
                    int addedBattery;
                    if (4 - currentDoor.batteries < batteryCount)
                    {
                        addedBattery = 4 - currentDoor.batteries;
                    }
                    else
                    {
                        addedBattery = batteryCount;
                    }
                    batteryCount = batteryCount - addedBattery;
                    currentDoor.AddBattery(addedBattery);
                    
                    collectibleText.text = "Documents: " + documentCount + " / " + totalDocument + 
                                "\nTest Tube R: " + testTubeRCount + " / " + totalTestTubeR +
                                "\nTest Tube G: " + testTubeGCount + " / " + totalTestTubeG +
                                "\nTest Tube B: " + testTubeBCount + " / " + totalTestTubeB +
                                "\nBatteries: " + batteryCount;
                }
                currentDoor = null;
            }
            if (hit.collider.gameObject.CompareTag("Collectible"))
            {
                currentCollectible = hit.collider.gameObject.GetComponentInParent<CollectibleScript>();
            }
        }
        if(currentCollectible != null) // Only collect something if the player is currently near a collectible
        {
            if (currentCollectible.gameObject.name.Contains("keycard"))
            {
                keycardClearance++;
                informationUI.BroadcastMessage("Clearance up");
            }
            else if (currentCollectible.gameObject.name.Contains("medkit"))
            {
                if (hitpoints >= 100)
                {
                    informationUI.BroadcastMessage("Max HP");
                    currentCollectible = null;
                    return;
                }
                hitpoints += 20;
                hitpoints = Math.Min(100, hitpoints);
            }
            else if (currentCollectible.gameObject.name.Contains("battery"))
            {
                batteryCount++;
            }
            else if (currentCollectible.gameObject.name.Contains("folder"))
            {
                documentCount++;
            }
            else if (currentCollectible.gameObject.name.Contains("testtubeRed"))
            {
                testTubeRCount++;
            }
            else if (currentCollectible.gameObject.name.Contains("testtubeGreen"))
            {
                testTubeGCount++;
            }
            else if (currentCollectible.gameObject.name.Contains("testtubeBlue"))
            {
                testTubeBCount++;
            }
            score += currentCollectible.collectibleScore;
            collectibleText.text = "Documents: " + documentCount + " / " + totalDocument + 
                                "\nTest Tube R: " + testTubeRCount + " / " + totalTestTubeR +
                                "\nTest Tube G: " + testTubeGCount + " / " + totalTestTubeG +
                                "\nTest Tube B: " + testTubeBCount + " / " + totalTestTubeB +
                                "\nBatteries: " + batteryCount;
            hitpointsText.text = "HP: " + hitpoints;
            currentCollectible.Collect(); // Call the Collect method on the collectible script to handle its collection logic
            currentCollectible = null; // Clear the reference so the player no longer has an active collectible selected 
        }
        else
        {
            print("Error: No CollectibleScript found"); // Log an error in the Unity Console if the collectible is missing its data component
            //return; // Exit the method early because we cannot safely collect the item without the script
        }

        if (currentDoor != null) // Check if the player is currently near a door they can interact with
        {
            currentDoor.Interact(); // Call the Interact method on the door script to toggle its open/closed state
            currentDoor = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hazards") // Check if the object entering the trigger is tagged as a hazard
        {
            damageTimer = 0f;
            if (other.gameObject.name.Contains("Green"))
            {
                hitpoints -= 5;
            }
            else if (other.gameObject.name.Contains("Red"))
            {
                isBurn = false;
                hitpoints -= 15;
            }
            PlayerDamaged();
            hitpointsText.text = "HP: " + hitpoints;
        }
        if (other.gameObject.tag == "Smoke")
        {
            vignette.intensity.value = 1f;
            vignette.smoothness.overrideState = true;
        }
    }
    void OnTriggerStay(Collider other) // Unity event called when another collider enters this GameObject's trigger collider
    {
        if(other.gameObject.tag == "Hazards") // Check if the object entering the trigger is tagged as a hazard
        {
            if (other.gameObject.name.Contains("laserBeam"))
            {
                hitpoints -= 2;
                Vector3 dir = (transform.position - other.ClosestPoint(transform.position)).normalized;

                // Apply a quick knockback move
                controller.Move(dir * 15f * Time.deltaTime);
                PlayerDamaged();
            }
            if (other.gameObject.name.Contains("Green"))
            {
                if (damageTimer > 0.1)
                {
                    hitpoints -= 5;
                    damageTimer = 0;
                    PlayerDamaged();
                }
            } 
            else if (other.gameObject.name.Contains("Red"))
            {
                if (damageTimer > 1)
                {
                    hitpoints -= 15;
                    damageTimer = 0;
                    PlayerDamaged();
                }
            } 
            hitpointsText.text = "HP: " + hitpoints;
            damageTimer += Time.deltaTime;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Hazards") // Check if the object entering the trigger is tagged as a hazard
        {
            if (other.gameObject.name.Contains("Red"))
            {
                isBurn = true;
                burnTimer = 3;
            } 
        }
        if (other.gameObject.tag == "Smoke")
        {
            vignette.intensity.value = 0.2f;
            vignette.smoothness.overrideState = false;
        }
    }
    void PlayerDamaged()
    {
        AudioSource.PlayClipAtPoint(damageSound, transform.position, 1);
        StartCoroutine(FlashRedTint(0.1f));
    }
    IEnumerator FlashRedTint(float duration)
    {
        colorAdjustments.active = true;
        yield return new WaitForSeconds(duration);
        colorAdjustments.active = false;
    }
    IEnumerator DeadVision()
    {
        GameObject[] uiInScene = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject uiText in uiInScene)
        {
            uiText.SetActive(false);
        }
        float tick = 0f;
        while (tick < 1f)
        {
            tick += Time.deltaTime / 2f; // 2 seconds duration
            vignette.intensity.value = Mathf.Lerp(0.2f, 1f, tick); // gradually darken
            yield return null;
        }
        colorAdjustments.colorFilter.value = Color.black;
        colorAdjustments.active = true;
        restartButton.SetActive(true);
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score\n" + score;
        scoreText.SetActive(true);    
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    IEnumerator Escaped()
    {
        int bonusScore = Math.Max(50, Mathf.RoundToInt(300 * Mathf.Exp(-Time.time / 300f))); //max of 300 bonus points if instantly complete level, slower completion will have lower score. Always gets at least 50 points.
        score += bonusScore;
        GameObject[] uiInScene = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject uiText in uiInScene)
        {
            uiText.SetActive(false);
        }
        float tick = 0f;
        colorAdjustments.active = true;
        while (tick < 1f)
        {
            tick += Time.deltaTime / 8f; // 2 seconds duration
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.black, tick); // gradually darken
            yield return null;
        }
        restartButton.SetActive(true);
        scoreText.GetComponent<TextMeshProUGUI>().text = "Successfully Escaped!\nScore\n" + score;
        scoreText.SetActive(true);    
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

