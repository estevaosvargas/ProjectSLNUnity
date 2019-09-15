using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbEntity : EntityLife
{
    public Transform target;
    public float damping = 1;
    public float Distance = 10;
    public bool RunAway = false;
    public Rigidbody2D body;
    public Animator Anim;


    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();

        target = WorldGenerator.Instance.Player;
    }

    private Vector3 velocity;
    private Vector3 prevPos;

    void FixedUpdate()
    {
        velocity = (transform.position - prevPos) / Time.deltaTime;
        prevPos = transform.position;
    }

    void Update()
    {
        float distances = Vector3.Distance(transform.position, target.position);

        var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
        var upDotProduct = Vector3.Dot(transform.up, velocity);
        var rightDotProduct = Vector3.Dot(transform.right, velocity);

        Vector3 velocityVector = new Vector3(rightDotProduct, upDotProduct, fwdDotProduct);

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
        else if (velocityVector.y >= 1)
        {
            Anim.SetInteger("Walk", 1);
            Anim.SetFloat("X", 90);
        }
        else if (velocityVector.y <= -1)
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
            GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y;

            if (distances <= 1)
            {
                body.velocity = new Vector2(0, 0) * damping;
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
            body.velocity = new Vector2(0,0) * damping;
        }
    }

    public override void OnDead()
    {
        Destroy(this.gameObject);
        base.OnDead();
    }
}