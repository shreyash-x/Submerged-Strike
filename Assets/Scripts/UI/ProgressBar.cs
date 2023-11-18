using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image ProgressBarImage;

    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);
        ProgressBarImage.fillAmount = progress;
        // change the color of the progress bar based on the progress
        ProgressBarImage.color = Color.Lerp(Color.red, Color.green, progress);
    }
}
