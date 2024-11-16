# Volume-based rendering technique inside Unity3D

https://github.com/user-attachments/assets/b9d1ca5f-ea69-4f45-bcb8-650b0387255c
## What is it
This is an example of a project showing a simple and fast rendering method inside an arbitrary volume. This scene shows the clipping of a cube under a lamp, as well as particles simulating snow.
![Difference](https://github.com/user-attachments/assets/d91a745d-aed4-4028-a948-cd00ae3f298d)

## Pros
- Fast implementation. Ther is 4 draw calls + 2 draw call per volume on scene. Each draw call is easy for GPU.
- Various meshes. I've made a cone for this, but you can draw another by your needs.
- Grow-ability. You can use instancing for draw a lot of zones. You can upgrade this system as post-processing.

## Cons
- Drawn mesh must be closed.
- Multi-camera system not supports (at least for this implementation).
- Require to edit or create new shaders.

## How it works
All the work of the component is divided into two parts - creating a mask through a component on the camera from **Assets/Scripts/ApplyDepth.cs** and applying the result through self-written shaders.

ApplyDepth has a RenderShape structure that describes which volumes and in which places on the scene need to be rendered. In this case, I made a cone with a diameter of 1m and a height of 1m.
Then, using special shaders, I record the projection depth for the backface and frontface into 2 different textures.
Then I write the result to the red (backface) and green (frontface) channels of a single texture, which I access globally via **SetGlobalTexture("_ResultDepth", Result)**:
![HowItWorks](https://github.com/user-attachments/assets/c9a9dff3-5f26-473a-8372-6dd2480d5afb)

Then requires to apply result into shaders. I provided two samples:
1. [Surface with depth clamp](/Assets/Shaders/SurfaceWithDepthClamp.shader/) - sample of surface shader that drawn only inside allowed volume. 
For this i make a link to texture:
```
sampler2D _ResultDepth;// 25 line of code
```
Then i read a depth of fragment in surf function. If fragment is not inside allowed volume - don't write pixel:
```
float3 normScreenPos = IN.screenPos.xyz / IN.screenPos.w;
fixed2 depth = tex2D(_ResultDepth, normScreenPos.xy).rg;
if (depth.r > normScreenPos.z || depth.g < normScreenPos.z) discard;
```
2. [Particles shader](Assets/Shaders/Particles%20Discarted.shader) - modified build-in legacy premultiplied shader.
To access to default version of shader you need to visit [Unity3D Archive](https://unity.com/ru/releases/editor/archive), 
select your engine version, then "See all" => "Other installs" => Download "Shaders.zip" => "DefaultResourcesExtra". I've selected "Legacy Shaders/Particles/Alpha Blended Premultiply" for demonstration.

Inside shader i make a link to texture:
```
sampler2D _ResultDepth; //49 line of code
```
Then i implement logic by fragment's projection depth:
```
float2 depth = tex2D(_ResultDepth, i.vertex.xy / _ScreenParams.xy).rg;  //71 line of code
if (depth.r > i.vertex.z || depth.g < i.vertex.z)
{
  #ifdef REQUIRE_DISCARD
    discard;
  #else
    i.color.a *= 0.05;
  #endif
}
```
**REQUIRE_DISCARD** is multi-compile tag, that controls from "Require discard" toggle on material
```
[Toggle(REQUIRE_DISCARD)] _RequireDiscard("Require discard", Float) = 0  //7 line of code
/**/
#pragma multi_compile __ REQUIRE_DISCARD  //24 line of code
```
