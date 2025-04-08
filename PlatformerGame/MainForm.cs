using System;
using System.Drawing;
using System.Windows.Forms;
using PlatformerGame.GameObjects;
using PlatformerGame.GameStates;

namespace PlatformerGame.Forms
{
    public partial class MainForm : Form
    {
        private GameState gameState = GameState.MainMenu;
        private Player player;
        private Level level;
        private GameTimer gameTimer;
        private BufferedGraphicsContext context;
        private BufferedGraphics bufferedGraphics;
        private bool isFullScreen = false;
        private FormWindowState previousWindowState;
        private Rectangle previousBounds;

        // Остальные методы класса Form (InitializeGraphics, DrawMainMenu и т.д.)
        // ...
    }
}