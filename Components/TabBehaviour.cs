using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DialogueSystem
{
	/// <summary>
	/// Tab Navigator for UI.  
	/// Single instance of this script per GUI.    
	/// An alternative would be to use a next/previous setting on a single GUI item, which would mean one script per InputField - not ideal.  
	/// from https://forum.unity.com/threads/tab-between-input-fields.263779/#post-2404236.
	/// </summary>
	public class TabBehaviour : MonoBehaviour
	{
		EventSystem system;

		void Start() {
			system = EventSystem.current;
		}

		void Update() {
			if ( system.currentSelectedGameObject == null || !Input.GetKeyDown( KeyCode.Tab ) )
				return;

			var current = system.currentSelectedGameObject.GetComponent<Selectable>();
			if ( current == null )
				return;

			var up = Input.GetKey( KeyCode.LeftShift ) || Input.GetKey( KeyCode.RightShift );
			var next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();

			// We are at the end or the beginning, go to either, depends on the direction we are tabbing in
			// The previous version would take the logical 0 selector, which would be the highest up in your editor hierarchy
			// But not certainly the first item on your GUI, or last for that matter
			// This code tabs in the correct visual order
			if ( next == null ) {
				next = current;

				Selectable pnext;
				if ( up )
					while ( (pnext = next.FindSelectableOnDown()) != null )
						next = pnext;
				else
					while ( (pnext = next.FindSelectableOnUp()) != null )
						next = pnext;
			}

			// Simulate Inputfield MouseClick
			var inputField = next.GetComponent<InputField>();
			if ( inputField != null ) inputField.OnPointerClick( new PointerEventData( system ) );

			// Select the next item in the tab order of our direction
			system.SetSelectedGameObject( next.gameObject );
		}
	}
}