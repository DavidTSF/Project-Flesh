using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum INTERACT_TYPE
    {
        GENERIC = 0,
        COLLIDER = 1,
        ITEM = 2,
    }
    
    public INTERACT_TYPE interactType = INTERACT_TYPE.GENERIC;

    public string interactTip = "Con esto puedo hacer algo";
    
    public virtual bool Interact()
    {
        Debug.Log("Interacción con un objeto.");
        return true;
    }

    public void SetInteractType(INTERACT_TYPE type)
    {
        this.interactType = type;
    }
    

}