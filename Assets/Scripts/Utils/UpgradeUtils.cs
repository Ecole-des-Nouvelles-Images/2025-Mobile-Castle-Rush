using System.Collections.Generic;
using System.Linq;
using ScriptableObjectsScripts.Upgrades;
using UnityEngine;

namespace Utils
{
    public static class UpgradeUtils
    {
        public static List<Upgrade> GetRandomUpgrade(int number)
        {
            List<Upgrade> upgrades = new List<Upgrade>();
            List<Upgrade> upgradeRessource = Enumerable.ToList(Resources.LoadAll<Upgrade>("Upgrades"));

            for (int i = 0; i < number; i++)
            {
                int[] indexArray = new int[upgradeRessource.Count];
                int randomIndex = Random.Range(0, indexArray.Length);

                Upgrade upgrade = upgradeRessource[randomIndex];
                if (upgrades.Contains(upgrade))
                {
                    number++;
                    continue;
                }

                upgrades.Add(upgrade);

            }

            return upgrades;
        }

        public static List<Upgrade> GetRandomUpgradeWithRange(int communCardNumber, int moyenCardNumber, int rareCardNumber, List<Upgrade> avaibleUpgrade)
        {
            List<Upgrade> upgrades = new List<Upgrade>();
            List<Upgrade> upgradeRessource = avaibleUpgrade;
            
            
            
            for (int i = 0; i < communCardNumber; i++)
            {
                int randomIndex = Random.Range(0, upgradeRessource.Count);
                Upgrade upgrade = upgradeRessource[randomIndex];
                if (upgrade.Rarity == Rarity.Commun)
                {
                    upgrades.Add(upgrade);
                    Debug.Log(upgrade.Rarity);
                }
                else
                {
                    i--;
                }
            }

            for (int i = 0; i < moyenCardNumber; i++)
            {
                int randomIndex = Random.Range(0, upgradeRessource.Count);
                Upgrade upgrade = upgradeRessource[randomIndex];
                if (upgrade.Rarity == Rarity.Moyen)
                {
                    upgrades.Add(upgrade);
                }
                else
                {
                    i--;
                }
            }

            for (int i = 0; i < rareCardNumber; i++)
            {
                int randomIndex = Random.Range(0, upgradeRessource.Count);
                Upgrade upgrade = upgradeRessource[randomIndex];
                if (upgrade.Rarity == Rarity.Rare)
                {
                    upgrades.Add(upgrade);
                }
                else
                {
                    i--;
                }
            }

            return upgrades;
        }

        public static List<Upgrade> ChoiceThreeRandomCardFromList(List<Upgrade> upgrades)
        {
            if (upgrades == null || upgrades.Count < 3)
            {
                Debug.LogError("Pas assez d'upgrades dans la liste");
                return new List<Upgrade>();
            }

            List<Upgrade> result = new List<Upgrade>();

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = Random.Range(0, upgrades.Count);
                Upgrade chosenUpgrade = upgrades[randomIndex];

                result.Add(chosenUpgrade);
                upgrades.RemoveAt(randomIndex);
            }

            return result;
        }

    }
}
