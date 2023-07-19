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
        WonX, WonO, NoWinner
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
        public void NextTurn(int row, int column)
        {
            if (Cells[row, column] == CellState.Blank)
            {
                if (CurrentGamer == Gamer.X)
                {
                    Cells[row, column] = CellState.X;
                    SwitchGamer();
                }
                else
                {
                    Cells[row, column] = CellState.O;
                    SwitchGamer();
                }
            }
        }

        private void SwitchGamer()
        {
            //тернарный оператор
            CurrentGamer = CurrentGamer == Gamer.X ? Gamer.O : Gamer.X;
        }

        // TODO: победа по события.
        // Задание: запрограммируйте проверку 8 вариантов победы крестиков
        public GameResult GetGameResult()
        {
            if (CheckWin(Gamer.X))
            {
                return GameResult.WonX;
            }
            else if (CheckWin(Gamer.O))
            {
                return GameResult.WonO;
            }
            else
            {
                return GameResult.NoWinner;
            }
        }

        private bool CheckWin(Gamer gamer)
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
                    return true;
                }
            }

            return false;
        }
    }
}
