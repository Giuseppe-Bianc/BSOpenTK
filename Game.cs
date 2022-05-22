using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace BSOpenTK {
	public class Game : GameWindow {
		private VertexBuffer vertexBuffer;
		private IndexBuffer indexBuffer;
		private VertexArray vertexArray;
		private int shaderProgramHandle, vertexCount, indexCount;

		public int WW { get; set; }
		public int HH { get; set; }

		public Game(int width = 1111, int height = 625, string title = "Game 1") :
			base(GameWindowSettings.Default,
			new NativeWindowSettings() {
				Title = title,
				Size = new Vector2i(width, height),
				WindowBorder = WindowBorder.Fixed,
				StartVisible = false,
				StartFocused = true,
				API = ContextAPI.OpenGL,
				Profile = ContextProfile.Core,
				APIVersion = new Version(3, 3)
			}) {
			this.CenterWindow();
			WW = width;
			HH = height;
		}

		/*protected override void OnUpdateFrame(FrameEventArgs args) {
			base.OnUpdateFrame(args);
		}*/

		private static float ColorInt(int c) => c / 255.0f;

		protected override void OnResize(ResizeEventArgs e) {
			GL.Viewport(0, 0, e.Width, e.Height);
			base.OnResize(e);
		}

		protected override void OnLoad() {
			this.IsVisible = true;
			GL.ClearColor(new Color4(ColorInt(10), ColorInt(20), ColorInt(30), ColorInt(255)));

			Random rand = new();

			int windowWidth = this.ClientSize.X;
			int windowHeight = this.ClientSize.Y;

			int boxCount = 1_00;

			VertexPositionColor[] vertices = new VertexPositionColor[boxCount * 4];
			this.vertexCount = 0;

			for (int i = 0; i < boxCount; i++) {
				int w = rand.Next(32, 128);
				int h = rand.Next(32, 128);
				int x = rand.Next(0, windowWidth - w);
				int y = rand.Next(0, windowHeight - h);

				float r = (float)rand.NextDouble();
				float g = (float)rand.NextDouble();
				float b = (float)rand.NextDouble();

				vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x, y + h), new Color4(r, g, b, 1f));
				vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x + w, y + h), new Color4(r, g, b, 1f));
				vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x + w, y), new Color4(r, g, b, 1f));
				vertices[this.vertexCount++] = new VertexPositionColor(new Vector2(x, y), new Color4(r, g, b, 1f));
			}


			int[] indices = new int[boxCount * 6];
			this.indexCount = 0;
			this.vertexCount = 0;

			for (int i = 0; i < boxCount; i++) {
				indices[this.indexCount++] = 0 + this.vertexCount;
				indices[this.indexCount++] = 1 + this.vertexCount;
				indices[this.indexCount++] = 2 + this.vertexCount;
				indices[this.indexCount++] = 0 + this.vertexCount;
				indices[this.indexCount++] = 2 + this.vertexCount;
				indices[this.indexCount++] = 3 + this.vertexCount;

				this.vertexCount += 4;
			}


			this.vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, vertices.Length, true);
			this.vertexBuffer.SetData(vertices, vertices.Length);

			this.indexBuffer = new IndexBuffer(indices.Length, true);
			this.indexBuffer.SetData(indices, indices.Length);

			this.vertexArray = new VertexArray(this.vertexBuffer);





			string vertexShaderCode = System.IO.File.ReadAllText(Consts.PATH1);

			string pixelShaderCode = System.IO.File.ReadAllText(Consts.PATH2);

			int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
			GL.CompileShader(vertexShaderHandle);

			string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
			if (vertexShaderInfo != String.Empty) {
				Console.WriteLine(vertexShaderInfo);
			}

			int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
			GL.CompileShader(pixelShaderHandle);

			string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderHandle);
			if (pixelShaderInfo != String.Empty) {
				Console.WriteLine(vertexShaderInfo);
			}

			this.shaderProgramHandle = GL.CreateProgram();

			GL.AttachShader(this.shaderProgramHandle, vertexShaderHandle);
			GL.AttachShader(this.shaderProgramHandle, pixelShaderHandle);

			GL.LinkProgram(this.shaderProgramHandle);

			GL.DetachShader(this.shaderProgramHandle, vertexShaderHandle);
			GL.DetachShader(this.shaderProgramHandle, pixelShaderHandle);

			GL.DeleteShader(vertexShaderHandle);
			GL.DeleteShader(pixelShaderHandle);

			int[] viewport = new int[4];
			GL.GetInteger(GetPName.Viewport, viewport);

			GL.UseProgram(this.shaderProgramHandle);
			int viewportSizeUniformLocation = GL.GetUniformLocation(this.shaderProgramHandle, "ViewportSize");
			GL.Uniform2(viewportSizeUniformLocation, (float)viewport[2], (float)viewport[3]);
			GL.UseProgram(0);

			base.OnLoad();
		}

		protected override void OnUnload() {
			this.vertexArray?.Dispose();
			this.indexBuffer?.Dispose();
			this.vertexBuffer?.Dispose();

			GL.UseProgram(0);
			GL.DeleteProgram(this.shaderProgramHandle);

			base.OnUnload();
		}

		protected override void OnRenderFrame(FrameEventArgs args) {
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.UseProgram(this.shaderProgramHandle);
			GL.BindVertexArray(this.vertexArray.VertexArrayHandle);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.IndexBufferHandle);
			GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, 0);

			this.Context.SwapBuffers();
			base.OnRenderFrame(args);
		}
	}
}
