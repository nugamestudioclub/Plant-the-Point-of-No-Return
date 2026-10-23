using UnityEngine;

public class OutroManager : MonoBehaviour
{
    public static bool isGood;
    [SerializeField]
    private GameObject goodEnding;
    [SerializeField]
    private GameObject badEnding;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.goodEnding.gameObject.SetActive(isGood);
        this.badEnding.gameObject.SetActive(!isGood);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
