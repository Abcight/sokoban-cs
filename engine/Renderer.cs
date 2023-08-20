using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Sokoban.assets;

namespace Sokoban.Engine
{
    /// <summary>
    /// Receives draw jobs and dispatches them into adequate OpenGL drawcalls
    /// </summary>
    public class Renderer
    {
        /// <summary>
        /// A singleton instance of the renderer.
        /// </summary>
        public static Renderer Instance = new();

        /// <summary>
        /// Dictates how much the screen should be cut off from one side.
        /// </summary>
        public int ScreenCutout;
        
        /// <summary>
        /// Dictates which side of the screen to cut off from.
        /// </summary>
        public int ScreenCutoutDirection;
        
		// all of the shaders used by the established render pipeline
        private readonly Shader deferredPassShader;
        private readonly Shader defaultShader;
        private readonly Shader lightShader;
        private readonly Shader lightCutoffShader;
        private readonly Texture defaultTexture;
        private readonly Quad quad;
        private Texture lastBoundTexture;

        // internal opengl deferred rendering pipeline buffers
        private int gBuffer;
        private int gColor;
        private int gDepth;
        private int gLight;
        private int gLightCut;
        private int gPosition;

        private readonly List<RenderItem> renderQueue = new();
        private readonly List<RenderItem> uiQueue = new();
        private readonly List<Vector2> lightQueue = new();

        private Renderer()
        {
            quad = new Quad();
            
            deferredPassShader = new Shader(
				AssetCache.LoadString(AssetLut.SHADER_COMPOSITE_VERTEX),
                AssetCache.LoadString(AssetLut.SHADER_COMPOSITE_FRAGMENT)
			);
            
            defaultShader = new Shader(
				AssetCache.LoadString(AssetLut.SHADER_QUAD_VERTEX),
                AssetCache.LoadString(AssetLut.SHADER_QUAD_FRAGMENT)
			);
            
            lightShader = new Shader(
				AssetCache.LoadString(AssetLut.SHADER_QUAD_VERTEX),
                AssetCache.LoadString(AssetLut.SHADER_QUAD_LIGHT_FRAGMENT)
			);
            
            lightCutoffShader = new Shader(
				AssetCache.LoadString(AssetLut.SHADER_COMPOSITE_VERTEX),
                AssetCache.LoadString(AssetLut.SHADER_COMPOSITE_LIGHTCUT_FRAGMENT)
			);
            
            defaultTexture = AssetCache.LoadTexture(AssetLut.TEXTURE_ATLAS);

            gBuffer = GL.GenFramebuffer();
            genBuffers();
        }

        /// <summary>
        /// A function to convert internally queued render jobs into OpenGL drawcalls.
        /// Each call will result in a new frame being rendered into the OpenGL backbuffer.
        /// </summary>
        public void Render()
        {
            // Clear the color from the backbuffer
            GL.ClearColor(Color4.Black);

            // Bind the deferred framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Calculate the projection matrix
            Matrix4 projection = Matrix4.CreateOrthographic(320, 180, 0, 100);

			renderTiles(projection);
			renderLight();
			renderComposite(projection);
			renderUi(projection);

            renderQueue.Clear();
            lightQueue.Clear();
            uiQueue.Clear();
        }

		private void renderTiles(Matrix4 projection)
		{
			// Drawing will be done using Quads in a 320x180 viewport
			quad.Use();
			GL.Viewport(0, 0, 320, 180);

			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Clear(ClearBufferMask.DepthBufferBit);

			// Render partially opaque geometry back to front
			foreach (var job in renderQueue.OrderBy((x => x.Position.Z)))
			{
				var matrix = Matrix4.CreateScale(job.Size.X * 0.5f, job.Size.Y * 0.5f, 1) *
							 Matrix4.CreateTranslation(job.Position.X, job.Position.Y, job.Position.Z);
				matrix *= projection;

				Shader targetShader = job.Shader;
				if (job.Shader == null)
					targetShader = defaultShader;

				targetShader.Use();
				targetShader.SetUniform("shadowCaster", job.ShadowCaster);
				targetShader.SetUniform("model", matrix);
				targetShader.SetUniform("offset", job.Offset);

				targetShader.SetUniform("texture0", 0);
				targetShader.SetUniform("texture1", 1);
				Texture targetTex = job.Texture;
				if (targetTex == null)
					targetTex = defaultTexture;

				bindTexturePrimary(targetTex);

				quad.Draw();
			}

			// Render into the light buffer
			GL.Clear(ClearBufferMask.DepthBufferBit);
			foreach (var light in lightQueue)
			{
				var matrix = Matrix4.CreateScale(16 * 0.5f, 16 * 0.5f, 1) *
							 Matrix4.CreateTranslation(light.X, light.Y + 11, -9);
				matrix *= projection;

				lightShader.Use();
				lightShader.SetUniform("model", matrix);
				lightShader.SetUniform("offset", new Vector2(0, 1));
				bindTexturePrimary(defaultTexture);

				quad.Draw();
			}
		}

