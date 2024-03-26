using Godot;
using System;

public enum EasingStyle
{
	Linear,
	QuadraticIn,
	QuadraticOut,
	QuadraticInOut,
	CubicIn,
	CubicOut,
	CubicInOut,
	QuarticIn,
	QuarticOut,
	QuarticInOut,
	QuinticIn,
	QuinticOut,
	QuinticInOut,
	SineIn,
	SineOut,
	SineInOut,
	CircularIn,
	CircularOut,
	CircularInOut,
	ExponentialIn,
	ExponentialOut,
	ExponentialInOut,
	ElasticIn,
	ElasticOut,
	ElasticInOut,
	BackIn,
	BackOut,
	BackInOut,
	BounceIn,
	BounceOut,
	BounceInOut,
	EarlyInOut,
	LogTwoOutCustom,
	LogTwoInCustom,
}

public static class Easing
{
	public static float Ease(EasingStyle style, float p)
	{
		switch (style)
		{
			case EasingStyle.Linear:           return Linear(p);
			case EasingStyle.QuadraticIn:      return QuadraticIn(p);
			case EasingStyle.QuadraticOut:     return QuadraticOut(p);
			case EasingStyle.QuadraticInOut:   return QuadraticInOut(p);
			case EasingStyle.CubicIn:          return CubicIn(p);
			case EasingStyle.CubicOut:         return CubicOut(p);
			case EasingStyle.CubicInOut:       return CubicInOut(p);
			case EasingStyle.QuarticIn:        return QuarticIn(p);
			case EasingStyle.QuarticOut:       return QuarticOut(p);
			case EasingStyle.QuarticInOut:     return QuarticInOut(p);
			case EasingStyle.QuinticIn:        return QuinticIn(p);
			case EasingStyle.QuinticOut:       return QuinticOut(p);
			case EasingStyle.QuinticInOut:     return QuinticInOut(p);
			case EasingStyle.SineIn:           return SineIn(p);
			case EasingStyle.SineOut:          return SineOut(p);
			case EasingStyle.SineInOut:        return SineInOut(p);
			case EasingStyle.CircularIn:       return CircularIn(p);
			case EasingStyle.CircularOut:      return CircularOut(p);
			case EasingStyle.CircularInOut:    return CircularInOut(p);
			case EasingStyle.ExponentialIn:    return ExponentialIn(p);
			case EasingStyle.ExponentialOut:   return ExponentialOut(p);
			case EasingStyle.ExponentialInOut: return ExponentialInOut(p);
			case EasingStyle.ElasticIn:        return ElasticIn(p);
			case EasingStyle.ElasticOut:       return ElasticOut(p);
			case EasingStyle.ElasticInOut:     return ElasticInOut(p);
			case EasingStyle.BackIn:           return BackIn(p);
			case EasingStyle.BackOut:          return BackOut(p);
			case EasingStyle.BackInOut:        return BackInOut(p);
			case EasingStyle.BounceIn:         return BounceIn(p);
			case EasingStyle.BounceOut:        return BounceOut(p);
			case EasingStyle.BounceInOut:      return BounceInOut(p);
			case EasingStyle.EarlyInOut:       return EarlyInOut(p);
			case EasingStyle.LogTwoOutCustom:  return LogTwoOutCustom(p);
			case EasingStyle.LogTwoInCustom:   return LogTwoInCustom(p);
			default: return Linear(p);
		}
	}
	public static float Inverse(EasingStyle style, float y)
	{
		switch (style)
		{
			case EasingStyle.Linear:           return InverseLinear(y);
			case EasingStyle.QuadraticIn:      return InverseQuadraticIn(y);
			case EasingStyle.QuadraticOut:     return InverseQuadraticOut(y);
			case EasingStyle.QuadraticInOut:   return InverseQuadraticInOut(y);
			// case EasingStyle.CubicIn:          return InverseCubicIn(y);
			// case EasingStyle.CubicOut:         return InverseCubicOut(y);
			// case EasingStyle.CubicInOut:       return InverseCubicInOut(y);
			// case EasingStyle.QuarticIn:        return InverseQuarticIn(y);
			// case EasingStyle.QuarticOut:       return InverseQuarticOut(y);
			// case EasingStyle.QuarticInOut:     return InverseQuarticInOut(y);
			// case EasingStyle.QuinticIn:        return InverseQuinticIn(y);
			// case EasingStyle.QuinticOut:       return InverseQuinticOut(y);
			// case EasingStyle.QuinticInOut:     return InverseQuinticInOut(y);
			// case EasingStyle.SineIn:           return InverseSineIn(y);
			// case EasingStyle.SineOut:          return InverseSineOut(y);
			// case EasingStyle.SineInOut:        return InverseSineInOut(y);
			// case EasingStyle.CircularIn:       return InverseCircularIn(y);
			// case EasingStyle.CircularOut:      return InverseCircularOut(y);
			// case EasingStyle.CircularInOut:    return InverseCircularInOut(y);
			// case EasingStyle.ExponentialIn:    return InverseExponentialIn(y);
			// case EasingStyle.ExponentialOut:   return InverseExponentialOut(y);
			// case EasingStyle.ExponentialInOut: return InverseExponentialInOut(y);
			// case EasingStyle.ElasticIn:        return InverseElasticIn(y);
			// case EasingStyle.ElasticOut:       return InverseElasticOut(y);
			// case EasingStyle.ElasticInOut:     return InverseElasticInOut(y);
			// case EasingStyle.BackIn:           return InverseBackIn(y);
			// case EasingStyle.BackOut:          return InverseBackOut(y);
			// case EasingStyle.BackInOut:        return InverseBackInOut(y);
			// case EasingStyle.BounceIn:         return InverseBounceIn(y);
			// case EasingStyle.BounceOut:        return InverseBounceOut(y);
			// case EasingStyle.BounceInOut:      return InverseBounceInOut(y);
			// case EasingStyle.EarlyInOut:       return InverseEarlyInOut(y);
			// case EasingStyle.LogTwoOutCustom:  return InverseLogTwoOutCustom(y);
			// case EasingStyle.LogTwoInCustom:   return InverseLogTwoInCustom(y);
			default: return InverseLinear(y);
		}
	}
	
