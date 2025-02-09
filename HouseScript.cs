using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HouseScript : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] StormManager stormManager;
    [SerializeField] UIManager UIManager;
    


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        // Pop-up saying go to sleep?
        if (!stormManager.IsStormActive())
        {
            UIManager.ToggleSleepPanel();
        }
        else
        {
            // Check if storm is complete, pop up to sleep and go back to day
            List<GameObject> aliveEnemies = stormManager.GetAliveList();
            if(aliveEnemies.Count == 0 && stormManager.HasFinalWaveSpawned())
            {
                UIManager.ToggleSleepPanel();
            }
            else
            {
                UIManager.ToggleStormMessage();
            }
        }
        
    }
}
