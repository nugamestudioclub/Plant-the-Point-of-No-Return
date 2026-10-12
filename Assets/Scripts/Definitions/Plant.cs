using UnityEngine;

[CreateAssetMenu(fileName = "PlantType", menuName = "PlantGame/PlantType")]
public class PlantType : ScriptableObject
{
    [SerializeField]
    private string plantName;
    [SerializeField, TextArea(3, 10)]
    private string description;
    [SerializeField]
    public Sprite Icon;
    [SerializeField]
    public Sprite FieldIcon;

}
