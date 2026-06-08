/*
* Author: Zhi Hng
* Date: 8 June 2026
* Description: Handles interactions of collectibles, doors
*/

using System; // Import standard .NET system types (not strictly needed here but common in C# files)
using UnityEngine; // Import Unity-specific classes like MonoBehaviour, GameObject, Collider, and print
using TMPro;

public class PlayerScript : MonoBehaviour
{
    CollectibleScript currentCollectible; // Store the collectible object the player is currently able to interact with
    DoorScript currentDoor;

    GameObject currentHighlighted;
    Material originalMaterial;
    [SerializeField]
    Material highlightMaterial;
    [SerializeField]
    LayerMask highlightable;

    int documentCount = 0; // Keep track of how many documents the player has collected so far
    float hitpoints = 100f;
    float damageTimer = 0f; //Determines the time interval between damage ticks
    bool isBurn = false;
    float burnTimer = 0;
    int keycardClearance = 1;
    [SerializeField]
    int targetScore = 0; // The goal score required to complete a task, editable from the Unity Inspector

    [SerializeField]
    TextMeshProUGUI documentText, hitpointsText; // Reference to the UI text element that displays the player's score

    void Start()
    {
        documentText.text = "Documents: " + documentCount;
        hitpointsText.text = "HP: " + hitpoints; // Initialize the score display to show the starting score of 0 when the game begins
    }
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1.5f, highlightable)) {
            GameObject target = hit.collider.gameObject;

            if (currentHighlighted != target) {
                // Remove highlight from old
                if (currentHighlighted != null) {
                    currentHighlighted.GetComponent<Renderer>().material = originalMaterial;
                }

                // Apply highlight to new
                currentHighlighted = target;
                originalMaterial = currentHighlighted.GetComponent<Renderer>().material;
                currentHighlighted.GetComponent<Renderer>().material = highlightMaterial;
            }
        } else {
            // No hit, remove highlight
            if (currentHighlighted != null) {
                currentHighlighted.GetComponent<Renderer>().material = originalMaterial;
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
            }
            damageTimer += Time.deltaTime;
            if (burnTimer <= 0)
            {
                isBurn = false;
            }
        }
    }
    void OnInteract() // Custom interaction method called when the player performs an interact action
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1.5f)) {
            if(hit.collider.gameObject.CompareTag("DoorButton")) // Check if the object entering the trigger is tagged as a door
            {
                currentDoor = hit.collider.gameObject.GetComponentInParent<DoorScript>(); // Get the DoorScript component from the parent of the collider
                if ((currentDoor.gameObject.name.Contains("Lvl2") && keycardClearance < 2) || (currentDoor.gameObject.name.Contains("Lvl3") && keycardClearance < 3)) //Checks level for clearance for door
                {
                    currentDoor = null;
                    print("Not enough clearance for access");
                }
            }
            print("test");
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
                print("Clearance up");
            }
            documentCount += currentCollectible.collectibleScore; // Add the collectible's score value to the player's total score
            documentText.text = "Documents: " + documentCount; // Update the on-screen score display to reflect the new score after collecting an item
            currentCollectible.Collect(); // Call the Collect method on the collectible script to handle its collection logic
            currentCollectible = null; // Clear the reference so the player no longer has an active collectible selected 
        }
        else
        {
            print("Error: No CollectibleScript found"); // Log an error in the Unity Console if the collectible is missing its data component
            //return; // Exit the method early because we cannot safely collect the item without the script
        }

        if(currentDoor != null) // Check if the player is currently near a door they can interact with
        {
            currentDoor.Interact(); // Call the Interact method on the door script to toggle its open/closed state
        }
    }

    void OnTriggerEnter(Collider other)
    {
        print("test");
        if(other.gameObject.tag == "Hazards") // Check if the object entering the trigger is tagged as a hazard
        {
            print("entered");
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
            hitpointsText.text = "HP: " + hitpoints;
        }
    }
    void OnTriggerStay(Collider other) // Unity event called when another collider enters this GameObject's trigger collider
    {
        if(other.gameObject.tag == "Hazards") // Check if the object entering the trigger is tagged as a hazard
        {
            if (other.gameObject.name.Contains("Green"))
            {
                if (damageTimer > 0.1)
                {
                    hitpoints -= 5;
                    damageTimer = 0;
                    hitpointsText.text = "HP: " + hitpoints;
                }
            } 
            else if (other.gameObject.name.Contains("Red"))
            {
                if (damageTimer > 1)
                {
                    hitpoints -= 15;
                    damageTimer = 0;
                    hitpointsText.text = "HP: " + hitpoints;
                }
            } 
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
    }
}
