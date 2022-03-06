using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DoubleSpaceSubtitles : MonoBehaviour
{
    void OnConversationLine(Subtitle subtitle)
    {
        // Adds a "newline" escape sequence to the subtitle.
        subtitle.formattedText.text += "\n";
    }
}