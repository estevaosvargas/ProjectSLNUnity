using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelleWeapon : FPItemBase
{
    public Animator Anim;
    public EntityPlayer _Player;
    public HandManager _HandManager;

    private float AttackRate = 1;
    private float Range = 0.5f;

    private bool HoldAttack = false;
    private float timetemp;

    void Start()
    {
        Range = 0.5f;
        _Player = GetComponentInParent<EntityPlayer>();
        _HandManager = GetComponentInParent<HandManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time > timetemp + AttackRate)
            {
                AttackTrigger();

                timetemp = Time.time;
            }
        }

        Anim.SetBool("Walking", _Player.IsWalking());
    }

    private void AttackTrigger()
    {
        Anim.SetTrigger("Attack");
        _HandManager.PhysicDamage(GetItem(), Range);
    }
}