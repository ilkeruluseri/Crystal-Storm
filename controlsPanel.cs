using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlsPanel : MonoBehaviour
{
    [SerializeField] GameObject controls;
    public void ToggleControlsPanel()
    {
        controls.SetActive(!controls.activeSelf);
    }
}
