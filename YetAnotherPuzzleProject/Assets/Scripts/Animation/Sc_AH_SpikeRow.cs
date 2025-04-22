using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_AH_SpikeRow : Sc_AnimationHandler
{
    [Header("SPIKEROW OBJECT REFS")]
    public Sc_SpikeRow SpikeRow;

    private void Start()
    {
        SpikeRow.Ejected -= OnEjected;
        SpikeRow.Reset -= OnReset;

        SpikeRow.Ejected += OnEjected;
        SpikeRow.Reset += OnReset;
    }

    private void OnEjected()
    {
        Anim.Play("Spikes_Eject");
    }

    private void OnReset()
    {
        Anim.Play("Spikes_Reset");
    }
}
