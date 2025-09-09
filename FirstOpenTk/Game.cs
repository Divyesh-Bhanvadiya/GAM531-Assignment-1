using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace FirstOpenTk
{
    public class Game : GameWindow
    {
        private int vertexBufferHandle;
        private int shaderProgramHandle;
        private int vertexArrayHandle;
        public Game() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(1280, 768));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            // Update the OpenGL viewport to match the new window dimensions
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(new Color4(0.318f, 0.592f,0.804f,1.0f));

            float[] vertices = new float[]
            {
                // First triangle
                -0.5f,  0.3f, 0.0f, // top-left
                -0.5f, -0.3f, 0.0f, // bottom-left
                0.5f,  0.3f, 0.0f, // top-right
    
                // Second triangle
                -0.5f, -0.3f, 0.0f, // bottom-left
                0.5f, -0.3f, 0.0f, // bottom-right
                0.5f,  0.3f, 0.0f  // top-right
            };
            
            vertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float),vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexArrayHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            string vertexShaderCode = 
                @"
                    #version 330 core
                    layout(location = 0) in vec3 aPosition;
                    void main () 
                    {
                        gl_Position = vec4(aPosition, 1.0);
                    }
                ";
            
            string fragmentShaderCode = @"
                #version 330 core
                out vec4 FragColor;

                void main()
                {
                    FragColor = vec4(0.6f, 0.2f, 0.8f, 1.0f); // Orange-red color
                }
            ";
            
            // Compile shaders
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);
            CheckShaderCompile(vertexShaderHandle, "Vertex Shader");

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);
            CheckShaderCompile(fragmentShaderHandle, "Fragment Shader");

            // Create shader program and link shaders
            shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.LinkProgram(shaderProgramHandle);

            // Cleanup shaders after linking (no longer needed individually)
            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
    
        }

        protected override void OnUnload()
        {
            // Unbind and delete buffers and shader program
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferHandle);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vertexArrayHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(shaderProgramHandle);

            base.OnUnload();
        }


        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Clear the screen with background color
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Use our shader program
            GL.UseProgram(shaderProgramHandle);

            // Bind the VAO and draw the triangle
            GL.BindVertexArray(vertexArrayHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);

            // Display the rendered frame
            SwapBuffers();
        }
        
        // Helper function to check for shader compilation errors
        private void CheckShaderCompile(int shaderHandle, string shaderName)
        {
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shaderHandle);
                Console.WriteLine($"Error compiling {shaderName}: {infoLog}");
            }
        }
    }
}