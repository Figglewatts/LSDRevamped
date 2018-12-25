﻿using UnityEngine;
using System.Collections;

namespace UI
{
	public class UIMainMenu : MonoBehaviour
	{
		public enum MenuState
		{
			MAIN,
			SETTINGS,
			GRAPH
		}

		public MenuState CurrentState;

		[SerializeField]
		private GameObject _mainMenuObject;
		[SerializeField]
		private GameObject _settingsMenuObject;
		[SerializeField]
		private GameObject _graphMenuObject;

		public void ChangeMenuState(int state) { ChangeMenuState((MenuState) state); }

		public void ChangeMenuState(MenuState state)
		{
			switch (state)
			{
				case MenuState.MAIN:
				{
					_mainMenuObject.SetActive(true);
					_settingsMenuObject.SetActive(false);
					_graphMenuObject.SetActive(false);
					break;
				}
				case MenuState.SETTINGS:
				{
					_mainMenuObject.SetActive(false);
					_settingsMenuObject.SetActive(true);
					_graphMenuObject.SetActive(false);
					break;
				}
				case MenuState.GRAPH:
				{
					_mainMenuObject.SetActive(false);
					_settingsMenuObject.SetActive(false);
					_graphMenuObject.SetActive(true);
					break;
				}
            }
			CurrentState = state;
		}

        public void ShowAfterFade()
        {
            Fader.FadeIn(Color.black, 0.5F, () =>
            {
                ChangeMenuState(MenuState.MAIN);
                Fader.FadeOut(0.5F);
            });
        }
	}
}