using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRadioGroupManager : MonoBehaviour
{
    public static InteractableRadioGroupManager instance;
    public Dictionary<string, InteractableRadioGroup> radioGroups = new Dictionary<string, InteractableRadioGroup>();

	private void Awake()
	{
        instance = this;
	}

	public void CreateRadioGroup(string group)
	{
        if (!radioGroups.ContainsKey(group))
        {
            InteractableRadioGroup irg = new InteractableRadioGroup();
            irg.identifier = group;
            radioGroups.Add(group, irg);
        }
	}

    public void DeleteRadioGroup(string group)
    {
        if (radioGroups.ContainsKey(group))
            radioGroups.Remove(group);
    }

    public void AddElementToGroup(string group, InteractableManager element)
	{
        if (radioGroups.ContainsKey(group))
            radioGroups[group].AddElement(element);
    }

    public void RemoveElementFromGroup(string group, InteractableManager element)
    {
        if (radioGroups.ContainsKey(group))
            radioGroups[group].RemoveElement(element);
    }

    public void EnableAllElementsFromGroup(string group)
	{
        if (radioGroups.ContainsKey(group))
            radioGroups[group].EnableAllElement();
    }

    public void DisableAllElementsFromGroup(string group)
    {
        if (radioGroups.ContainsKey(group))
            radioGroups[group].DisableAllElement();
    }
}
