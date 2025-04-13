using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ShedMesh : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public List<Collider> Colliders = new List<Collider>();
    public List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    public virtual void OnMeshShed()
    {
        if (Sc_Level.instance != null)
        {
            this.transform.parent = Sc_Level.instance.gameObject.transform;
        }
        else
        {
            this.transform.parent = null;
        }


        foreach(Rigidbody rb in Rigidbodies)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddTorque(transform.rotation * new Vector3(1f, 0f, 0f), ForceMode.Impulse);
            float randomX = Random.Range(-1f, 1f);
            float randomZ = Random.Range(-1f, 1f);
            rb.AddForce(new Vector3(randomX, 0f, randomZ), ForceMode.Impulse);
            rb.AddForce(transform.forward * 3f, ForceMode.Impulse);
        }
        foreach(Collider coll in Colliders)
        {
            coll.enabled = true;
        }
    }
}
