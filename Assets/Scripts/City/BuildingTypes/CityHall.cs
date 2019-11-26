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
        City CurrentCity = Game.CityManager.GetCity(citzen.CurrentCity);

        CitzenCredential status = Game.CityManager.GetCitzenInfo(citzen.ID, CurrentCity);

        switch (status.Vocation)
        {
            case VilagerVocation.ferreiro:
                //citzen.SetNewTask(new NPCTASK(NPCTasks.BlackSmith, Game.CityManager.GetBuildType(CurrentCity, BuildType.CityHall)));
                break;
            default:
                citzen.SetNoneJob();
                break;
        }
        base.WantInteract(entity);
    }
}
