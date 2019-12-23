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
    private bool ISVISIBLE;
    private Vector3 velocity;
    private Vector3 prevPos;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();

        target = Game.WorldGenerator.Player;
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
            if (target)
            {
                float distances = Vector3.Distance(transform.position, target.position);

                var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
                var upDotProduct = Vector3.Dot(transform.up, velocity);
                var rightDotProduct = Vector3.Dot(transform.right, velocity);

                Vector3 velocityVector = new Vector3(rightDotProduct * 2, upDotProduct * 2, fwdDotProduct * 2);

                if (velocityVector.x >= 1f)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", 0);
                }
                else if (velocityVector.x <= -1)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", 180);
                }
                else if (velocityVector.z >= 1)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", 90);
                }
                else if (velocityVector.z <= -1)
                {
                    Anim.SetInteger("Walk", 1);
                    Anim.SetFloat("X", -90);
                }
                else
                {
                    Anim.SetInteger("Walk", 0);
                }

                if (distances <= Distance)
                {
                    if (distances <= 1)
                    {
                        body.velocity = new Vector3(0, 0, 0) * damping;
                    }
                    else
                    {
                        if (RunAway == true)
                        {
                            Vector3 displacement = target.position - transform.position;
                            displacement = displacement.normalized;

                            body.velocity = displacement * -damping;
                        }
                        else
                        {
                            Vector3 displacement = target.position - transform.position;
                            displacement = displacement.normalized;

                            body.velocity = displacement * damping;
                        }
                    }
                }
                else
                {
                    body.velocity = new Vector3(0, 0, 0) * damping;
                }
            }
        }
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

    public override void OnDead()
    {
        DarckNet.Network.Destroy(this.gameObject);
        base.OnDead();
    }
}