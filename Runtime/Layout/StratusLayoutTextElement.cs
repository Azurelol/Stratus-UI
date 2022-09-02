using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Stratus;

namespace Stratus.UI
{
	#region Data
	public class StratusLayoutTextElementEntry : StratusLayoutElementEntry
	{
		public StratusProvider<Sprite> icon { get; set; }
		public StratusProvider<string> header { get; set; }

		public StratusLayoutTextElementEntry()
		{
		}

		public StratusLayoutTextElementEntry(string label) : base(label)
		{
		}

		public StratusLayoutTextElementEntry(string label, Action action) : base(label, action)
		{
		}

		public StratusLayoutTextElementEntry(StratusLabeledAction labeledAction) : this(labeledAction.label, labeledAction.action)
		{
		}
	}

	[Serializable]
	public class StratusTextStyle
	{
		public int fontSize = 12;
		public bool richText = true;
		public bool autoSizeText = false;
		public bool hideIconFrame = true;
		public int iconSize = 0;
		public TextAlignmentOptions textAlignment = TextAlignmentOptions.MidlineGeoAligned;
	}

	[Serializable]
	public class StratusLayoutTextElementStyle : StratusLayoutElementStyle
	{
		public StratusTextStyle textStyle = new StratusTextStyle();
	} 
	#endregion

	/// <summary>
	/// An layout entry that uses a text component 
	/// </summary>
	public class StratusLayoutTextElement : StratusLayoutElement<TextMeshProUGUI,
		StratusLayoutTextElementEntry, StratusLayoutTextElementStyle>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		private LayoutGroup layout;
		[SerializeField]
		private Image iconFrame;
		[SerializeField]
		private TextMeshProUGUI headerFrame;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public string text
		{
			get => this.body.text;
			set => this.body.text = value;
		}

		public override Color bodyColor
		{
			get => this.body.color;
			protected set => this.body.color = value;
		}

		public Sprite icon
		{
			get => iconFrame.sprite;
			set => iconFrame.sprite = value;
		}

		public float targetVerticalSize => (rectTransform.rect.height - layout.padding.top + layout.padding.bottom);

		public bool hasIconFrame => iconFrame != null;
		public bool hasHeaderFrame => headerFrame != null;

		private StratusLayoutController iconLayout { get; set; }
		protected override void OnInitialize(StratusLayoutTextElementEntry entry, StratusLayoutTextElementStyle style)
		{
			iconLayout = iconFrame.GetComponent<StratusLayoutController>();
			iconFrame.enabled = false;

			if (style != null)
			{
				if (style.textStyle.autoSizeText)
				{
					body.enableAutoSizing = true;
				}
				else
				{
					if (style.textStyle.fontSize != default)
					{
						body.fontSize = style.textStyle.fontSize;
					}
				}
				body.richText = style.textStyle.richText;
				body.alignment = style.textStyle.textAlignment;
			}
		}

		protected override void OnUpdateContent(StratusLayoutTextElementEntry entry)
		{
			text = entry.text.value;

			UpdateIconSize();

			if (entry.icon != null && entry.icon.valid && hasIconFrame)
			{
				this.icon = entry.icon.value;
				iconFrame.enabled = hasIconFrame;				
				iconLayout.gameObject.SetActive(true);
			}
			else if (style.textStyle.hideIconFrame)
			{
				iconLayout.gameObject.SetActive(false);
			}

			if (entry.header != null && entry.header.valid && hasHeaderFrame)
			{
				this.headerFrame.text = entry.header.value;
				this.headerFrame.gameObject.SetActive(true);
			}
			else
			{
				this.headerFrame.gameObject.SetActive(false);
			}
		}

		private void UpdateIconSize()
		{
			float iconSize = style.textStyle.iconSize > 0 ? style.textStyle.iconSize
			: targetVerticalSize;
			iconLayout.size = new Vector2(iconSize, iconSize);
		}


	}

}