using NUnit.Framework;
using UnityEngine;

public class RaycastRefTest : UnityEngineTest {

    private RaycastHit MakeHit(Vector3 point, Vector3 normal, float distance) {
        RaycastHit hit = new RaycastHit();
        hit.point = point;
        hit.normal = normal;
        hit.distance = distance;
        return hit;
    }

    private void AssertAverage(
        RaycastRef avg, 
        bool wasHit, 
        Vector3 point, 
        Vector3 normal, 
        float distance) {

        Assert.AreEqual(wasHit, avg.WasHit, "WasHit");
        Assert.AreEqual(point, avg.Point, "Point");
        Assert.AreEqual(true, normal == avg.Normal, "Normal: expected " + normal + ", got " + avg.Normal);
        Assert.AreEqual(distance, avg.Distance, "Distance");
    }

    [Test]
    public void EmptyAverage() {
        RaycastRef avg = new RaycastRef();
        AssertAverage(avg, false, Vector3.zero, Vector3.zero, 0);
    }

    [Test]
    public void SingleHitAverage() {
        RaycastHit hit = MakeHit(V(1, 0, 0), V(0, 1, 0), 5);
        RaycastRef avg = new RaycastRef(hit);
        AssertAverage(avg, true, V(1, 0, 0), V(0, 1, 0), 5);
    }

    [Test]
    public void DoubleHitAverage() {
        RaycastHit hit1 = MakeHit(V(10, 0, 0), V(0, 1, 0), 10);
        RaycastHit hit2 = MakeHit(V(0, 5, 0), V(1, 0, 0), 5);
        RaycastRef avg = new RaycastRef(hit1, hit2);
        AssertAverage(avg, true, V(5, 2.5f, 0), V(1, 1, 0).normalized, 7.5f);
    }

    [Test]
    public void TripleHitAverage() {
        RaycastHit hit1 = MakeHit(V(0, 0, -9), V(1, 0, 0), 12);
        RaycastHit hit2 = MakeHit(V(0, 6, 9), V(0, 1, 0), 0);
        RaycastHit hit3 = MakeHit(V(3, 12, -3), V(0, 0, 1), 6);
        RaycastRef avg = new RaycastRef(hit1, hit2, hit3);
        AssertAverage(avg, true, V(1, 6, -1), V(1, 1, 1).normalized, 6);
    }

    [Test]
    public void AverageWithOriginal() {
        RaycastHit hit1 = MakeHit(V(1, 0, 0), V(1, 0, 0), 10);
        RaycastHit hit2 = MakeHit(V(3, 6, 0), V(0, 1, 0), 8);
        RaycastRef orig = new RaycastRef(hit1, hit2);
        RaycastHit hit3 = MakeHit(V(5, 0, 3), V(0, 0, 1), 3);
        RaycastRef avg = new RaycastRef(orig, hit3);
        AssertAverage(avg, true, V(3, 2, 1), V(1, 1, 1).normalized, 7);
    }

    [Test]
    public void AssertMiss() {
        RaycastRef miss = RaycastRef.Miss;
        AssertAverage(miss, false, Vector3.zero, Vector3.zero, 0);
    }
}
