using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vilanger : Pathfindingentity
{
    [System.NonSerialized] private Vector3 prevPos;
    [System.NonSerialized] public Rigidbody2D body;
    [System.NonSerialized] public Animator Anim;
    [System.NonSerialized] public float Distance = 10;

    public string Name = "";
    public VilagerVocation Type = VilagerVocation.none;
    public Vector2 LastPosition;
    private SpriteRenderer Render;
    public Chunk Cuerrent_Chunk;
    private Vector3 velocity;
    public float damping = 1;
    public bool RunAway = false;
    public int direction = 0;
    public bool ISVISIBLE = false;    

    public void Born(string name, Chunk CurrentChunk)
    {
        Net = GetComponent<NetWorkView>();
        Cuerrent_Chunk = CurrentChunk;
        GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value, 1);
        GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.z;
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0.05f);
        Name = name;
        Type = VilagerVocation.none;
		transform.Rotate(new Vector3(-87.839f, 0,0), Space.Self);


        if (DarckNet.Network.IsServer)
        {
            Net.RPC("RPC_BORN", DarckNet.RPCMode.AllNoOwner, name);
        }
    }

    public void GetVocation()
    {
        if (RandomInt() == 1)
        {
           
        }
    }

    int RandomInt()
    {
        return Random.Range(0, 100);
    }

    public override void Startoverride()
    {
        body = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Render = GetComponent<SpriteRenderer>();
    }

    internal void GetNewPostion()
    {
        Run(new Vector3(Random.Range(transform.position.x - 10, transform.position.x + 10), 0, Random.Range(transform.position.z - 10, transform.position.z + 10)));
    }

    public int X;
    public int Z;

    public override void Updateoverride()
    {
        if (DarckNet.Network.IsServer || Game.GameManager.SinglePlayer)
        {
            if (transform.position == target)
            {
                if (HaveTarget)
                {
                    Stop();
                }
            }

            if (transform.position.x != X || transform.position.z != Z)
            {
                Chunk chunk = Game.WorldGenerator.GetChunkAt((int)transform.position.x, (int)transform.position.z);

                if (chunk != null)
                {
                    if (chunk != Cuerrent_Chunk)
                    {
                        Chunk lastchunk = Cuerrent_Chunk;
                        lastchunk.Entitys.Remove(this);
                        Cuerrent_Chunk = chunk;
                        Cuerrent_Chunk.Entitys.Add(this);
                    }
                }

                Net.RPC("RPC_SyncPos", DarckNet.RPCMode.AllNoOwner, transform.position);
            }
            X = (int)transform.position.x;
            Z = (int)transform.position.z;
        }
        else if (DarckNet.Network.IsClient || Game.GameManager.SinglePlayer)
        {
            if (ISVISIBLE)
            {
                var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
                var upDotProduct = Vector3.Dot(transform.up, velocity);
                var rightDotProduct = Vector3.Dot(transform.right, velocity);
                Render.sortingOrder = -(int)transform.position.z;

                Vector3 velocityVector = new Vector3(rightDotProduct, upDotProduct, fwdDotProduct);

                if (velocityVector.x >= 1f)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", 0);
                    direction = 0;
                }
                else if (velocityVector.x <= -1)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", 180);
                    direction = 3;
                }
                else if (velocityVector.z >= 1)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", 90);
                    direction = 1;
                }
                else if (velocityVector.z <= -1)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", -90);
                    direction = 2;
                }
                else
                {
                    Anim.SetInteger("Walk", 0);
                }
            }

        }
    }

    public void FootPrintRight()
    {
        
    }

    public void FootPrintLeft()
    {
        
    }

    void FixedUpdate()
    {
        velocity = (transform.position - prevPos) * 5 / Time.deltaTime;
        prevPos = transform.position;
    }

    private void OnBecameVisible()
    {
        ISVISIBLE = true;
        Anim.enabled = true;
    }

    private void OnBecameInvisible()
    {
        ISVISIBLE = false;
        Anim.enabled = false;
    }

    [RPC]
    void RPC_SyncPos(Vector3 pos)
    {
        transform.position = pos;
    }

    [RPC]
    void RPC_BORN(string name)
    {
        GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value, 1);
        GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.z;
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0.05f);
        Name = name;
        Type = VilagerVocation.none;
        transform.Rotate(new Vector3(-87.839f, 0, 0), Space.Self);
    }
}