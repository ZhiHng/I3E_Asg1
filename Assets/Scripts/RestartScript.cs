/*
* Author: Zhi Hng
* Date: 12 June 2026
* Description: Handles the restarting of the scene
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
