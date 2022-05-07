using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace BSOpenTK {
	public class Game : GameWindow {

		private int vertexBufferHandle, shaderProgramHandle, vertexArryHandle;
		public Game() : base(GameWindowSettings.Default,
				NativeWindowSettings.Default) {
			this.CenterWindow(new Vector2i(960, 576));
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
			GL.ClearColor(new Color4(ColorInt(10), ColorInt(20), ColorInt(30), ColorInt(255)));

			float[] vertices = new float[] {
				 0.0f,  0.5f, 0.0f,
				 0.5f, -0.5f, 0.0f,
				-0.5f, -0.5f, 0.0f
			};

			this.vertexBufferHandle = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			this.vertexArryHandle = GL.GenVertexArray();
			GL.BindVertexArray(this.vertexArryHandle);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
			GL.EnableVertexAttribArray(0);

			GL.BindVertexArray(0);

			string vertexShaderSources =
				@"
				#version 330 core
				
				layout (location = 0) in vec3 aPosition;

				//in vec4 aColor;

				void main(){
					gl_Position = vec4(aPosition,1f);
				}";

			string pixelShaderSources =
				@"
				#version 330 core
				
				out vec4 pixelColor;

				void main(){
					pixelColor = vec4(0.8f, 0.8f, 0.1f, 1f);
				}";

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
			GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

			this.Context.SwapBuffers();
			base.OnRenderFrame(args);
		}
	}
}
