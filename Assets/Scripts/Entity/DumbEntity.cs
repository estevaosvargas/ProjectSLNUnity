using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbEntity : EntityLife
{
    public Transform target;
    public float damping = 1;
    public float Distance = 10;
    public bool RunAway = false;
    public Rigidbody body;
    public Animator Anim;
    private Vector3 velocity;
    private Vector3 prevPos;
    private float timetemp;
    private float timetempD;
    public float damageCoolDown = 1;
    public float cooldown = 2;
    private bool knocked = false;

    public Transform attackPoint;
    public LayerMask EntitysLayer;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();

        target = Game.GameManager.Player.PlayerObj.transform;

        timetempD = Time.time;
    }

    public void SetUp(Chunk _chunk)
    {
        Cuerrent_Chunk = _chunk;
    }

    void FixedUpdate()
    {
        if (ISVISIBLE)
        {
            velocity = (transform.position - prevPos) / Time.deltaTime;
            prevPos = transform.position;
        }
    }

    void Update()
    {
        if (ISVISIBLE)
        {
            if (Time.time > timetemp + cooldown)
            {
                knocked = false;
                timetemp = Time.time;
            }

            if (target && !knocked)
            {
                float distances = Vector3.Distance(transform.position, target.position);

                var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
                var upDotProduct = Vector3.Dot(transform.up, velocity);
                var rightDotProduct = Vector3.Dot(transform.right, velocity);

                Vector3 velocityVector = new Vector3(rightDotProduct * 5, upDotProduct * 5, fwdDotProduct * 5);

                Anim.SetFloat("speedh", velocityVector.x / 20);
                Anim.SetFloat("speedv", velocityVector.z / 20);

                if (distances <= Distance)
                {
                    if (distances <= 1)
                    {
                        if (Time.time > timetempD + damageCoolDown)
                        {
                            GetComponent<Animator>().SetTrigger("Attack1h1");
                            timetempD = Time.time;
                        }
                    }
                    else
                    {
                        Vector3 displacement = target.position - transform.position;
                        displacement = displacement.normalized;

                        body.MovePosition(transform.position + displacement * damping * Time.deltaTime);
                        displacement.y = 0;
                        transform.rotation = Quaternion.LookRotation(displacement, transform.up);
                    }
                }
                else
                {
                    
                }
            }
            else//if dont have a target
            {
                
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, 0.7f);
        }
    }

    public void PhysicDamage()
    {
        Collider[] Entitys = Physics.OverlapSphere(attackPoint.position, 0.7f, EntitysLayer);

        foreach (var item in Entitys)
        {
            if (item != null)
            {
                item.gameObject.GetComponent<EntityPlayer>().DoDamage(null, 15, "AI", true);
            }
        }

        if (Entitys.Length <= 0)
        {
            Debug.Log("Attacked : Air");
        }
    }


    public override void OnHit(int damage, string attckerid, ItemData item)
    {
        knocked = true;
        Vector3 knockback = -transform.forward + new Vector3(0, 0.5f, 0);

        knockback *= 300;

        body.AddForce(knockback, ForceMode.Force);

        timetemp = Time.time;

        base.OnHit(damage, attckerid, item);
    }

    public override void BecameVisible()
    {
        ISVISIBLE = true;
        Anim.enabled = true;
        Game.Entity_viewing.Add(this);
    }

    public override void BecameInvisible()
    {
        ISVISIBLE = false;
        Anim.enabled = false;
        Game.Entity_viewing.Remove(this);
    }

    public override void OnDead()
    {
        Cuerrent_Chunk.RemoveEntity(this);
        DarckNet.Network.Destroy(this.gameObject);
        base.OnDead();
    }
}