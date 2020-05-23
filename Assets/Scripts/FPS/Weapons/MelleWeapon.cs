using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelleWeapon : FPItemBase
{
    public EntityPlayer _Player;
    public HandManager _HandManager;

    public float Range = 0.5f;

    private bool HoldAttack = false;

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
            AttackTrigger();
        }
    }

    private void AttackTrigger()
    {
        _HandManager.PhysicDamage(GetItem(), Range);
    }
}