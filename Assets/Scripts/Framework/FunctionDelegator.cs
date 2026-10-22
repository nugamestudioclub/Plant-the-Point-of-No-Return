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
            

        }
    }

}
