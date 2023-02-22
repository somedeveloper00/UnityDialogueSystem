using System;

namespace DialogueSystem
{
    /// <summary>
    /// used to specify the uri-line path of the dialogue. see <see cref="DialogueManager.GetOrCreateByPath"/>
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
    public class DialoguePathAttribute : Attribute
    {
        public readonly string Path;

        public DialoguePathAttribute( string path ) {
            Path = path;
        }
    }
}