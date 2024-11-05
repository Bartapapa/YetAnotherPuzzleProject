using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_RestartCircle : MonoBehaviour
{
    public Image FillImage;
    public Animator Anim;

    public void FillCircle(float alpha)
    {
        FillImage.fillAmount = alpha;
    }
}
