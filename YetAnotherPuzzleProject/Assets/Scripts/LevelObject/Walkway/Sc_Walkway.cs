using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_Walkway : MonoBehaviour
{
    [Header("CUBES")]
    public List<Rigidbody> Cubes = new List<Rigidbody>();

    [Header("BREAKING")]
    public float BreakDuration = 5f;
    public AnimationCurve BreakCurve;

    public void BreakWalkway()
    {
        foreach(Rigidbody rb in Cubes)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            float randomForce = UnityEngine.Random.Range(-3f, 3f);
            rb.AddForce(randomForce * Vector3.down, ForceMode.VelocityChange);
        }

        StartCoroutine(DisappearCubesCo());
    }

    private IEnumerator DisappearCubesCo()
    {
        float timer = 0f;
        while (timer < BreakDuration)
        {
            timer += Time.deltaTime;
            foreach(Rigidbody rb in Cubes)
            {
                rb.transform.localScale = rb.transform.localScale * BreakCurve.Evaluate(timer / BreakDuration);
            }
            yield return null;
        }
        foreach (Rigidbody rb in Cubes)
        {
            rb.gameObject.SetActive(false);
        }
    }
}
