using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityHall : CityBase
{
    void Start()
    {
        
    }

    public override void WantInteract(Entity entity)
    {
        Vilanger citzen = entity.GetComponent<Vilanger>();
        City CurrentCity = Game.CityManager.GetCity(citzen.Status.currentcity.ToUnityVector());

        switch (citzen.Status.Vocation)
        {
            case VilagerVocation.ferreiro:
                citzen.SetNewTask(new NPCTASK(NPCTasks.BlackSmith, CurrentCity.blackpos.ToUnityVector()));
                break;
            default:
                citzen.SetNoneJob();
                break;
        }
        base.WantInteract(entity);
    }
}
