using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image ProgressBarImage;
    [SerializeField] private TextMeshProUGUI ProgressText;

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        ProgressBarImage.fillAmount = progress;
        ProgressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        // change the color of the progress bar based on the progress
        ProgressBarImage.color = Color.Lerp(Color.red, Color.green, progress);
    }
}
