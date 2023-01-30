using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace testing
{
    public class Game : GameWindow
    {
        // hello
        private int vertexBufferHandle;
        private int indexBufferHandle;
        private int shaderProgrameHandle;
        private int vertexArrayHandle;
        private int testNum = 0;
        public Game() 
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(Size = (500, 500));
            this.Title = "game";
            this.UpdateFrequency = 60;
            this.WindowBorder = WindowBorder.Fixed;
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(new Color4(0.2f, 0.0f, 0.0f, 1.0f));
            float[] vertices =
            {
                -0.5f, 0.5f, 0.0f, 0f, 0f, 1f, 1f,
                0.5f, 0.5f, 0.0f, 1f, 0f, 0f, 1f,
                0.5f, -0.5f, 0.0f, 0f, 1f, 0f, 1f,
                -0.5f, -0.5f, 0.0f, 0f, 0f, 1f, 1f
            };

            int[] indecies = new int[]
            {
                0, 1, 2, 0, 2, 3
            };

            this.vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.indexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indecies.Length * sizeof(int), indecies, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            string vertexShaderCode = 
                @"
                #version 330 core
                
                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec4 aColor;

                out vec4 vColor;

                void main()
                {
                    vColor = aColor;
                    gl_Position = vec4(aPosition, 1f);
                }
                ";
            string pixelShaderCode =
                @"
                #version 330
                
                in vec4 vColor;

                out vec4 pixelColor;

                void main() 
                {
                    pixelColor = vColor;
                }
                ";

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexBufferHandle);

            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            this.shaderProgrameHandle = GL.CreateProgram();

            GL.AttachShader(this.shaderProgrameHandle, vertexShaderHandle);
            GL.AttachShader(this.shaderProgrameHandle, pixelShaderHandle);

            GL.LinkProgram(this.shaderProgrameHandle);

            GL.DetachShader(this.shaderProgrameHandle, vertexShaderHandle);
            GL.DetachShader(this.shaderProgrameHandle, pixelShaderHandle);

            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(pixelShaderHandle);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(this.vertexBufferHandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(this.indexBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(this.shaderProgrameHandle);
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            Console.WriteLine(testNum);
            testNum++;
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.shaderProgrameHandle);

            GL.BindVertexArray(this.vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            this.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
