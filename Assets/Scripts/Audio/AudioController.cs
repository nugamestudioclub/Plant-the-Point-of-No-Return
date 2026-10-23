using System.Collections.Generic;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource TitleAudio;
    [SerializeField]
    private AudioSource MainAudioPart1;
    [SerializeField]
    private AudioSource MainAudioPart2;
    [SerializeField]
    private AudioSource RadioAudio;
    [SerializeField]
    private AudioSource RadioEffects;
    [SerializeField]
    private AudioSource TickingEffect;

    private float radioTimer = 0f;
    private int preRadioSel = 0;
    [SerializeField]
    private float radioDur = 90;
    [SerializeField]
    private Button radio;

    public int targetAudio = 0;
    private int pTargA = 0;
    public float volume = 1;

    private List<AudioSource> sources;
    [SerializeField]
    private AudioClip[] randomRadioClips;
    [SerializeField]
    private AudioClip[] randomTickingEffects;

    public UnityEvent onTrackChange;
    private int tickIndex = 0;


    private bool playTicking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sources = new List<AudioSource> { TitleAudio, MainAudioPart1, MainAudioPart2, RadioAudio };
        radio.onClick.AddListener(OnRadioClick);
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < sources.Count; i++) {
            AudioSource source = sources[i];
            float target = 0f;

            if (i == targetAudio)
            {
                target = volume;
            }
            if (target != 0 && source.volume < 0.01 && !source.isPlaying)
            {
                source.Play();
            } 
            else if (source.volume < 0.01 && target == 0)
            {
                source.Pause();
            }
            source.volume = Mathf.Lerp(source.volume, target, Time.deltaTime);
        }

        

        if (pTargA != targetAudio)
        {
            onTrackChange.Invoke();
            pTargA = targetAudio;
        }
        //Radio
        if (this.targetAudio == 3)
        {
            if(radioTimer > radioDur)
            {
                this.targetAudio = preRadioSel;
            }
            radioTimer += Time.deltaTime;
        }
        else
        {
            this.radioTimer = 0f;
        }

      
        
    }

    private void OnRadioClick()
    {
        preRadioSel = this.targetAudio;
        this.targetAudio = 3;
        int sel = Random.Range(0, randomRadioClips.Length);
        this.RadioEffects.clip = randomRadioClips[sel];
        this.RadioEffects.Play();
    }

    
    public void PlayTicking()
    {
        tickIndex = (tickIndex + 1) % this.randomTickingEffects.Length;
        this.TickingEffect.clip = randomTickingEffects[tickIndex];
        this.TickingEffect.Play();
    }
    

    
}
