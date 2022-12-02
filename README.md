# rttvgi
Realtime GI Refreshing For Dynamic Light Intensities In Unity

Add the script to the GameObject with the VideoPlayer component.
You need to create and assign a RenderTexture for your VideoPlayer.

# How does it work?
1. Get the RenderTexture output from VideoPlayer.
2. Convert the Render Texture to Texture2D.
3. Calculate luminance, then calculate perceived brightness from luminance.
4. Calculate average color of Texture2D for later.
5. Remap perceived brightness between 0f-1f instead of 0-100.
6. Update the material which is affecting the scene with its emissive color. Pass the multiplication of average color and remapped perceived brightness to DynamicGI.SetEmissive function. You can change intensity multiplier for more intense effect.

# Drawbacks
I tried to calculate brightness in a job. There may be some improvements, for example calculating average color with IJobParallelFor. The biggest problem is converting a RenderTexture to Texture2D is computationally heavy. You may calculate on GPU with compute shaders or you can create a downscaled Texture2D in the conversion section which I tried and got decent results.

# References
https://stackoverflow.com/a/44265122

https://stackoverflow.com/a/56678483

https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/#post-800377

https://docs.unity3d.com/ScriptReference/DynamicGI.SetEmissive.html
