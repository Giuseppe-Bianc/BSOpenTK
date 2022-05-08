using System;
using OpenTK.Mathematics;

namespace BSOpenTK {

	public readonly struct VertexAtrib {
		public readonly String Name;
		public readonly int Ind, CompCount, Offset;

		public VertexAtrib(String Name, int Index, int CompCount, int Offset){
			this.Name = Name;
			this.Ind = Index;
			this.CompCount = CompCount;
			this.Offset = Offset;
		}
	}

	public sealed class VertexInf {
		public readonly Type Type;
		public readonly int SzInBytes;
		public readonly VertexAtrib[] VertexAtribs;

		public VertexInf(Type Type, params VertexAtrib[] atributes) {
			this.Type = Type;
			this.VertexAtribs = atributes;

			for (int i = 0; i < this.VertexAtribs.Length; i++) {
				VertexAtrib atribute = this.VertexAtribs[i];
				this.SzInBytes += atribute.CompCount * sizeof(float);
			}
		}
	}
	public readonly struct VertexPosColor {
		public readonly Vector2 Position;
		public readonly Color4 Color;

		public static readonly VertexInf VertexInfo = new(typeof(VertexPosColor),
			new VertexAtrib("Position", 0, Consts.PSZ, 0),
			new VertexAtrib("Color", 1, 4, Consts.PSZ * sizeof(float))
			);

		public VertexPosColor(Vector2 position, Color4 color) {
			Position = position;
			Color = color;
		}
	}
}
