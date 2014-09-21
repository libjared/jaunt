using System;
using SFML.Window;

namespace Jaunt
{
	public static class Extensions
	{
		public static Vector2f Floor(this Vector2f input)
		{
			return new Vector2f(Math.Floor (input.X), Math.Floor (input.Y));
		}
	}
}

