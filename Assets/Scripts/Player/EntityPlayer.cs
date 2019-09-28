using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[System.Serializable]
public class PlayerNetStats
{
    public bool walking = false;
    public bool handhide = false;
    public bool swiming = false;
    public float angle = 0;
    public int HandLayer = 5;
    public TypeBlock CurrentTile;

    public int Side = -1;
}

public class EntityPlayer : EntityLife
{
    public AudioSource AUDIOSOURCE;
    public Transform World;
    public Rigidbody body;
    public Animator Anim;
    public ParticleSystem FootPArticle;
    public float Speed = 5;
    public PlayerNetStats NetStats;
    public Transform HandRoot;
    public Inventory myinve;
    public LifeStatus Status;
    protected SpriteRenderer SPRITERENDER;
    protected SpriteRenderer SPRITERENDERHAND;

    private Vector3 mousePos;
    public int animstatus;
    public Transform Vector;
    public Animator AttackSword;
    private Vector3 lastposition;
    int lastlayer = 0;

    public GameObject villagerteste;

    private int LastPostitionIntX;
    private int LastPostitionIntZ;

    public NetWorkView Net;
    public int FootParticleCount = 1;
    public int pathsize = 20;

    void Start()
    {
        Anim = GetComponent<Animator>();
        SPRITERENDER = GetComponent<SpriteRenderer>();
        SPRITERENDERHAND = HandRoot.GetComponentInChildren<SpriteRenderer>();

        Game.TileAnimations.StartTileAnimation();

		transform.Rotate(new Vector3(3.6f, 0,0), Space.Self);

        if (Net.isMine)
        {
            if (WorldGenerator.Instance != null)
            {
                World = WorldGenerator.Instance.transform;
            }

            body.isKinematic = false;
        }
        else
        {
            body.isKinematic = true;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "TreeTrigger")
        {
            SpriteRenderer sprite = collision.transform.GetComponentInParent<SpriteRenderer>();

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.3f);
        }
        else if (collision.tag == "ItemDrop")
        {
            collision.GetComponent<ItemDrop>().GetThisItem(myinve);
        }
        else if (collision.tag == "City")
        {
            Game.MenuManager.PopUpName("My Homes - City");
        }
        else if (collision.tag == "Entity")
        {
            //collision.GetComponent<Pathfindingentity>().Run(transform);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "TreeTrigger")
        {
            SpriteRenderer sprite = collision.transform.GetComponentInParent<SpriteRenderer>();

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }
        else if (collision.tag == "ItemDrop")
        {

        }
        else if (collision.tag == "Entity")
        {
            //collision.GetComponent<Pathfindingentity>().Stop();
        }
    }

    void UpdateOnMove()
    {
        if (DarckNet.Network.IsClient)
        {
            Net.RPC("UpdatePosition", DarckNet.RPCMode.AllNoOwner, new Vector3(transform.position.x, transform.position.y, 0));
        }

        if (World)
        {
           // World.position = transform.position;
        }

        /*layer -= (int)transform.position.y;

        if (layer > lastlayer)
        {
            GetComponent<SpriteRenderer>().sortingOrder -= (int)transform.position.y;
        }

        lastlayer = GetComponent<SpriteRenderer>().sortingOrder;*/
    }

    void UpdateOnMoveInt()
    {
        if (WorldGenerator.Instance)
        {
            WorldGenerator.Instance.UpdateFindChunk();

            Tile tile = WorldGenerator.Instance.GetTileAt(transform.position.x, transform.position.z);

            NetStats.CurrentTile = tile.type;

            if (tile.type == TypeBlock.Water)
            {
                Speed = 0.5f;
            }
            else if (tile.type == TypeBlock.IceWater)
            {
                Speed = 4;
            }
            else
            {
                Speed = 2f;
            }

            if (tile.type == TypeBlock.Grass)
            {
                FootPArticle.startColor = Color.green;
            }
            else if (tile.type == TypeBlock.Dirt)
            {
                FootPArticle.startColor = Color.magenta;
            }
            else if (tile.type == TypeBlock.DirtRoad)
            {
                FootPArticle.startColor = Color.magenta;
            }
            else if (tile.type == TypeBlock.Sand || tile.type == TypeBlock.BeachSand)
            {
                FootPArticle.startColor = Color.yellow;
            }
            else if (tile.type == TypeBlock.Rock || tile.type == TypeBlock.RockGround)
            {
                FootPArticle.startColor = Color.gray;
            }
            else if (tile.type == TypeBlock.Snow)
            {
                FootPArticle.startColor = Color.white;
            }
        }

        if (MiniMapManager.manager)
        {
            //MiniMapManager.manager.UpdateMap();
        }
    }

