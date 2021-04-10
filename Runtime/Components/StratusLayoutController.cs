using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Stratus.UI
{
	[ExecuteInEditMode]
	public class StratusLayoutController : UIBehaviour, ILayoutSelfController, ILayoutElement
	{
		[SerializeField]
		private RectTransform rectTransform;

		[SerializeField]
		protected Vector2 m_maxSize = Vector2.zero;

		[SerializeField]
		protected Vector2 m_minSize = Vector2.zero;

		public Vector2 maxSize
		{
			get { return m_maxSize; }
			set
			{
				if (m_maxSize != value)
				{
					m_maxSize = value;
					SetDirty();
				}
			}
		}

		public Vector2 minSize
		{
			get { return m_minSize; }
			set
			{
				if (m_minSize != value)
				{
					m_minSize = value;
					SetDirty();
				}
			}
		}

		/// <summary>
		/// Sets both <see cref="minSize"/> and <see cref="maxSize"/>
		/// </summary>
		public Vector2 size
		{
			set
			{
				m_minSize = m_maxSize = value;
				SetDirty();
			}
		}

		public float minWidth => minSize.x;
		public float preferredWidth => minSize.x;
		public float minHeight => minSize.y;
		public float preferredHeight => minSize.y;
		public float flexibleWidth => 0f;
		public float flexibleHeight => 0f;
		public int layoutPriority => 1;


		private DrivenRectTransformTracker m_Tracker;

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			m_Tracker.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		protected override void Reset()
		{
			base.Reset();
			rectTransform = GetComponent<RectTransform>();
		}

		protected void SetDirty()
		{
			if (!IsActive())
				return;

			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		public void SetLayoutHorizontal()
		{
			if (m_maxSize.x > 0f && rectTransform.rect.width > m_maxSize.x)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize.x);
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
			}

			if (m_minSize.x > 0f && rectTransform.rect.width < m_minSize.x)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, minSize.x);
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);
			}

		}

		public void SetLayoutVertical()
		{
			if (m_maxSize.y > 0f && rectTransform.rect.height > m_maxSize.y)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize.y);
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
			}

			if (m_minSize.y > 0f && rectTransform.rect.height < m_minSize.y)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, minSize.y);
				m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
			}

		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			SetDirty();
		}

		public void CalculateLayoutInputHorizontal()
		{
			
		}

		public void CalculateLayoutInputVertical()
		{
		}
#endif

	}

}