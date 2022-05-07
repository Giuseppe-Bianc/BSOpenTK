using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace BSOpenTK {
	public class Game : GameWindow {
		private int vertexBufferHandle, indexBufferHandle, shaderProgramHandle, vertexArryHandle;
		public Game(int whidt = 960, int heigth = 576, String title = "Game 1") :
			base(GameWindowSettings.Default,
			new NativeWindowSettings() {
				Title = title,
				Size = new Vector2i(whidt, heigth),
				WindowBorder = WindowBorder.Fixed,
				StartVisible = false,
				StartFocused = true,
				API = ContextAPI.OpenGL,
				Profile = ContextProfile.Core,
				APIVersion = new Version(3, 3)
			}) {
			this.CenterWindow();
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

			float[] vertices = new float[] {
				-.5f,  .5f, 0f, 1f, 0f, 0f, 1f,
				 .5f,  .5f, 0f, 0f, 1f, 0f, 1f,
				 .5f, -.5f, 0f, 0f, 0f, 1f, 1f,
				-.5f, -.5f, 0f, 1f, 1f, 0f, 1f
			};

			int[] indices = new int[] { 
				0,1,2,
				0,2,3
			};

			this.vertexBufferHandle = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			this.indexBufferHandle = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			this.vertexArryHandle = GL.GenVertexArray();
			GL.BindVertexArray(this.vertexArryHandle);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
			GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.BindVertexArray(0);

			string vertexShaderSources = System.IO.File.ReadAllText(Consts.PATH1);
			string pixelShaderSources = System.IO.File.ReadAllText(Consts.PATH2);

			int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShaderHandle, vertexShaderSources);
			GL.CompileShader(vertexShaderHandle);

			int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(pixelShaderHandle, pixelShaderSources);
			GL.CompileShader(pixelShaderHandle);

			this.shaderProgramHandle = GL.CreateProgram();

			GL.AttachShader(this.shaderProgramHandle, vertexShaderHandle);
			GL.AttachShader(this.shaderProgramHandle, pixelShaderHandle);

			GL.LinkProgram(this.shaderProgramHandle);

			GL.DetachShader(this.shaderProgramHandle, vertexShaderHandle);
			GL.DetachShader(this.shaderProgramHandle, pixelShaderHandle);

			GL.DeleteShader(vertexShaderHandle);
			GL.DeleteShader(pixelShaderHandle);

			base.OnLoad();
		}

		protected override void OnUnload() {
			GL.BindVertexArray(0);
			GL.DeleteVertexArray(this.vertexArryHandle);

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.DeleteBuffer(this.indexBufferHandle);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.DeleteBuffer(this.vertexBufferHandle);

			GL.UseProgram(0);
			GL.DeleteProgram(this.shaderProgramHandle);

			base.OnUnload();
		}

		protected override void OnRenderFrame(FrameEventArgs args) {
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.UseProgram(this.shaderProgramHandle);
			GL.BindVertexArray(this.vertexArryHandle);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
			GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt,0);

			this.Context.SwapBuffers();
			base.OnRenderFrame(args);
		}
	}
}
