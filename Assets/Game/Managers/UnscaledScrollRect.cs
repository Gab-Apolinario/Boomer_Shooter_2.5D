using UnityEngine;
using UnityEngine.UI;

public class UnscaledScrollRect : ScrollRect
{
    protected override void LateUpdate()
    {
        var originalTime = Time.timeScale;
        Time.timeScale = 1;
        base.LateUpdate();
        Time.timeScale = originalTime;
    }
}