using UnityEngine;

public class RaycastRef {

    private static readonly RaycastRef MissSingleton = new RaycastRef();
    public static RaycastRef Miss { get => MissSingleton; }

    private Vector3 _rawNormal;

    public bool WasHit { get; private set; }
    public Vector3 Point { get; private set; }
    public Vector3 Normal { get => _rawNormal.normalized; }
    public float Distance { get; private set; }

    private int _numHits;

    public RaycastRef(RaycastRef original, params RaycastHit[] hits) {
        Copy(original);
        AddAllToAverage(hits);
    }

    public RaycastRef(params RaycastHit[] hits) {
        WasHit = hits.Length > 0;
        AddAllToAverage(hits);
    }

    private void Copy(RaycastRef original) {
        WasHit = original.WasHit;
        Point = original.Point;
        _rawNormal = original._rawNormal;
        Distance = original.Distance;
        _numHits = original._numHits;
    }

    private void AddAllToAverage(params RaycastHit[] hits) { 
        foreach (RaycastHit hit in hits)
            AddToAverage(hit);
    }

    private void AddToAverage(RaycastHit hit) {
        int numHitsIncrement = _numHits + 1;

        Point = (Point * _numHits + hit.point) / numHitsIncrement;

        _rawNormal = (_rawNormal * _numHits + hit.normal) / numHitsIncrement;

        Distance = (Distance * _numHits + hit.distance) / numHitsIncrement;

        _numHits = numHitsIncrement;
    }

    public void DebugLog() {
        Debug.Log("Ground info: was hit: " + WasHit + ", Point: " + Point + ", Normal: " + Normal + " Distance: " + Distance);
        Debug.DrawRay(Point, Normal * 5, Color.red);
    }
}