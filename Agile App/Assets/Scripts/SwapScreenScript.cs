using UnityEngine;
using UnityEngine.SceneManagement;

/* ChangeScreenScript: Handles the functionality to switch to different scenes based on a specified index. */
/* Written by Michael */
public class ChangeScreenScript : MonoBehaviour
{
    public int index;

    /* change: Switches to a different scene based on the specified index */
    public void change()
    {
        /* Log a message indicating the intent to switch scenes */
        Debug.Log("ChangeScreenScript::change -> switching to a different scene");

        /* Get the index of the current active scene and add the specified index */
        int targetSceneIndex = SceneManager.GetActiveScene().buildIndex + index;

        /* Load the scene with the calculated index */
        SceneManager.LoadScene(targetSceneIndex);
    }
}