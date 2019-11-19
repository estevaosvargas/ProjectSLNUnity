using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmartEntity : Pathfindingentity
{
    public Vector2 LastPosition;
    private SpriteRenderer Render;

    private Vector3 velocity;
    [System.NonSerialized]
    private Vector3 prevPos;

    public float damping = 1;
    [System.NonSerialized]
    public float Distance = 10;

    public bool RunAway = false;
    [System.NonSerialized]
    public Rigidbody2D body;
    [System.NonSerialized]
    public Animator Anim;

    public int direction = 0;


    public void Born(string name)
    {
        GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value, 1);
        GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y;
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
        Go(new Vector3(Random.Range(transform.position.x - 10, transform.position.x + 10), Random.Range(transform.position.y - 10, transform.position.y + 10), transform.position.z));
    }

    public override void Updateoverride()
    {
        if (transform.position == target)
        {
            if (HaveTarget)
            {
                Stop();
            }
        }

        var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
        var upDotProduct = Vector3.Dot(transform.up, velocity);
        var rightDotProduct = Vector3.Dot(transform.right, velocity);
        Render.sortingOrder = -(int)transform.position.y;
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
        else if (velocityVector.y >= 1)
        {
            Anim.SetInteger("Walk", 1);
            Anim.SetFloat("X", 90);
            direction = 1;
        }
        else if (velocityVector.y <= -1)
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

    void FixedUpdate()
    {
        velocity = (transform.position - prevPos) * 5 / Time.deltaTime;
        prevPos = transform.position;
    }

    /*public override void Updateoverride()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
           
            //CalculateTiles();
        }

        float distances = Vector3.Distance(transform.position, target.position);

        var fwdDotProduct = Vector3.Dot(transform.forward, velocity);
        var upDotProduct = Vector3.Dot(transform.up, velocity);
        var rightDotProduct = Vector3.Dot(transform.right, velocity);
        Render.sortingOrder = -(int)transform.position.y;
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
        else if (velocityVector.y >= 1)
        {
            Anim.SetInteger("Walk", 1);
            Anim.SetFloat("X", 90);
            direction = 1;
        }
        else if (velocityVector.y <= -1)
        {
            Anim.SetInteger("Walk", 1);
            Anim.SetFloat("X", -90);
            direction = 2;
        }
        else
        {
            Anim.SetInteger("Walk", 0);
        }

        /*if (distances <= Distance)
        {
            if (distances <= 1)
            {
                body.velocity = new Vector2(0, 0) * damping;
            }
            else
            {
                Vector3 displacement = target.position - transform.position;
                displacement = displacement.normalized;

                if (RunAway == true)
                {
                    //body.velocity = LookCanWalkPosition((int)transform.position.x, (int)transform.position.y, displacement) * -damping;
                }
                else
                {
                    //transform.position = LookCanWalkPosition((int)transform.position.x, (int)transform.position.y, displacement) * damping;
                }
            }
        }
        else
        {
            body.velocity = new Vector2(0, 0) * damping;
        }
    }*/
}