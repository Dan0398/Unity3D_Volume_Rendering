using UnityEngine;

[System.Serializable]
public class RenderShape
{
    [field:SerializeField] public Mesh SourceMesh       { get; private set; }
    [field:SerializeField] public Transform[] OnScene   { get; private set; }
}