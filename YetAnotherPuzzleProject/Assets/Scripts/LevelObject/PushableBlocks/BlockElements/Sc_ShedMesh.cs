using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ShedMesh : MonoBehaviour
{
    [Header("OBJECT REFS")]
    public List<Collider> Colliders = new List<Collider>();
    public List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    private List<Material> _mats = new List<Material>();

    private void Start()
    {
        foreach(Collider coll in Colliders)
        {
            Renderer rend = coll.GetComponent<Renderer>();
            if (rend)
            {
                _mats.Add(rend.material);
            }
        }
    }

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

        //Dissolve mesh
        //Destroy after dissolve
        StartCoroutine(DissolveCo());
    }

    private IEnumerator DissolveCo()
    {
        float timer = 0f;
        float duration = 3f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        while (timer < duration)
        {
            float alpha = timer / duration;
            foreach(Material mat in _mats)
            {
                mat.SetFloat("_dissolveLerp", alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        foreach(Collider coll in Colliders)
        {
            Destroy(coll.gameObject);
        }
        Destroy(this.gameObject);
    }
}
