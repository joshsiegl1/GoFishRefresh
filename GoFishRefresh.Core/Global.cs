using Microsoft.Xna.Framework;
using System;
public static class Global
{
    public const float HandsLayerDepth = 1.0f;
    public const float ShowCardButtonLayerDepth = 1.0f;
    public const float BackgroundLayerDepth = 0.95f;
    public const float DisplayCardLayerDepth = 0.9f;
    public static Matrix createTransformMatrix(GraphicsDeviceManager graphics)
    {
        // Virtual resolution - the resolution the game is designed for
        int virtualWidth = 1920;
        int virtualHeight = 1080;
        // Get the actual backbuffer size
        int screenWidth = graphics.GraphicsDevice.Viewport.Width;
        int screenHeight = graphics.GraphicsDevice.Viewport.Height;
        // Calculate scaling
        float scaleX = (float)screenWidth / virtualWidth;
        float scaleY = (float)screenHeight / virtualHeight;
        // Option A: uniform scale (keep aspect ratio, may letterbox)
        float scale = Math.Min(scaleX, scaleY);
        // Create transform matrix
        Matrix scaleMatrix = Matrix.CreateScale(scale, scale, 1f);
        return scaleMatrix;
    }
}