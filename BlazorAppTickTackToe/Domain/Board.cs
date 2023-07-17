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
}
