using Mirror;
using UnityEngine;

abstract public class ThrowableItem : NetworkBehaviour, IDamageable
{
    [Header("Friendly fire")]
    [SerializeField, Tooltip("FriendlyFire ?")] private bool allowFriendlyFire = false;
    [SerializeField,
        Tooltip("Small period of invincibility for the user's team when deploying the item"),
        SyncVar]
    private float teamInvincibilityTime = 0.1f;

    [Header("Collisions")]
    [SerializeField] private bool alsoRequireTag = false;
    [SerializeField] private string collidingTag;

    [Header("Bouncing")]
    [SerializeField] private LayerMask boucingLayers;
    [SerializeField, Tooltip("Does not have priority over IDamageable objects.")] private bool enableBouncing = false;
    [SerializeField] private int maxBounces = 0;
    [SerializeField] private bool destroyOnLastBounce = true;

    // Teams can reference unique players or groups of players, 0 is neutral and affects everyone
    // Works for up to 32 teams (uint is 32 bits)
    private uint immuneTeams;
    private bool isTeamSet;

    protected virtual void Update()
    {
        if (teamInvincibilityTime > 0 && isServer)
        {
            teamInvincibilityTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer && alsoRequireTag && !other.CompareTag(collidingTag)) return;

        if (enableBouncing)
        {
            ManageBounce(other);
        }

        if (other.TryGetComponent(out IDamageable damageable) && isTeamSet)
        {
            bool isEnemy = (immuneTeams & damageable.GetTeam()) == 0;
            bool friendlyFireInvincibilityEnded = teamInvincibilityTime <= 0;
            bool friendlyFireEnabled = (allowFriendlyFire && friendlyFireInvincibilityEnded);

            if (isEnemy || friendlyFireEnabled)
            {
                // The object is not immune to this item, so it gets hit
                OnHit(other);
            }
        }
    }

    private void ManageBounce(Collider other)
    {
        if (boucingLayers == (boucingLayers & (1 << other.gameObject.layer)))
        {
            // The object can bounce
            if (maxBounces > 0)
            {
                maxBounces--;
                // TODO: Bounce the object
            }
            else
            {
                // Ran out of bounces
                if (destroyOnLastBounce)
                {
                    Destroy(gameObject);
                }
                else
                {
                    enableBouncing = false;
                }
            }
        }
        else
        {
            // The object cannot bounce
            maxBounces = 0;
        }
    }

    /// <summary>
    /// Called when hitting a damageable object that is not immune to it
    /// </summary>
    /// <param name="damageable"></param>
    abstract protected void OnHit(Collider damageable);

    public uint GetTeam()
    {
        return immuneTeams;
    }

    public void SetTeam(uint team)
    {
        isTeamSet = true;
        immuneTeams = team;
    }
}
