using OpenTK.Graphics.OpenGL4;

namespace Omsi64.Engine.Rendering;

internal sealed class LineMesh : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;

    public int VertexCount { get; }

    public LineMesh(IReadOnlyList<float> vertices)
    {
        VertexCount = vertices.Count / 3;
        var data = vertices.ToArray();

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Draw()
    {
        if (VertexCount == 0) return;

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Lines, 0, VertexCount);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
    }
}