using NUnit.Framework;
using UnityEngine;

public class MathUtilsTest : UnityEngineTest
{
    [Test]
    public void NormalizeTo0() => Assert.AreEqual(0, MathUtils.Normalize(5, 10, 5));

    [Test]
    public void NormalizeTo1() => Assert.AreEqual(1, MathUtils.Normalize(21.5f, 21.5f, 0));

    [Test]
    public void NormalizeWithin() => Assert.AreEqual(0.25, MathUtils.Normalize(-5, 10, -10));

    [Test]
    public void NormalizeOutOfBoundsMin() => Assert.AreEqual(0, MathUtils.Normalize(-100, 10, 5));

    [Test]
    public void NormalizeOutOfBoundsMax() => Assert.AreEqual(1, MathUtils.Normalize(100, 10, 5));

    [Test]
    public void IsFurtherAlongY() => 
        Assert.IsTrue(MathUtils.IsFurther(
            V(1, 2, 0), 
            V(20, 1, 0), 
            V(0, 1, 0)));

    [Test]
    public void IsNotFurtherAlongY() =>
        Assert.IsFalse(MathUtils.IsFurther(
            V(10, 10, 100), 
            V(0, 11, -100), 
            V(0, 1, 0)));

    [Test]
    public void IsFurtherAlongCompoundVector() =>
        Assert.IsTrue(MathUtils.IsFurther(
            V(2, 2, 2),
            V(-1, -1, -1),
            V(1, 1, 1)));

    [Test]
    public void IsFurtherAlongEqual() =>
        Assert.IsFalse(MathUtils.IsFurther(
            V(1, 1, 1),
            V(1, 1, 1),
            V(1, 0, 0)));
}
