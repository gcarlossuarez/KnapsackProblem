using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KnapsackProblem;

public class CeldaMochila : INotifyPropertyChanged
{
	private string _valor = "";

	private string _colorFondo = "White";

	public int Fila { get; set; }

	public int Columna { get; set; }

	public TipoCelda Tipo { get; set; }

	public string Valor
	{
		get
		{
			return _valor;
		}
		set
		{
			_valor = value;
			OnPropertyChanged("Valor");
		}
	}

	public string ColorFondo
	{
		get
		{
			return _colorFondo;
		}
		set
		{
			_colorFondo = value;
			OnPropertyChanged("ColorFondo");
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? name = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
