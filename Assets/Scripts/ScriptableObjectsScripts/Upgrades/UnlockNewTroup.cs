using System.Collections.Generic;
using Managers;
using Player;
using ScriptableObjectsScripts.Upgrades;
using Structs;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockNewCard", menuName = "Scriptable Objects/Upgrade/UnlockNewTroup")]
public class UnlockNewTroup : Upgrade
{
    public EnemyClass EnemyToAdd;
    public List<Upgrade> UpgradesToAdd;
    public override void Apply(InventoryHandler inventary)
    {
        inventary.AddEnemy(EnemyToAdd);
        CardManager.Instance.AddUpgrades(UpgradesToAdd);
        CardManager.Instance.RemoveUpgrade(this);
    }
}
