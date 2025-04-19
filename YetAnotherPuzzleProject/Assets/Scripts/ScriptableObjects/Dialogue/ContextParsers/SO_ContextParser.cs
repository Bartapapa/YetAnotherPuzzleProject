using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_ContextParser : ScriptableObject
{
    public virtual int ParseContext(StoryContext context)
    {
        Debug.LogWarning(this.name + " found no appropriate plan for the given context! Returning 0.");
        return 0;
    }
}
