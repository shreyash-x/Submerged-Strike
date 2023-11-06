using GameAnalyticsSDK;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    private void Start()
    {
        GameAnalytics.Initialize();
    }
}