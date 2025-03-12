using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

public class TaskbarEyesForm : Form
{
    System.Windows.Forms.Timer timer;
    // Base sizes for drawing (will be multiplied by scale)
    const int eyeRadius = 20, pupilRadius = 8, maxPupilOffset = 8;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT { public int Left, Top, Right, Bottom; }
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    const uint SWP_NOACTIVATE = 0x0010;
    const uint SWP_SHOWWINDOW = 0x0040;

    NotifyIcon trayIcon;
    IntPtr taskbarHandle;
    RECT taskbarRect;
    bool doubleEyesEnabled = false;

    // Eye centers relative to the form.
    // Scale 1: form 120x60, centers at (40,30) and (80,30).
    // Scale 2: form 240x120, centers at (80,60) and (160,60).
    Point leftEyeCenter, rightEyeCenter;
    Size defaultSize = new Size(120, 60);
    Size doubleSize = new Size(240, 120);
    Point defaultLeftCenter = new Point(40, 30);
    Point defaultRightCenter = new Point(80, 30);
    Point doubleLeftCenter = new Point(80, 60);
    Point doubleRightCenter = new Point(160, 60);

    // Blink animation state per eye: -1 = not blinking; otherwise tick (0..totalBlinkTicks)
    int leftBlinkTick = -1, rightBlinkTick = -1;
    const int totalBlinkTicks = 12; // ~360ms total at 30ms ticks

