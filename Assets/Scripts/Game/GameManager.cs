using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float spawnRate = 0.5f;

    public float playerXP = 0f;
    public float xpGain = 0.1f;

    public float currentXPThreshold = 1f;
    public float maxThreshold = 100f;

    private bool upgradeReady = false;

    public void RollForXP()
    {
        float roll = Random.value;

        if (roll <= spawnRate)
        {
            playerXP += xpGain;
            CheckThreshold();
            Debug.Log($"{xpGain} EXP Gained. Current Player EXP: {playerXP}");
        }
    }

    void CheckThreshold()
    {
        if (playerXP >= currentXPThreshold)
        {
            upgradeReady = true;

            if (currentXPThreshold < maxThreshold)
            {
                currentXPThreshold *= 2f;

                if (currentXPThreshold > maxThreshold)
                    currentXPThreshold = maxThreshold;
            }

            Debug.Log($"Threshold reached! Next Threshold: {currentXPThreshold}");
        }
    }

    public void UseUpgrade()
    {
        if (!upgradeReady)
            return;

        Upgrade();
        upgradeReady = false;
        playerXP = 0f;
    }

    void Upgrade()
    {
        xpGain *= 2f;
        Debug.Log("XP Upgrade Activated");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
