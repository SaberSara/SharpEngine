﻿using System;
using System.IO;
using GLFW;
using static OpenGL.Gl;

namespace SharpEngine
{

    struct Vector
    {
        public float x, y, z;

        public Vector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public Vector(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }
    }
    class Program
    {
        static Vector[] vertices = new Vector[]
        {
            new Vector(-.1f, -.1f), //Vertex1 (x,y,z)
            new Vector(.1f, -.1f), //vertex2 (x,y,z)
            new Vector( 0f, .1f), //vertex3(x,y,z)
            new Vector(.4f, .4f), //Vertex4 (x,y,z)
            new Vector(.6f, .4f), //vertex5 (x,y,z)
            new Vector(.5f, .6f)//vertex6(x,y,z)
        };

        private const int vertexX = 0;
        private const int vertexY = 1;
        private const int vertexSize = 3;
        static void Main(string[] args) {
            
            var window = CreateWindow();

            LoadTrianglesIntoBuffer();
            
            CreateShaderProgram();

            //Engine rendering loop
            while (!Glfw.WindowShouldClose(window)) {
                Glfw.PollEvents(); // react to window changes (position etc.)
                ClearScreen();
                Render(window);
                for (var i = 0; i < vertices.Length; i++)
                {
                    vertices[i].x += 0.001f; ///99.99% /-0.01%
                }
                UpdateTriangleBuffer();
            }
        }

        private static void Render(Window window)
        {
            glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
            Glfw.SwapBuffers(window);
            //glFlush();
        }

        private static void ClearScreen()
        {
            glClearColor(0, 0, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT);
        }

        private static unsafe void LoadTrianglesIntoBuffer()
        {
            

            //Load the verices into a buffer
            var vertexArray = glGenVertexArray();
            var vertexBuffer = glGenBuffer();
            glBindVertexArray(vertexArray);

            glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
            unsafe
            {
                UpdateTriangleBuffer();
                glVertexAttribPointer(0, vertexSize, GL_FLOAT, false,  sizeof(Vector), NULL);
            }

            glEnableVertexAttribArray(0);
        }

        static unsafe void UpdateTriangleBuffer()
        {
            fixed (Vector* vertex = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(Vector) * vertices.Length, vertex, GL_DYNAMIC_DRAW);
            }

        }
        private static void CreateShaderProgram()
        {
            //Create vertex shader
            var vertexShader = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertexShader, File.ReadAllText("shaders/screen-coordinates.vert"));
            glCompileShader(vertexShader);

            //Create fragment shader
            var fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragmentShader, File.ReadAllText("shaders/green.frag"));
            glCompileShader(fragmentShader);

            //Create shader program - rendering pipeline
            var program = glCreateProgram();
            glAttachShader(program, vertexShader);
            glAttachShader(program, fragmentShader);
            glLinkProgram(program);
            glUseProgram(program);
        }

        private static Window CreateWindow()
        {
            //Initialize and configure
            Glfw.Init();
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.Decorated, true);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.OpenglForwardCompatible, Constants.True);
            Glfw.WindowHint(Hint.Doublebuffer, Constants.True);

            //Create and launch a window
            var window = Glfw.CreateWindow(1024, 768, "SharpEngine", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);
            return window;
        }
    }
}