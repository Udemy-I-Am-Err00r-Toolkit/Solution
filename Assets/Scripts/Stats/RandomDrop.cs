using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDrop : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> randomDrops = new List<GameObject>();
    [SerializeField]
    protected int[] dropPercentages;

    public virtual void Roll()
    {
        int randomNumber = 0;
        int total = 0;

        foreach(int item in dropPercentages)
        {
            total += item;
        }
        randomNumber = Random.Range(1, total);
        for(int i = 0; i < dropPercentages.Length; i ++)
        {
            if(randomNumber <= dropPercentages[i])
            {
                Instantiate(randomDrops[i], transform.position, Quaternion.identity);
                return;
            }
            else
            {
                randomNumber -= dropPercentages[i];
            }
        }
    }
}
