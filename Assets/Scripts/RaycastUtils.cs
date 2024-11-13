using UnityEngine;

public static class RaycastUtils
{
    public static Ray[] GetViewRays(Transform transform, int rayCount, float angle)
    {
        var rays = new Ray[rayCount];
        var direction = transform.forward;
        var halfAngle = angle * 0.5f;
        var startAngle = -halfAngle;
        var angleStep = angle / (rayCount - 1);

        var basePosition = transform.position + Vector3.up * 0.5f;

        for (int i = 0; i < rayCount; i++)
        {
            var currentAngle = startAngle + angleStep * i;
            var rotation = Quaternion.Euler(0, currentAngle, 0);
            rays[i] = new Ray(basePosition, rotation * direction);
        }

        return rays;
    }
}
