using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MathUtils {
    private MathUtils() => throw new InvalidOperationException();

    public static float Normalize(float value, float max, float min) {
        return Mathf.Clamp((value - min) / (max - min), 0, 1);
    }

    public static bool IsFurther(Vector3 pointA, Vector3 pointB, Vector3 direction) {
        Vector3 difference = pointB - pointA;
        float dot = Vector3.Dot(difference, direction);
        return dot < 0;
    }

    public static List<Vector3> SlicePoints(Vector3 start, Vector3 end, int slices) {
        List<Vector3> points = new List<Vector3>();
        points.Add(start);
        if (slices > 2) {
            Vector3 sliceDelta = (end - start) / (slices - 1);
            int innerPoints = slices - 2;
            for (int i = 0; i < innerPoints; i++) {
                Vector3 innerPoint = start + sliceDelta * (i + 1);
                points.Add(innerPoint);
            }
        }
        points.Add(end);
        return points;
    }
}