	// +==============================+
	// |            Linear            |
	// +==============================+
	// Modeled after the line y = x
	public static float Linear(float p)
	{
		return p;
	}
	public static float InverseLinear(float y)
	{
		return y;
	}

	// +==============================+
	// |          Quadratic           |
	// +==============================+
	// Modeled after the parabola y = x^2
	public static float QuadraticIn(float p)
	{
		return p * p;
	}
	public static float InverseQuadraticIn(float y)
	{
		return Mathf.Sqrt(y);
	}

	// Modeled after the parabola y = -x^2 + 2x
	public static float QuadraticOut(float p)
	{
		return -(p * (p - 2));
	}
	public static float InverseQuadraticOut(float y)
	{
		return 1 - Mathf.Sqrt(-y + 1);
	}

	// Modeled after the piecewise quadratic
	// y = (1/2)((2x)^2)             ; [0, 0.5)
	// y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
	public static float QuadraticInOut(float p)
	{
		if (p < 0.5f)
		{
			return 2 * p * p;
		}
		else
		{
			return (-2 * p * p) + (4 * p) - 1;
		}
	}
	public static float InverseQuadraticInOut(float y)
	{
		if (y < 0.5f)
		{
			return Mathf.Sqrt(y / 2);
		}
		else
		{
			return (8 - Mathf.Sqrt(-32 * y + 32)) / 8;
		}
	}

	// +==============================+
	// |            Cubic             |
	// +==============================+
	// Modeled after the cubic y = x^3
	public static float CubicIn(float p)
	{
		return p * p * p;
	}

	// Modeled after the cubic y = (x - 1)^3 + 1
	public static float CubicOut(float p)
	{
		float f = (p - 1);
		return f * f * f + 1;
	}

	// Modeled after the piecewise cubic
	// y = (1/2)((2x)^3)       ; [0, 0.5)
	// y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
	public static float CubicInOut(float p)
	{
		if (p < 0.5f)
		{
			return 4 * p * p * p;
		}
		else
		{
			float f = ((2 * p) - 2);
			return 0.5f * f * f * f + 1;
		}
	}

	// +==============================+
	// |           Quartic            |
	// +==============================+
	// Modeled after the quartic x^4
	public static float QuarticIn(float p)
	{
		return p * p * p * p;
	}

