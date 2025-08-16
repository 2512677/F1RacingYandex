using UnityEngine;

public static class bl_HudUtility
{
	public static float GetScreenSlope
	{
		get
		{
			float num = mCamera.pixelHeight;
			float num2 = mCamera.pixelWidth;
			return num / num2;
		}
	}

	public static float MidlleHeight => Camera.main.pixelHeight / 2;

	public static float MiddleWidth => Camera.main.pixelWidth / 2;

	public static Camera mCamera
	{
		get
		{
			if (Camera.main != null)
			{
				return Camera.main;
			}
			return Camera.current;
		}
	}

	public static float RetineAspect
	{
		get
		{
			Vector2 vector = new Vector2(Screen.width, Screen.height);
			float num = 0f;
			float a = Mathf.Log(vector.x / bl_HudManager.instance.m_ReferenceResolution.x, 2f);
			float b = Mathf.Log(vector.y / bl_HudManager.instance.m_ReferenceResolution.y, 2f);
			float p = Mathf.Lerp(a, b, bl_HudManager.instance.m_MatchWidthOrHeight);
			return Mathf.Pow(2f, p);
		}
	}

	public static float GetRotation(float x1, float y1, float x2, float y2)
	{
		float num = 3.141593f;
		float num2 = x2 - x1;
		float num3 = y2 - y1;
		float num4 = Mathf.Atan(num3 / num2) * 180f / num;
		if (num2 < 0f)
		{
			num4 += 180f;
		}
		return num4;
	}

	public static Vector2 GetPivot(float h, float v, float size)
	{
		float num = h - (float)mCamera.pixelWidth * 0.5f;
		float num2 = v - (float)mCamera.pixelHeight * 0.5f;
		float num3 = num2 / num;
		Vector2 zero = Vector2.zero;
		float num4;
		if (num3 > GetScreenSlope || num3 < 0f - GetScreenSlope)
		{
			num4 = (MidlleHeight - HalfSize(size)) / num2;
			if (num2 < 0f)
			{
				zero.y = HalfSize(size);
				num4 *= -1f;
			}
			else
			{
				zero.y = (float)mCamera.pixelHeight - HalfSize(size);
			}
			zero.x = MiddleWidth + num * num4;
			return zero;
		}
		num4 = (MiddleWidth - HalfSize(size)) / num;
		if (num < 0f)
		{
			zero.x = HalfSize(size);
			num4 *= -1f;
		}
		else
		{
			zero.x = (float)mCamera.pixelWidth - HalfSize(size);
		}
		zero.y = MidlleHeight + num2 * num4;
		return zero;
	}

	public static float HalfSize(float s)
	{
		return s / 2f;
	}

	public static Vector2 Marge(Vector2 v, float size)
	{
		float num = 50f;
		Vector2 result = v;
		float num2 = (float)Screen.width * 0.5f;
		float num3 = (float)Screen.height * 0.5f;
		if (v.x + size >= num2)
		{
			result.x -= num2;
			result.x = 0f - result.x;
			result.x = Mathf.Clamp(result.x, 0f - num, 0f);
		}
		else
		{
			result.x = num2 - result.x;
			result.x = Mathf.Clamp(result.x, 0f, num);
		}
		if (v.y + size >= num3)
		{
			result.y -= num3;
			result.y = 0f - result.y;
			result.y = Mathf.Clamp(result.y, 0f - num, 0f);
		}
		else
		{
			result.y = num3 - result.y;
			result.y = Mathf.Clamp(result.y, 0f, num);
		}
		return result;
	}

	public static Rect ScalerRect(Rect _rect)
	{
		float retineAspect = RetineAspect;
		_rect.width *= retineAspect;
		_rect.height *= retineAspect;
		_rect.x *= retineAspect;
		_rect.y *= retineAspect;
		return _rect;
	}

	public static Vector3 ScreenPosition(Transform t)
	{
		Vector3 result;
		if (mCamera != null)
		{
			result = mCamera.WorldToScreenPoint(t.position);
			result.x /= mCamera.pixelWidth;
			result.y /= mCamera.pixelHeight;
			Vector3 position = t.position;
			result.z = position.z;
		}
		else
		{
			result = Vector3.one * 0.5f;
		}
		return result;
	}

	public static bool isOnScreen(Vector3 pos, Transform t)
	{
		Vector3 lhs = t.position - mCamera.transform.position;
		if (Vector3.Dot(lhs, mCamera.transform.forward) <= 0f)
		{
			return false;
		}
		float num = 0.001f;
		if (pos.x < num || pos.x > 1f - num || pos.y < num || pos.y > 1f - num)
		{
			return false;
		}
		return true;
	}
}
