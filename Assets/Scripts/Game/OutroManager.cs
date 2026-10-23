using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroManager : MonoBehaviour
{
    public static bool isGood;
    [SerializeField]
    private GameObject goodEnding;
    [SerializeField]
    private GameObject badEnding;

    [SerializeField]
    private AudioSource badClip;
    [SerializeField]
    private AudioSource goodClip;

    [SerializeField]
    private float badDur;
    [SerializeField]
    private float goodDur;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGood = true;
        this.goodEnding.gameObject.SetActive(isGood);
        this.goodClip.gameObject.SetActive(isGood);
        this.badEnding.gameObject.SetActive(!isGood);
        this.badClip.gameObject.SetActive(!isGood);
        if (isGood)
        {
            StartCoroutine(TransitionBack(goodDur));
        }
        else
        {
            StartCoroutine(TransitionBack(badDur));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TransitionBack(float dur)
    {
        yield return new WaitForSeconds(dur);
        SceneManager.LoadScene("Merged");
    }
}
