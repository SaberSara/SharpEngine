using System;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

namespace SharpEngine {
	public class Triangle {
            
		Vertex[] vertices;
		uint vertexArray;
		uint vertexBuffer;

		public float CurrentScale { get; private set; }
		
		public Material material;
            
		public Triangle(Vertex[] vertices, Material material) {
			this.vertices = vertices;
			this.material = material;
			LoadTriangleIntoBuffer();
			this.CurrentScale = 1f;
		}
		
		 unsafe void LoadTriangleIntoBuffer() {
			vertexArray = glGenVertexArray();
			vertexBuffer = glGenBuffer();
			glBindVertexArray(vertexArray);
			glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
			glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
			glVertexAttribPointer(1, 4, GL_FLOAT, false, sizeof(Vertex), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
			glEnableVertexAttribArray(0);
			glEnableVertexAttribArray(1);
			glBindVertexArray(0);
		}

		public Vector GetMinBounds() {
			var min = this.vertices[0].position;
			for (var i = 1; i < this.vertices.Length; i++) {
				min = Vector.Min(min, this.vertices[i].position);
			}
			return min;
		}
            
		public Vector GetMaxBounds() {
			var max = this.vertices[0].position;
			for (var i = 1; i < this.vertices.Length; i++) {
				max = Vector.Max(max, this.vertices[i].position);
			}

			return max;
		}

		public Vector GetCenter() {
			return (GetMinBounds() + GetMaxBounds()) / 2;
		}

		public void Scale(float multiplier) {
			// We first move the triangle to the center, to avoid
			// the triangle moving around while scaling.
			// Then, we move it back again.
			var center = GetCenter();
			Move(-center);
			for (var i = 0; i < this.vertices.Length; i++) {
				this.vertices[i].position *= multiplier;
			}
			Move(center);

			this.CurrentScale *= multiplier;
		}

		public void Move(Vector direction) {
			for (var i = 0; i < this.vertices.Length; i++) {
				this.vertices[i].position += direction;
			}
		}

		public unsafe void Render() {
			this.material.Use();
			glBindVertexArray(vertexArray);
			glBindBuffer(GL_ARRAY_BUFFER, this.vertexBuffer);
			fixed (Vertex* vertex = &this.vertices[0]) {
				glBufferData(GL_ARRAY_BUFFER, sizeof(Vertex) * this.vertices.Length, vertex, GL_DYNAMIC_DRAW);
			}
			glDrawArrays(GL_TRIANGLES, 0, this.vertices.Length);
			glBindVertexArray(0);
		}

		public void Rotate(float rotation) {
			var center = GetCenter();
			Move(-center);
			for (int i = 0; i < this.vertices.Length; i++) {
				var currentRotation = Vector.Angle(this.vertices[i].position);
				var distance = vertices[i].position.GetMagnitude();
				var newX = MathF.Cos(currentRotation + rotation);
				var newY = MathF.Sin(currentRotation + rotation);
				vertices[i].position = new Vector(newX, newY) * distance;
			}
			Move(center);
		}
	}

	public struct Matrix
	{
		public float m11, m12, m13, m14;
		public float m21, m22, m23, m24;
		public float m31, m32, m33, m34;
		public float m41, m42, m43, m44;

		public Matrix(float m11, float m12, float m13,float m14, float m21, float m22, float m23, float m24, float m31, float m32,
			float m33, float m34, float m41, float m42, float m43, float m44)
		{
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m14 = m14;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m24 = m24;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
			this.m34 = m34;
			this.m41 = m41;
			this.m42 = m42;
			this.m43 = m43;
			this.m44 = m44;
		}
		public static Matrix Identity => new Matrix(
			1,0,0,0,
			0,1,0,0,
			0,0,1,0,
			0,0,0,1);
		
	}
	
	
	
}