/*
* Author: Zhi Hng
* Date: 13 June 2026
* Description: Handles the restarting of the scene
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
    /// <summary>
    /// Reloads the current scene
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
