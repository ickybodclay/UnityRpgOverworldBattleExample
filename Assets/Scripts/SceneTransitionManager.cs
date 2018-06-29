using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour {
    public delegate void OnFadeComplete();

    public Image fadeImage;
    private OnFadeComplete onFadeCompleteHandler;

    private float currentFillAmount;
    private float lerpSpeed = 5f;
    private static readonly float EPSILON = 0.0001f;
    private bool isAnimating = false;
	
	void Update () {
        if (isAnimating) {
            if (Mathf.Abs(currentFillAmount - fadeImage.fillAmount) > EPSILON) {
                fadeImage.fillAmount = Mathf.Lerp(fadeImage.fillAmount, currentFillAmount, Time.deltaTime * lerpSpeed);
            }
            else {
                Invoke("TriggerHandler", 0.1f);
                isAnimating = false;
            }
        }
	}

    private void TriggerHandler() {
        onFadeCompleteHandler();
    }

    public void FadeOut(OnFadeComplete onFadeCompleteHandler) {
        fadeImage.enabled = true;
        fadeImage.fillClockwise = false;
        currentFillAmount = 1f;
        fadeImage.fillAmount = 0f;
        isAnimating = true;
        this.onFadeCompleteHandler = onFadeCompleteHandler;
    }

    public void FadeIn(OnFadeComplete onFadeCompleteHandler) {
        fadeImage.enabled = true;
        fadeImage.fillClockwise = true;
        currentFillAmount = 0f;
        fadeImage.fillAmount = 1f;
        isAnimating = true;
        this.onFadeCompleteHandler = onFadeCompleteHandler;
        this.onFadeCompleteHandler += DisableFadeImage;
    }

    private void DisableFadeImage() {
        fadeImage.enabled = false;
    }

    public bool IsAnimating() {
        return isAnimating;
    }
}
