﻿using Stratus.Extensions;
using Stratus.Logging;

using System;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stratus.UI
{
	public abstract class StratusLayoutElement : StratusBehaviour, ISelectHandler
	{
		public RectTransform rectTransform => GetComponentCached<RectTransform>();

		public event Action onSelect;

		public abstract Selectable selectable { get; }
		public abstract void OnSelect(BaseEventData eventData);
		public abstract void Select();
		public abstract Selectable SelectNext();
		public abstract Selectable SelectPrevious();
		public abstract void Submit();
		public abstract void UpdateContent();

		protected void NotifyElementSelected() => onSelect?.Invoke();
	}

	public interface IStratusLayoutElement<ParameterType, StyleType>
		where ParameterType : StratusLayoutElementEntry
		where StyleType : StratusLayoutElementStyle
	{
		void Initialize(ParameterType parameters, StyleType style);
	}

	/// <summary>
	/// An layout entry
	/// </summary>
	public abstract class StratusLayoutElement<LayoutElementType, EntryType, StyleType>
		: StratusLayoutElement, IStratusLayoutElement<EntryType, StyleType>
		where LayoutElementType : UIBehaviour
		where EntryType : StratusLayoutElementEntry
		where StyleType : StratusLayoutElementStyle
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField]
		protected Image background;
		[SerializeField]
		protected Button button;
		[SerializeField]
		protected LayoutElementType _body;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public LayoutElementType body
		{
			get
			{
				if (_body == null)
				{
					_body = this.GetComponentCached<LayoutElementType>();
				}
				return _body;
			}
		}

		public override Selectable selectable => button;

		public bool highlighted { get; private set; }

		public bool hasBackground => !background.IsNull();
		public abstract Color bodyColor { get; protected set; }
		public Color backgroundColor
		{
			get => this.hasBackground ? background.color : Color.clear;
			set
			{
				if (this.hasBackground)
				{
					background.color = value;
				}
			}
		}

		public EntryType entry { get; private set; }
		public StyleType style { get; private set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnInitialize(EntryType parameters, StyleType style);
		protected abstract void OnUpdateContent(EntryType entry);

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public void Initialize(EntryType entry, StyleType style)
		{
			this.entry = entry;
			this.style = style;
			this.entry.onUpdated += UpdateContent;

			if (style != null)
			{
				background.enabled = style.background;
				if (style.bodyHeight != default)
				{
					rectTransform.SetHeight(style.bodyHeight);
				}

				button.navigation = style.buttonStyle.navigation;
				button.colors = style.buttonStyle.colorBlock;

				if (!entry.colors.assigned)
				{
					entry.colors = style.defaultStyle;
				}
			}

			OnInitialize(entry, style);
			UpdateContent();
		}


		public override void UpdateContent()
		{
			if (entry.onSubmit != null)
			{
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(entry.onSubmit);
			}
			else
			{
				button.interactable = false;
			}

			if (entry.colors.assigned)
			{
				if (entry.colors.bodyColor != default)
				{
					bodyColor = entry.colors.bodyColor;
				}
				if (entry.colors.backgroundColor != default)
				{
					backgroundColor = entry.colors.backgroundColor;
				}
			}

			gameObject.name = entry.text.value.Truncate(20);

			OnUpdateContent(this.entry);
		}

		public override void Select()
		{
			button.Select();			
		}

		public override void OnSelect(BaseEventData eventData)
		{
			entry.onSelect?.Invoke();
			NotifyElementSelected();
			button.OnSelect(eventData);
		}

		public override Selectable SelectNext()
		{
			Selectable selectable = null;
			switch (button.navigation.mode)
			{
				case Navigation.Mode.None:
					break;
				case Navigation.Mode.Horizontal:
					break;
				case Navigation.Mode.Vertical:
					selectable = SelectDown();
					break;
				case Navigation.Mode.Automatic:
					break;
				case Navigation.Mode.Explicit:
					break;
			}
			return selectable;
		}

		public override Selectable SelectPrevious()
		{
			Selectable selectable = null;
			switch (button.navigation.mode)
			{
				case Navigation.Mode.None:
					break;
				case Navigation.Mode.Horizontal:
					break;
				case Navigation.Mode.Vertical:
					selectable = SelectUp();
					break;
				case Navigation.Mode.Automatic:
					break;
				case Navigation.Mode.Explicit:
					break;
			}
			return selectable;
		}

		public Selectable SelectUp()
		{
			Selectable selectable = button.FindSelectableOnUp();
			selectable.Select();
			return selectable;
		}

		public Selectable SelectDown()
		{
			Selectable selectable = button.FindSelectableOnDown();
			selectable.Select();
			return selectable;
		}

		public void Highlight(bool toggle)
		{
			if (!hasBackground)
			{
				return;
			}

			if (toggle && !highlighted)
			{
				bodyColor = style.highlightStyle.bodyColor;
				backgroundColor = style.highlightStyle.backgroundColor;
				highlighted = true;
			}
			else if (!toggle && highlighted)
			{
				if (entry.colors == null)
				{
					this.LogError("no entry colors???");
				}
				bodyColor = entry.colors.bodyColor;
				backgroundColor = entry.colors.backgroundColor;
				highlighted = false;
			}
		}

		public override void Submit()
		{
			if (button != null)
			{
				button.onClick.Invoke();
			}
		}
	}





}