    public void Direita()
    {
        //Anim.SetFloat("X", 0);
        NetStats.Side = 0;
        if (NetStats.swiming == false)
        {
            NetStats.handhide = false;
            NetStats.HandLayer = SPRITERENDER.sortingOrder;
        }
    }
    public void Esquerda()
    {
        //Anim.SetFloat("X", 180);
        NetStats.Side = 1;
        if (NetStats.swiming == false)
        {
            NetStats.HandLayer = SPRITERENDER.sortingOrder - 1;
            NetStats.handhide = false;
        }
    }
    public void Cima()
    {
        if (NetStats.swiming == false)
        {
            NetStats.handhide = true;
            NetStats.HandLayer = SPRITERENDER.sortingOrder;
        }
    }
    public void Baixo()
    {
        if (NetStats.swiming == false)
        {
            NetStats.handhide = false;
            NetStats.HandLayer = SPRITERENDER.sortingOrder;
        }
    }

    void Update()
    {
        #region Client-Single

        if (Input.GetKey(KeyCode.P))
        {
            GameObject obj = Instantiate(villagerteste, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity);
            obj.GetComponent<Vilanger>().Born("VillagerTeste");
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject obj = Instantiate(villagerteste, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)), Quaternion.identity);
            obj.GetComponent<Vilanger>().Born("VillagerTeste");
        }

        if (Game.GameManager.MultiPlayer || Game.GameManager.SinglePlayer)
        {
            Status.UpdateStatus();
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            
            Vector3 movement = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, CrossPlatformInputManager.GetAxis("Vertical"));

            body.velocity = movement.normalized * Speed;
            SPRITERENDER.sortingOrder = -(int)transform.position.z;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                DoDamage(5, "Teste", true);
            }

            if (Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
                lookPos = lookPos - transform.position;

                float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
                //Vector.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                NetStats.angle = (int)angle;

                Anim.SetFloat("X", (int)angle);
            }

            #region UpDateOnMove
            if (lastposition != transform.position)
            {
                UpdateOnMove();
            }
            lastposition = transform.position;

            if (LastPostitionIntX != (int)transform.position.x || LastPostitionIntZ != (int)transform.position.z)
            {
                UpdateOnMoveInt();
            }
            LastPostitionIntX = (int)transform.position.x;
            LastPostitionIntZ = (int)transform.position.z;
            #endregion

            if (NetStats.handhide == true)
            {
                HandRoot.gameObject.SetActive(false);
            }
            else
            {
                HandRoot.gameObject.SetActive(true);
            }

            if (SPRITERENDERHAND)
            {
                SPRITERENDERHAND.sortingOrder = NetStats.HandLayer;
            }

            if (Input.GetAxis("Horizontal") >= 0.1f)
            {
                Direita();
                Anim.SetInteger("Walk", 1);
                Anim.SetFloat("X", 0);
            }
            else if (Input.GetAxis("Horizontal") <= -0.1f)
            {
                Esquerda();
                Anim.SetInteger("Walk", 1);
                Anim.SetFloat("X", 180);
            }
            else if (Input.GetAxis("Vertical") >= 0.1f)
            {
                Cima();
                Anim.SetInteger("Walk", 1);
                Anim.SetFloat("X", 90);
            }
            else if (Input.GetAxis("Vertical") <= -0.1f)
            {
                Baixo();
                Anim.SetInteger("Walk", 1);
                Anim.SetFloat("X", -90);
            }
            else
            {
                Anim.SetInteger("Walk", 0);
                FootPArticle.Stop();
            }
        }
        #endregion

        #region Server
        #endregion
    }

    public void FootPrintRight()
    {
        AUDIOSOURCE.PlayOneShot(Game.AudioManager.GetFootSound(NetStats.CurrentTile));
        FootPArticle.Emit(FootParticleCount);
    }

    public void FootPrintLeft()
    {
        AUDIOSOURCE.PlayOneShot(Game.AudioManager.GetFootSound(NetStats.CurrentTile));
        FootPArticle.Emit(FootParticleCount);
    }

    public override void OnDead()
    {
        DarckNet.Network.Destroy(this.gameObject);
        myinve.DeletSave();
        Game.MenuManager.OpenRespawn();
        base.OnDead();
    }

    private void OnDestroy()
    {
        if (IsAlive)
        {
            myinve.Save();
            Debug.Log("Saved Your Player!");
        }
    }

    public override void FinishDamage()
    {
        Game.MenuManager.LifeBar.RefreshBar(HP);
        base.FinishDamage();
    }

    public override void FinishCura()
    {
        Game.MenuManager.LifeBar.RefreshBar(HP);
        base.FinishCura();
    }

    public bool IsOnWater(int x, int y)
    {
        if (WorldGenerator.Instance.GetTileAt(x, y) .type == TypeBlock.Water)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class LifeStatus
{
    [Header("CharStatus")]
    public float Water = 80;
    public float Food = 80;
    public float Mana = 80;
    [Header("SocialStatus")]
    public int Age = 19;
    public int Friends = 0;
    public int SocialFactor = 100;
    public List<SkillStruc> SkillsList = new List<SkillStruc>();
    public List<WepoSkillStruc> WPSkillsList = new List<WepoSkillStruc>();
    public float XP_PLAYER = 0;

    public void AddSkillDefault(Skills skill)
    {
        switch (skill)
        {
            case Skills.Build:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Cartography:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Combat:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Cook:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Fishing:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Politic:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Survival:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            case Skills.Wirter:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
            default:
                SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
                break;
        }
    }

    public void CalculateXp(Skills skill, float Xpadd)
    {
        SkillStruc CurrentSkill = null;

        foreach (var skillitem in SkillsList)
        {
            if (skillitem.Type == skill)
            {
                CurrentSkill = skillitem;
            }
        }

        if (CurrentSkill == null)
        {
            SkillsList.Add(new SkillStruc(skill, 0, 0, 100, 100));
            CurrentSkill = SkillsList[SkillsList.Count -1];
        }

        if (!CurrentSkill.LevelMax)
        {
            CurrentSkill.SkillXp += Xpadd;

            if (CurrentSkill.SkillXp >= CurrentSkill.MaxSkillXp)
            {
                if (CurrentSkill.SkillLevel >= CurrentSkill.MaxSkillLevel)
                {
                    CurrentSkill.LevelMax = true;
                }
                else
                {
                    CurrentSkill.SkillLevel += 1;
                }

                CurrentSkill.SkillXp = 0;
            }
            StatusWindow.Instance.Refresh();
        }
    }

    public void CalculateXp(ItemData Item, float Xpadd)
    {
        if (Item.ITEMTYPE == ItemType.Weapon)
        {
            WepoSkillStruc CurrentSkill = null;

            foreach (var skillitem in WPSkillsList)
            {
                if (skillitem.Itemid == Item.Index)
                {
                    CurrentSkill = skillitem;
                }
            }

            if (CurrentSkill == null)
            {
                WPSkillsList.Add(new WepoSkillStruc(Item.Index, 0, 0, 100, 100));
                CurrentSkill = WPSkillsList[WPSkillsList.Count - 1];
            }

            if (!CurrentSkill.LevelMax)
            {
                CurrentSkill.SkillXp += Xpadd;

                if (CurrentSkill.SkillXp >= CurrentSkill.MaxSkillXp)
                {
                    if (CurrentSkill.SkillLevel >= CurrentSkill.MaxSkillLevel)
                    {
                        CurrentSkill.LevelMax = true;
                    }
                    else
                    {
                        CurrentSkill.SkillLevel += 1;
                    }
                    CurrentSkill.SkillXp = 0;
                }

                StatusWindow.Instance.Refresh();
            }
        }
    }

    public void UpdateStatus()
    {
        if (Input.GetKey(KeyCode.K))
        {
            CalculateXp((Skills)Random.Range(0, 11), 1);
            CalculateXp(ItemManager.Instance.GetItem(Random.Range(0, 6)), 1);
        }

        if (Input.GetKey(KeyCode.L))
        {
            CalculateXp((Skills)Random.Range(0, 11), -1);
            CalculateXp(ItemManager.Instance.GetItem(Random.Range(0, 6)), -1);
        }
    }

    public void Eat()
    {

    }

    public void Drink()
    {

    }

    public void DrinkMana()
    {

    }
}

[System.Serializable]
public class SkillStruc
{
    public Skills Type;
    public int SkillLevel;
    public float SkillXp;
    public int MaxSkillXp;
    public int MaxSkillLevel;
    public bool LevelMax = false;

    public SkillStruc(Skills type,int skilllevel, float skillxp, int maxskillxp, int maxskilllevel)
    {
        Type = type;
        SkillLevel = skilllevel;
        SkillXp = skillxp;
        MaxSkillXp = maxskillxp;
        MaxSkillLevel = maxskilllevel;
    }
}

[System.Serializable]
public class WepoSkillStruc
{
    public int Itemid;
    public int SkillLevel;
    public float SkillXp;
    public int MaxSkillXp;
    public int MaxSkillLevel;
    public bool LevelMax = false;

    public WepoSkillStruc(int itemid, int skilllevel, float skillxp, int maxskillxp, int maxskilllevel)
    {
        Itemid = itemid;
        SkillLevel = skilllevel;
        SkillXp = skillxp;
        MaxSkillXp = maxskillxp;
        MaxSkillLevel = maxskilllevel;
    }
}

public enum Skills : byte
{
    none, Combat, Survival, Cook, Fishing, Build, Politic, Wirter, Cartography, Mage, Baker, Merchant
}

public enum Language : byte
{
    none, StrageLanguage, OldHumanLanguage, HumanLanguage, OrcsLanguage, ElfLanguage
}