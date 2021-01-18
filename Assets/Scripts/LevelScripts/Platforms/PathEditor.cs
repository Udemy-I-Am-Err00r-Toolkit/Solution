using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MetroidvaniaTools
{
    //This is a complicated script that allows custom Scene Editor tools to work with inspector values and allow paths to be drawn from a MoveablePlatform visually
    [CustomEditor(typeof(MoveablePlatform), true)]
    public class PathEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {
            //This is the color you want the tool to appear in the Scene Editor window; you can make this any color you want
            Handles.color = Color.green;
            //This is the type of objects that will have visual tools visible in the Scene Editor window; so any game object that contains the MoveablePlatform script will have access to these tools
            MoveablePlatform platform = target as MoveablePlatform;
            // This is to help organize the list of numberOfPaths with the custom Editor tools so you can easily spot the different iterations visually in the Scene Editor window
            int next = new int();
            //This for loop will count the numberOfPaths that object has withing the MoveablePlatform component
            for (int i = 0; i < platform.numberOfPaths.Count; i++)
            {
                //Logic that manages if the Inspector values in the numberOfPaths list on the MoveablePlatform component need to be adjusted; this is the first check to see
                EditorGUI.BeginChangeCheck();
                Vector3 position = platform.numberOfPaths[i];
                next = i + 1;
                if (next == platform.numberOfPaths.Count)
                {
                    next = 0;
                }
                //This is the new position that was moved in the Scene Editor window; this line of code is highly customizable. The '2' here is how big the circle would be that you see in the Scene Editor window, the Vector3 values are how much the values should move when moving by snap (holding control while moving), and the Handles.CircleHandleCap is the shape in which that tool is.
                Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, 2, new Vector3(.5f, .5f, .5f), Handles.CircleHandleCap);

                //This line draws a dotted line between the different paths so you can see how the platform will move between each point on the path; the '5' value here represents how big each dotted line should bel
                Handles.DrawDottedLine(platform.numberOfPaths[i], platform.numberOfPaths[next], 5);

                //This line names each of the points in the path so you can determine if it's the first point, second point, and so on within the path
                Handles.Label(platform.numberOfPaths[i], platform.gameObject.name + i.ToString());

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "MoveablePlatform");
                    platform.numberOfPaths[i] = newPosition;
                }
            }
        }
    }
}