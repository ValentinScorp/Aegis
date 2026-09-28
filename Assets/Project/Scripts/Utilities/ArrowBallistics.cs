using UnityEngine;

namespace Aegis.Utilities
{
    public static class ArrowBallistics
    {
        public const float SimStep = 0.01f;

        // Повертає стартову швидкість, з якою стріла з drag влучить у aimPoint
        // public static Vector3 SolveLaunchVelocity(
        //     Vector3 from, Vector3 aimPoint, float horizontalSpeed, float gravity, float drag)
        // {
        //     Vector3 delta = aimPoint - from;
        //     Vector3 flat = new Vector3(delta.x, 0f, delta.z);
        //     float d = flat.magnitude;
        //     if (d < 0.01f) return Vector3.up;             // ціль майже над нами
        //     Vector3 dir = flat / d;

        //     float aimY = delta.y;                          // висота, в яку "цілимось"
        //     Vector3 v = Vector3.zero;

        //     for (int i = 0; i < 5; i++) {
        //         float t = Mathf.Max(d / horizontalSpeed, 0.05f);
        //         v = dir * (d / t) + Vector3.up * (aimY / t + 0.5f * gravity * t);

        //         float simY = SimulateHeightAt(v, dir, d, gravity, drag);
        //         float error = delta.y - simY;              // + означає недоліт
        //         if (Mathf.Abs(error) < 0.02f) break;
        //         aimY += error;
        //     }
        //     return v;
        // }

        private static float SimulateFlight(Vector3 v, Vector3 dir, float distance,
                                    float gravity, float drag, out float time)
        {
            Vector3 pos = Vector3.zero;
            time = 0f;

            for (int i = 0; i < 1000; i++) {
                Vector3 prev = pos;
                Step(ref pos, ref v, SimStep, gravity, drag);

                float a = Vector3.Dot(prev, dir);
                float b = Vector3.Dot(pos, dir);

                if (b >= distance) {
                    // інтерполяція всередині останнього кроку, щоб не перестрибнути дистанцію
                    float k = Mathf.Clamp01((distance - a) / Mathf.Max(b - a, 1e-5f));
                    time += SimStep * k;
                    return Mathf.Lerp(prev.y, pos.y, k);
                }
                time += SimStep;
            }
            return pos.y;
        }

        public static Vector3 SolveLaunchVelocity(
            Vector3 from, Vector3 aimPoint, float horizontalSpeed, float gravity, float drag,
            out float flightTime)
        {
            Vector3 delta = aimPoint - from;
            Vector3 flat = new Vector3(delta.x, 0f, delta.z);
            float d = flat.magnitude;
            flightTime = 0f;
            if (d < 0.01f) return Vector3.up;
            Vector3 dir = flat / d;

            float aimY = delta.y;
            Vector3 v = Vector3.zero;

            for (int i = 0; i < 5; i++) {
                float t = Mathf.Max(d / horizontalSpeed, 0.05f);
                v = dir * (d / t) + Vector3.up * (aimY / t + 0.5f * gravity * t);

                float simY = SimulateFlight(v, dir, d, gravity, drag, out flightTime);
                float error = delta.y - simY;
                if (Mathf.Abs(error) < 0.02f) break;
                aimY += error;
            }
            return v;
        }

        // Цілиться з упередженням за швидкістю цілі
        public static Vector3 SolveLeadVelocity(
            Vector3 from, Vector3 targetPos, Vector3 targetVelocity,
            float horizontalSpeed, float gravity, float drag)
        {
            Vector3 aim = targetPos;
            Vector3 v = Vector3.zero;

            for (int i = 0; i < 4; i++) {
                v = SolveLaunchVelocity(from, aim, horizontalSpeed, gravity, drag, out float t);
                aim = targetPos + targetVelocity * t;   // де буде ціль через t секунд
            }
            return v;
        }
        public static float PitchDegrees(Vector3 v)
        {
            float flat = new Vector2(v.x, v.z).magnitude;
            return Mathf.Atan2(v.y, flat) * Mathf.Rad2Deg;
        }
        public static void Step(ref Vector3 pos, ref Vector3 v, float dt, float gravity, float drag)
        {
            v.y -= gravity * dt;
            v -= v * (drag * v.magnitude * dt);
            pos += v * dt;
        }
    }
}