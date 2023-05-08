using System;
using System.Drawing;
using System.Windows.Forms;

namespace gamechess
{
    public class ChessboardUi : Form
    {
        private const int TileSize = 100;
        private const int BoardSize = 8 * TileSize;

        private const int ResetButtonWidth = 100;
        private const int ResetButtonHeight = 30;
        private const int ResetButtonMargin = 10;

        private Button resetButton;

        private Point? selectedPiece = null;

        public Color boardBlack = Color.FromArgb(153, 63, 44);
        public Color boardWhite = Color.FromArgb(247, 155, 136);

        // Update the board array to hold data about the pieces
        public string[,] board = new string[8, 8]{
            {"r", "n", "b", "q", "k", "b", "n", "r"},
            {"p", "p", "p", "p", "p", "p", "p", "p"},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {"P", "P", "P", "P", "P", "P", "P", "P"},
            {"R", "N", "B", "Q", "K", "B", "N", "R"}
        };

        public ChessboardUi()
        {
            // Set the size and title of the window
            this.ClientSize = new Size(BoardSize * 2, BoardSize);
            this.Text = "Chess Game";

            // Add an event handler for when the window is painted
            this.Paint += new PaintEventHandler(OnPaint);

            // Add event handlers for mouse events
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseUp += new MouseEventHandler(OnMouseUp);

            // Add the reset button
            resetButton = new Button();
            resetButton.Text = "Reset";
            resetButton.Width = ResetButtonWidth;
            resetButton.Height = ResetButtonHeight;
            resetButton.Location = new Point(BoardSize * 2 - ResetButtonWidth - ResetButtonMargin, BoardSize / 2 - ResetButtonHeight / 2);
            resetButton.Click += new EventHandler(OnResetButtonClick);
            Controls.Add(resetButton);
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            // Draw the chessboard
            Brush brush = Brushes.White;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 == 0)
                        brush = new SolidBrush(boardWhite);
                    else
                        brush = new SolidBrush(boardBlack);

                    e.Graphics.FillRectangle(brush, x * TileSize, y * TileSize, TileSize, TileSize);
                    // Draw the piece, if there is one
                    string piece = board[y, x];
                    if (piece != ".")
                    {
                        Font font = new Font("Arial", 50);
                        brush = (piece == piece.ToUpper()) ? Brushes.White : Brushes.Black; // Set the text color based on the piece color
                        e.Graphics.DrawString(piece, font, brush, x * TileSize + 20, y * TileSize + 10);
                    }
                }
            }
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (selectedPiece == null) return;

            int startX = selectedPiece.Value.X;
            int startY = selectedPiece.Value.Y;
            int endX = e.X / TileSize;
            int endY = e.Y / TileSize;

            if (startX == endX && startY == endY) return;

            if (board[startY, startX] != ".")
            {
                // Check if the move is valid
                if (IsValidMove(startX, startY, endX, endY))
                {
                    // Update the board array
                    board[endY, endX] = board[startY, startX];
                    board[startY, startX] = ".";
                }
            }

            selectedPiece = null;

            // Redraw the chessboard
            this.Invalidate();
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            int x = e.X / TileSize;
            int y = e.Y / TileSize;

            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                return;

            if (selectedPiece == null)
            {
                // Select a piece
                if (board[y, x] != ".")
                {
                    selectedPiece = new Point(x, y);
                }
            }
            else
            {
                // Deselect a piece
                selectedPiece = null;
            }

            this.Invalidate();
        }


        private bool IsValidMove(int startX, int startY, int endX, int endY)
        {
            string piece = board[startY, startX];

            // Check if the move is valid for the piece
            // ...

            return true;
        }
        private void OnResetButtonClick(object? sender, EventArgs e)
        {
            // Reset the board to its default state
            board = new string[8, 8]{
            {"r", "n", "b", "q", "k", "b", "n", "r"},
            {"p", "p", "p", "p", "p", "p", "p", "p"},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {".", ".", ".", ".", ".", ".", ".", "."},
            {"P", "P", "P", "P", "P", "P", "P", "P"},
            {"R", "N", "B", "Q", "K", "B", "N", "R"}
        };

            // Invalidate the form to trigger a repaint
            Invalidate();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create and show the chessboard window
            Application.EnableVisualStyles();
            Application.Run(new ChessboardUi());
        }
    }
}
