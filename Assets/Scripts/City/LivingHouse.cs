using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingHouse : CityBase
{
    public Vector3 SpawnPosition;
    public bool FamilyHouse;
    public bool HaveFather;
    public bool HaveMother;

    void Start()
    {
    }

    public override void NewBuild()
    {
        Debug.Log("NewHouse : " + BuildId);

        City currentcity = Game.CityManager.GetCity(citypoint.ToUnityVector());
        SpawnPosition = new Vector3(SpawnPosition.x + transform.position.x, SpawnPosition.y + transform.position.y, SpawnPosition.z + transform.position.z);
        int amount_entity = Random.Range(1, 5);
        FamilyHouse = System.Convert.ToBoolean(Random.Range(0, 2));

        for (int i = 0; i < amount_entity; i++)
        {
            string CitzentId = Game.UniqueID(2).GetHashCode().ToString();

            if (currentcity != null)
            {
                if (!currentcity.LivingEntity.ContainsKey(CitzentId))
                {
                    if (FamilyHouse && amount_entity > 1)
                    {
                        if (!HaveFather)
                        {
                            HaveFather = true;
                            SexualType sex_type = SexualType.Man;
                            currentcity.LivingEntity.Add(CitzentId, new CitzenCredential(GetPersonName(sex_type), CitzentId, BuildId, citypoint, Random.Range(28, 55), sex_type, FamilyPostiton.Father, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100), Random.Range(1, 100), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100), new NPCTASK(NPCTasks.none, DarckNet.DataVector3.zero)));
                        }
                        else if (!HaveMother)
                        {
                            HaveMother = true;
                            SexualType sex_type = SexualType.Woman;
                            currentcity.LivingEntity.Add(CitzentId, new CitzenCredential(GetPersonName(sex_type), CitzentId, BuildId, citypoint, Random.Range(28, 55), sex_type, FamilyPostiton.Mother, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100), Random.Range(1, 100), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100), new NPCTASK(NPCTasks.none, DarckNet.DataVector3.zero)));
                        }
                        else
                        {
                            int age = Random.Range(18, 80);
                            SexualType sex_type = (SexualType)Random.Range(0, 3);
                            FamilyPostiton familyPostiton = FamilyPostiton.none;

                            if (age >= 65)
                            {
                                sex_type = (SexualType)Random.Range(1, 3);

                                if (sex_type == SexualType.Man)
                                {
                                    familyPostiton = FamilyPostiton.GrandFather;
                                }
                                else if (sex_type == SexualType.Woman)
                                {
                                    familyPostiton = FamilyPostiton.GrandMother;
                                }
                                currentcity.LivingEntity.Add(CitzentId, new CitzenCredential(GetPersonName(sex_type), CitzentId, BuildId, citypoint, age, sex_type, familyPostiton, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100 / age), Random.Range(1, 100 / age), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100 * age), new NPCTASK(NPCTasks.none, DarckNet.DataVector3.zero)));
                            }
                            else if (age <= 25)
                            {
                                familyPostiton = FamilyPostiton.Son;
                                currentcity.LivingEntity.Add(CitzentId, new CitzenCredential(GetPersonName(sex_type), CitzentId, BuildId, citypoint, age, sex_type, familyPostiton, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100 / age), Random.Range(1, 100 / age), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100 * age), new NPCTASK(NPCTasks.none, DarckNet.DataVector3.zero)));
                            }
                        }
                    }
                    else
                    {
                        int age = Random.Range(18, 80);
                        SexualType sex_type = (SexualType)Random.Range(0, 3);
                        FamilyPostiton familyPostiton = FamilyPostiton.none;

                        currentcity.LivingEntity.Add(CitzentId, new CitzenCredential(GetPersonName(sex_type), CitzentId, BuildId, citypoint, age, sex_type, familyPostiton, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100 / age), Random.Range(1, 100 / age), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100 * age), new NPCTASK(NPCTasks.none, DarckNet.DataVector3.zero)));
                    }

                    if (currentcity.LivingEntity.ContainsKey(CitzentId))//Cehck again the entity is sucessed add to city list
                    {
                        GameObject obj = Game.CityManager.SpawnNewEntity(currentcity.LivingEntity[CitzentId], SpawnPosition);
                        currentcity.LivingEntity[CitzentId].IsOutSide = true;

                        currentcity.LivingEntity[CitzentId].CurrentTask = new NPCTASK(NPCTasks.GoGetTask, currentcity.hallpos);
                        obj.GetComponent<Vilanger>().Go(currentcity.hallpos.ToUnityVector() + new Vector3(+1, 0, -1));
                    }
                }
            }
        }
        base.NewBuild();
    }

    public override void LoadBuild()
    {
        City currentcity = Game.CityManager.GetCity(citypoint.ToUnityVector());
        SpawnPosition = new Vector3(SpawnPosition.x + transform.position.x, SpawnPosition.y + transform.position.y, SpawnPosition.z + transform.position.z);
        foreach (var entity in currentcity.LivingEntity.Values)
        {
            if (entity.LivingHouseId == BuildId)
            {
                if (entity.IsOutSide)
                {
                    GameObject obj = Game.CityManager.SpawnNewEntity(entity, entity.WorldPostion.ToUnityVector());
                    currentcity.LivingEntity[entity.Citzen_Id].IsOutSide = true;
                }
            }
        }
        base.LoadBuild();
    }

    public override void WantInteract(Entity entity)
    {
        Vilanger citzen = entity.GetComponent<Vilanger>();

        Game.CityManager.RemoveEntityFromWorld(citzen.CurrentCity, citzen.ID, citzen.gameObject);
        base.WantInteract(entity);
    }

    private string GetPersonName(SexualType sex_type)
    {
        string Name_ready = "";

        if (sex_type == SexualType.Man)
        {
            MaleHumanNames M = (MaleHumanNames)Random.Range(0, 14);
            Name_ready = M.ToString();
        }
        else if (sex_type == SexualType.Woman)
        {
            FemaleHumanNames F = (FemaleHumanNames)Random.Range(0, 20);
            Name_ready = F.ToString();
        }
        else
        {
            Name_ready = "NotReady Now";
        }

        return Name_ready;
    }
}
