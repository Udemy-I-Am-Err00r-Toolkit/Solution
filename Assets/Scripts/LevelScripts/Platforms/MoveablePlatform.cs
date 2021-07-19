using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    //This script will allow platforms to move in three different types
    public class MoveablePlatform : PlatformManager
    {
        //The different types of movement the platform can move around the paths from the PathEditor script
        [SerializeField]
        protected enum PlatformTypes { Ascending, PingPong, StopOnEnd }
        [SerializeField]
        protected PlatformTypes platformType;
        //The number of points within the path the platform can move in; these points will change the direction the platform moves in based on the next point it needs to move to
        public List<Vector3> numberOfPaths = new List<Vector3>();
        //How fast the platform should be moving
        [SerializeField]
        protected float speed;
        //Determines if the platform has reached the end of its current point and needs to keep moving to the next
        protected bool needsToMove = true;
        //Determines if the platform is currently moving, meaning it hasn't reached the destination of a point
        protected bool moving;
        //Determines if the platform is a PingPong type, if it is going backwards in the iteration now
        protected bool pingPongGoingDown;
        //The point in the path the platform is on; by default all paths should start at numberOfPahts iteration value 0, so we hardcode this value here
        protected int currentPath = 0;
        //Finds out based on the currentPath value what the next point in the path should be
        protected int nextPath;

        public bool placePlatformOnFirstPath;

        protected override void Initialization()
        {
            base.Initialization();
            //Sets up the platform in the correct position based on the currentPath value; initially this is set to 0
            transform.position = numberOfPaths[currentPath];
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            FindThePath();
            MoveToPosition();
        }

        protected virtual void FindThePath()
        {
            //A different for loop that we usually write, this for loop starts the iteration at the currentPath value; typically we use for loops to start at 0
            for (int i = currentPath; i < numberOfPaths.Count; i++)
            {
                //If the platform is at the end of a path, it checks to see if the needsToMove bool is true still
                if (needsToMove)
                {
                    //If needsToMove is still true, we quickly set it to false so we can calculate the next path the platform needs to move in
                    needsToMove = false;
                    //Changes the currentPath iteration value to the most appropriate value based on what platformType it is
                    if (!pingPongGoingDown)
                    {
                        currentPath = i;
                    }
                    else
                    {
                        currentPath = i - 2;
                    }
                    //If the platform is Ascending, it will constantly move to the next path ahead of it, and then reset back to the begining once it reaches the end
                    if (platformType == PlatformTypes.Ascending)
                    {
                        nextPath = i + 1;
                        //This logic helps ensure the platform goes right back to the begining again in the most direct path if it reaches the end of the numberOfPaths list
                        if (nextPath == numberOfPaths.Count)
                        {
                            nextPath = 0;
                        }
                    }
                    //If the platform is PingPong, once it reaches the end it will go in reverse and pass through each point in the numberOfPaths list
                    if (platformType == PlatformTypes.PingPong)
                    {
                        //If the path is still going to the final iteration in the numberOfPaths list, the i value for the nextPath will be added
                        if (!pingPongGoingDown)
                        {
                            nextPath = i + 1;
                            //If the path reaches the end, we set the next path to the previous point by subtracting it by 2, and then set the pingPongGoingDown bool to true
                            if (nextPath == numberOfPaths.Count)
                            {
                                nextPath = i - 2;
                                currentPath--;
                                pingPongGoingDown = true;
                            }
                        }
                        //If the pingPongGoingDown bool is true, rather than adding one to the i value at the end of each point, it subtracts one
                        if (pingPongGoingDown)
                        {
                            nextPath = i - 1;
                            //If it gets back to the begining of the numberOfPaths list, it sets everything up so the platform will start going forward through that list again
                            if (nextPath == 0)
                            {
                                nextPath = 0;
                                currentPath = -1;
                                pingPongGoingDown = false;
                            }
                        }
                    }
                    //If the platform is simply supposed to stop at the end of the path, it doesn't iterate anymore and stops once it reaches the end of the numberOfPaths list
                    if (platformType == PlatformTypes.StopOnEnd)
                    {
                        nextPath = i + 1;
                        if (nextPath == numberOfPaths.Count)
                        {
                            return;
                        }
                    }
                    //Makes sure the platform is constantly moving as long as needsToMove is true
                    moving = true;
                }
            }
        }

        //This method will manage propper movement between the different paths for all three different types, it uses the nextPath value to calculate where it should move the platform towards next
        protected virtual void MoveToPosition()
        {
            if (moving)
            {
                if (transform.position == numberOfPaths[nextPath])
                {
                    moving = false;
                    needsToMove = true;
                    currentPath++;
                }
                //Debug.Log(nextPath);
                if (transform.position == numberOfPaths[nextPath] && currentPath == numberOfPaths.Count)
                {
                    currentPath = 0;
                }
                transform.position = Vector2.MoveTowards(transform.position, numberOfPaths[nextPath], speed * Time.deltaTime);
            }
        }
    }
}