using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using Stratus.Extensions;

namespace Stratus.UI
{
	public class StratusLogWindow<T> : StratusCanvasWindow<T>
		where T : StratusLogWindow<T>
	{
		public enum DisplayMode
		{
			Merged,
			Line
		}

		public class AddLineEvent : StratusEvent
		{
			public string line { get; set; }
		}

		#region Fields
		[SerializeField]
		private DisplayMode mode;
		[SerializeField]
		private ScrollRect scrollRect;
		[SerializeField]
		private TextMeshProUGUI text;
		#endregion

		private static AddLineEvent lineEvent = new AddLineEvent();
		private StringBuilder stringBuilder;
		private List<string> lines;

		#region Virtual
		protected virtual void OnLogWindowAwake()
		{
		}
		#endregion

		#region Messages
		protected override void OnWindowAwake()
		{
			stringBuilder = new StringBuilder();
			lines = new List<string>();
			StratusScene.Connect<AddLineEvent>(OnAddLineEvent);
			OnLogWindowAwake();
		}

		protected override void OnWindowClose()
		{
		}

		protected override void OnWindowOpen()
		{
		}

		private void Reset()
		{
			scrollRect = GetComponent<ScrollRect>();
		}
		#endregion

		#region Event Handlers
		private void OnAddLineEvent(AddLineEvent e)
		{
			AddLine(e.line);
		}
		#endregion

		#region Interface
		public static void SubmitLine(string line)
		{
			lineEvent.line = line;
			StratusScene.Dispatch<AddLineEvent>(lineEvent);
		}

		public void AddLine(string line)
		{
			lines.Add(line);
			stringBuilder.AppendLine(line);
			UpdateText();
		}

		public void AddLines(params string[] lines)
		{
			lines.ForEach(l => AddLine(l));
		}

		public void Clear()
		{
			stringBuilder.Clear();
			UpdateText();
		}
		#endregion

		private void UpdateText()
		{
			text.text = stringBuilder.ToString();
			scrollRect.ApplyScrollPosition(0);
		}
	}

	public class StratusLogWindow : StratusLogWindow<StratusLogWindow>
	{
	}
}
