using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScreenScript : MonoBehaviour
{
    public int index;
    public void change()
    {
        Debug.Log("ChangeScreenScript::change -> switching to different scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
    }
}
