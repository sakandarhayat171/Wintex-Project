using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private int comboCount = 0;
    private int comboMultiplier = 1;

    public void ResetCombo()
    {
        comboCount = 0;
        comboMultiplier = 1;
    }

    public void IncrementCombo()
    {
        comboCount++;
        comboMultiplier = Mathf.Clamp(comboMultiplier + 1, 1, 5);
    }

    public int GetComboMultiplier()
    {
        return comboMultiplier;
    }
}