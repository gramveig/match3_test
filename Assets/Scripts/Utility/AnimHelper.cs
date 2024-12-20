using System.Collections.Generic;
using UnityEngine;

namespace Match3Test.Utility
{
    public static class AnimHelper
    {
        /// <summary>
        /// Calculates the point of parabolic 2D-trajectory between start and end with a specified height.
        /// r is a part of trajectory passed (between 0 and 1)
        /// </summary>
        public static Vector3 ParabolicLerp(Vector3 start, Vector3 end, float height, float r)
        {
            Vector3 travelDirection = end - start;
            Vector3 result = start + r * travelDirection;
            result.y += Mathf.Sin(r * Mathf.PI) * height;

            return result;
        }

        public static Vector3 ParabolicLerp3d(Vector3 start, Vector3 end, float height, float r)
        {
            Vector3 travelDirection = end - start;
            Vector3 result = start + r * travelDirection;
            result.z -= Mathf.Sin(r * Mathf.PI) * height;

            return result;
        }

        public static Vector3 BounceLerp(Vector3 pos, float height, float r)
        {
            if (r < 0.5)
                return new Vector3(pos.x, pos.y + Mathf.Lerp(0, height, r), pos.z);
            else
                return new Vector3(pos.x, pos.y + height - Mathf.Lerp(0, height, r), pos.z);
        }

        //easing

        public static float EaseInExpo(float r)
        {
            return Mathf.Pow(2, 10 * (r - 1));
        }

        public static float EaseOutExpo(float r)
        {
            return -Mathf.Pow(2, -10 * r) + 1;
        }

        public static float EaseInOutExpo(float r)
        {
            r /= .5f;
            if (r < 1) return 0.5f * Mathf.Pow(2, 10 * (r - 1));
            r--;
            return 0.5f * (-Mathf.Pow(2, -10 * r) + 2);
        }

        public static float Spring(float r)
        {
            r = (Mathf.Sin(r * Mathf.PI * (0.2f + 2.5f * r * r * r)) * Mathf.Pow(1f - r, 2.2f) + r) * (1f + (1.2f * (1f - r)));
            return r;
        }

        public static float EaseInQuad(float r)
        {
            return r * r;
        }

        public static float EaseOutQuad(float r)
        {
            return - r * (r - 2);
        }

        public static float EaseInOutQuad(float r)
        {
            r /= .5f;
            if (r < 1) return 0.5f * r * r;
            r--;
            return - 0.5f * (r * (r - 2) - 1);
        }

        public static float EaseInCubic(float r)
        {
            return r * r * r;
        }

        public static float EaseOutCubic(float r)
        {
            r--;
            return r * r * r + 1;
        }

        public static float EaseInOutCubic(float r)
        {
            r /= .5f;
            if (r < 1) return 0.5f * r * r * r;
            r -= 2;
            return 0.5f * (r * r * r + 2);
        }

        public static float EaseInQuart(float r)
        {
            return r * r * r * r;
        }

        public static float EaseOutQuart(float r)
        {
            r--;
            return - (r * r * r * r - 1);
        }

        public static float EaseInOutQuart(float r)
        {
            r /= .5f;
            if (r < 1) return 0.5f * r * r * r * r;
            r -= 2;
            return - 0.5f * (r * r * r * r - 2);
        }

        public static float EaseInQuint(float r)
        {
            return r * r * r * r * r;
        }

        public static float EaseOutQuint(float r)
        {
            r--;
            return r * r * r * r * r + 1;
        }

        public static float EaseInOutQuint(float r)
        {
            r /= .5f;
            if (r < 1) return 0.5f * r * r * r * r * r;
            r -= 2;
            return 0.5f * (r * r * r * r * r + 2);
        }

        public static float EaseInSine(float r)
        {
            return - Mathf.Cos(r * (Mathf.PI * 0.5f)) + 1;
        }

        public static float EaseOutSine(float r)
        {
            return Mathf.Sin(r * (Mathf.PI * 0.5f));
        }

        public static float EaseInOutSine(float r)
        {
            return - 0.5f * (Mathf.Cos(Mathf.PI * r) - 1);
        }

        public static float EaseInCirc(float r)
        {
            return - (Mathf.Sqrt(1 - r * r) - 1);
        }

        public static float EaseOutCirc(float r)
        {
            r--;
            return Mathf.Sqrt(1 - r * r);
        }

        public static float EaseInOutCirc(float r)
        {
            r /= .5f;
            if (r < 1) return - 0.5f * (Mathf.Sqrt(1 - r * r) - 1);
            r -= 2;
            return 0.5f * (Mathf.Sqrt(1 - r * r) + 1);
        }

