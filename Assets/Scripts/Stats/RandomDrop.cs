using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles the logic to have an item randomly drop when Enemy is killed
public class RandomDrop : MonoBehaviour
{
    //The different items that the Enemy will drop when they are killed
    [SerializeField]
    protected List<GameObject> randomDrops = new List<GameObject>();
    //The percentage value out of 100 (or really whatever value you want to roll with) that each item would drop at; this value is best setup by having the most common drop percentages at the top of the array, and rarerst drop percentages at the bottom of the array
    [SerializeField]
    protected int[] dropPercentages;

    //The method that handles the random value for the drop
    public virtual void Roll()
    {
        //The initial value of the roll is always 0
        int randomNumber = 0;
        //The total amount of value that the random drops would drop at; this value will then be set to the sum of all the dropPercentag values in that array; in the course I have this set at 100, but it can be grown or shrank from there
        int total = 0;
        //This finds the total value from above by adding all the different values in the dropPercentages array
        foreach (int item in dropPercentages)
        {
            total += item;
        }
        //Picks a random number between 1 and whatever the total value is
        randomNumber = Random.Range(1, total);
        for (int i = 0; i < dropPercentages.Length; i++)
        {
            //Checks to see if the randomNumber is less than the dropPercentage values; it checks the most common value first, and if the randomNumber is less than the most common value, it drops the most common item
            if (randomNumber <= dropPercentages[i])
            {
                Instantiate(randomDrops[i], transform.position, Quaternion.identity);
                return;
            }
            //If the randomNumber is greater than the value listed above, it iterates again and negates the randomNumber value from whatever that tier of dropPercentage value is; for example, if the most common drop will drop at 50, and the randomNumber value is 90, it will negate 50 from 90 leaving you with a value of 40 and use this new value to see if it is less than the next value for dropPercentage.
            else
            {
                randomNumber -= dropPercentages[i];
            }
        }
    }
}
