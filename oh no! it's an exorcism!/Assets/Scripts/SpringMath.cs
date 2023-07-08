// https://youtu.be/omU_G5TV4YA
// based off math from allenchou.net

using UnityEngine;

public class SpringMath
{
    /// <summary>
    /// i have no idea how this works but it does
    /// </summary>
    /// <param name="value"></param>
    /// <param name="velocity"> if used on an angle, use the Mathf.DeltaAngle between current and  </param>
    /// <param name="targetValue"></param>
    /// <param name="dampingRatio">Damping of the oscillation from 0 (no damping) to 1 (no springing)</param>
    /// <param name="angularFreq">Oscillations per second</param>
    /// <param name="timeStep">Put Time.deltaTime here</param>
    /// <returns></returns>
    public static void Lerp(ref float value, ref float velocity, float targetValue, float dampingRatio, float angularFreq, float timeStep)
    {

        float delta, delta_x, delta_v;

        delta = (1 + 2 * timeStep * dampingRatio * angularFreq) + Mathf.Pow(timeStep, 2) * Mathf.Pow(angularFreq, 2);

        delta_x = (1 + 2 * timeStep * dampingRatio * angularFreq) * value
            + timeStep * velocity
            + Mathf.Pow(timeStep, 2) * Mathf.Pow(angularFreq, 2) * targetValue;

        delta_v = velocity + timeStep * Mathf.Pow(angularFreq, 2) * (targetValue - value);

        velocity = delta_v / delta;
        value = delta_x / delta;


    }

    public static void Lerp(ref Vector2 value, ref Vector2 velocity, Vector2 target, float dampingRatio, float angularFreq, float timeStep)
	{
        Lerp(ref value.x, ref velocity.x, target.x, dampingRatio, angularFreq, timeStep);
        Lerp(ref value.y, ref velocity.y, target.y, dampingRatio, angularFreq, timeStep);

	}

    public static void LerpAngle(ref float angle, ref float velocity, float targetAngle, float dampingRatio, float angularFreq, float timeStep)
	{
        // get the shortest direction to the target angle
        targetAngle = angle - Mathf.DeltaAngle(angle, targetAngle);


        float delta, delta_x, delta_v;

        delta = (1 + 2 * timeStep * dampingRatio * angularFreq) + Mathf.Pow(timeStep, 2) * Mathf.Pow(angularFreq, 2);

        delta_x = (1 + 2 * timeStep * dampingRatio * angularFreq) * angle
            + timeStep * velocity
            + Mathf.Pow(timeStep, 2) * Mathf.Pow(angularFreq, 2) * targetAngle;

        delta_v = velocity + timeStep * Mathf.Pow(angularFreq, 2) * (targetAngle - angle);

        angle = delta_x / delta;
        velocity = delta_v / delta;

    }



}
