using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utlity 
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        System.Random RandomGeneratingNum = new System.Random(seed);
        for(int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = RandomGeneratingNum.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }
}
