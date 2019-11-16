using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingHouse : CityBase
{
    public List<VillangerStaus> LivingEntity = new List<VillangerStaus>();
    public List<Entity> EntitysSpawned = new List<Entity>();
    public Vector3 SpawnPosition;
    public bool FamilyHouse;
    public bool HaveFather;
    public bool HaveMother;

    void Start()
    {
        SpawnPosition = new Vector3(SpawnPosition.x + transform.position.x, SpawnPosition.y+ transform.position.y, SpawnPosition.z + transform.position.z);
        int amount_entity = Random.Range(1, 5);
        FamilyHouse = System.Convert.ToBoolean(Random.Range(0, 2));

        for (int i = 0; i < amount_entity; i++)
        {
            if (FamilyHouse && amount_entity > 1)
            {
                if (!HaveFather)
                {
                    HaveFather = true;
                    SexualType sex_type = SexualType.Man;
                    LivingEntity.Add(new VillangerStaus(GetPersonName(sex_type), Game.UniqueID(2).GetHashCode().ToString(), BuildId, citypoint,Random.Range(28, 55), sex_type, FamilyPostiton.Father, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100), Random.Range(1, 100), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100)));
                }
                else if (!HaveMother)
                {
                    HaveMother = true;
                    SexualType sex_type = SexualType.Woman;
                    LivingEntity.Add(new VillangerStaus(GetPersonName(sex_type), Game.UniqueID(2).GetHashCode().ToString(), BuildId, citypoint, Random.Range(28, 55), sex_type, FamilyPostiton.Mother, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100), Random.Range(1, 100), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100)));
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
                        LivingEntity.Add(new VillangerStaus(GetPersonName(sex_type), Game.UniqueID(2).GetHashCode().ToString(), BuildId, citypoint, age, sex_type, familyPostiton, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100 / age), Random.Range(1, 100 / age), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100 * age)));
                    }
                    else if (age <= 25)
                    {
                        familyPostiton = FamilyPostiton.Son;
                        LivingEntity.Add(new VillangerStaus(GetPersonName(sex_type), Game.UniqueID(2).GetHashCode().ToString(), BuildId, citypoint, age, sex_type, familyPostiton, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100 / age), Random.Range(1, 100 / age), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100 * age)));
                    }
                }
            }
            else
            {
                int age = Random.Range(18, 80);
                SexualType sex_type = (SexualType)Random.Range(0, 3);
                FamilyPostiton familyPostiton = FamilyPostiton.none;

                LivingEntity.Add(new VillangerStaus(GetPersonName(sex_type), Game.UniqueID(2).GetHashCode().ToString(), BuildId, citypoint, age, sex_type, familyPostiton, (VilagerVocation)Random.Range(1, 18), Random.Range(1, 100 / age), Random.Range(1, 100 / age), Random.Range(80, 200), Random.Range(1, 100), Random.Range(1, 100 * age)));
            }

            SpawnNewEntity(LivingEntity[LivingEntity.Count - 1]);
        }
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

    public void SpawnNewEntity(VillangerStaus status)
    {
        GameObject obj = DarckNet.Network.Instantiate(Game.SpriteManager.GetPrefabOnRecources("Prefabs/Villager/Villager"), new Vector3(SpawnPosition.x, SpawnPosition.y, SpawnPosition.z), Quaternion.identity, Game.WorldGenerator.World_ID);
        EntitysSpawned.Add(obj.GetComponent<Vilanger>());
        obj.GetComponent<Vilanger>().Status = status;
    }

    private void OnDestroy()
    {
        foreach (var entity in EntitysSpawned)
        {
            DarckNet.Network.Destroy(entity.gameObject);
        }
    }
}
