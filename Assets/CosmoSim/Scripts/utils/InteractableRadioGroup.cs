using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRadioGroup : MonoBehaviour
{
    public string identifier;
    public List<InteractableManager> radioGroup = new List<InteractableManager>();

    public void AddElement(InteractableManager element)
	{
        if (!radioGroup.Contains(element))
            radioGroup.Add(element);
    }

    public void RemoveElement(InteractableManager element)
    {
        if (!radioGroup.Contains(element))
            radioGroup.Remove(element);
    }

    public void DisableElement(InteractableManager element)
    {
        element.GetComponent<BoxCollider>().enabled = false;
    }

    public void EnableElement(InteractableManager element)
    {
        element.GetComponent<BoxCollider>().enabled = true;
    }

    public void DisableAllElement()
    {        
        foreach (InteractableManager im in radioGroup)
        {            
            im.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void EnableAllElement()
    {
        foreach (InteractableManager im in radioGroup)
        {
            im.GetComponent<BoxCollider>().enabled = true;
        }
    }
}
