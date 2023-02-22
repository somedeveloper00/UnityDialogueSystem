using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(GraphicRaycaster))]
	[DeclareFoldoutGroup("init", Title = "Init")]
	public abstract class Dialogue : MonoBehaviour
	{
		public event Action onClose;

		public Canvas canvas { get; private set; }
		public CanvasGroup canvasGroup { get; private set; }
		public GraphicRaycaster canvasRaycaster { get; private set; }

		Dialogue Parent;
		List<Dialogue> Children = new ( 1 );


		public int order {
			get {
				return canvas.sortingOrder;
			}
			set {
				canvas.overrideSorting = true;
				canvas.sortingOrder = value;
			}
		}

	
		/// <summary>
		/// called when the dialogue is opened through a path string
		/// </summary>
		public virtual void InitByPath(string path) { }

		public void SetInteractable(bool value) {
			canvasGroup.interactable = value;
		}

		/// <summary>
		/// destroys the dialogue and all it's children
		/// </summary>
		public void Close() {
			for ( int i = 0; i < Children.Count; i++ ) Children[i].Close();
			Destroy( gameObject );
		}

		/// <summary>
		/// returns the topmost parent of this dialogue
		/// </summary>
		public Dialogue GetTopmostParent() {
			if (Parent == null) return this;
			return Parent.GetTopmostParent();
		}
		
		/// <summary>
		/// makes the other dialogue a child of this one. (removes any previous parent link to the child)
		/// </summary>
		public void AddChild(Dialogue child) => Connect( this, child );

		/// <summary>
		/// removes the child from this dialogue's children list
		/// </summary>
		public void RemoveChild(Dialogue child) => Disconnect( this, child );

		/// <summary>
		/// sets the parent of this dialogue. (removes any previous parent link to this dialogue)
		/// </summary>
		public void SetParent(Dialogue parent) => Connect( parent, this );

		static void Connect(Dialogue parent, Dialogue child) {
			if ( ReferenceEquals( parent, child ) ) return;
			if ( parent == null || child == null ) return;
			parent.Children.Add( child );
			child.Parent = parent;
		}
		
		static void Disconnect(Dialogue parent, Dialogue child) {
			if ( ReferenceEquals( parent, child ) ) return;
			if ( parent == null || child == null ) return;
			parent.Children.Remove( child );
			child.Parent = null;
		}

#region Helper Methods

		/// <summary>
		/// awaits the closing of the dialogue
		/// </summary>
		public async Task AwaitClose() {
			var tc = new TaskCompletionSource<bool>();
			onClose += () => tc.SetResult(true);
			await tc.Task;
		}

#endregion

		void Awake() {
			Debug.Log( $"dialogue {GetType().Name} {logColor( "opened", Color.green )}" );
			getComponents();
			// not allowing children to use Awake, so the callers (those who create dialogue) can add their parameters
			// and tweak the dialogue before it's functionalities start (earliest one is at Start)
		}

		protected virtual void OnEnable() {
			// preserving it in case we'll need it
		}

		protected virtual void Start() {
			DialogueManager.Current.ResolveEventSystem();
			DialogueManager.Current.focusManager.AddDialogue( this );
		}

		protected virtual void OnDestroy() {
			Debug.Log( $"dialogue {GetType().Name} {logColor( "closed", Color.blue )}" );
			if ( Parent != null ) Parent.Children.Remove( this );
			DialogueManager.Current.focusManager.RemoveDialogue( this );
			onClose?.Invoke();
		}

		void getComponents() {
			canvas = GetComponent<Canvas>();
			canvasGroup = GetComponent<CanvasGroup>();
			canvasRaycaster = GetComponent<GraphicRaycaster>();
		}

		string logColor(string text, Color color) => $"<color=#{toHexString( color )}>{text}</color>";
		string toHexString(Color color) =>
			((byte)(color.r * 255)).ToString("X2") +
			((byte)(color.g * 255)).ToString("X2") +
			((byte)(color.b * 255)).ToString("X2") +
			((byte)(color.a * 255)).ToString("X2");
	}
}