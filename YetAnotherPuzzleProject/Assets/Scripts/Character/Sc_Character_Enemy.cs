using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Character_Enemy : Sc_Character
{
    [Header("ENEMY OBJECT REFERENCES")]
    public Sc_AIBrain Brain;

    public override void Hurt(Sc_Character attacker, Vector3 directionOfAttack)
    {
        base.Hurt(attacker, directionOfAttack);
        //Go to state "Hurt", parsing in the attacker
    }
}
