/*
* Author: Zhi Hng
* Date: 13 June 2026
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
    /// <summary>
    /// Reference the InformationUIScript
    /// </summary>
    [SerializeField]
    InformationUIScript informationUI; // Used to guide players for the starting tutorial and when they cannot unlock a door etc.
    CollectibleScript currentCollectible; // Store the collectible object's script the player is currently able to interact with
    DoorScript currentDoor; // Store the door's script the player is currently able to interact with
    GameObject currentHighlighted; // Stores the gameobject of the hovered object
    Material originalMaterial; // Stores the original material of the hovered object
    /// <summary>
    /// Reference the material to be used for highlighting of interactable objects
    /// </summary>
    [SerializeField]
    Material highlightMaterial;
    /// <summary>
    /// Objects in the highlightable layer will be highlighted when raycast hits it
    /// </summary>
    [SerializeField]
    LayerMask highlightable;
    /// <summary>
    /// Reference the global volume in charge of post processing
    /// </summary>
    [SerializeField]
    Volume globalVolume; // Uses global volume post processing to change screen colour and vignette
    Vignette vignette;
    ColorAdjustments colorAdjustments;
    /// <summary>
    /// Reference the audio used when player takes damage
    /// </summary>
    [SerializeField]
    AudioClip damageSound;

    int documentCount, testTubeRCount, testTubeGCount, testTubeBCount = 0; // Keep track of how many documents the player has collected so far
    int totalDocument, totalTestTubeR, totalTestTubeG, totalTestTubeB; // Total amount of each collectible in the scene
    /// <summary>
    /// How many batteries the player current has
    /// </summary>
    [SerializeField]
    int batteryCount = 0;
    [SerializeField]
    float hitpoints = 100f;
    float damageTimer = 0f; //Determines the time interval between damage ticks
    bool isBurn = false; // Used for the red substance lasting damage mechanic
    float burnTimer = 0; // Used for the red substance lasting damage mechanic and tracks the number of damage ticks passed
    /// <summary>
    /// The clearance level of doors the player can enter
    /// </summary>
    [SerializeField]
    int keycardClearance = 1;
    [SerializeField]
    TextMeshProUGUI collectibleText, hitpointsText; // Reference to the UI text element that displays the total collectibles in the scene and the player's HP
    CharacterController controller;
    bool isDead, isDeadAnimation, isEscape = false;
    /// <summary>
    /// Reference the button that is used to restart the game.
    /// </summary>
    [SerializeField]
    GameObject restartButton;
    /// <summary>
    /// Reference the TextMeshProUGUI that shows the score of the player.
    /// </summary>
    [SerializeField]
    GameObject scoreText;
    /// <summary>
    /// Reference the area that the player needs to be in to start the lift to end the game
    /// </summary>
    [SerializeField]
    GameObject liftCheckZone;
    
    void Start()
    {
        hitpointsText.text = "HP: " + hitpoints;
        controller = GetComponent<CharacterController>();
        globalVolume.profile.TryGet<Vignette>(out vignette); // Get reference to vignette
        globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments); // Get reference to colour adjustments
        vignette.intensity.value = 0.2f;
        vignette.smoothness.overrideState = false;
        colorAdjustments.active = false;

        // Find the total amount of each collectibles in the scene
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
            if (!isDeadAnimation) // Ensures coroutine animation plays once
            {
                GetComponent<CharacterController>().enabled = false; // Make sure player cannot move while dying
                isDeadAnimation = true;
                StartCoroutine(DeadVision());
            }
        }
        if (isEscape)
        {
            if (!isDeadAnimation) // Ensures coroutine animation plays once
            {
                GetComponent<CharacterController>().enabled = false; // Make sure player cannot move while in the lift
                isDeadAnimation = true;
                StartCoroutine(Escaped());
            }
        }
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1.5f, highlightable)) { // Shoots a raycast out every frame to check if the object at the center of the camera is interactable
            GameObject target = hit.collider.gameObject;

            if (currentHighlighted != target) { // Changed interactable object
                
                // Remove highlight from old object
                if (currentHighlighted != null) {
                    Renderer[] childrenRenderer = currentHighlighted.GetComponentsInChildren<Renderer>();
                    foreach (Renderer child in childrenRenderer)
                    {
                        child.material = originalMaterial;
                    }
                }

                // Apply highlight to new object
                currentHighlighted = target;
                originalMaterial = currentHighlighted.GetComponentInChildren<Renderer>().material;
                Renderer[] childrenRenderers = currentHighlighted.GetComponentsInChildren<Renderer>();
                foreach (Renderer child in childrenRenderers)
                    {
                        child.material = highlightMaterial;
                    }
            }
        } else {
            // No hit, remove highlight from old object
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
                hitpoints -= 10;
                burnTimer--; // Counts down the number of burn ticks
                damageTimer = 0;
                hitpointsText.text = "HP: " + hitpoints;
                PlayerDamaged();
            }
            damageTimer += Time.deltaTime; // When damageTimer = 1 > 1 second in real time
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
    void OnInteract() // Custom interaction method called when the player performs an interact action by clicking the key 'E"
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1.5f, highlightable)) { // Cast a ray to detect interactable objects only
            if(hit.collider.gameObject.CompareTag("DoorButton")) // Check if the object hit by the ray is tagged as a door
            {
                currentDoor = hit.collider.gameObject.GetComponentInParent<DoorScript>(); // Get the DoorScript component from the parent of the collider
                if ((currentDoor.gameObject.name.Contains("Lvl2") && keycardClearance < 2) || (currentDoor.gameObject.name.Contains("Lvl3") && keycardClearance < 3) || (currentDoor.gameObject.name.Contains("Lvl4") && keycardClearance < 4)) //Checks level of clearance on keycard for door
                {
                    currentDoor = null;
                    informationUI.BroadcastMessage("Not enough clearance on your keycard for access");
                }
                if (currentDoor.gameObject.name.Contains("Battery") && currentDoor.batteries != 4) // Check the door has 4 batteries before opening it
                {
                    currentDoor = null;
                    informationUI.BroadcastMessage("Not enough batteries to power the door");
                }
            }
            if (hit.collider.gameObject.CompareTag("LiftButton"))
            {
                if (liftCheckZone.GetComponent<Collider>().bounds.Contains(gameObject.transform.position)) // Check if the player transform position is in the collider bounds (zone)
                {
                    Animator liftAnimator = hit.collider.transform.parent.gameObject.GetComponent<Animator>();
                    gameObject.transform.SetParent(liftAnimator.transform); // Links the player to the transform of the animation by becoming a child of it, allowing the player to move along with the animation
                    liftAnimator.SetBool("startLift", true);
                    isEscape = true;
                }
            }
            if(hit.collider.gameObject.CompareTag("PowerUnit")) 
            {
                currentDoor = hit.collider.gameObject.GetComponentInParent<DoorScript>(); // Get the DoorScript component from the parent of the collider
                if (currentDoor.batteries != 4 && batteryCount != 0) // Check if the door does not have max batteries and the player has batteries
                {
                    int addedBattery;
                    if (4 - currentDoor.batteries < batteryCount) // Runs if the player has more batteries than required
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
        }

        if (currentDoor != null) // Check if the player interacting with a door
        {
            currentDoor.Interact(); // Call the Interact method on the door script to toggle its open/closed state
            currentDoor = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hazards") // Check if the player is entering the trigger is tagged as a hazard
        {
            damageTimer = 0f;
            // First tick of damage when entering
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
        if (other.gameObject.tag == "Smoke") // Makes the vision harder to see
        {
            vignette.intensity.value = 1f;
            vignette.smoothness.overrideState = true; // enables the smoothness parameter which has its values preset already. By default it is left disabled when the game starts
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Hazards") // Check if the player stays in the trigger which is tagged as a hazard
        {
            if (other.gameObject.name.Contains("laserBeam"))
            {
                hitpoints -= 2;

                // Knocks back the player when they touch the laser
                Vector3 dir = (transform.position - other.ClosestPoint(transform.position)).normalized; 
                controller.Move(dir * 15f * Time.deltaTime);

                PlayerDamaged();
            }
            if (other.gameObject.name.Contains("Green"))
            {
                if (damageTimer > 0.1) // Runs every 0.1 second
                {
                    hitpoints -= 5;
                    damageTimer = 0;
                    PlayerDamaged();
                }
            } 
            else if (other.gameObject.name.Contains("Red"))
            {
                if (damageTimer > 1) // Runs every 1 second
                {
                    hitpoints -= 10;
                    damageTimer = 0;
                    PlayerDamaged();
                }
            } 
            hitpointsText.text = "HP: " + hitpoints;
            damageTimer += Time.deltaTime; // When damageTimer = 1 > 1 second in real time
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Hazards") // Check if the player leaves the trigger tagged as a hazard
        {
            if (other.gameObject.name.Contains("Red"))
            {
                isBurn = true;
                burnTimer = 3; // Starts the 3 second tick damage
            } 
        }
        if (other.gameObject.tag == "Smoke") // Return vision to normal
        {
            vignette.intensity.value = 0.2f;
            vignette.smoothness.overrideState = false;
        }
    }
    /// <summary>
    /// Plays damage audio and flashes the screen red when taking damage
    /// </summary>
    void PlayerDamaged()
    {
        AudioSource.PlayClipAtPoint(damageSound, transform.position, 1);
        StartCoroutine(FlashRedTint(0.1f));
    }
    /// <summary>
    /// Flashes the screen red
    /// </summary>
    /// <param name="duration">Duration the red screen colour shows each time the player is damaged</param>
    /// <returns></returns>
    IEnumerator FlashRedTint(float duration)
    {
        colorAdjustments.active = true; // Color adjustments is preset to red colour and by default is disabled when the game starts
        yield return new WaitForSeconds(duration);
        colorAdjustments.active = false;
    }
    /// <summary>
    /// Slowly applies vignette and blanks the screen to show eyes closing. Enables the restart screen with score.
    /// </summary>
    /// <returns></returns>
    IEnumerator DeadVision()
    {
        // Gets all UI and disables them
        GameObject[] uiInScene = GameObject.FindGameObjectsWithTag("UI"); 
        foreach (GameObject uiText in uiInScene)
        {
            uiText.SetActive(false);
        }

        //Controls the gradual change in vignette
        float tick = 0f;
        while (tick < 1f)
        {
            tick += Time.deltaTime / 2f; // 2 seconds duration
            vignette.intensity.value = Mathf.Lerp(0.2f, 1f, tick); // gradually darken
            yield return null;
        }

        colorAdjustments.colorFilter.value = Color.black;
        colorAdjustments.active = true; // Shows black screen
        restartButton.SetActive(true);
        scoreText.GetComponent<TextMeshProUGUI>().text = "You Passed Out...\nScore\n" + score;
        scoreText.SetActive(true);    
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    /// <summary>
    /// Slowly darkens the and blanks the screen. Enables the restart screen with score adding in bonus points for escaping.
    /// </summary>
    /// <returns></returns>
    IEnumerator Escaped()
    {
        int bonusScore = Math.Max(50, Mathf.RoundToInt(300 * Mathf.Exp(-Time.time / 500f))); //max of 300 bonus points if instantly complete level, slower completion will have lower score. Always gets at least 50 points.
        score += bonusScore;

        // Gets all UI and disables them
        GameObject[] uiInScene = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject uiText in uiInScene)
        {
            uiText.SetActive(false);
        }

        //Controls the gradual darken of the screen
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

