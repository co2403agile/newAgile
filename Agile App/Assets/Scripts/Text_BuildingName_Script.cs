using TMPro;
using UnityEngine;

/* Text_BuildingName_Script: Updates a TextMeshProUGUI component with the building name retrieved from PlayerPrefs. */
/* Written by James */

public class Text_BuildingName_Script : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // Reference to the TextMeshProUGUI component to update with the building name

    /* Start: called before the first frame update */
    void Start()
    {
        /* Set the text of the TextMeshProUGUI component to the building name stored in PlayerPrefs */
        textMesh.text = PlayerPrefs.GetString("building");
    }
}