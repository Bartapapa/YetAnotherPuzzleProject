using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "YetAnotherPuzzleProject/Dialogue/ContextParser_Test", fileName = "Dialogue_ContextParser_Test")]
public class SO_ContextParser_Test : SO_ContextParser
{
    public override int ParseContext(StoryContext context)
    {
        if (context == null)
        {
            Debug.LogWarning("No found context! Returning 0.");
            return 0;
        }
        else
        {
            Debug.LogWarning("Context found!");
        }

        if (context.GetQuestCurrentStep(0) == 1)
        {
            //Has already spoken to the test character. Use dialogue 1.
            return 1;
        }
        else
        {
            base.ParseContext(context);
            return 0;
        }
    }
}
