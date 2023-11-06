using UnityEngine;

public static class Utility
{
    private static Camera _camera;

    public static Bounds GetScreenBounds(float depth, Camera camera)
    {
        Vector3 center = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, depth));
        Vector3 size = ScreenToWorldSize(depth, camera);
        return new Bounds(center, size);
    }

    private static Vector4 ScreenMinMaxToWorld(float depth, Camera camera)
    {
        var cameraTransform = camera.transform;
        var min = cameraTransform.TransformVector(camera.ViewportToWorldPoint(new Vector3(0, 0, depth)));
        var max = cameraTransform.TransformVector(camera.ViewportToWorldPoint(new Vector3(1, 1, depth)));
        return new Vector4(min.x, min.y, max.x, max.y);
    }
    
    public static Vector2 ScreenToWorldSize(float depth, Camera camera)
    {
        var screenMinMax = ScreenMinMaxToWorld(depth, camera);

        return new Vector2(screenMinMax.z - screenMinMax.x, screenMinMax.w - screenMinMax.y);
    }
    
    public static Vector3 WorldPosToBorder(Vector3 worldPos, float screenBorder, Camera camera)
    {
        var screenMinMax = ScreenMinMaxToWorld(0, camera);
        
        var minX = screenMinMax.x + screenBorder;
        var maxX = screenMinMax.z - screenBorder;
        var minY = screenMinMax.y + screenBorder;
        var maxY = screenMinMax.w - screenBorder;
    
        var pos = worldPos;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        return pos;
    }
}