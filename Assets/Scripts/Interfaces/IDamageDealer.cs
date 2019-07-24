using UnityEngine;

public interface IDamageDealer
{
    int DamageAmount { get; set; }
    GameObject DamageSource { get; }
}
