using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AH_BatteryHole : Sc_AnimationHandler
{
    [Header("OBJECT REFS")]
    public Sc_RoboArmInputListener BatteryHoleRAIL;

    private void Start()
    {
        if (BatteryHoleRAIL)
        {
            BatteryHoleRAIL.CodeUp -= OnCodeUp;
            BatteryHoleRAIL.CodeUp += OnCodeUp;
            BatteryHoleRAIL.CodeDown -= OnCodeDown;
            BatteryHoleRAIL.CodeDown += OnCodeDown;
            BatteryHoleRAIL.CodeRight -= OnCodeRight;
            BatteryHoleRAIL.CodeRight += OnCodeRight;
            BatteryHoleRAIL.CodeLeft -= OnCodeLeft;
            BatteryHoleRAIL.CodeLeft += OnCodeLeft;
        }
    }

    private void OnCodeUp()
    {
        PlayAnimation("CodeUp", 0);
    }
    private void OnCodeDown()
    {
        PlayAnimation("CodeDown", 0);
    }
    private void OnCodeLeft()
    {
        PlayAnimation("CodeLeft", 0);
    }
    private void OnCodeRight()
    {
        PlayAnimation("CodeRight", 0);
    }
}
