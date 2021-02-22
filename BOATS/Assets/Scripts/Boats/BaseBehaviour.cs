using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBehaviour : BoatBehavior
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Turn()
    {    }

    protected override bool Move() { return false; }

    protected override bool Attack()
    {
        return false;
    }
}
