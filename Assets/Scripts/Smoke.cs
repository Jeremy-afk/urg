using UnityEngine;

public class Smoke : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidBody;
    public Movements moves;
    [SerializeField]
    private ParticleSystem smoke;

    private void Update()
    {
        var emitParams = new ParticleSystem.EmitParams();
        smoke.Emit(emitParams, Mathf.RoundToInt(rigidBody.velocity.magnitude / (moves.GetMaxSpeed() - 10) * 1.5f));
    }
}
