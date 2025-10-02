using System.Collections.Generic;

namespace KnapsackProblem;

public class FilaMochila
{
	public int Peso { get; set; }

	public int Valor { get; set; }

	public List<CeldaDP> Celdas { get; set; } = new List<CeldaDP>();
}
