using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace BSOpenTK {
	public class Game : GameWindow {
		private int vertexBufferHandle, indexBufferHandle, shaderProgramHandle, vertexArryHandle;

		public int WW { get; set; }
		public int HH { get; set; }

		public Game(int whidt = 1111, int heigth = 625, String title = "Game 1") :
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
			WW = whidt;
			HH = heigth;
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
			float x = 240, y = 228, w = 512, h = 256;

			//float[] vertices = new float[] {
			//	    x, y + h, 1f, 0f, 0f, 1f,
			//	x + w, y + h, 0f, 1f, 0f, 1f,
			//	x + w,     y, 0f, 0f, 1f, 1f,
			//	    x,     y, 1f, 1f, 0f, 1f
			//};

			VertexPosColor[] vertices = new VertexPosColor[] {
				new VertexPosColor(new Vector2(x, y + h), new Color4(1f, 0f, 0f, 1f)),
				new VertexPosColor(new Vector2(x + w, y + h), new Color4(0f, 1f, 0f, 1f)),
				new VertexPosColor(new Vector2(x + w,y), new Color4(0f, 0f, 1f, 1f)),
				new VertexPosColor(new Vector2(x,y), new Color4(1f, 1f, 0f, 1f)),
			};

			int[] indices = new int[] { 
				0,1,2,
				0,2,3
			};

			int vertBytes = VertexPosColor.VertexInfo.SzInBytes;

			this.vertexBufferHandle = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * vertBytes, vertices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			this.indexBufferHandle = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			this.vertexArryHandle = GL.GenVertexArray();
			GL.BindVertexArray(this.vertexArryHandle);

			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);

			VertexAtrib attr0 = VertexPosColor.VertexInfo.VertexAtribs[0];
			VertexAtrib attr1 = VertexPosColor.VertexInfo.VertexAtribs[1];

			GL.VertexAttribPointer(attr0.Ind, attr0.CompCount, VertexAttribPointerType.Float, false, vertBytes, attr0.Offset);
			GL.VertexAttribPointer(attr1.Ind, attr1.CompCount, VertexAttribPointerType.Float, false, vertBytes, attr1.Offset);

			GL.EnableVertexAttribArray(attr0.Ind);
			GL.EnableVertexAttribArray(attr1.Ind);
			GL.BindVertexArray(0);

			string vertexShaderSources = System.IO.File.ReadAllText(Consts.PATH1);
			string pixelShaderSources = System.IO.File.ReadAllText(Consts.PATH2);

			int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShaderHandle, vertexShaderSources);
			GL.CompileShader(vertexShaderHandle);

			String vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
			if(vertexShaderInfo != String.Empty) {
				Console.WriteLine(vertexShaderInfo);
			}

			int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(pixelShaderHandle, pixelShaderSources);
			GL.CompileShader(pixelShaderHandle);

			String pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderHandle);
			if (pixelShaderInfo != String.Empty) {
				Console.WriteLine(pixelShaderInfo);
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
			Vector2 viewportv = new((float)viewport[2], (float)viewport[3]);

			GL.UseProgram(this.shaderProgramHandle);
			int viewportSizeUnifornLocation = GL.GetUniformLocation(this.shaderProgramHandle, "ViewportSize");
			GL.Uniform2(viewportSizeUnifornLocation, viewportv);
			GL.UseProgram(0);

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