    public TaskbarEyesForm()
    {
        // Transparent, borderless, no taskbar icon.
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.LimeGreen;
        TransparencyKey = Color.LimeGreen;
        Size = defaultSize;
        leftEyeCenter = defaultLeftCenter;
        rightEyeCenter = defaultRightCenter;

        timer = new System.Windows.Forms.Timer { Interval = 30 };
        timer.Tick += Timer_Tick;
        timer.Start();

        // Setup tray icon and context menu.
        trayIcon = new NotifyIcon
        {
            Icon = new Icon("eyes.ico"), // Ensure "eyes.ico" is in the executable folder.
            Visible = true,
            ContextMenuStrip = new ContextMenuStrip()
        };

        // Toggle Double Eye Size option.
        var doubleEyesMenuItem = new ToolStripMenuItem("Double Eye Size");
        doubleEyesMenuItem.Click += (s, e) =>
        {
            doubleEyesEnabled = !doubleEyesEnabled;
            doubleEyesMenuItem.Text = doubleEyesEnabled ? "Normal Eye Size" : "Double Eye Size";
            if (doubleEyesEnabled)
            {
                Size = doubleSize;
                leftEyeCenter = doubleLeftCenter;
                rightEyeCenter = doubleRightCenter;
            }
            else
            {
                Size = defaultSize;
                leftEyeCenter = defaultLeftCenter;
                rightEyeCenter = defaultRightCenter;
            }
            PositionWindow();
            Invalidate();
        };
        trayIcon.ContextMenuStrip.Items.Add(doubleEyesMenuItem);
        trayIcon.ContextMenuStrip.Items.Add("Exit", null!, (s, e) => Application.Exit());
        trayIcon.DoubleClick += (s, e) => Visible = !Visible;

        // Listen for right-clicks on the form.
        this.MouseDown += Form_MouseDown;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        taskbarHandle = FindWindow("Shell_TrayWnd", null!);
        if (taskbarHandle != IntPtr.Zero && GetWindowRect(taskbarHandle, out taskbarRect))
        {
            PositionWindow();
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        // Advance blink animations if active.
        if (leftBlinkTick >= 0)
        {
            leftBlinkTick++;
            if (leftBlinkTick > totalBlinkTicks)
                leftBlinkTick = -1;
        }
        if (rightBlinkTick >= 0)
        {
            rightBlinkTick++;
            if (rightBlinkTick > totalBlinkTicks)
                rightBlinkTick = -1;
        }
        Invalidate();
        if (taskbarHandle != IntPtr.Zero)
            PositionWindow();
    }

    private void PositionWindow()
    {
        int taskbarWidth = taskbarRect.Right - taskbarRect.Left;
        int taskbarHeight = taskbarRect.Bottom - taskbarRect.Top;
        int x = taskbarRect.Left + ((taskbarWidth - Width) / 2);
        int y = taskbarRect.Top + ((taskbarHeight - Height) / 2);
        SetWindowPos(Handle, HWND_TOPMOST, x, y, Width, Height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        e.Graphics.Clear(Color.LimeGreen);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        double scale = doubleEyesEnabled ? 2.0 : 1.0;
        Point mousePos = PointToClient(Cursor.Position);
        DrawEye(e.Graphics, leftEyeCenter, mousePos, scale, leftBlinkTick);
        DrawEye(e.Graphics, rightEyeCenter, mousePos, scale, rightBlinkTick);
    }

    void DrawEye(Graphics g, Point eyeCenter, Point target, double scale, int blinkTick)
    {
        int scaledEyeRadius = (int)(eyeRadius * scale);
        int scaledPupilRadius = (int)(pupilRadius * scale);
        int scaledMaxPupilOffset = (int)(maxPupilOffset * scale);

        Rectangle eyeRect = new Rectangle(eyeCenter.X - scaledEyeRadius, eyeCenter.Y - scaledEyeRadius,
                                          scaledEyeRadius * 2, scaledEyeRadius * 2);
        // Draw eyeball.
        g.FillEllipse(Brushes.White, eyeRect);
        g.DrawEllipse(Pens.Black, eyeRect);

        // Draw pupil.
        int dx = target.X - eyeCenter.X, dy = target.Y - eyeCenter.Y;
        double distance = Math.Sqrt(dx * dx + dy * dy);
        double offsetX = distance > 0 ? (dx / distance) * scaledMaxPupilOffset : 0;
        double offsetY = distance > 0 ? (dy / distance) * scaledMaxPupilOffset : 0;
        int pupilX = eyeCenter.X + (int)offsetX, pupilY = eyeCenter.Y + (int)offsetY;
        Rectangle pupilRect = new Rectangle(pupilX - scaledPupilRadius, pupilY - scaledPupilRadius,
                                            scaledPupilRadius * 2, scaledPupilRadius * 2);
        g.FillEllipse(Brushes.Black, pupilRect);

        // If blinking, overlay animated eyelids using Simpson's yellow.
        if (blinkTick >= 0)
        {
            double progress = Math.Sin((Math.PI * blinkTick) / totalBlinkTicks);
            int overlayHeight = (int)(eyeRect.Height / 2 * progress);

            using (GraphicsPath eyePath = new GraphicsPath())
            {
                eyePath.AddEllipse(eyeRect);

                using (Region topRegion = new Region(eyePath))
                {
                    Rectangle topRect = new Rectangle(eyeRect.Left, eyeRect.Top, eyeRect.Width, overlayHeight);
                    topRegion.Intersect(topRect);
                    using (SolidBrush simpsonsYellowBrush = new SolidBrush(Color.FromArgb(255, 204, 51)))
                    {
                        g.FillRegion(simpsonsYellowBrush, topRegion);
                    }
                }

                using (Region bottomRegion = new Region(eyePath))
                {
                    Rectangle bottomRect = new Rectangle(eyeRect.Left, eyeRect.Bottom - overlayHeight, eyeRect.Width, overlayHeight);
                    bottomRegion.Intersect(bottomRect);
                    using (SolidBrush simpsonsYellowBrush = new SolidBrush(Color.FromArgb(255, 204, 51)))
                    {
                        g.FillRegion(simpsonsYellowBrush, bottomRegion);
                    }
                }
            }
        }
    }

    private void Form_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            double scale = doubleEyesEnabled ? 2.0 : 1.0;
            int scaledEyeRadius = (int)(eyeRadius * scale);
            if (Distance(e.Location, leftEyeCenter) <= scaledEyeRadius)
                TriggerBlink(true);
            else if (Distance(e.Location, rightEyeCenter) <= scaledEyeRadius)
                TriggerBlink(false);
        }
    }

    private void TriggerBlink(bool left)
    {
        if (left)
            leftBlinkTick = 0;
        else
            rightBlinkTick = 0;
    }

    private double Distance(Point p1, Point p2)
    {
        int dx = p1.X - p2.X, dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new TaskbarEyesForm());
    }
}