        public static float EaseInBounce(float r)
        {
            float d = 1f;
            return 1f - EaseOutBounce(d - r);
        }

        public static float EaseOutBounce(float r)
        {
            r /= 1f;
            if (r < (1 / 2.75f))
                return 7.5625f * r * r;
            else if (r < (2 / 2.75f))
            {
                r -= (1.5f / 2.75f);
                return 7.5625f * (r) * r + .75f;
            }
            else if (r < (2.5 / 2.75))
            {
                r -= (2.25f / 2.75f);
                return 7.5625f * (r) * r + .9375f;
            }
            else
            {
                r -= (2.625f / 2.75f);
                return 7.5625f * (r) * r + .984375f;
            }
        }

        public static float EaseInOutBounce(float r)
        {
            float d = 1f;
            if (r < d * 0.5f) return EaseInBounce(r * 2) * 0.5f;
            else return EaseOutBounce(r * 2 - d) * 0.5f + 0.5f;
        }

        public static float EaseInBack(float r)
        {
            r /= 1;
            float s = 1.70158f;
            return (r) * r * ((s + 1) * r - s);
        }

        public static float EaseOutBack(float r)
        {
            float s = 1.70158f;
            r = (r) - 1;
            return (r) * r * ((s + 1) * r + s) + 1;
        }

        public static float EaseInOutBack(float r)
        {
            float s = 1.70158f;
            r /= .5f;
            if ((r) < 1)
            {
                s *= (1.525f);
                return 0.5f * (r * r * (((s) + 1) * r - s));
            }
            r -= 2;
            s *= 1.525f;
            return 0.5f * ((r) * r * (((s) + 1) * r + s) + 2);
        }

        public static float Punch(float amplitude, float r)
        {
            float s = 9;
            if (r == 0)
            {
                return 0;
            }
            else if (r == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * r) * Mathf.Sin((r * 1 - s) * (2 * Mathf.PI) / period));
        }

        public static float EaseInElastic(float r)
        {
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (r == 0) return 0;

            if ((r /= d) == 1) return 1f;

            if (a == 0f || a < Mathf.Abs(1f))
            {
                a = 1f;
                s = p / 4;
            }
            else
                s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);

            return -a * Mathf.Pow(2, 10 * (r -= 1)) * Mathf.Sin((r * d - s) * (2 * Mathf.PI) / p);
        }

        public static float EaseOutElastic(float r)
        {
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (r == 0) return 0;

            if ((r /= d) == 1) return 1f;

            if (a == 0f || a < 1f)
            {
                a = 1f;
                s = p * 0.25f;
            }
            else
                s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);

            return (a * Mathf.Pow(2, -10 * r) * Mathf.Sin((r * d - s) * (2 * Mathf.PI) / p) + 1f);
        }

        public static float EaseInOutElastic(float r)
        {
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (r == 0) return 0;

            if ((r /= d * 0.5f) == 2) return 1f;

            if (a == 0f || a < 1f)
            {
                a = 1f;
                s = p / 4;
            }
            else
                s = p / (2 * Mathf.PI) * Mathf.Asin(1f / a);

            if (r < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (r -= 1)) * Mathf.Sin((r * d - s) * (2 * Mathf.PI) / p));
            return a * Mathf.Pow(2, -10 * (r -= 1)) * Mathf.Sin((r * d - s) * (2 * Mathf.PI) / p) * 0.5f + 1f;
        }

        /// <summary>
        /// Returns the set of points which form the smooth curve
        /// </summary>
        /// <param name="arrayToCurve">Initial set of points</param>
        /// <param name="smoothness">The larger is this parameter, the smoother is the curve, but the function works slower</param>
        public static Vector3[] SmoothCurve(Vector3[] arrayToCurve, float smoothness = 3)
        {
            if (smoothness < 1.0f) smoothness = 1.0f;

            int pointsLength = arrayToCurve.Length;
            int curvedLength = pointsLength * Mathf.RoundToInt(smoothness) - 1;
            List<Vector3> curvedPoints = new List<Vector3>(curvedLength);
            float t;
            for (int i = 0; i < curvedLength + 1; i++)
            {
                t = Mathf.InverseLerp(0, curvedLength, i);
                List<Vector3> points = new List<Vector3>(arrayToCurve);
                for (int j = pointsLength - 1; j > 0; j--)
                for (int k = 0; k < j; k++)
                    points[k] = (1 - t) * points[k] + t * points[k + 1];

                curvedPoints.Add(points[0]);
            }

            return curvedPoints.ToArray();
        }
    }
}
