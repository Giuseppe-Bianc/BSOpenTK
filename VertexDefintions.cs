using System;
using OpenTK.Mathematics;

namespace BSOpenTK {
	public readonly struct VertexAttribute {
		public readonly string Name;
		public readonly int Index, ComponentCount, Offset;

		/// <summary> Initializes a new instance of the <see cref="VertexAttribute"/> class. </summary>
		/// <param name="name">The name.</param>
		/// <param name="index">The index.</param>
		/// <param name="componentCount">The component count.</param>
		/// <param name="offset">The offset.</param>
		public VertexAttribute(string name, int index, int componentCount, int offset) {
			this.Name = name;
			this.Index = index;
			this.ComponentCount = componentCount;
			this.Offset = offset;
		}
	}

	/// <summary> The vertex info. </summary>
	public sealed class VertexInfo {
		public readonly Type Type;
		public readonly int SizeInBytes;
		public readonly VertexAttribute[] VertexAttributes;

		/// <summary>Initializes a new instance of the <see cref="VertexInfo"/> class.</summary>
		/// <param name="type">The type.</param>
		/// <param name="attributes">The attributes.</param>
		public VertexInfo(Type type, params VertexAttribute[] attributes) {
			this.Type = type;
			this.SizeInBytes = 0;

			this.VertexAttributes = attributes;

			for (int i = 0; i < this.VertexAttributes.Length; i++) {
				VertexAttribute attribute = this.VertexAttributes[i];
				this.SizeInBytes += attribute.ComponentCount * sizeof(float);
			}
		}
	}


	public readonly struct VertexPositionColor {
		public readonly Vector2 Position;
		public readonly Color4 Color;

		public static readonly VertexInfo VertexInfo = new(
			typeof(VertexPositionColor),
			new VertexAttribute("Position", 0, 2, 0),
			new VertexAttribute("Color", 1, 4, 2 * sizeof(float))
			);

		/// <summary> Initializes a new instance of the <see cref="VertexPositionColor"/> class. </summary>
		/// <param name="position">The position.</param>
		/// <param name="color">The color.</param>
		public VertexPositionColor(Vector2 position, Color4 color) {
			this.Position = position;
			this.Color = color;
		}
	}

	public readonly struct VertexPositionTexture {
		public readonly Vector2 Position, TexCoord;

		public static readonly VertexInfo VertexInfo = new(
			typeof(VertexPositionTexture),
			new VertexAttribute("Positon", 0, 2, 0),
			new VertexAttribute("TexCoord", 1, 2, 2 * sizeof(float))
			);

		/// <summary> Initializes a new instance of the <see cref="VertexPositionTexture"/> class. </summary>
		/// <param name="position">The position.</param>
		/// <param name="texCoord">The tex coord.</param>
		public VertexPositionTexture(Vector2 position, Vector2 texCoord) {
			this.Position = position;
			this.TexCoord = texCoord;
		}
	}
}
