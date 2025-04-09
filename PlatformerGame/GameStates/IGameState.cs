using System.Drawing;
using System.Windows.Forms;

namespace PlatformerGame.GameStates
{
    public interface IGameState
    {
        void Update();
        void Draw(Graphics g);
        void HandleInput(KeyEventArgs e);
        void HandleMouseClick(MouseEventArgs e);
        void OnEnter();
        void OnExit();
    }
}