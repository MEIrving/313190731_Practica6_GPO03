using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayCreator : MonoBehaviour
{
    public Color lineColor=Color.white;
    public List<Transform> path_objs=new List<Transform>();
    public float sizePoints=0.01f;
    Transform[] pointsArray; 
    private void OnDrawGizmos()
    {
        Gizmos.color=lineColor;
        pointsArray=GetComponentsInChildren<Transform>();
        path_objs.Clear();

        foreach(Transform path_obj in pointsArray)
        {
            if(path_obj!=this.transform)
            {
                path_objs.Add(path_obj);
            }
        }

        for(int i=0;i<path_objs.Count;i++)
        {
            Vector3 position=path_objs[i].position;
            if(i>0)
            {
                Vector3 previous=path_objs[i-1].position;    
                Gizmos.DrawLine(previous,position);
                Gizmos.DrawWireSphere(position,sizePoints);          
            }
        }

    }
}