		private void renderLight() {
			// Blending must be disabled for the light-cutoff pass
			GL.Disable(EnableCap.Blend);
			GL.Clear(ClearBufferMask.DepthBufferBit);

			lightCutoffShader.Use();
			lightCutoffShader.SetUniform("gPosition", 0);
			lightCutoffShader.SetUniform("gLight", 1);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, gPosition);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, gLight);

			quad.Draw();
		}

		private void renderComposite(Matrix4 projection)
		{
			// Upscaling the native 320x180 res into the game window
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
			GL.Viewport(0, 0, Game.Instance.Size.X, Game.Instance.Size.Y);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			deferredPassShader.Use();
			deferredPassShader.SetUniform("advancement", ScreenCutout);
			deferredPassShader.SetUniform("reverse", ScreenCutoutDirection);
			deferredPassShader.SetUniform("blurAmount", Time.Pause ? 0.25f : 0);
			deferredPassShader.SetUniform("gPosition", 0);
			deferredPassShader.SetUniform("gLight", 1);
			deferredPassShader.SetUniform("gColor", 2);
			deferredPassShader.SetUniform("gLightCut", 3);
			deferredPassShader.SetUniform("projection", projection);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, gColor);
			if (Time.Pause)
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
					(int)TextureMinFilter.Linear);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
					(int)TextureMinFilter.Linear);
			}
			else
			{
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
					(int)TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
					(int)TextureMinFilter.Nearest);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, gPosition);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, gLight);

			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, gColor);

			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, gLightCut);

			lastBoundTexture = null;

			quad.Draw();
		}

		private void renderUi(Matrix4 projection)
		{
			// Enable blending for the UI
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GL.Clear(ClearBufferMask.DepthBufferBit);

			// Iterate through internally queued UI draw jobs and convert them into OpenGL drawcalls
			foreach (var job in uiQueue.OrderBy((x => x.Position.Z)))
			{
				var matrix = Matrix4.CreateScale(job.Size.X * 0.5f, job.Size.Y * 0.5f, 1) *
							 Matrix4.CreateTranslation(job.Position.X, job.Position.Y, job.Position.Z);
				matrix *= projection;

				Shader targetShader = job.Shader;
				if (job.Shader == null)
					targetShader = defaultShader;
				targetShader.Use();

				FontSettings targetSettings = job.FontSettings;
				if (job.FontSettings == null)
					targetSettings = FontSettings.Default;
				targetShader.SetUniform("color", targetSettings.Color);

				targetShader.SetUniform("model", matrix);
				targetShader.SetUniform("offset", job.Offset);
				targetShader.SetUniform("tileSize", job.TileSize);
				targetShader.SetUniform("texture0", 0);

				Texture targetTex = job.Texture;
				if (targetTex == null)
					targetTex = defaultTexture;
				bindTexturePrimary(targetTex);

				quad.Draw();
			}
		}

        /// <summary>
        /// Queues a render job into the internal render queue.
        /// </summary>
        public void Queue(RenderItem item) => renderQueue.Add(item);

        /// <summary>
        /// Queues a UI render job into the internal render queue.
        /// </summary>
        public void QueueUI(RenderItem item) => uiQueue.Add(item);

        /// <summary>
        /// Queues a light render job into the internal render queue.
        /// </summary>
        public void QueueLight(Vector2 pos) => lightQueue.Add(pos);

        /// <summary>
        /// Helper function for binding texture but only if it hasn't been bound already
        /// </summary>
        /// <param name="texture">texture to bind</param>
        private void bindTexturePrimary(Texture texture)
        {
            if (lastBoundTexture != texture)
            {
                texture.Use();
                lastBoundTexture = texture;
            }
        }
        
        /// <summary>
        /// Internal function used to make sure the OpenGL gBuffer is initialized and has the correct native size
        /// </summary>
        private void genBuffers()
        {
            const int width = 320;
            const int height = 180;
            
            // gbuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, gBuffer);

            // position color buffer + heightmap
            gPosition = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gPosition);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba,
                PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, gPosition, 0);

            // normal color buffer
            gLight = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gLight);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb,
                PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1,
                TextureTarget.Texture2D, gLight, 0);

            // color buffer
            gColor = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gColor);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2,
                TextureTarget.Texture2D, gColor, 0);
            
            // color buffer
            gLightCut = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, gLightCut);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMinFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3,
                TextureTarget.Texture2D, gLightCut, 0);

            GL.DrawBuffers(4,
                new[]
                {
                    DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1,
                    DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3
                }
            );

            // depth buffer
            gDepth = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, gDepth);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, gDepth);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }

    public struct RenderItem
    {
        public Vector2 Size;
        public Vector3 Position;
        public Vector2 Offset;
        public Vector2 TileSize;
        public Texture Texture;
        public Shader Shader;
        public FontSettings FontSettings;
        public int ShadowCaster;
    }
}