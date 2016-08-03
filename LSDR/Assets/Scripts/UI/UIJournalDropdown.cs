﻿using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.UI;

namespace UI
{
	public class UIJournalDropdown : MonoBehaviour
	{
		public Dropdown JournalDropdown;

		void Start() { RepopulateDropdown(); }

		void OnEnable() { RepopulateDropdown(); }

		public void OnValueChange(int i)
		{
			DreamJournalManager.SwitchJournal(i);
		}

		private void RepopulateDropdown()
		{
			JournalDropdown.ClearOptions();
			JournalDropdown.AddOptions(DreamJournalManager.LoadedJournals);
		}
	}
}