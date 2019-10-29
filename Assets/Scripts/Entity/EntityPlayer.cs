using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class EntityPlayer : EntityLife
{
    public Animator Anim;
    public Rigidbody body;
    public Inventory Inve;
    public Transform World;
    public LifeStatus Status;
    public Transform HandRoot;
    public PlayerNetStats NetStats;
    public ParticleSystem FootPArticle;
    public UniversalStatusPlayer UniPlayer;
    public List<MonoBehaviour> ScriptToRemove = new List<MonoBehaviour>();
    public List<GameObject> ObjectToDisable = new List<GameObject>();
    public Transform Vector;
    public Animator AttackSword;
    public int animstatus;
    public float Speed = 5;
    public bool IsVisible = false;
    public bool IsMe = false;
    public int FootParticleCount = 1;
    public int pathsize = 20;

    protected SpriteRenderer SPRITERENDER;
    protected SpriteRenderer SPRITERENDERHAND;
    private Vector3 mousePos;
    private Vector3 lastposition;
    private int LastPostitionIntX;
    private int LastPostitionIntZ;

    void Start()
    {
        Net = GetComponent<NetWorkView>();
        Anim = GetComponent<Animator>();
        Inve = GetComponent<Inventory>();

        IsMe = Net.isMine;

        if (Net.isMine)
        {
            SPRITERENDER = GetComponent<SpriteRenderer>();
            SPRITERENDERHAND = HandRoot.GetComponentInChildren<SpriteRenderer>();
            //Game.TileAnimations.StartTileAnimation();//disabel for now
            transform.Rotate(new Vector3(3.6f, 0, 0), Space.Self);

            if (Game.WorldGenerator != null)
            {
                World = Game.WorldGenerator.transform;
            }

            Game.GameManager.CurrentPlayer.MyObject = gameObject;
            Game.GameManager.CurrentPlayer.MyInventory = GetComponent<Inventory>();
            Game.GameManager.CurrentPlayer.MyPlayerMove = this;
            Game.GameManager.CurrentPlayer.MyPlayerMove.IsAlive = true;

            Game.WorldGenerator.Setplayer_data();

            body.isKinematic = false;
        }
        else
        {
            foreach (var item in ScriptToRemove)
            {
                Destroy(item);
            }

            body.isKinematic = true;
        }
    }

    void UpdateOnMove()
    {
        if (DarckNet.Network.IsClient)
        {
            Net.RPC("UpdatePosition", DarckNet.RPCMode.AllNoOwner, new Vector3(transform.position.x, transform.position.y, transform.position.z));
        }
    }

    void UpdateOnMoveInt()
    {
        if (Game.WorldGenerator)
        {
            Game.WorldGenerator.UpdateFindChunk();

            Tile tile = Game.WorldGenerator.GetTileAt(transform.position.x, transform.position.z);
            var main = FootPArticle.main;

            NetStats.CurrentTile = tile.type;
            NetStats.CurrentBiome = tile.TileBiome;

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
                main.startColor = Color.green;
            }
            else if (tile.type == TypeBlock.Dirt)
            {
                main.startColor = Color.magenta;
            }
            else if (tile.type == TypeBlock.DirtRoad)
            {
                main.startColor = Color.magenta;
            }
            else if (tile.type == TypeBlock.Sand || tile.type == TypeBlock.BeachSand)
            {
                main.startColor = Color.yellow;
            }
            else if (tile.type == TypeBlock.Rock || tile.type == TypeBlock.RockGround)
            {
                main.startColor = Color.gray;
            }
            else if (tile.type == TypeBlock.Snow)
            {
                main.startColor = Color.white;
            }
        }

        if (MiniMapManager.manager)
        {
            //MiniMapManager.manager.UpdateMap();
        }
    }

    #region DirectionsMethods
    public void Direita()
    {
        if (NetStats.swiming == false)
        {
            NetStats.handhide = false;
            NetStats.HandLayer = SPRITERENDER.sortingOrder;
        }
    }
    public void Esquerda()
    {
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
    #endregion

    void Update()
    {
        if (IsVisible)//Do the Client Update, and Server.
        {
            if (DarckNet.Network.IsClient || Game.GameManager.SinglePlayer)///Client Update
            {
                if (IsMe)//check if this player is me.
                {
                    #region MyPlayerFunctions
                    if (Game.GameManager.MultiPlayer || Game.GameManager.SinglePlayer)
                    {
                        Status.UpdateStatus();
                        mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);

                        Vector3 movement = new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), 0, CrossPlatformInputManager.GetAxis("Vertical"));

                        body.velocity = movement.normalized * Speed;
                        SPRITERENDER.sortingOrder = -(int)transform.position.z;

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

                            NetStats.walking = true;
                            NetStats.Side = 0;
                        }
                        else if (Input.GetAxis("Horizontal") <= -0.1f)
                        {
                            Esquerda();
                            Anim.SetInteger("Walk", 1);
                            Anim.SetFloat("X", 180);

                            NetStats.walking = true;
                            NetStats.Side = 1;
                        }
                        else if (Input.GetAxis("Vertical") >= 0.1f)
                        {
                            Cima();
                            Anim.SetInteger("Walk", 1);
                            Anim.SetFloat("X", 90);

                            NetStats.walking = true;
                            NetStats.Side = 2;
                        }
                        else if (Input.GetAxis("Vertical") <= -0.1f)
                        {
                            Baixo();
                            Anim.SetInteger("Walk", 1);
                            Anim.SetFloat("X", -90);

                            NetStats.walking = true;
                            NetStats.Side = 3;
                        }
                        else
                        {
                            Anim.SetInteger("Walk", 0);
                            FootPArticle.Stop();

                            NetStats.walking = false;
                        }

                        UpdateNetStatus();
                    }
                    #endregion
                }
                else
                {

                }
            }
            else///Server Update
            {

            }
        }
    }

    void UpdateNetStatus()//for now is every frame send to all, just for stress test
    {
        Net.RPC("RPC_Syncplayervalues", DarckNet.RPCMode.AllNoOwner, NetStats.angle, NetStats.Side, NetStats.walking);
    }

    public void FootPrintRight()
    {
        FootPArticle.Emit(FootParticleCount);
    }

    public void FootPrintLeft()
    {
        FootPArticle.Emit(FootParticleCount);
    }

    private void OnBecameVisible()
    {
        IsVisible = true;
        Anim.enabled = true;
        Game.Entity_viewing.Add(this);
    }

    private void OnBecameInvisible()
    {
        IsVisible = false;
        Anim.enabled = false;
        Game.Entity_viewing.Remove(this);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (IsMe || DarckNet.Network.IsServer)
        {
            if (collision.tag == "TreeTrigger")
            {
                SpriteRenderer sprite = collision.transform.GetComponentInParent<SpriteRenderer>();

                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.3f);
            }
            else if (collision.tag == "ItemDrop")
            {
                collision.GetComponent<ItemDrop>().GetThisItem(Inve);
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
    }

    void OnTriggerExit(Collider collision)
    {
        if (IsMe || DarckNet.Network.IsServer)
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
    }

    public override void OnDead()
    {
        DarckNet.Network.Destroy(this.gameObject);
        Inve.DeletSave();
        Game.MenuManager.OpenRespawn();
        base.OnDead();
    }

    private void OnDestroy()
    {
        if (IsAlive)
        {
            if (IsMe)
            {
                Inve.Save();
                Debug.Log("Saved Your Player!");
            }
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
        if (Game.WorldGenerator.GetTileAt(x, y) .type == TypeBlock.Water)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

#region RPCs
    [RPC]
    void RPC_CURAITEM(int slot)
    {
        if (Inve.ItemList[slot].Index >= 0)
        {

        }
    }

    [RPC]
    void UpdatePosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [RPC]
    void RPC_Syncplayervalues(float angle, int side, bool iswalking)
    {
        if (IsVisible)// if this player is showing on the camera, the can do any update
        {
            int realside = 0;//is gone show the real direction of player in graus EX: 180, 90, 360 etc.

            switch (side)
            {
                case 0:
                    realside = 0;
                    break;
                case 1:
                    realside = 180;
                    break;
                case 2:
                    realside = 90;
                    break;
                case 3:
                    realside = -90;
                    break;
            } //Decode side of player, with this we can save transfer data

            Anim.SetInteger("Walk", iswalking ? 1 : 0);
            Anim.SetFloat("X", realside);

            NetStats.angle = angle;
            NetStats.Side = side;
            NetStats.walking = iswalking;
        }
    }
#endregion
}

[System.Serializable]
public class LifeStatus
{
    [Header("CharCracterstic")]
    public CharRace Race;
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
        if (Item.ITEMTYPE == ItemType.Weapon || Item.ITEMTYPE == ItemType.Tools)
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
            CalculateXp(ItemManager.Instance.GetItem(Random.Range(0, 7)), 1);
        }

        if (Input.GetKey(KeyCode.L))
        {
            CalculateXp((Skills)Random.Range(0, 11), -1);
            CalculateXp(ItemManager.Instance.GetItem(Random.Range(0, 7)), -1);
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

[System.Serializable]
public class UniversalStatusPlayer
{
    public AudioSource AUDIOSOURCE;
}

[System.Serializable]
public class PlayerNetStats
{
    public bool walking = false;
    public bool handhide = false;
    public bool swiming = false;
    public float angle = 0;
    public int HandLayer = 5;
    public TypeBlock CurrentTile;
    public BiomeType CurrentBiome;

    public int Side = -1;
}

public enum Skills : byte
{
    none, Combat, Survival, Cook, Fishing, Build, Politic, Wirter, Cartography, Mage, Baker, Merchant
}

public enum Language : byte
{
    none, StrageLanguage, OldHumanLanguage, HumanLanguage, OrcsLanguage, ElfLanguage
}