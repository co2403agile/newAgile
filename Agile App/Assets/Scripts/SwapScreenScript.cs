using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScreenScript : MonoBehaviour
{
    public int index;
    public void change()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
    }


    // Start is called before the first frame update
 
}
