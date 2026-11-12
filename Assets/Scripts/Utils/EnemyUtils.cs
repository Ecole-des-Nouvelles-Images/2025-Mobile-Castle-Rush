using UnityEngine;

namespace Utils
{
    public class EnemyUtils : MonoBehaviourSingleton<EnemyUtils>
    {
        private void OnEnable()
        {
            EventBus.OnLevelFinished += KillAllEnemy;
        }

        private void OnDisable()
        {
            EventBus.OnLevelFinished -= KillAllEnemy;
        }

        public void KillAllEnemy()
        {
            Debug.Log("Kill all enemies");
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
        }
    }
}
