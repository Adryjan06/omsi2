using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Omsi64.Engine.Rendering;

internal sealed class ShaderProgram : IDisposable
{
    private readonly int _handle;

    public ShaderProgram(string vertexSource, string fragmentSource)
    {
        var vertex = Compile(ShaderType.VertexShader, vertexSource);
        var fragment = Compile(ShaderType.FragmentShader, fragmentSource);

        _handle = GL.CreateProgram();
        GL.AttachShader(_handle, vertex);
        GL.AttachShader(_handle, fragment);
        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var ok);
        if (ok == 0)
            throw new InvalidOperationException(GL.GetProgramInfoLog(_handle));

        GL.DetachShader(_handle, vertex);
        GL.DetachShader(_handle, fragment);
        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    public void Use() => GL.UseProgram(_handle);

    public void SetMatrix4(string name, Matrix4 value)
    {
        var location = GL.GetUniformLocation(_handle, name);
        GL.UniformMatrix4(location, true, ref value);
    }

    public void SetColor(string name, Vector4 value)
    {
        var location = GL.GetUniformLocation(_handle, name);
        GL.Uniform4(location, value);
    }

    public void Dispose() => GL.DeleteProgram(_handle);

    private static int Compile(ShaderType type, string source)
    {
        var shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var ok);
        if (ok != 0)
            return shader;

        var log = GL.GetShaderInfoLog(shader);
        GL.DeleteShader(shader);
        throw new InvalidOperationException(log);
    }
}