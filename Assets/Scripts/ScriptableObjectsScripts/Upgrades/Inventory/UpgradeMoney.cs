using Player;
using UnityEngine;

namespace ScriptableObjectsScripts.Upgrades.Inventory
{
    [CreateAssetMenu(fileName = "Money", menuName = "Scriptable Objects/Upgrade/Divert/MoneyMax")]
    public class UpgradeMoney : UpgradeDivert
    {
        public int MoneyToAdd;
    
        public override void Apply(InventoryHandler inventary)
        {
            inventary.MaxMoney += MoneyToAdd;
        }
    }
}
