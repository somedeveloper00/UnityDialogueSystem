using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DialogueSystem
{
    /// <summary>
    /// a singleton dialogue instancing manager system that handles the creation and linking of
    /// dialogues
    /// </summary>
    [CreateAssetMenu( fileName = "DialogueInventory", menuName = "HitViking/Dialogue/Dialogue Inventory", order = 0 )]
    public class DialogueManager : ScriptableObject 
	{
        public static DialogueManager Current { get; private set; }

        public DialogueFocusManager focusManager;
        
        public bool closeOnEscape = true;

        [SerializeField] internal DialogueCreator[] creators;

        static EventSystem _eventSystem;   

        void OnEnable() {
			Current = this;
            runEscapeChecks();
        }

        /// <summary>
        /// Gets the proper dialogue from the dialogue list and returns it.
        /// </summary>
        public T GetOrCreate<T>(Transform parentTransform = null, Dialogue parent = null) where T : Dialogue {
	        Current = this;
	        for ( int i = 0; i < creators.Length; i++ ) {
		        if ( creators[i].PrefabIs<T>() ) {
			        if ( parentTransform == null ) parentTransform = getParent();
			        var dialogue = creators[i].GetOrCreate( this, parentTransform ) as T;
			        if ( parent != null ) dialogue!.SetParent( parent );
			        return dialogue;
		        }
	        }

	        throw new Exception( $"Dialogue {typeof(T).Name} not found. make sure to add it's dialogue creator Scriptable " +
	                             $"Object to this Dialogue Manager asset: {name}" );
        }

        /// <summary>
		/// gets the proper dialogue from the dialogue list according to the given path,
		/// and then initializes the dialogue with the given argsv
		/// </summary>
		public Dialogue GetOrCreateByPath(string dialoguePath, string argvs, Transform parent = null) {
			Current = this;
			for ( int i = 0; i < creators.Length; i++ ) {
				if ( creators[i].prefab == null ) continue;
				if ( GetPath(creators[i].prefab.GetType()) == dialoguePath ) {
					if ( parent == null ) parent = getParent();
					var d = creators[i].GetOrCreate( this, parent );
					d.InitByPath( argvs );
					return d;
				}
			}
			
			throw new Exception( $"dialogue path {dialoguePath} not found" );
		}

		/// <summary>
		/// To resolve the issue where multiple scenes might have multiple Event Systems, we instead
		/// make a single event system and make it be the only event system used
		/// </summary>
		public void ResolveEventSystem() {
			if ( EventSystem.current != _eventSystem ) {
				Destroy( EventSystem.current.gameObject );
			}
			if ( _eventSystem == null ) {
				var obj = new GameObject( "Event System" );
				_eventSystem = obj.AddComponent<EventSystem>();
				obj.AddComponent<StandaloneInputModule>();
				obj.AddComponent<TabBehaviour>(); // support tab
				DontDestroyOnLoad( obj );
			}
			EventSystem.current = _eventSystem;
		}

		// ReSharper disable once FunctionNeverReturns
		async void runEscapeChecks() {
			while ( true ) {
				await Task.Yield();
				if ( closeOnEscape ) {
					if ( Input.GetKeyDown( KeyCode.Escape ) ) {
						if ( focusManager == null ) continue;
						var focusedDialogue = focusManager.GetFocusedDialogue();
						if ( focusedDialogue == null ) continue;
						if ( focusedDialogue is IEscapableDialogue escapableDialogue && 
						     !await escapableDialogue.CanEscape ) continue;
						focusedDialogue.Close();
					}
				}

			}
		}

		Transform getParent() {
			var d  = focusManager.GetFocusedDialogue();
			if ( d != null ) return d.transform.parent;
			return null;
		}

		/// <summary>
		/// returns the path of the given dialogue type
		/// </summary>
		public static string GetPath(Type dialogueType) {
			if ( dialogueType.IsAbstract
			     || !dialogueType.IsSubclassOf( typeof(Dialogue) ) )
				return string.Empty;
			return dialogueType.GetCustomAttribute<DialoguePathAttribute>()?.Path ?? dialogueType.FullName;
		}
	}
}