	// Modeled after the quartic y = 1 - (x - 1)^4
	public static float QuarticOut(float p)
	{
		float f = (p - 1);
		return f * f * f * (1 - p) + 1;
	}

	// Modeled after the piecewise quartic
	// y = (1/2)((2x)^4)        ; [0, 0.5)
	// y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
	public static float QuarticInOut(float p) 
	{
		if (p < 0.5f)
		{
			return 8 * p * p * p * p;
		}
		else
		{
			float f = (p - 1);
			return -8 * f * f * f * f + 1;
		}
	}

	// +==============================+
	// |           Quintic            |
	// +==============================+
	// Modeled after the quintic y = x^5
	public static float QuinticIn(float p) 
	{
		return p * p * p * p * p;
	}

	// Modeled after the quintic y = (x - 1)^5 + 1
	public static float QuinticOut(float p) 
	{
		float f = (p - 1);
		return f * f * f * f * f + 1;
	}

	// Modeled after the piecewise quintic
	// y = (1/2)((2x)^5)       ; [0, 0.5)
	// y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
	public static float QuinticInOut(float p) 
	{
		if (p < 0.5f)
		{
			return 16 * p * p * p * p * p;
		}
		else
		{
			float f = ((2 * p) - 2);
			return  0.5f * f * f * f * f * f + 1;
		}
	}

	// +==============================+
	// |             Sine             |
	// +==============================+
	// Modeled after quarter-cycle of sine wave
	public static float SineIn(float p)
	{
		return (float)Math.Sin((p - 1) * (Math.PI*2)) + 1;
	}

	// Modeled after quarter-cycle of sine wave (different phase)
	public static float SineOut(float p)
	{
		return (float)Math.Sin(p * (Math.PI*2));
	}

	// Modeled after half sine wave
	public static float SineInOut(float p)
	{
		return 0.5f * (1 - (float)Math.Cos(p * Math.PI));
	}

	// +==============================+
	// |           Circular           |
	// +==============================+
	// Modeled after shifted quadrant IV of unit circle
	public static float CircularIn(float p)
	{
		return 1 - Mathf.Sqrt(1 - (p * p));
	}

	// Modeled after shifted quadrant II of unit circle
	public static float CircularOut(float p)
	{
		return Mathf.Sqrt((2 - p) * p);
	}

