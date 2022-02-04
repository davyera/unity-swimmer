using System;
using System.Collections.Generic;
using UnityEngine;

public interface IRaycastSweeper {
    bool Sweep(out RaycastHit hit);
    void DebugRays(bool enable);
}

//public class FixedOriginRaycastSweeper : IRaycastSweeper {
//    private Transform _transform;
//    private Func<Vector3> _getOriginDelta;
//    private Func<float> _getRayDistance;
//    private int _iterations;
//    private LayerMask _layerMask;

//    public FixedOriginRaycastSweeper(
//        Transform transform,
//        Func<Vector3> originDeltaFunc,
//        Func<float> rayDistanceFunc,
//        int iterations,
//        LayerMask layerMask) {

//        _transform = transform;
//        _getOriginDelta = originDeltaFunc;
//        _getRayDistance = rayDistanceFunc;
//        _layerMask = layerMask;
//        _iterations = iterations;
//    }

//    public bool Sweep(out RaycastHit hit) {
//        Vector3 delta = _getOriginDelta();

//        Vector3 transformOrigin = _transform.position;
//        Vector3 rayOrigin = transformOrigin + delta;

//        Vector3 rayDestinationStart = rayOrigin - _transform.up;
//        Vector3 rayDestinationEnd = transformOrigin - _transform.up;

//        List<Vector3> rayDestinations = MathUtils.SlicePoints(rayDestinationStart, rayDestinationEnd, _iterations);

//        float rayDistance = _getRayDistance();
//        foreach (Vector3 rayDestination in rayDestinations) {
//            Ray ray = new Ray(rayOrigin, rayDestination - rayOrigin);
//            bool raycastWasHit = Physics.Raycast(ray, out hit, rayDistance, _layerMask);
//            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.white);
//            if (raycastWasHit)
//                return true;
//        }
//        hit = new RaycastHit();
//        return false;
//    }
//}

public class RaycastSweeper : IRaycastSweeper {

    private bool _debugEnabled;

    Func<float> _rayDistanceFunc;
    LayerMask _layerMask;
    Func<(Vector3, Vector3)>[] _originDestinationPairFuncs;

    public RaycastSweeper(
        Func<float> rayDistanceFunc,
        LayerMask layerMask,
        params Func<(Vector3, Vector3)>[] originDestinationPairFuncs) {

        _rayDistanceFunc = rayDistanceFunc;
        _layerMask = layerMask;
        _originDestinationPairFuncs = originDestinationPairFuncs;
    }

    public void DebugRays(bool enable) => _debugEnabled = enable;

    public bool Sweep(out RaycastHit hit) {
        float rayDistance = _rayDistanceFunc();

        foreach (Func<(Vector3, Vector3)> originDesinationPairFunc in _originDestinationPairFuncs) {
            (Vector3 origin, Vector3 destination) = originDesinationPairFunc();
            Vector3 direction = destination - origin;

            Ray ray = new Ray(origin, direction);

            bool raycastWasHit = Physics.Raycast(ray, out hit, rayDistance, _layerMask);
            if (_debugEnabled)
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, raycastWasHit ? Color.green : Color.white);

            if (raycastWasHit)
                return true;
        }
        hit = new RaycastHit();
        return false;
    }
}