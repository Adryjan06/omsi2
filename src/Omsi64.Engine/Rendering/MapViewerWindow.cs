using Omsi64.Engine.Omsi;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Omsi64.Engine.Rendering;

internal sealed class MapViewerWindow : GameWindow
{
    private const string VertexShader = """
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        uniform mat4 uView;
        uniform mat4 uProjection;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0) * uView * uProjection;
        }
        """;

    private const string FragmentShader = """
        #version 330 core
        out vec4 FragColor;
        uniform vec4 uColor;

        void main()
        {
            FragColor = uColor;
        }
        """;

    private readonly OmsiLoadedMap _loaded;
    private ShaderProgram? _shader;
    private LineMesh? _tiles;
    private LineMesh? _objects;
    private LineMesh? _splines;

    private Vector3 _target;
    private float _distance;
    private float _farPlane;

    public MapViewerWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, OmsiLoadedMap loaded)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _loaded = loaded;
        (_target, _distance, _farPlane) = CalculateInitialCamera(loaded.Map.Tiles);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.055f, 0.065f, 0.08f, 1f);
        GL.Enable(EnableCap.DepthTest);

        _shader = new ShaderProgram(VertexShader, FragmentShader);
        _tiles = DebugGeometryBuilder.BuildTileMesh(_loaded.Map.Tiles);
        _objects = DebugGeometryBuilder.BuildObjectMesh(_loaded.TileContents);
        _splines = DebugGeometryBuilder.BuildSplineMesh(_loaded.TileContents);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
            return;
        }

        var dt = (float)e.Time;
        var speed = MathF.Max(150f, _distance * 0.35f) * dt;

        if (KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift))
            speed *= 3f;

        if (KeyboardState.IsKeyDown(Keys.W)) _target.Z -= speed;
        if (KeyboardState.IsKeyDown(Keys.S)) _target.Z += speed;
        if (KeyboardState.IsKeyDown(Keys.A)) _target.X -= speed;
        if (KeyboardState.IsKeyDown(Keys.D)) _target.X += speed;

        var zoomSpeed = MathF.Max(250f, _distance * 1.2f) * dt;
        if (KeyboardState.IsKeyDown(Keys.Q)) _distance += zoomSpeed;
        if (KeyboardState.IsKeyDown(Keys.E)) _distance -= zoomSpeed;

        _distance = MathHelper.Clamp(_distance, 150f, _farPlane * 0.45f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (_shader is null) return;

        var eye = _target + new Vector3(0f, _distance * 0.72f, _distance);
        var view = Matrix4.LookAt(eye, _target, Vector3.UnitY);
        var aspect = Math.Max(1f, Size.X / (float)Math.Max(1, Size.Y));
        var projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(55f), aspect, 1f, _farPlane);

        _shader.Use();
        _shader.SetMatrix4("uView", view);
        _shader.SetMatrix4("uProjection", projection);

        _shader.SetColor("uColor", new Vector4(0.20f, 0.80f, 1.00f, 1.00f));
        _tiles?.Draw();

        _shader.SetColor("uColor", new Vector4(1.00f, 0.70f, 0.20f, 1.00f));
        _splines?.Draw();

        _shader.SetColor("uColor", new Vector4(0.35f, 1.00f, 0.45f, 1.00f));
        _objects?.Draw();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
        _tiles?.Dispose();
        _objects?.Dispose();
        _splines?.Dispose();
        _shader?.Dispose();
        base.OnUnload();
    }

    private static (Vector3 target, float distance, float farPlane) CalculateInitialCamera(IReadOnlyList<OmsiMapTile> tiles)
    {
        var minX = tiles.Min(t => t.X * OmsiMapTile.SizeMeters);
        var maxX = tiles.Max(t => (t.X + 1) * OmsiMapTile.SizeMeters);
        var minZ = tiles.Min(t => -(t.Y + 1) * OmsiMapTile.SizeMeters);
        var maxZ = tiles.Max(t => -t.Y * OmsiMapTile.SizeMeters);

        var extent = MathF.Max(MathF.Max(300f, maxX - minX), MathF.Max(300f, maxZ - minZ));

        return (
            new Vector3((minX + maxX) * 0.5f, 0f, (minZ + maxZ) * 0.5f),
            MathF.Max(600f, extent * 1.15f),
            MathF.Max(20_000f, extent * 8f));
    }
}