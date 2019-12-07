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
    public Vector2 LastPosition;
    public Chunk Cuerrent_Chunk;
    private Vector3 velocity;
    public float damping = 1;
    public bool RunAway = false;
    public int direction = 0;
    public bool ISVISIBLE = false;

    public MeshRenderer meshRenderer;

    public int X;
    public int Z;

    public string ID;
    public Vector3 CurrentCity;

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

        Net = GetComponent<NetWorkView>();
        Cuerrent_Chunk = Game.WorldGenerator.GetChunkAt((int)transform.position.x, (int)transform.position.z);
        Cuerrent_Chunk.Entitys.Add(this);

        GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value, 1);
        GetComponent<MeshRenderer>().sortingOrder = -(int)transform.position.z;
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0.05f);
    }

    public void SetNewTask(NPCTASK newtask)
    {
        //Status = Game.CityManager.UpdateEntityTask(newtask, Status.currentcity.ToUnityVector(), Status.Citzen_Id);
        //Go(Status.CurrentTask.TaskPosition.ToUnityVector() + new Vector3(+1, 0 ,-1));
    }

    public void SetNoneJob()
    {
        //Game.CityManager.UpdateEntityTask(new NPCTASK(NPCTasks.none, DarckNet.DataVector3.zero), Status.currentcity.ToUnityVector(), Status.Citzen_Id);
        GetNewPostion();
    }

    internal void GetNewPostion()
    {
        Go(new Vector3(Random.Range(transform.position.x - 10, transform.position.x + 10), 0, Random.Range(transform.position.z - 10, transform.position.z + 10)));
    }

    public override void Updateoverride()
    {
        if (DarckNet.Network.IsServer || Game.GameManager.SinglePlayer)
        {
            if (new Vector3(transform.position.x, transform.position.y, transform.position.z) == new Vector3(target.x, target.y, target.z))
            {
                if (HaveTarget)
                {
                    Game.CityManager.WantInteract(CurrentCity, ID, this);
                    Stop();
                }
            }

            if ((int)transform.position.x != X || (int)transform.position.z != Z)
            {
                Chunk chunk = Game.WorldGenerator.GetChunkAt((int)transform.position.x, (int)transform.position.z);

                Game.CityManager.UpdatePositionStaus(transform.position, CurrentCity, ID);

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

                Vector3 velocityVector = new Vector3(rightDotProduct * 2, upDotProduct * 2, fwdDotProduct * 2);

                if (velocityVector.x >= 1f)
                {
                    Anim.SetInteger("Walk", 1);

                    Anim.SetFloat("X", Input.GetAxis("Horizontal"));
                    Anim.SetFloat("Y", Input.GetAxis("Vertical"));

                    transform.LookAt(new Vector3(transform.position.x + velocityVector.x, 0, transform.position.z + velocityVector.z));

                    direction = 0;
                }
                else if (velocityVector.x <= -1)
                {
                    Anim.SetInteger("Walk", 1);

                    Anim.SetFloat("X", Input.GetAxis("Horizontal"));
                    Anim.SetFloat("Y", Input.GetAxis("Vertical"));

                    transform.LookAt(new Vector3(transform.position.x + velocityVector.x, 0, transform.position.z + velocityVector.z));

                    direction = 3;
                }
                else if (velocityVector.z >= 1)
                {
                    Anim.SetInteger("Walk", 1);

                    Anim.SetFloat("X", Input.GetAxis("Horizontal"));
                    Anim.SetFloat("Y", Input.GetAxis("Vertical"));

                    transform.LookAt(new Vector3(transform.position.x + velocityVector.x, 0, transform.position.z + velocityVector.z));

                }
                else if (velocityVector.z <= -1)
                {
                    Anim.SetInteger("Walk", 1);

                    Anim.SetFloat("X", Input.GetAxis("Horizontal"));
                    Anim.SetFloat("Y", Input.GetAxis("Vertical"));

                    transform.LookAt(new Vector3(transform.position.x + velocityVector.x, 0, transform.position.z + velocityVector.z));

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
        Game.Entity_viewing.Add(this);
    }

    private void OnBecameInvisible()
    {
        ISVISIBLE = false;
        Anim.enabled = false;
        Game.Entity_viewing.Remove(this);
    }

    private void OnDestroy()
    {
        Game.CityManager.EntitysSpawned.Remove(ID);
    }

    [RPC]
    void RPC_SyncPos(Vector3 pos)
    {
        transform.position = pos;
    }

    [RPC]
    void RPC_BORN(string name)
    {
        GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value, 1);
        GetComponent<MeshRenderer>().sortingOrder = -(int)transform.position.z;
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0.05f);

        transform.Rotate(new Vector3(-87.839f, 0, 0), Space.Self);
    }
}