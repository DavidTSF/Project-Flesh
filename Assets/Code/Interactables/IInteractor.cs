


using UnityEngine;

public interface IInteractor
{
    Transform GetTransform();
    string GetInteractorName();
    bool HasKey(string keyId); 
}