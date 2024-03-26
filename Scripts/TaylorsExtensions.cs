using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class ColorExtensions
{
	public static Color SetNonAlpha(this Color @this, Color nonAlphaColor)
	{
		return new Color(nonAlphaColor, @this.A);
	}
	public static void SetModulateNonAlpha(this CanvasItem @this, Color nonAlphaColor)
	{
		@this.Modulate = new Color(nonAlphaColor, @this.Modulate.A);
	}
	public static void SetAlpha(this CanvasItem @this, float alpha)
	{
		@this.Modulate = new Color(@this.Modulate, alpha);
	}
}

public static class Monokai
{
	public static Color Back => Color.FromHtml("#3B3A32");
	public static Color Yellow => Color.FromHtml("#E6DB74");
	public static Color LightYellow => Color.FromHtml("#FFE792");
	public static Color FadedYellow => Color.FromHtml("#FFEFB7");
	public static Color Purple => Color.FromHtml("#AE81FF");
	public static Color LightPurple => Color.FromHtml("#E777FF");
	public static Color Green => Color.FromHtml("#A6E22E");
	public static Color DarkGreen => Color.FromHtml("#829520");
	public static Color Orange => Color.FromHtml("#FD971F");
	public static Color Brown => Color.FromHtml("#9D550F");
	public static Color Magenta => Color.FromHtml("#F92672");
	public static Color Red => Color.FromHtml("#F83333");
	public static Color LightRed => Color.FromHtml("#FF5959");
	public static Color Blue => Color.FromHtml("#66D9EF");
	public static Color LightBlue => Color.FromHtml("#A9FFFF");
	public static Color White => Color.FromHtml("#F8F8F2");
	public static Color LightGray => Color.FromHtml("#BBBBBB");
	public static Color Gray1 => Color.FromHtml("#AFAFA2");
	public static Color Gray2 => Color.FromHtml("#75715E");
	public static Color DarkGray => Color.FromHtml("#212121");
}

public static class PositionExtensions
{
	public static void SetX(this Control @this, float value)
	{
		@this.Position = new Vector2(value, @this.Position.Y);
	}
	public static void SetY(this Control @this, float value)
	{
		@this.Position = new Vector2(@this.Position.X, value);
	}

	public static void AddX(this Control @this, float value)
	{
		@this.Position = new Vector2(@this.Position.X + value, @this.Position.Y);
	}
	public static void AddY(this Control @this, float value)
	{
		@this.Position = new Vector2(@this.Position.X, @this.Position.Y + value);
	}
}

public static class StringExtensions
{
	public static bool IsNullOrEmpty(this string @this)
	{
		return (@this == null || @this == "");
	}

	public static bool IContains(this string @this, string substring)
	{
		return @this.ToLower().Contains(substring.ToLower());
	}

	public static bool MatchesSearch(this string @this, string searchString)
	{
		if (searchString.IsNullOrEmpty()) { return true; }
		string[] wordParts = searchString.Split(' ');
		foreach (string word in wordParts)
		{
			if (!word.IsNullOrEmpty() && !@this.IContains(word)) { return false; }
		}
		return true;
	}
}

public static class EnumerableExtensions
{
	public static string StringJoin(this IEnumerable<string> @this, string delimeter = "")
	{
		StringBuilder builder = new StringBuilder();
		int itemIndex = 0; 
		foreach (string item in @this)
		{
			if (itemIndex > 0) { builder.Append(delimeter); }
			builder.Append(item);
			itemIndex++;
		}
		return builder.ToString();
	}
}
