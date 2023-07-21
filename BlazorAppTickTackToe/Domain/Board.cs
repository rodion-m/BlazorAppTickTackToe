using System.Data.Common;
using System.Text;
using System.Text.Json;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models.ChatCompletion;
using OpenAI.ChatGpt.Modules.StructuredResponse;

namespace BlazorAppTickTackToe.Domain
{
    public enum CellState
    {
        Blank, X, O
    }

    public enum Gamer
    {
        X, O
    }

    public enum GameResult
    {
        WonX, 
        WonO, 
        NoWinner,
        Unknown
    }

    public class Board
    {
        public int ColumnCount => 3;
        public int RowCount => 3;

        public CellState[,] Cells { get; set; }
        public Gamer CurrentGamer { get; set; } = Gamer.X;

        public Board()
        {
            Cells = new CellState[RowCount, ColumnCount];
        }

        /// <summary>
        /// Сделать следующий ход
        /// </summary>
        public bool NextTurn(int row, int column)
        {
            if (GetGameResult(out _) != GameResult.Unknown) 
                return false;

            if (Cells[row, column] == CellState.Blank)
            {
                if (CurrentGamer == Gamer.X)
                {
                    Cells[row, column] = CellState.X;
                    SwitchGamer();
                    MakeTurnByChatGPT();
                }
                else
                {
                    Cells[row, column] = CellState.O;
                    SwitchGamer();
                }
                return true;
            }
            return false;
        }

        private void MakeTurnByChatGPT()
        {
            var client = new OpenAiClient("СЮДА НУЖНО ПОДСТАВИТЬ КЛЮЧ");
            var board = $"{Cells[0, 0]} {Cells[0, 1]} {Cells[0, 2]}" +
                        $"{Cells[1, 0]} {Cells[1, 1]} {Cells[1, 2]}" +
                        $"{Cells[2, 0]} {Cells[2, 1]} {Cells[2, 2]}"
                        ;
            var prompt =
                "You are an AI playing a game of Tic Tac Toe as the 'O' player. Your goal is to place three of your marks in a horizontal, vertical, or diagonal row to win the game. The game starts with the 'X' player making the first move. Players alternate turns, placing their mark in an empty square. You are highly skilled and strive to play the optimal move. Each square on the game board can be identified by its row (from top to bottom) and column (from left to right), both ranging from 0 to 2. The current state of the board is represented by a 3x3 grid. 'X' represents the opponent's pieces, 'O' represents your own pieces, and '_' represents an empty space. Here's the current state of the game:\n"
                + board 
                + "\nReading from left to right, the columns are 0, 1, and 2, and reading from top to bottom, the rows are 0, 1, and 2. It's your turn to play as the 'O' player. Please specify your move as a row and column number. What is your next move?";

            var dialog = Dialog.StartAsSystem(prompt);
            CellPosition nextTurn;
            nextTurn = client.GetStructuredResponse<CellPosition>(
                dialog, model: ChatCompletionModels.Gpt4).Result;
            NextTurn(nextTurn.Row, nextTurn.Column);
        }

        private void SwitchGamer()
        {
            //тернарный оператор
            CurrentGamer = CurrentGamer == Gamer.X ? Gamer.O : Gamer.X;
        }

        // TODO: победа по событию.
        // Задание: запрограммируйте проверку 8 вариантов победы крестиков
        public GameResult GetGameResult(out CellPosition[] winCells)
        {
            if (CheckWin(Gamer.X, out winCells))
            {
                return GameResult.WonX;
            }
            else if (CheckWin(Gamer.O, out winCells))
            {
                return GameResult.WonO;
            }
            else if (CheckNoWinner()) //ДЗ: Реализовать метод определения ничьей (CheckNoWinner)
            {
                return GameResult.NoWinner;
            }
            else
            {
                return GameResult.Unknown;
            }
        }

        private bool CheckNoWinner()
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int column = 0; column < ColumnCount; column++)
                {
                    if (Cells[row, column] == CellState.Blank)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckWin(Gamer gamer, out CellPosition[] winCells)
        {
            //ожидание от урока
            CellState expectedCellState;
            if (gamer == Gamer.X)
                expectedCellState = CellState.X;
            else
                expectedCellState = CellState.O;

            for (int row = 0; row < RowCount; row++)
            {
                var expectedCellsCount = 0;
                for (int column = 0; column < ColumnCount; column++)
                {
                    if (Cells[row, column] == expectedCellState)
                    {
                        expectedCellsCount++;
                    }
                }
                if(expectedCellsCount == 3)
                {
                    winCells = new CellPosition[]
                    {
                        new CellPosition(Row: row, Column: 0),
                        new CellPosition(Row: row, Column: 1),
                        new CellPosition(Row: row, Column: 2),
                    };
                    return true;
                }
            }

            for (int column = 0; column < RowCount; column++)
            {
                var expectedCellsCount = 0;
                for (int row = 0; row < ColumnCount; row++)
                {
                    if (Cells[row, column] == expectedCellState)
                    {
                        expectedCellsCount++;
                    }
                }
                if (expectedCellsCount == 3)
                {
                    winCells = new CellPosition[]
                    {
                        new CellPosition(Row: 0, Column: column),
                        new CellPosition(Row: 1, Column: column),
                        new CellPosition(Row: 2, Column: column),
                    };
                    return true;
                }
            }

            int expectedCellsCountInDiagonal = 0;
            int r, c;
            for (r = 0, c = 0; r < RowCount && c < ColumnCount; r++, c++)
            {
                if (Cells[r, c] == expectedCellState) 
                {
                    expectedCellsCountInDiagonal++; 
                }
                if(expectedCellsCountInDiagonal == 3)
                {
                    winCells = new CellPosition[]
                    {
                        new CellPosition(Row: 0, Column: 0),
                        new CellPosition(Row: 1, Column: 1),
                        new CellPosition(Row: 2, Column: 2),
                    };
                    return true;
                }
            }

            expectedCellsCountInDiagonal = 0;
            for (r = 0, c = ColumnCount - 1; r < RowCount && c >= 0; r++, c--)
            {
                if (Cells[r, c] == expectedCellState)
                {
                    expectedCellsCountInDiagonal++;
                }
                if (expectedCellsCountInDiagonal == 3)
                {
                    winCells = new CellPosition[]
                    {
                        new CellPosition(Row: 0, Column: 2),
                        new CellPosition(Row: 1, Column: 1),
                        new CellPosition(Row: 2, Column: 0),
                    };
                    return true;
                }
            }

            winCells = new CellPosition[0];
            return false;
        }
    }

    public record CellPosition(int Row, int Column);
}
