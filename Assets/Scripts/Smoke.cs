using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidBody;
    public Movements moves;
    [SerializeField]
    private ParticleSystem smoke;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var emitParams = new ParticleSystem.EmitParams();
        smoke.Emit(emitParams, Mathf.RoundToInt(rigidBody.velocity.magnitude / (moves.GetMaxSpeed()-10) * 1.5f));
    }
}
