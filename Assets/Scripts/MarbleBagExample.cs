using UnityEngine;

public class MarbleBagExample : MonoBehaviour
{
    private MarbleBag<string> critBag;

    void Start()
    {
        critBag = new MarbleBag<string>();

        // 25% chance example: 2 critical hits, 6 normal hits
        critBag.Add("Critical Hit", 2);
        critBag.Add("Normal Hit", 6);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string outcome = critBag.Draw();
            Debug.Log($"Attack Result: {outcome} | Marbles left: {critBag.RemainingMarbles}");
        }
    }
}
