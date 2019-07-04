using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tusken : Enemy
{
    public override void Init()
    {
        base.Init();
        health = 5;
        speed = 3;
    }
}
