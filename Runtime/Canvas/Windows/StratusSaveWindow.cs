﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Stratus.Extensions;
using Stratus.Models.Saves;
using Stratus.Logging;

namespace Stratus.UI
{
	public enum StratusSaveWindowMode
	{
		Save,
		Load
	}

	public abstract class StratusSaveWindow<T, SaveType, SaveSystem> : StratusCanvasWindow<T>
		where T : StratusSaveWindow<T, SaveType, SaveSystem>
		where SaveType : class, IUnityStratusSave, new()
		where SaveSystem : ISaveSystem<SaveType>
	{
		//------------------------------------------------------------------------/
		// Fields
		//------------------------------------------------------------------------/
		[SerializeField] private StratusLayoutTextElementGroupBehaviour layout;
		[SerializeField] private TextMeshProUGUI titleText;
		[SerializeField] private TextMeshProUGUI saveDetailsText;
		[SerializeField] private RawImage saveSnapshotImage;
		[SerializeField] private int saveLimit = 0;

		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public StratusSaveWindowMode mode { get; private set; }
		public abstract SaveSystem saveSystem { get; }
		public SaveType currentSave { get; private set; }
		private Action onLoaded { get; set; }

		//------------------------------------------------------------------------/
		// Virtual
		//------------------------------------------------------------------------/
		protected abstract void OnSaved(SaveType save);
		protected abstract void OnLoaded(SaveType save);

		//------------------------------------------------------------------------/
		// Messages
		//------------------------------------------------------------------------/
		protected override void OnWindowAwake()
		{
		}

		protected override void OnWindowClose()
		{
			currentSave = null;
		}

		protected override void OnWindowOpen()
		{
			titleText.text = mode.ToString();
			Refresh();
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public static void OpenForSaving(SaveType save, IStratusCanvasWindow parent = null)
		{
			instance.currentSave = save;
			Open(StratusSaveWindowMode.Save, parent);
		}

		public static void OpenForLoading(Action onLoaded, IStratusCanvasWindow parent = null)
		{
			instance.onLoaded = onLoaded;
			Open(StratusSaveWindowMode.Load, parent);
		}

		private static void Open(StratusSaveWindowMode mode, IStratusCanvasWindow parent)
		{
			instance.mode = mode;
			OpenWindow(new StratusCanvasWindowOpenArguments(parent, true)
			{
				ignoreInpuLayerBlocking = true,
			});
		}

		public void Refresh()
		{
			layout.Reset();
			saveSystem.LoadAllSaves();

			List<StratusLayoutTextElementEntry> entries = new List<StratusLayoutTextElementEntry>();
			foreach(SaveType save in saveSystem.saves)
			{
				StratusLayoutTextElementEntry entry = new StratusLayoutTextElementEntry(GenerateSaveEntryName(save), 
					() => Submit(save));
				entry.onSelect = () => Select(save);
			}

			layout.Set(entries);
			layout.Select();
		}

		private void Submit(SaveType save)
		{
			switch (mode)
			{
				case StratusSaveWindowMode.Save:
					this.Log($"Saving {save}");
					saveSystem.Save(save);
					OnSaved(save);
					break;
				case StratusSaveWindowMode.Load:
					this.Log($"Loading {save}");
					onLoaded?.Invoke();
					onLoaded = null;
					Close();
					OnLoaded(save);
					break;
			}
		}

		protected virtual string GenerateSaveEntryName(SaveType save)
		{
			return $"{save.name}";
		}

		private void CreateSaveAtIndex(SaveType save, int index, StratusLayoutTextElementEntry entry)
		{
			this.Log($"Saving current save to index {index}");
			saveSystem.Save(currentSave);

			entry.text = new StratusProvider<string>(GenerateSaveEntryName(save));
			entry.onSelect = () => Select(currentSave);
			entry.onSubmit = () => Submit(currentSave);
			entry.SetDirty();
		}

		private void Select(SaveType save)
		{
			if (save == null)
			{
				saveSnapshotImage.texture = null;
				saveDetailsText.text = string.Empty;
				return;
			}

			saveDetailsText.text = GetDetails(save);
			if (save.LoadSnapshot())
			{
				if (save.snapshot == null)
				{
					this.LogError($"Snapshot for {save} didn't load property...");
				}
				else
				{
					saveSnapshotImage.texture = save.snapshot;
				}
			}
		}

		protected virtual string GetDetails(SaveType save)
		{
			Dictionary<string, string> details = save.ComposeDetailedStringMap();
			return details.ToString(" : ");
		}
	}

}