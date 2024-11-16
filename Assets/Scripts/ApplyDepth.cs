using UnityEngine.Rendering;
using UnityEngine;

public class ApplyDepth : MonoBehaviour
{
    static readonly int ResultDepthHash = Shader.PropertyToID("_ResultDepth");
    
    [SerializeField] Camera drawCamera;
    [SerializeField] Material BackFaceMaterial, FrontFaceMaterial, ResultMaterial;
    [SerializeField] RenderShape[] Shapes;
    [SerializeField] RenderTexture BackFace, FrontFace, Result;

    void Start()
    {
        var width = Screen.width;
        var height = Screen.height;
        
        BackFace = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat);
        BackFace.Create();
        
        FrontFace = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat);
        FrontFace.Create();
        
        Result = new RenderTexture(width, height, 0, RenderTextureFormat.RGFloat);
        Result.Create();
        
        ResultMaterial.SetTexture("_Backface", BackFace);
        ResultMaterial.SetTexture("_Frontface", FrontFace);
    }

    void Update()
    {
        TryChangeResolution();
        var cmd = new CommandBuffer();
        cmd.SetProjectionMatrix(drawCamera.projectionMatrix);
        cmd.SetViewMatrix(drawCamera.worldToCameraMatrix);
        DrawBackFaces(cmd);
        DrawFrontFaces(cmd);
        DrawResult(cmd);
        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Dispose();
    }
    
    void TryChangeResolution()
    {
        if (Screen.width == BackFace.width && Screen.height == BackFace.height) return;
        
        BackFace.Release();
        FrontFace.Release();
        Result.Release();
        
        var width = Screen.width;
        var height = Screen.height;
        
        BackFace.width = width;
        BackFace.height = height;
        BackFace.Create();
        
        FrontFace.width = width;
        FrontFace.height = height;
        FrontFace.Create();
        
        Result.width = width;
        Result.height = height;
        Result.Create();
    }
    
    void DrawBackFaces(CommandBuffer cmd)
    {
        cmd.SetRenderTarget(BackFace);
        cmd.ClearRenderTarget(true, true, Color.clear);
        foreach(var shape in Shapes)
        {
            foreach(var point in shape.OnScene)
            {
                cmd.DrawMesh(shape.SourceMesh, point.localToWorldMatrix, BackFaceMaterial);
            }
        }
    }
    
    void DrawFrontFaces(CommandBuffer cmd)
    {
        cmd.SetRenderTarget(FrontFace);
        cmd.ClearRenderTarget(true, true, Color.clear);
        foreach(var shape in Shapes)
        {
            foreach(var point in shape.OnScene)
            {
                cmd.DrawMesh(shape.SourceMesh, point.localToWorldMatrix, FrontFaceMaterial);
            }
        }
    }
    
    void DrawResult(CommandBuffer cmd)
    {
        cmd.SetRenderTarget(Result);
        cmd.ClearRenderTarget(true, true, Color.clear);
        cmd.Blit(null, Result, ResultMaterial);
        cmd.SetGlobalTexture(ResultDepthHash, Result);
    }
}