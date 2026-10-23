using System.Collections.Generic;
using UnityEngine;

public class FunctionDelegator : MonoBehaviour
{
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DelegateFunction(string name, float val)
    {
        switch (name) {
            // Function to handle the action of accepting professor X's proposal.
            case "Action_ProfXAccept":
                GameState.Instance.EnableFertilizerCreator();
                print("[Function Delegator] Fertilizer Creator enabled!");
                break;
            case "Action_ProfXAcceptMore":
                GameState.Instance.ExpandFertilizerCreator();
                print("[Function Delegator] Fertilizer Creator expanded!");
                break;
            case "the_revolution":
                foreach(GameState.Plot plot in GameState.Instance.GamePlots)
                {
                    plot.SetPlant("corn");
                }
                print("[Function Delegator] Corn is everything!");
                break;
            case "redistribute_the_wealth":
                GameState.Instance.Spend("fertilizer", GameState.Instance.GetResourceCount("fertilizer")*0.9f);
                print("[Function Delegator] 90% of fertilizer gone!");
                break;
            

        }
    }

}
