using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
	[CreateAssetMenu( fileName = "DialogueFocusManager", menuName = "Dialogue/Focus Manager", order = 0 )]
	public class DialogueFocusManager : ScriptableObject
	{      
		public int startingSortingLayer = 5;
		public int sortingLayerIncrement = 1;
		
		List<Dialogue> dialogues = new List<Dialogue>(16);

		/// <summary>
		/// Adds the dialogue to the list of dialogues.
		/// </summary>
		public void AddDialogue(Dialogue dialogue) {
			for ( int i = 0; i < dialogues.Count; i++ ) {
				if (dialogues[i] == dialogue) {
					Debug.LogWarning( $"dialogue {dialogue.name} is already added." );
					return;
				}
			}
			
			dialogues.Add( dialogue );
			Flush();
		}
		
		/// <summary>
		/// Removes the dialogue from the dialogues list
		/// </summary>
		public void RemoveDialogue(Dialogue dialogue) {
			if ( dialogues.Remove( dialogue ) )
				Flush();
		}

		/// <summary>
		/// Sets the dialogue as the topmost dialogue.
		/// </summary>
		public void SetFocusDialogue(Dialogue dialogue) {
			setDialogueToLast();
			Flush();

			void setDialogueToLast() {
				for ( int i = 0; i < dialogues.Count; i++ ) {
					if ( dialogues[i] == dialogue ) {
						// bobble the dialogue to the end of the list
						for ( int j = i + 1; j < dialogues.Count; j++ ) {
							(dialogues[i], dialogues[j]) = (dialogues[j], dialogues[i]);
							i = j;
						}
						return;
					}
				}

				// if not found, add
				Debug.LogWarning( $"Dialogue {dialogue.name} was not added to dialogues. adding it now." );
				AddDialogue( dialogue );
			}
		}

		/// <summary>
		/// Returns whether or not the dialogue is the top-most dialogue, a.k.a. focused
		/// </summary>
		public bool IsDialogueFocused(Dialogue dialogue) {
			for ( int i = 0; i < dialogues.Count; i++ ) {
				if ( dialogues[i] == dialogue ) return i == dialogues.Count - 1;
			}

			return false;
		}

		public Dialogue GetFocusedDialogue() {
			if ( dialogues.Count == 0 ) return null;
			return dialogues[^1];
		}

		/// <summary>
		/// cleanups the list
		/// </summary>
		public void Flush() {
			// validity check
			for ( int i = 0; i < dialogues.Count; i++ ) {
				if ( dialogues[i] == null ) {
					dialogues.RemoveAt( i );
					i--;
				}
			}

			// sort orders
			for ( int i = 0; i < dialogues.Count; i++ ) {
				dialogues[i].order = startingSortingLayer + sortingLayerIncrement * i;
				dialogues[i].SetInteractable( i == dialogues.Count - 1 );
			}
		}
	}
}