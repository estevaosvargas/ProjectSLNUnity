using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEntity : EntityLife
{
    public CharacterController Body;
    public Animator Anim;
    public AudioSource Source;
    public AudioClip[] AudioLoop;
    public Vector3 NextTarget;
    private bool ISVISIBLE;
    public bool HaveTarget = false;
    public int RandomRange = 10;
    public float Speed = 1;
    public float TickSpeed = 5;
    private float timestep;

    void Start()
    {
        Anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (ISVISIBLE)
        {
            if (Time.time > TickSpeed + timestep)
            {
                if (Random.Range(0, 2) == 1)
                {
                    Source.PlayOneShot(AudioLoop[Random.Range(0, AudioLoop.Length)]);
                }

                if (HaveTarget)
                {
                    if (transform.position.y <= 1)
                    {
                        Anim.SetBool("Fly", false);
                    }
                    Anim.SetBool("Fly", true);
                }
                else
                {
                    Anim.SetBool("Fly", true);
                    HaveTarget = true;
                    NextTarget = new Vector3(Random.Range(-RandomRange, RandomRange), Random.Range(0, RandomRange), Random.Range(-RandomRange, RandomRange)) + transform.position;

                    if (Game.WorldGenerator.GetTileAt(NextTarget.x, NextTarget.z) == null)
                    {
                        NextTarget = new Vector3(Random.Range(-1, 1), Random.Range(0, 2), Random.Range(-1, 1)) + transform.position;
                    }

                    if(NextTarget.y >= RandomRange)
                    {
                        NextTarget = new Vector3(Random.Range(-RandomRange, RandomRange), Random.Range(0, RandomRange - 5), Random.Range(-RandomRange, RandomRange)) + transform.position;
                    }

                    
                    transform.LookAt(NextTarget);
                }
                timestep = Time.time;
            }
        }
        if (HaveTarget)
        {
            if (transform.position == NextTarget)
            {
                HaveTarget = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, NextTarget, Speed * Time.deltaTime);
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