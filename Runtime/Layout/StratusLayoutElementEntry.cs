using System;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stratus.UI
{
	public abstract class StratusLayoutElementEntry
	{
		/// <summary>
		/// The displayed string for this entry in the UI
		/// </summary>
		public StratusValue<string> text { get; set; }

		/// <summary>
		/// Action to be invoked when this entry is submitted (UI) / executed
		/// </summary>
		public Action onSubmit { get; set; }
		/// <summary>
		/// Action to be invoked when this entry has been selected
		/// </summary>
		public Action onSelect { get; set; }

		/// <summary>
		/// Invoked whenever this entry has been updated
		/// </summary>
		public event Action onUpdated;

		/// <summary>
		/// Set colors for this entry
		/// </summary>
		public StratusLayoutElementColors colors { get; set; } = new StratusLayoutElementColors();

		public StratusLayoutElementEntry()
		{
		}

		protected StratusLayoutElementEntry(string text)
		{
			this.text = new StratusValue<string>(text);
		}

		protected StratusLayoutElementEntry(string text, Action onSubmit)
			: this(text)
		{
			this.onSubmit = onSubmit;
		}

		/// <summary>
		/// Marks this entry as having been updated
		/// </summary>
		public void SetDirty()
		{
			onUpdated?.Invoke();
		}
	}

	[Serializable]
	public class StratusLayoutElementColors
	{
		public Color bodyColor = default;
		public Color backgroundColor = default;

		public StratusLayoutElementColors()
		{
		}

		public StratusLayoutElementColors(Color bodyColor, Color backgroundColor)
		{
			this.bodyColor = bodyColor;
			this.backgroundColor = backgroundColor;
		}

		public bool assigned => bodyColor != default || backgroundColor != default;
	}

	[Serializable]
	public class StratusLayoutElementButtonStyle
	{
		public Navigation navigation = Navigation.defaultNavigation;
		public ColorBlock colorBlock = defaultColorBlock.Value;

		public static readonly Lazy<ColorBlock> defaultColorBlock = new Lazy<ColorBlock>(() => new ColorBlock()
		{
			normalColor = ColorBlock.defaultColorBlock.normalColor,
			colorMultiplier = ColorBlock.defaultColorBlock.colorMultiplier,
			disabledColor = ColorBlock.defaultColorBlock.disabledColor,
			highlightedColor = ColorBlock.defaultColorBlock.highlightedColor,
			pressedColor = ColorBlock.defaultColorBlock.pressedColor,
			selectedColor = ColorBlock.defaultColorBlock.selectedColor,
			fadeDuration = ColorBlock.defaultColorBlock.fadeDuration
		});
	}

	[Serializable]
	public class StratusLayoutElementStyle
	{
		public bool background = true;
		public float bodyHeight = 30;
		public StratusLayoutElementButtonStyle buttonStyle = new StratusLayoutElementButtonStyle();
		public StratusLayoutElementColors defaultStyle = new StratusLayoutElementColors(Color.black, Color.white);
		public StratusLayoutElementColors highlightStyle = new StratusLayoutElementColors(Color.white, Color.blue.ScaleSaturation(0.5f));
	}
}