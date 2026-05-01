using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;
using ShaderIdCommon = LeTai.Asset.TranslucentImage.ShaderId;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
struct TISPassData
{
    public RenderTargetIdentifier cameraColorTarget;
    public TranslucentImageSource blurSource;
    public IBlurAlgorithm         blurAlgorithm;
    public bool                   isPreviewing;
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public class TranslucentImageBlurRenderPass : ScriptableRenderPass
{
    private const string PROFILER_TAG = "Translucent Image Source";

    static readonly int AfterPostProcessTextureId = Shader.PropertyToID("_AfterPostProcessTexture");

    TISPassData currentPassData;
    Material    previewMaterial;

    public Material PreviewMaterial
    {
        get
        {
            if (!previewMaterial)
                previewMaterial = CoreUtils.CreateEngineMaterial("Hidden/FillCrop_UniversalRP");

            return previewMaterial;
        }
    }

    public TranslucentImageBlurRenderPass()
    {
    }

    ~TranslucentImageBlurRenderPass()
    {
        CoreUtils.Destroy(previewMaterial);
    }

    internal void Setup(TISPassData passData)
    {
        currentPassData = passData;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var cmd = CommandBufferPool.Get(PROFILER_TAG);
        var source = renderingData.cameraData.postProcessEnabled
                         ? new RenderTargetIdentifier(AfterPostProcessTextureId)
                         : currentPassData.cameraColorTarget;

        currentPassData.blurAlgorithm.Blur(cmd,
                                           source,
                                           currentPassData.blurSource.BlurRegion,
                                           currentPassData.blurSource.BlurredScreen);

        if (currentPassData.isPreviewing)
        {
            PreviewMaterial.SetVector(ShaderIdCommon.CROP_REGION,
                                      currentPassData.blurSource.BlurRegion.ToMinMaxVector());
            cmd.BlitFullscreenTriangle(currentPassData.blurSource.BlurredScreen,
                                       source,
                                       PreviewMaterial,
                                       0);
        }

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
}
