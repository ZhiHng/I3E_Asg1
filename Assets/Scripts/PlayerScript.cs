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

    int playerScore = 0; // Keep track of how many points the player has collected so far
    int keycardClearance = 1;
    [SerializeField]
    int targetScore = 0; // The goal score required to complete a task, editable from the Unity Inspector

    [SerializeField]
    TextMeshProUGUI scoreText; // Reference to the UI text element that displays the player's score

    void Start()
    {
        scoreText.text = "Score: " + playerScore; // Initialize the score display to show the starting score of 0 when the game begins
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
            playerScore += currentCollectible.collectibleScore; // Add the collectible's score value to the player's total score
            scoreText.text = "Score: " + playerScore; // Update the on-screen score display to reflect the new score after collecting an item
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

    void OnTriggerEnter(Collider other) // Unity event called when another collider enters this GameObject's trigger collider
    {
        if(other.gameObject.tag == "Collectible") // Check if the object entering the trigger is tagged as a collectible
        {
            currentCollectible = other.GetComponentInParent<CollectibleScript>(); // Store the collectible script so the player can interact with it later
        }

        if(other.gameObject.tag == "Door") // Check if the object entering the trigger is tagged as a door
        {
            currentDoor = other.GetComponentInParent<DoorScript>(); // Get the DoorScript component from the parent of the collider
        }

        if(other.gameObject.tag == "GoalArea" && playerScore >= targetScore) // Check if the player entered the goal area and has enough points
        {
            print("Player entered trigger zone with " + playerScore + " points"); // Print a success message when the player reaches the goal with enough score
        }
    }

    void OnTriggerExit(Collider other) // Unity event called when another collider leaves this GameObject's trigger collider
    {
        if(other.gameObject.GetComponentInParent<CollectibleScript>() == currentCollectible) // If the collectible leaving the trigger is the one we were tracking
        {
            currentCollectible = null; // Clear the current collectible because it is no longer in range
        }

        if(other.gameObject.GetComponentInParent<DoorScript>() == currentDoor) // If the door leaving the trigger is the one we were tracking
        {
            currentDoor = null; // Clear the current door because it is no longer in range
        }
    }

}
