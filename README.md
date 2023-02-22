# UnityDialogueSystem
General purpose Dialogue system  

![GitHub repo file count](https://img.shields.io/github/directory-file-count/somedeveloper00/UnityDialogueSystem)
![GitHub repo size](https://img.shields.io/github/repo-size/somedeveloper00/UnityDialogueSystem)
![GitHub release (latest by date)](https://img.shields.io/github/downloads/somedeveloper00/UnityDialogueSystem/release/total)
![GitHub](https://img.shields.io/github/license/somedeveloper00/UnityDialogueSystem)

## Usage
### Create new dialogue

```csharp
[DialoguePath("sample-message-dialogue")]
public class SampleMessageDialogue : Dialogue
{
    public override void InitByPath(string argvs) {
        showingMessage = argvs;
    }

    public string showingMessage;
    [SerializeField] TMP_Text msgTxt;
    
    protected override void Start() {
        base.Start();
        msgTxt.text = showingMessage;
    }
}
```

### Either open through inspector (by path) or in code

#### Through Inspector

after adding DialogueOpener component, you can assign the dialogue's path and give it arguments (if needed) and a delay

![image](https://user-images.githubusercontent.com/79690923/220678833-71178e9a-7314-4092-a3b9-2c2fb7b005cc.png)

#### Through code

you can use the current dialogue manager (last used) to create the dialogue, then assign variables to it.  
(note that in most scenarios you'd want to take advantage of the parent/child system and pass in a parent to a new dialogue)
```csharp
var dialogue = DialogueManager.Current.GetOrCreate<SampleMessageDialogue>(parent: null /*or assign a parent dialogue*/);
dialogue.showingMessage = "Hello World!";
```
