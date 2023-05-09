using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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
        private Label lblTurn;

        private Point? selectedPiece = null;

        public Color boardBlack = Color.FromArgb(153, 63, 44);
        public Color boardWhite = Color.FromArgb(247, 155, 136);
        private Rectangle? selectedPieceRect = null;

        // White to move
        private bool whiteToMove = true;

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

            // Add a visual representation of who's turn it is
            lblTurn = new Label();
            lblTurn.Name = "lblTurn";
            lblTurn.Text = "White to move";
            lblTurn.AutoSize = true;
            this.Controls.Add(lblTurn);
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
                    // Highlight the selected piece
                    if (selectedPiece != null && selectedPiece.Value.X == x && selectedPiece.Value.Y == y)
                    {
                        brush = new SolidBrush(Color.FromArgb(100, Color.Yellow));
                        e.Graphics.FillRectangle(brush, x * TileSize, y * TileSize, TileSize, TileSize);
                    }
                }
            }
            // Update the label's position
            lblTurn.Location = new Point(this.ClientSize.Width - lblTurn.Width - 10, 10);
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (selectedPiece == null) return;

            int startX = selectedPiece.Value.X;
            int startY = selectedPiece.Value.Y;
            int endX = e.X / TileSize;
            int endY = e.Y / TileSize;

            if (startX == endX && startY == endY)
            {
                selectedPiece = null;
                selectedPieceRect = null;
                // Redraw the chessboard to remove the highlighting
                this.Invalidate();
                return;
            }

            if (board[startY, startX] != ".")
            {
                // Check if the move is valid
                if (IsValidMove(startX, startY, endX, endY))
                {
                    // Update the board array
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Console.Write(board[i, j] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("======================A MOVE WAS PLAYED========================");
                    board[endY, endX] = board[startY, startX];
                    board[startY, startX] = ".";
                    whiteToMove = !whiteToMove;
                }
            }

            selectedPiece = null;
            lblTurn.Text = whiteToMove ? "White to move" : "Black to move";
            // Redraw the chessboard
            this.Invalidate();
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            int x = e.X / TileSize;
            int y = e.Y / TileSize;
            

            if (x < 0 || x >= 8 || y < 0 || y >= 8)
                return;
            bool isWhitePiece = char.IsUpper(board[y, x][0]);
            if (selectedPiece == null)
            {
                // Select a piece
                if (board[y, x] != "." && (isWhitePiece && whiteToMove) || (!isWhitePiece && !whiteToMove))
                {
                    
                    selectedPiece = new Point(x, y);
                    Console.WriteLine("Selected piece: "+ board[y, x]);
                    selectedPieceRect = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                }
                if (board[y, x] != "." && (whiteToMove && board[y, x] == board[y, x].ToUpper() || !whiteToMove && board[y, x] == board[y, x].ToLower()))
                {
                    // A piece was clicked, so highlight the possible moves
                    selectedPiece = new Point(x, y);
                    selectedPieceRect = new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);

                    // Determine the possible moves for the selected piece
                    List<Point> possibleMoves = GetPossibleMoves(x, y);

                    // Highlight the possible moves
                    using (var g = this.CreateGraphics())
                    {
                        foreach (var move in possibleMoves)
                        {
                            var rect = new Rectangle(move.X * TileSize, move.Y * TileSize, TileSize, TileSize);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Yellow)), rect);
                        }
                    }
                }
            }
            else
            {
                // Deselect a piece
                selectedPiece = null;
            }

            this.Invalidate();
        }
        private List<Point> GetPossibleMoves(int x, int y)
        {
            List<Point> possibleMoves = new List<Point>();

            // Iterate over all the squares on the board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    return possibleMoves;
                }
            }

            return possibleMoves;
        }


        private bool IsValidMove(int startX, int startY, int endX, int endY)
        {
            string piece = board[startY, startX];
            if (piece == "p" || piece == "P")
            {
                Console.WriteLine("Checking the PAWN's move legality");
                if (canBeTaken(piece, endX, endY) == true) { return true; }
            }
            if (piece == "q" || piece == "Q")
            {
                Console.WriteLine("Checking the QUEEN's move legality");
                if (queenMove(startX, startY, endX, endY) == true) { return true; }
            }
            if (piece == "n" || piece == "N")
            {
                Console.WriteLine("Checking the KNIGHT's move legality");
                if (knightMove(startX, startY, endX, endY) == true) { return true; }
            }
            if (piece == "b" || piece == "B")
            {
                Console.WriteLine("Checking the BISHOP's move legality");
                if (bishopMove(startX, startY, endX, endY) == true) { return true; }
            }
            if (piece == "r" || piece == "R")
            {
                Console.WriteLine("Checking the ROOK's move legality");
                if (rookMove(startX, startY, endX, endY) == true) { return true; }
            }
            if (piece == "k" || piece == "K")
            {
                Console.WriteLine("Checking the KING's move legality");
                return true; 
            }
            return false;
        }
        private bool canBeTaken(string piece, int endX, int endY)
        {
            string endPiece = board[endY, endX];
            bool pieceWhite = Char.IsUpper(piece[0]);
            bool endPieceWhite = Char.IsUpper(endPiece[0]);

            // Check if the pieces are of opposite colors
            if (pieceWhite != endPieceWhite)
            {
                // Check if white piece can take black piece
                if (pieceWhite && !endPieceWhite)
                {
                    return true;
                }
                // Check if black piece can take white piece
                else if (!pieceWhite && endPieceWhite)
                {
                    return true;
                }
            }

            // If the pieces are not of opposite colors or if the move is invalid, return false
            Console.WriteLine("A piece of the same color cannot be taken.");
            return false;
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
            selectedPiece = null;
            whiteToMove = true;
            // Invalidate the form to trigger a repaint
            Invalidate();
        }
        private bool knightMove(int originalX, int originalY, int newX, int newY)
        {
            // Calculate the horizontal and vertical distance between the current position and the destination position
            int deltaX = Math.Abs(originalX - newX);
            int deltaY = Math.Abs(originalY - newY);

            // Knights can only move in L-shape, two squares horizontally and one square vertically or two squares vertically and one square horizontally.
            if ((deltaX == 2 && deltaY == 1) || (deltaX == 1 && deltaY == 2))
            {
                Console.WriteLine("Knight moved succesfully");
                return true;
            }
            else
            {
                Console.WriteLine("Knight cannot be moved there");
                return false;
            }
        }
        public bool bishopMove(int startX, int startY, int endX, int endY)
        {
            // Check if the destination square is on the same diagonal as the starting square
            if (Math.Abs(endX - startX) != Math.Abs(endY - startY))
            {
                Console.WriteLine("The final square is not on the same diagonal, as the bishop");
                return false;
                
            }

            // Check if there are any pieces obstructing the bishop's path
            int xDir = Math.Sign(endX - startX);
            int yDir = Math.Sign(endY - startY);
            int x = startX + xDir;
            int y = startY + yDir;
            while (x != endX || y != endY)
            {
                if (board[y, x] != ".")
                {
                    Console.WriteLine("The movement was blocked by: "+ board[y, x]);
                    return false;
                }
                x += xDir;
                y += yDir;
            }

            // If we get here, the move is legal
            Console.WriteLine("Bishop was moved succesfully");
            return true;
        }
        public bool rookMove(int startX, int startY, int endX, int endY)
        {
            // Check if the destination square is on the same rank or file as the starting square
            if (startX != endX && startY != endY)
            {
                Console.WriteLine("The destination square is not on the same rank or file as the starting square");
                return false;
            }

            // Check if there are any pieces obstructing the rook's path
            int xDir = startX == endX ? 0 : Math.Sign(endX - startX);
            int yDir = startY == endY ? 0 : Math.Sign(endY - startY);
            int x = startX + xDir;
            int y = startY + yDir;
            while (x != endX || y != endY)
            {
                if (board[y, x] != ".")
                {
                    Console.WriteLine("The movement was blocked by: " + board[y, x]);
                    return false;
                }
                x += xDir;
                y += yDir;
            }

            // If we get here, the move is legal
            Console.WriteLine("Rook moved succesfully");
            return true;
        }
        public bool queenMove(int startX, int startY, int endX, int endY)
        {
            // Check if the destination square is on the same rank or file or diagonal as the starting square
            if (startX != endX && startY != endY)
            {
                if (Math.Abs(endX - startX) != Math.Abs(endY - startY))
                {
                    Console.WriteLine("The destination square is not within reach of the queen.");
                    return false;
                }
            }

            // Check if there are any pieces obstructing the queens's path
            int xDir = startX == endX ? 0 : Math.Sign(endX - startX);
            int yDir = startY == endY ? 0 : Math.Sign(endY - startY);
            int x = startX + xDir;
            int y = startY + yDir;
            while (x != endX || y != endY)
            {
                if (board[y, x] != ".")
                {
                    Console.WriteLine("The movement was blocked by: " + board[y, x]);
                    return false;
                }
                x += xDir;
                y += yDir;
            }
            xDir = Math.Sign(endX - startX);
            yDir = Math.Sign(endY - startY);
            x = startX + xDir;
            y = startY + yDir;
            while (x != endX || y != endY)
            {
                if (board[y, x] != ".")
                {
                    Console.WriteLine("The movement was blocked by: " + board[y, x]);
                    return false;
                }
                x += xDir;
                y += yDir;
            }

            // If we get here, the move is legal
            return true;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("AHOJ");
            // Create and show the chessboard window
            Application.EnableVisualStyles();
            Application.Run(new ChessboardUi());
        }
    }
}
