using System;
using System.Collections.Generic;
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
    [SerializeField]

    //string is the tag (i.e. identify which effect it does), float is the magnitude
    private Dictionary<string, float> choice1Effects;
    [SerializeField]
    private Dictionary<string, float> choice2Effects;
    [SerializeField]
    private int baseWeight = 10;

    public string Title { get { return title; } }
    public Sprite Art { get { return art; } }
    public string TextDisplay { get { return textDisplay; } }
    public string Choice1 {  get { return choice1; } }
    public string Choice2 { get { return choice2; } }
    public Dictionary<string, float> Choice1Effects {  get { return choice1Effects; } }
    public Dictionary<string, float> Choice2Effects {  get { return choice2Effects; } }
    
    public int BaseWeight { get { return baseWeight; } }

}
