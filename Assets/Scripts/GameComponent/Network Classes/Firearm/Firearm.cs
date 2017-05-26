using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public abstract class Firearm : NetworkBehaviour
{
    public BucketInt ammunition;

    public float reload_time;

    [HideInInspector]
    public bool is_reloading = false;

    [HideInInspector]
    public float reload_percent = 0;
    
    [SerializeField]
    protected float max_distance;

    [SerializeField]
    protected float damage;

    [SerializeField]
    protected FalloffType fall_off_type;
    
    [SerializeField]
    protected GameObject projectile;

    public Character owner;

    private void Start()
    {
        ammunition.current = ammunition.max;
    }
    
    /// <summary>
    /// Start the reload sequence.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Reload()
    {
        if (!is_reloading && ammunition.current < ammunition.max)
        {
            is_reloading = true;
            reload_percent = 0;
            ReloadStart();
            while (reload_percent < 100)
            {
                reload_percent++;
                yield return new WaitForSeconds(reload_time / 100.0f);
            }
            is_reloading = false;
            ReloadEnd();
            ammunition.Refill();
        }
    }

    protected virtual void ReloadStart() { }
    protected virtual void ReloadEnd() { }

    /// <summary>
    /// Returns true if you're not reloading and have ammunition.
    /// </summary>
    /// <returns></returns>
    protected bool CheckAmmo()
    {
        if (is_reloading || ammunition.IsEmpty())
            return false;
        return true;
    }

    public abstract void Fire(float angle);
}
