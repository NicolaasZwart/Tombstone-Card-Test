using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Prepends speaker's name to first dialogue line but not subsequent lines
/// until switching to a new speaker. Also indents using TMPro tags and 
/// applies Dialogue Actor color to name.
/// 
/// NOTE: You *MUST* untick Dialogue Actor's Set Subtitle Color so Dialogue
/// Actor doesn't itself prepend actor name with color.
/// </summary>
public class PrependNamesAndIndent : MonoBehaviour
{
    private string currentName;

    void OnConversationStart(Transform actor)
    {
        currentName = string.Empty;
    }

    void OnConversationLine(Subtitle subtitle)
    {
        if (!string.IsNullOrEmpty(subtitle.formattedText.text))
        {
            // Indent:
            var text = subtitle.formattedText.text;
            text = $"<indent=20%>{text}</indent>";

            // If a new speaker, prepend the name:
            if (subtitle.speakerInfo.Name != currentName)
            {
                currentName = subtitle.speakerInfo.Name;
                var dialogueActor = DialogueActor.GetDialogueActorComponent(subtitle.speakerInfo.transform);
                if (dialogueActor != null)
                {
                    // Apply Dialogue Actor color to name:
                    var webcolor = Tools.ToWebColor(dialogueActor.standardDialogueUISettings.subtitleColor);
                    text = $"<color={webcolor}>{currentName}</color> - {text}";
                }
                else
                {
                    text = $"{currentName} - {text}";
                }
            }
            subtitle.formattedText.text = text;
        }
    }
}