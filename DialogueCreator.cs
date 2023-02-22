using System;
using System.Collections.Generic;
using System.Reflection;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
	[CreateAssetMenu( fileName = "New Dialogue Creator", menuName = "HitViking/Dialogue/Creator", order = 0 )]
	public class DialogueCreator : ScriptableObject
	{
		[SerializeField] Dialogue prefab;
		
		[PropertyTooltip("The maximum amount of instances that can simultaneously exist. -1 means unlimited")]
		[SerializeField] [Min(-1)] int maxInstances = -1;

		[DisableIf(nameof(maxInstances), 1)]
		[SerializeField] MaximumReachedBehaviour maximumReachedBehaviour;

		enum MaximumReachedBehaviour
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
				dialogue = Instantiate( prefab, parent );
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

		/// <summary>
		/// returns the path of the dialogue (prefab). if none, it returns <see cref="string.Empty"/>
		/// </summary>
		public string GetPath() {
			if ( prefab == null ) return string.Empty;
			return prefab.GetType().GetCustomAttribute<DialoguePathAttribute>()?.Path ?? string.Empty;
		}

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