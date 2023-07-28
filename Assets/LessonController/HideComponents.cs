using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple controller to prevent students from losing focus. Toggle it here to full enable/disable anything in the list.
public class HideComponents : MonoBehaviour
{
    [SerializeField] Component[] componentsToHide;
    [SerializeField] GameObject[] gameObjectsToHide;
    [SerializeField] bool willComponenetsBeHidden;
    [SerializeField] bool willGameObjectsBeHidden;


    private void OnValidate()
    {
        gameObject.hideFlags |= HideFlags.HideInHierarchy;
        gameObject.hideFlags |= HideFlags.HideInInspector;

        //  Uncomment this line to show the LessonController.
        //gameObject.hideFlags = HideFlags.None;
        foreach (var component in componentsToHide)
        {
            if (willComponenetsBeHidden)
            {
                component.hideFlags |= HideFlags.HideInInspector;
            }
            else
            {
                component.hideFlags = HideFlags.None;
            }
        }
        foreach (var g in gameObjectsToHide)
        {
            if (willGameObjectsBeHidden)
            {
                g.hideFlags |= HideFlags.HideInHierarchy;
            }
            else
            {
                g.hideFlags = HideFlags.None;
            }
        }
    }
}
