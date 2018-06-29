using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour {
    public Text nameText;
    public Image healthBar;

    private float currentFillAmount;
    private float lerpSpeed = 5f;
    private static readonly float EPSILON = 0.0001f;

    private void Update() {
        if (Mathf.Abs(currentFillAmount - healthBar.fillAmount) > EPSILON) {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentFillAmount, Time.deltaTime * lerpSpeed);
        }
    }

    /// <summary>
    /// Setting health fill amount via this method will animate from it's current fill value.
    /// </summary>
    /// <param name="healthFillAmount">the new health bar fill amount</param>
    public void AnimateHeathFillAmount(float healthFillAmount) {
        currentFillAmount = healthFillAmount;
    }

    /// <summary>
    /// Sets health fill amount without animating.
    /// </summary>
    /// <param name="healthFillAmount"></param>
    public void SetHealthFillAmount(float healthFillAmount) {
        currentFillAmount = healthFillAmount;
        healthBar.fillAmount = healthFillAmount;
    }
}
