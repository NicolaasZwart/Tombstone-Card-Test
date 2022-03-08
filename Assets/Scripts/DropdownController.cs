using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour
{
    [Header("Names of disableable toggles.")]
    [Tooltip("Enter the string name for the toggles you want to disable.")]
    [SerializeField] string toggleName1;
    [SerializeField] string toggleName2;
    [SerializeField] string toggleName3;
    [SerializeField] string toggleName4;

    void Start()
    {
        //This script should be attached to Item
        Toggle toggle = gameObject.GetComponent<Toggle>();
        Debug.Log(toggle);
        if (toggle != null && toggle.name == toggleName1)
        {
            toggle.interactable = false;
        }
        if (toggle != null && toggle.name == toggleName2)
        {
            toggle.interactable = false;
        }
        if (toggle != null && toggle.name == toggleName3)
        {
            toggle.interactable = false;
        }
        if (toggle != null && toggle.name == toggleName4)
        {
            toggle.interactable = false;
        }
    }
}
