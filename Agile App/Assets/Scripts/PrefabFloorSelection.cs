using TMPro;

public class PrefabFloorSelection : AbstractPrefab
{
    public TextMeshProUGUI label;


    public override void Setup(object obj) {
        label.text = obj.ToString(); //sets label
    }
}
