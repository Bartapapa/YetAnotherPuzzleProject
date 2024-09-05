using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PlayerRepel : MonoBehaviour
{
    [Header("PARAMETERS")]
    public Vector2 _minMaxRepelForce;
    public float _repelRange = 1f;

    private List<Sc_Character> _characters = new List<Sc_Character>();

    private void FixedUpdate()
    {
        foreach(Sc_Character character in _characters)
        {
            if (!character.Controller.CanBeRepelled) return;

            Vector3 targetIgnoreY = new Vector3(character.transform.position.x, 0f, character.transform.position.z);
            Vector3 repellerIgnoreY = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 repelDir = targetIgnoreY - repellerIgnoreY;
            repelDir = repelDir.normalized;

            float targetDistance = Vector3.Distance(targetIgnoreY, repellerIgnoreY);
            float alpha = targetDistance / _repelRange;
            float repelForce = Mathf.Lerp(_minMaxRepelForce.y, _minMaxRepelForce.x, alpha);

            character.Controller.RB.AddForce(repelDir * repelForce, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Sc_Character character = other.GetComponent<Sc_Character>();
        if (character)
        {
            _characters.Add(character);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sc_Character character = other.GetComponent<Sc_Character>();
        if (character)
        {
            if (_characters.Contains(character))
            {
                _characters.Remove(character);
            }
        }
    }
}
