using NUnit.Framework;
using SlotDefense;
using UnityEngine;

public class LowPolyWindAnimatorTests
{
    [Test]
    public void Configure_ProfilesHaveDistinctMotion()
    {
        var go = new GameObject("WindTest");
        try
        {
            var wind = go.AddComponent<LowPolyWindAnimator>();
            wind.Configure(LowPolyWindProfile.Tree);
            float treeRotation = wind.RotationAmplitude;
            float treeSpeed = wind.Speed;

            wind.Configure(LowPolyWindProfile.Grass);
            Assert.Greater(wind.RotationAmplitude, treeRotation);
            Assert.Greater(wind.Speed, treeSpeed);
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }

    [Test]
    public void Samples_ReturnVisibleNonZeroOffsets()
    {
        var go = new GameObject("WindTest");
        try
        {
            var wind = LowPolyWindAnimator.Attach(go, LowPolyWindProfile.Banner, 0.7f);
            Assert.IsNotNull(wind);
            Assert.Greater(wind.SamplePositionOffset(1.25f).sqrMagnitude, 0f);
            Assert.AreNotEqual(Quaternion.identity, wind.SampleRotation(1.25f));
            Assert.AreNotEqual(Vector3.one, wind.SampleScale(1.25f));
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }
}
