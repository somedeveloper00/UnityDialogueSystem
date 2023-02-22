using System.Threading.Tasks;

namespace DialogueSystem
{
	/// <summary>
	/// marks the dialogue as being able to close by Escape input methods
	/// </summary>
	public interface IEscapableDialogue
	{
		/// <summary>
		///		whether or not it can escape
		/// </summary>
		public Task<bool> CanEscape => Task.FromResult( true );
	}
}