	// Modeled after the piecewise circular function
	// y = (1/2)(1 - Mathf.Sqrt(1 - 4x^2))           ; [0, 0.5)
	// y = (1/2)(Mathf.Sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
	public static float CircularInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * (1 - Mathf.Sqrt(1 - 4 * (p * p)));
		}
		else
		{
			return 0.5f * (Mathf.Sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1);
		}
	}

	// +==============================+
	// |         Exponential          |
	// +==============================+
	// Modeled after the exponential function y = 2^(10(x - 1))
	public static float ExponentialIn(float p)
	{
		return (p == 0.0f) ? p : Mathf.Pow(2, 10 * (p - 1));
	}

	// Modeled after the exponential function y = -2^(-10x) + 1
	public static float ExponentialOut(float p)
	{
		return (p == 1.0f) ? p : 1 - Mathf.Pow(2, -10 * p);
	}

	// Modeled after the piecewise exponential
	// y = (1/2)2^(10(2x - 1))         ; [0,0.5)
	// y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
	public static float ExponentialInOut(float p)
	{
		if (p == 0.0f || p == 1.0f) { return p; }
		
		if (p < 0.5f)
		{
			return 0.5f * Mathf.Pow(2, (20 * p) - 10);
		}
		else
		{
			return -0.5f * Mathf.Pow(2, (-20 * p) + 10) + 1;
		}
	}

	// +==============================+
	// |           Elastic            |
	// +==============================+
	// Modeled after the damped sine wave y = (float)Math.Sin(13pi/2*x)*Mathf.Pow(2, 10 * (x - 1))
	public static float ElasticIn(float p)
	{
		return (float)Math.Sin(13 * (Math.PI*2) * p) * Mathf.Pow(2, 10 * (p - 1));
	}

	// Modeled after the damped sine wave y = (float)Math.Sin(-13pi/2*(x + 1))*Mathf.Pow(2, -10x) + 1
	public static float ElasticOut(float p)
	{
		return (float)Math.Sin(-13 * (Math.PI*2) * (p + 1)) * Mathf.Pow(2, -10 * p) + 1;
	}

	// Modeled after the piecewise exponentially-damped sine wave:
	// y = (1/2)*(float)Math.Sin(13pi/2*(2*x))*Mathf.Pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
	// y = (1/2)*((float)Math.Sin(-13pi/2*((2x-1)+1))*Mathf.Pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
	public static float ElasticInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * (float)Math.Sin(13 * (Math.PI*2) * (2 * p)) * Mathf.Pow(2, 10 * ((2 * p) - 1));
		}
		else
		{
			return 0.5f * ((float)Math.Sin(-13 * (Math.PI*2) * ((2 * p - 1) + 1)) * Mathf.Pow(2, -10 * (2 * p - 1)) + 2);
		}
	}

	// +==============================+
	// |             Back             |
	// +==============================+
	// Modeled after the overshooting cubic y = x^3-x*(float)Math.Sin(x*pi)
	public static float BackIn(float p)
	{
		return p * p * p - p * (float)Math.Sin(p * Math.PI);
	}

	// Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*(float)Math.Sin((1-x)*pi))
	public static float BackOut(float p)
	{
		float f = (1 - p);
		return 1 - (f * f * f - f * (float)Math.Sin(f * Math.PI));
	}

	// Modeled after the piecewise overshooting cubic function:
	// y = (1/2)*((2x)^3-(2x)*(float)Math.Sin(2*x*pi))           ; [0, 0.5)
	// y = (1/2)*(1-((1-x)^3-(1-x)*(float)Math.Sin((1-x)*pi))+1) ; [0.5, 1]
	public static float BackInOut(float p)
	{
		if (p < 0.5f)
		{
			float f = 2 * p;
			return 0.5f * (f * f * f - f * (float)Math.Sin(f * Math.PI));
		}
		else
		{
			float f = (1 - (2*p - 1));
			return 0.5f * (1 - (f * f * f - f * (float)Math.Sin(f * Math.PI))) + 0.5f;
		}
	}

	// +==============================+
	// |            Bounce            |
	// +==============================+
	public static float BounceOut(float p)
	{
		if (p < 4/11.0f)
		{
			return (121 * p * p)/16.0f;
		}
		else if (p < 8/11.0f)
		{
			return (363/40.0f * p * p) - (99/10.0f * p) + 17/5.0f;
		}
		else if (p < 9/10.0f)
		{
			return (4356/361.0f * p * p) - (35442/1805.0f * p) + 16061/1805.0f;
		}
		else
		{
			return (54/5.0f * p * p) - (513/25.0f * p) + 268/25.0f;
		}
	}

	public static float BounceIn(float p)
	{
		return 1 - BounceOut(1 - p);
	}

	public static float BounceInOut(float p)
	{
		if (p < 0.5f)
		{
			return 0.5f * BounceIn(p*2);
		}
		else
		{
			return 0.5f * BounceOut(p * 2 - 1) + 0.5f;
		}
	}

	// +==============================+
	// |            Early             |
	// +==============================+
	public static float EarlyInOut(float p)
	{
		float p2 = (1.2f * p);
		if (p < 0.418f)
		{
			return 2 * p2 * p2;
		}
		else if (p < 0.833f)
		{
			return (-2 * p2 * p2) + (4 * p2) - 1;
		}
		else
		{
			return 1;
		}
		
		
		// else if (p < 0.74735f)
		// {
		// 	float offsetP = (p - 0.867928f);
		// 	return (6.4814814f * offsetP * offsetP * offsetP * offsetP) + (9.074074f * offsetP * offsetP * offsetP) + (offsetP * offsetP) + 1;
		// }
		// else
		// {
		// 	return 1;
		// }
		
		// else
		// {
		// 	return 4.62962963f * (p - 1) * (p - 1) * (p - 1) + 1.66666666f * (p - 1) * (p - 1) + 1;
		// }
	}

	// +==============================+
	// |         LogTwoCustom         |
	// +==============================+
	public static float LogTwoOutCustom(float p)
	{
		return (1 / 3.16987f) * (float)Math.Log2((8.0f * p) + 1.0f);
	}
	public static float LogTwoInCustom(float p)
	{
		return (Mathf.Pow(2, (3.16987f * p)) - 1.0f) / 8.0f;
	}
}
