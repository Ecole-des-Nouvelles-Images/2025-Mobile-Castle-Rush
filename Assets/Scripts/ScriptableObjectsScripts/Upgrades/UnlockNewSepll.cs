using System.Collections.Generic;
using Class;
using Managers;
using Player;
using ScriptableObjectsScripts.Upgrades;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockNewCard", menuName = "Scriptable Objects/Upgrade/UnlockNewSpell")]
public class UnlockNewSpell : Upgrade
{
    public SpellClass SpellToAdd;
    public List<Upgrade> UpgradesToAdd;
    public override void Apply(InventoryHandler inventary)
    {
        inventary.AddSpell(SpellToAdd);
        CardManager.Instance.AddUpgrades(UpgradesToAdd);
        CardManager.Instance.RemoveUpgrade(this);
        CardManager.Instance.LoadListCard();
    }
}
