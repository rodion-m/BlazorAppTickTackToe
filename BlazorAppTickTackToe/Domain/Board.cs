namespace BlazorAppTickTackToe.Domain
{
    public enum CellState
    {
        Blank, X, O
    }

    public class Board
    {
        public int ColumnCount => 3;
        public int RowCount => 3;

        public CellState Cell_0_0 { get; set; } = CellState.X;
        public CellState Cell_0_1 { get; set; }
        public CellState Cell_0_2 { get; set; }
        public CellState Cell_1_0 { get; set; }
        public CellState Cell_1_1 { get; set; }
        public CellState Cell_1_2 { get; set; }
        public CellState Cell_2_0 { get; set; }
        public CellState Cell_2_1 { get; set; }
        public CellState Cell_2_2 { get; set; }


    }
}
