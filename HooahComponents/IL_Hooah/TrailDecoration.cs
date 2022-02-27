using System;
using System.Linq;
using HooahUtility.Model;
using MessagePack;
using UnityEngine;

namespace HooahComponents
{
    public class TrailDecoration : HooahBehavior
    {
        public Transform pointA; // dip
        public Transform pointB; // target
        [Key(1), Range(0, 100f)] public float recoverDistance;
        [Key(2), Range(0.001f, 1000f)] public float recoverRate = 0.1f;
        public float factor = 0f;
        [Key(3), Range(0.001f, 100f)] public float thicknessLoseRate = 0.01f;
        [Key(4), Range(0.001f, 10f)] public float maxThickness = 5;
        [Key(5), Range(0.001f, 10f)] public float dripOffset = 1;
        public AnimationCurve curve;

        public LineRenderer lineRenderer;

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2;
        }

        private const int VertCount = 10;
        private const float VertCountFloat = 9f;
        private Vector3[] vertices = new Vector3[VertCount];

        public void Start()
        {
            lineRenderer.material = new Material(lineRenderer.material);
        }

        // draw curve.
        public void Update()
        {
            var position = pointA.position;
            var position1 = pointB.position;
            var midpoint = Vector3.Lerp(position, position1, 0.5f);
            var diff = position - position1;
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = GetPoint(
                    position,
                    midpoint + Vector3.down * (1 - factor) * dripOffset *
                    (1 - Mathf.Abs(Vector3.Dot(Vector3.up, diff.normalized))),
                    position1,
                    i / VertCountFloat
                );
            }

            lineRenderer.positionCount = VertCount;
            lineRenderer.SetPositions(vertices);
            lineRenderer.widthMultiplier = factor * maxThickness;
            lineRenderer.material.color = new Color(1, 1, 1, curve.Evaluate(factor)); // todo: instantiate

            var dist = diff.sqrMagnitude;
            if (dist < recoverDistance)
            {
                factor = Mathf.Min(factor + recoverRate * Time.deltaTime, 1);
            }
            else
            {
                if (factor <= 0)
                {
                    factor = 0;
                    // shift offset when it's completely invisible.
                    return;
                }

                factor = Mathf.Max(factor - thicknessLoseRate * Time.deltaTime, 0);
            }
        }
    }
}
