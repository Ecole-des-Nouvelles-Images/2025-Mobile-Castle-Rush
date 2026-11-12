using Player;
using UnityEngine;

namespace ScriptableObjectsScripts.Upgrades.Inventory
{
    [CreateAssetMenu(fileName = "Elixir", menuName = "Scriptable Objects/Upgrade/Divert/ElixirMax")]
    public class UpgradeElixir : UpgradeDivert
    {
        public int ElexirToAdd;
    
        public override void Apply(InventoryHandler inventary)
        {
            inventary.MaxElixir += ElexirToAdd;
        }
    }
}
