using UnityEngine;
using UnityEngine.Video;
using Unity.Collections;
using Unity.Jobs;

public class RTTVGI : MonoBehaviour
{
    VideoPlayer videoPlayer;
    MeshRenderer meshRenderer;
    public int intensityMultiplier = 1;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        NativeArray<float> perceivedBrightnessResult = new NativeArray<float>(1, Allocator.Temp);
        NativeArray<Color> colorResult = new NativeArray<Color>(1, Allocator.Temp);
        MyJob jobData = new MyJob();
        jobData.averageColor = AverageColorFromTexture(toTexture2D(videoPlayer.targetTexture));
        jobData.perceivedBrightness = perceivedBrightnessResult;
        jobData.color = colorResult;
        JobHandle handle = jobData.Schedule();
        handle.Complete();
        DynamicGI.SetEmissive(meshRenderer, colorResult[0] * perceivedBrightnessResult[0] * intensityMultiplier);
        perceivedBrightnessResult.Dispose();

    }
    public struct MyJob : IJob
    {
        public Color32 averageColor;
        public NativeArray<float> perceivedBrightness;
        public NativeArray<Color> color;

        public void Execute()
        {
            float luminance = (0.2126f * sRGBtoLin(averageColor.r / 255f)) + (0.7152f * sRGBtoLin(averageColor.g / 255f)) + (0.0722f * sRGBtoLin(averageColor.b / 255f));
            perceivedBrightness[0] = YtoLstar(luminance).Remap(0f, 100f, 0f, 1f);
            color[0] = new Color(averageColor.r / 255f, averageColor.g / 255f, averageColor.b / 255f);
        }
        float sRGBtoLin(float colorChannel)
        {
            if (colorChannel <= 0.04045f)
            {
                return colorChannel / 12.92f;
            }
            else
            {
                return Mathf.Pow(((colorChannel + 0.055f) / 1.055f), 2.4f);
            }
        }
        float YtoLstar(float luminance)
        {
            if (luminance <= (216f / 24389f))
            {
                return luminance * (24389f / 27f);
            }
            else
            {
                return Mathf.Pow(luminance, (1f / 3f)) * 116f - 16f;
            }
        }
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    Color32 AverageColorFromTexture(Texture2D tex)
    {
        Color32[] texColors = tex.GetPixels32();

        int total = texColors.Length;

        float r = 0;
        float g = 0;
        float b = 0;

        for (int i = 0; i < total; i++)
        {

            r += texColors[i].r;

            g += texColors[i].g;

            b += texColors[i].b;

        }

        return new Color32((byte)(r / total), (byte)(g / total), (byte)(b / total), 0);
    }
}
public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
