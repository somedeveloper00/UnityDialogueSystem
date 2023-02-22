using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
	[Serializable]
	public class DialogueCreator
	{
		[SerializeField] internal Dialogue prefab;
		
		[Tooltip("The maximum amount of instances that can simultaneously exist. -1 means unlimited")]
		[SerializeField] [Min(-1)] internal int maxInstances = -1;

		[SerializeField] internal MaximumReachedBehaviour maximumReachedBehaviour;

		internal enum MaximumReachedBehaviour
		{
			GetOldest, GetNewest
		}

		List<Dialogue> _dialogues = new List<Dialogue>( 1 );

		/// <summary>
		/// Returns a dialogue ready for use. it can be a new one, or it can be an old one that's already in use.
		/// </summary>
		public Dialogue GetOrCreate(DialogueManager dialogueManager, Transform parent) {
			if ( prefab == null ) throw new NullReferenceException( nameof(prefab) );
			flushList();

			Dialogue dialogue = null;
			// don't create new one
			if ( maxInstances != -1 && _dialogues.Count >= maxInstances ) {
				dialogue = maximumReachedBehaviour switch {
					MaximumReachedBehaviour.GetNewest => _dialogues[^1],
					MaximumReachedBehaviour.GetOldest => _dialogues[0],
					_ => throw new ArgumentOutOfRangeException()
				};
				dialogueManager.focusManager.SetFocusDialogue( dialogue );
				dialogue.transform.SetParent( parent );
			}
			else {
				// create one	
				dialogue = UnityEngine.Object.Instantiate( prefab, parent );
				_dialogues.Add( dialogue );
			}
			
			if ( parent == null ) {
				// preparing proper default canvas scaling
				CanvasScaler scaler = dialogue.GetComponent<CanvasScaler>();
				if ( scaler == null )
					scaler = dialogue.gameObject.AddComponent<CanvasScaler>();
				scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				scaler.referenceResolution = new Vector2( 1920, 1080 );
				
				// preparing proper default canvas render mode
				Canvas canvas = dialogue.canvas;
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
			}

			return dialogue;
		}

		/// <summary>
		/// does a <see cref="Component.TryGetComponent"/> on the prefab to check if it has the component on it
		/// </summary>
		public bool PrefabIs<T>() where T : Dialogue => prefab.TryGetComponent<T>( out _ );

		void flushList() {
			for ( int i = 0; i < _dialogues.Count; i++ ) {
				if ( _dialogues[i] == null ) {
					_dialogues.RemoveAt( i );
					i--;
				}
			}
		}
	}
}