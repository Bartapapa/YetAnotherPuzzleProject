using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PowerGenerator : MonoBehaviour
{
    public delegate void BoolEvent(bool active);
    public event BoolEvent GeneratingPower;

    public bool GeneratesPower { get { return _generatesPower; } set { _generatesPower = value; } }
    private bool _generatesPower = false;

    public void GeneratePower(bool generate)
    {
        _generatesPower = true;

        GeneratingPower?.Invoke(generate);
    }
}
