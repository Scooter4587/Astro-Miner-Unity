using System;
using AstroMiner.Presentation;
using NUnit.Framework;
using UnityEngine;

namespace AstroMiner.Tests
{
    public sealed class ParallaxLayer2DTests
    {
        [Test]
        public void DistantLayerMovesAtItsOwnScreenRatio()
        {
            Vector3 result = ParallaxLayer2D.CalculatePosition(Vector3.zero, new Vector3(10f, -5f, 9f), 0.2f, 64f);
            Assert.That(result.x - 10f, Is.EqualTo(-2f).Within(0.0001f));
            Assert.That(result.y + 5f, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(result.z, Is.Zero);
        }

        [Test]
        public void ZeroRatioStaysFixedOnScreen()
        {
            Vector3 origin = new Vector3(3f, 4f, 7f);
            Vector3 result = ParallaxLayer2D.CalculatePosition(origin, new Vector3(-300f, 250f, -8f), 0f, 64f);
            Assert.That(result, Is.EqualTo(new Vector3(-297f, 254f, 7f)));
        }

        [TestCase(4000f, -3000f)]
        [TestCase(-4000f, 3000f)]
        public void RepeatedTilesStayCenteredForLongTravel(float x, float y)
        {
            Vector3 cameraDelta = new Vector3(x, y, 0f);
            Vector3 result = ParallaxLayer2D.CalculatePosition(Vector3.zero, cameraDelta, 0.3f, 64f);
            Assert.That(Mathf.Abs(result.x - x), Is.LessThanOrEqualTo(32.001f));
            Assert.That(Mathf.Abs(result.y - y), Is.LessThanOrEqualTo(32.001f));
        }

        [Test]
        public void ReturningCameraToOriginRestoresLayerPosition()
        {
            Vector3 origin = new Vector3(2f, -3f, 4f);
            Assert.That(ParallaxLayer2D.CalculatePosition(origin, Vector3.zero, 0.15f, 64f), Is.EqualTo(origin));
        }

        [Test]
        public void InvalidTileSizeIsRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ParallaxLayer2D.CalculatePosition(Vector3.zero, Vector3.zero, 0.1f, 0f));
        }
    }
}
