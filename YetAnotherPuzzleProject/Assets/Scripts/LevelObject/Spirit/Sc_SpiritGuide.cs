using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_SpiritGuide : MonoBehaviour
{
    [Header("GUIDED SPIRITS")]
    [ReadOnly] public List<Sc_Spirit> GuidedSpirits = new List<Sc_Spirit>();
    public int NumberOfSpirits { get { return GuidedSpirits.Count; } }

    [Header("SPIRIT FOLLOW PARAMETERS")]
    public Vector2 MinMaxFollowDistance = new Vector2(1.5f, 3f);
    public Vector2 MinMaxUpwardsAngle = new Vector2(-15f, 75f);
    public Vector2 MinMaxOrbitSpeed = new Vector2(-60f, 60f);

    public void AddSpirit(Sc_Spirit spirit)
    {
        if (!GuidedSpirits.Contains(spirit))
        {
            GuidedSpirits.Add(spirit);
        }
    }

    public void RemoveSpirit(Sc_Spirit spirit)
    {
        GuidedSpirits.Remove(spirit);
    }
}
