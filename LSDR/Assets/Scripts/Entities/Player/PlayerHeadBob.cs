﻿using UnityEngine;
using LSDR.Game;
using LSDR.InputManagement;
using UnityEngine.Audio;
using LSDR.Util;
using Torii.Audio;
using Torii.Resource;
using Torii.Util;

namespace LSDR.Entities.Player
{
	/// <summary>
	/// Handles player head bobbing and footstep sounds. Footstep sounds are handled here as footsteps need to be
	/// played when a player's head bob is at its lowest level.
	/// </summary>
	public class PlayerHeadBob : MonoBehaviour
	{
		public SettingsSystem Settings;
		public ControlSchemeLoaderSystem ControlScheme;
		
		/// <summary>
		/// The speed at which to bob the player's head. Set in editor.
		/// </summary>
		public float BobbingSpeed;
		
		/// <summary>
		/// The amount to bob the player's head. Set in editor.
		/// </summary>
		public float BobbingAmount;
		
		/// <summary>
		/// The midpoint of the player's head bobbing. Set in editor.
		/// </summary>
		public float Midpoint = 1F;
		
		/// <summary>
		/// The camera to bob. Set in editor.
		/// </summary>
		public Camera TargetCamera;

		/// <summary>
		/// The AudioSource to play footstep sounds from. Set in editor.
		/// </summary>
		private AudioSource _footstepAudioSource;

		// mixer used to control footstep sound volume/effects
		private AudioMixer _masterMixer;

		// timer used to control headbob frequency
		private float _timer;

		// whether or not we should play a footstep sound
		private bool _canPlayFootstepSound = true;

		// the target transform
		private Transform _targetCamTransform;

		void Start()
		{
			// get the master mixer
			_masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");
		
			// setup the audio source for the footstep sounds
			_footstepAudioSource = gameObject.AddComponent<AudioSource>();
			_footstepAudioSource.spatialBlend = 0; // 2d audio
			_footstepAudioSource.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("SFX")[0];
			
			// load a footstep sound into the audio source
			// TODO: add more footstep sounds, and play a sound based on what player is currently walking on
			var footstepClip =
				ResourceManager.Load<ToriiAudioClip>(
					PathUtil.Combine(Application.streamingAssetsPath, "sfx", "SE_00003.ogg"), "global");
			_footstepAudioSource.clip = footstepClip;

			// cache a reference to the target camera's transform
			_targetCamTransform = TargetCamera.transform;
		}

		void FixedUpdate()
		{
			// if we can't control the player, we don't want to bob the head
			if (!Settings.CanControlPlayer) return;

			// headbobbing uses a sine wave, this is the initial value of it
			float sineWave = 0F;
			
			// get the movement magnitude - if we're in FPS control mode then we want to include X axis movement
			// (due to strafing), otherwise we only want Y movement as X will be rotational and shouldn't headbob
			float movementMagnitude = ControlScheme.Current.FpsControls
				? ControlScheme.Current.Actions.Move.Value.sqrMagnitude
				: ControlScheme.Current.Actions.MoveY.Value;

			// if the forward movement vector is zero, reset the timer
			if (Mathf.Abs(movementMagnitude) < float.Epsilon)
			{
				_timer = 0;
			}
			else
			{
				// otherwise, advance sine wave and increment the timer
				sineWave = Mathf.Sin(_timer);
				_timer = _timer + (BobbingSpeed*Time.deltaTime);
				
				// reset timer when sine wave repeats
				if (_timer > Mathf.PI*2)
				{
					_timer = 0;
					_canPlayFootstepSound = true;
				}

				// check to see if we're at the bottom of the curve (3Pi / 2)
				if (Settings.Settings.EnableFootstepSounds && _timer > (Mathf.PI * 3f) / 2f)
				{
					// and play a footstep sound if we're able to
					if (_canPlayFootstepSound)
					{
						_footstepAudioSource.Play();
						_canPlayFootstepSound = false;
					}
				}
			}
			
			// update 'head' position when sine wave isn't near 0
			if (Mathf.Abs(sineWave) > float.Epsilon)
			{
				float translateChange = sineWave * BobbingAmount;
				Vector3 pos = _targetCamTransform.localPosition;
				pos.y = Midpoint + (Settings.Settings.HeadBobEnabled ? translateChange : 0);
				_targetCamTransform.localPosition = pos;
			}
			else
			{
				// if it is near zero we want to just set it to the midpoint, as we may not be bobbing
				Vector3 pos = _targetCamTransform.localPosition;
				pos.y = Midpoint;
				_targetCamTransform.localPosition = pos;
			}
		}
	}
}