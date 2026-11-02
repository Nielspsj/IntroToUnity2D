using System.Collections.Generic;
using UnityEngine;

public class MarbleBag<T>
{
    private List<T> bag = new List<T>();
    private List<T> initialContents = new List<T>();

    // Add a marble (item) to the bag
    public void Add(T item, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            initialContents.Add(item);
            bag.Add(item);
        }
        Shuffle();
    }

    // Pull a random marble from the bag
    public T Draw()
    {
        if (bag.Count == 0)
        {
            Refill();
        }

        int index = Random.Range(0, bag.Count);
        T drawnItem = bag[index];
        bag.RemoveAt(index);
        return drawnItem;
    }

    // Refill and shuffle the bag when empty
    private void Refill()
    {
        bag.AddRange(initialContents);
        Shuffle();
    }

    // Shuffle the list using Fisher-Yates algorithm
    public void Shuffle()
    {
        for (int i = 0; i < bag.Count; i++)
        {
            T temp = bag[i];
            int randomIndex = Random.Range(i, bag.Count);
            bag[i] = bag[randomIndex];
            bag[randomIndex] = temp;
        }
    }

    public int RemainingMarbles => bag.Count;
}
