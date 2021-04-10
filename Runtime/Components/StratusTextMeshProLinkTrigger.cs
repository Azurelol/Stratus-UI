using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Stratus.UI
{
	public class StratusTextMeshProLinkTrigger : StratusTriggerBehaviour, IPointerClickHandler
	{
		[SerializeField]
		private TextMeshProUGUI target;
		[SerializeField]
		private new Camera camera;
		
		protected override void OnAwake()
		{
			if (camera == null)
			{
				camera = Camera.main;
			}
		}
		protected override void OnReset()
		{
			target = GetComponent<TextMeshProUGUI>();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			int linkIndex = TMP_TextUtilities.FindIntersectingLink(target, Input.mousePosition, camera);
			if (linkIndex != -1)
			{
				TMP_LinkInfo linkInfo = target.textInfo.linkInfo[linkIndex];
				var text = linkInfo.GetLinkText();
				Activate(text);
			}
		}


	}
}