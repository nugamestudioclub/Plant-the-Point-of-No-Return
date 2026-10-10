using UnityEngine;

[CreateAssetMenu(fileName = "RandomEvent", menuName = "PlantGame/RandomEvent")]
public class RandomEvent : ScriptableObject
{
    [SerializeField]
    private string title;
    [SerializeField]
    private Sprite art;
    [SerializeField, TextArea(3, 10)]
    private string textDisplay;
    [SerializeField]
    private string choice1;
    [SerializeField]
    private string choice2;

    public string Title { get { return title; } }
    public Sprite Art { get { return art; } }
    public string TextDisplay { get { return textDisplay; } }
    public string Choice1 {  get { return choice1; } }
    public string Choice2 { get { return choice2; } }

}
