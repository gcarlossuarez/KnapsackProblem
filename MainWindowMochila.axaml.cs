using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


namespace KnapsackProblem;

public partial class MainWindowMochila : Window
{
	private enum EstadoMochila
	{
		Inicio,
		LoopExterno,
		LoopInterno,
		Condicion,
		IncluirExcluir,
		Asignacion,
		Avanzar,
		Fin
	}


	private List<Item> _items = new List<Item>();

	private int _capacidad = 7;

	private int[,] _dp = new int[0, 0];

	private ObservableCollection<CeldaMochila> _tabla = new ObservableCollection<CeldaMochila>();

	private bool _modoJuego = false;

	private int _score = 0;

	private int _pasoI = 1; // Primer ítem

	private int _pasoW = 0; // Capcadiad 0

	private object _objStopResolverTodo = new object();

	private bool _stopResolverTodo = false;

	private object _lockResolviendoTodo = new object();

	private bool _resolviendoTodo = false;

	private EstadoMochila _estado = EstadoMochila.Inicio;

	public int NumFilas => _items.Count + 2;  // +1 para encabezado capacidades, +1 porque empieza en 0

	public int NumColumnas => _capacidad + 3; // col Peso + col Valor + capacidades

    private CancellationTokenSource _ctsResolverTodo = new CancellationTokenSource();
    

	private bool StopResolverTodo
	{
		get
		{
			lock (_objStopResolverTodo)
			{
				return _stopResolverTodo;
			}
		}
		set
		{
			lock (_objStopResolverTodo)
			{
				_stopResolverTodo = value;
			}
		}
	}

	private bool ResolviendoTodo
	{
		get
		{
			lock (_lockResolviendoTodo)
			{
				return _resolviendoTodo;
			}
		}
		set
		{
			lock (_lockResolviendoTodo)
			{
				_resolviendoTodo = value;
			}
		}
	}

	private void InitIndexesWeightValue()
	{
		_pasoI = 1;
		_pasoW = 0;
	}

	public MainWindowMochila()
	{
		InitializeComponent();
		InitKnapsack();
		HookEventos();
	}

	private void HookEventos()
	{
		PasoBtn.Click += async delegate
		{
            try
            {
			    await GeneralStepAlgorithmStep();
            }
            catch(OperationCanceledException )
            {
                //Console.WriteLine("Excepcion atrapada");
                InitKnapsack();
                DisableMainButtons();
                ReiniciarBtn.IsEnabled = true;
		    }
        };

		ResolverBtn.Click += async delegate
		{
			await SolveAll();
		};
		ModoBtn.Click += async delegate
		{
			_modoJuego = !_modoJuego;
			_score = 0;
			UpdateScore();
			InitKnapsack();
			ModoBtn.Content = (_modoJuego ? "Modo Juego" : "Modo Estudio");
            var dlg = new Window
            {
                Title = "Cambio de Modo",
                Width = 300,
                Height = 150,
                Content = new TextBlock
                {
                    Text = $"Se reinició en modo {(_modoJuego ? "Juego" : "Estudio")}.",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    FontSize = 16
                }
            };
            await dlg.ShowDialog(this);
            
		};
		ReiniciarBtn.Click += async delegate
		{
			int counter = 0;
			StopResolverTodo = true;
            //Console.WriteLine($"ResolviendoTodo:{ResolviendoTodo}");
			while (ResolviendoTodo && counter <= 100)
			{
				await Task.Delay(1000);
				int num = counter + 1;
				counter = num;
			}
			InitKnapsack();
			StopResolverTodo = false;
            EnableMainButtons();
		};
		RandomBtn.Click += async delegate
		{
			await Task.Delay(1000);
			GenerateRandomValues();
		};
	}

	private void GenerateRandomValues()
	{
		Random random = new Random();
		int num = random.Next(3, 7);
		_items = new List<Item>();
		for (int i = 0; i < num; i++)
		{
			int peso = random.Next(1, 10);
			int valor = random.Next(1, 20);
			_items.Add(new Item
			{
				Peso = peso,
				Valor = valor
			});
		}
		_capacidad = random.Next(8, 26);
		InitKnapsack();
		base.DataContext = null;
		base.DataContext = this;
		ItemsView.ItemsSource = _items.Select((Item it, int idx) => $"Item {idx + 1}: Peso={it.Peso}, Valor={it.Valor}");
		TablaView.ItemsSource = _tabla;
		InfoLabel.Text = $"Generado random: {_items.Count} ítems, capacidad = {_capacidad}";
	}

	private void InitKnapsack()
	{
        //Console.WriteLine("Dentro de InitKnapsack");
		if (_items.Count == 0)
		{
			_items = new List<Item>
			{
				new Item
				{
					Peso = 3,
					Valor = 4
				},
				new Item
				{
					Peso = 4,
					Valor = 5
				},
				new Item
				{
					Peso = 2,
					Valor = 3
				}
			};
			_capacidad = 7;
		}
		InitIndexesWeightValue();
		_dp = new int[_items.Count + 1, _capacidad + 1];
		for (int i = 0; i <= _items.Count; i++)
		{
			for (int j = 0; j <= _capacidad; j++)
			{
				_dp[i, j] = 0;
			}
		}
		_tabla.Clear();
		_tabla.Add(new CeldaMochila
		{
			Fila = 0,
			Columna = 0,
			Tipo = TipoCelda.Peso,
			Valor = "Peso",
			ColorFondo = "LightBlue"
		});
		_tabla.Add(new CeldaMochila
		{
			Fila = 0,
			Columna = 1,
			Tipo = TipoCelda.Valor,
			Valor = "Valor",
			ColorFondo = "LightGreen"
		});
		for (int k = 0; k <= _capacidad; k++)
		{
			_tabla.Add(new CeldaMochila
			{
				Fila = 0,
				Columna = k + 2,
				Tipo = TipoCelda.Encabezado,
				Valor = k.ToString(),
				ColorFondo = "LightYellow"
			});
		}
		for (int l = 1; l <= _items.Count; l++)
		{
			Item item = _items[l - 1];
			_tabla.Add(new CeldaMochila
			{
				Fila = l,
				Columna = 0,
				Tipo = TipoCelda.Peso,
				Valor = item.Peso.ToString(),
				ColorFondo = "LightBlue"
			});
			_tabla.Add(new CeldaMochila
			{
				Fila = l,
				Columna = 1,
				Tipo = TipoCelda.Valor,
				Valor = item.Valor.ToString(),
				ColorFondo = "LightGreen"
			});
			for (int m = 0; m <= _capacidad; m++)
			{
				_tabla.Add(new CeldaMochila
				{
					Fila = l,
					Columna = m + 2,
					Tipo = TipoCelda.DP,
					Valor = "0"
				});
			}
		}
		TablaView.ItemsSource = _tabla;
		base.DataContext = this;
		ItemsView.ItemsSource = _items.Select((Item it, int idx) => $"Item {idx + 1}: Peso={it.Peso}, Valor={it.Valor}");
		InitIndexesWeightValue();
		ResultadoLabel.Text = "Resultado: -";
		InfoLabel.Text = "Listo para comenzar";
		CleanLineHighlight();
		_estado = EstadoMochila.Inicio;

        MedalLabel.Text = "-";
        ScoreLabel.Text = "-";
        ScoreProgress.Value = 0;

        _ctsResolverTodo = new CancellationTokenSource();
	}

	private async Task GeneralStep()
	{
		if (_pasoI > _items.Count)
		{
			return;
		}
		EnableDisabledButtosAfectedByPass(isEnabled: false);
		if (_pasoW > _capacidad)
		{
			_pasoI++;
			_pasoW = 0;
			if (_pasoI > _items.Count)
			{
				return;
			}
		}
		await StepAlgorithm();
		_pasoW++;
		EnableDisabledButtosAfectedByPass(isEnabled: true);
	}

	private async Task GeneralStepAlgorithmStep()
	{
		if (_pasoI <= _items.Count)
		{
			if (_pasoW <= _capacidad || _pasoI <= _items.Count)
			{
			    EnableDisabledButtosAfectedByPass(isEnabled: false);
				await StepAlgorithm();
				EnableDisabledButtosAfectedByPass(isEnabled: true);
			}
		}
	}

	private async Task PasoEstudio(int i, int w)
	{
		Item item = _items[i - 1];
		int valor;
		if (item.Peso <= w)
		{
			int incluir = ((w - item.Peso >= 0) ? (_dp[i - 1, w - item.Peso] + item.Valor) : 0);
			int excluir = _dp[i - 1, w];
			valor = Math.Max(excluir, incluir);
		}
		else
		{
			valor = _dp[i - 1, w];
		}
		_dp[i, w] = valor;
		CeldaMochila celdaPeso = _tabla.First((CeldaMochila c) => c.Fila == i && c.Columna == 0);
		CeldaMochila celdaValor = _tabla.First((CeldaMochila c) => c.Fila == i && c.Columna == 1);
		CeldaMochila celdaCapacidad = _tabla.First((CeldaMochila c) => c.Fila == 0 && c.Columna == w + 2);
		CeldaMochila celdaDP = _tabla.First((CeldaMochila c) => c.Fila == i && c.Columna == w + 2);
		celdaPeso.ColorFondo = "LightBlue";
		celdaValor.ColorFondo = "LightGreen";
		celdaCapacidad.ColorFondo = "LightYellow";
		celdaDP.Valor = valor.ToString();
		celdaDP.ColorFondo = "LightGreen";
		await Task.Delay(500);
		celdaDP.ColorFondo = "White";
		celdaCapacidad.ColorFondo = "LightYellow";
		celdaPeso.ColorFondo = "LightBlue";
		celdaValor.ColorFondo = "LightGreen";
		InfoLabel.Text = $"Estudio: Item {i} (Peso={item.Peso}, Valor={item.Valor}), Capacidad {w}, DP={valor}";
	}

	private void EnableDisabledButtosAfectedByPass(bool isEnabled)
	{
		PasoBtn.IsEnabled = isEnabled;
		ReiniciarBtn.IsEnabled = isEnabled;
		ResolverBtn.IsEnabled = isEnabled;
		ModoBtn.IsEnabled = isEnabled;
		RandomBtn.IsEnabled = isEnabled;
	}

	private async Task PasoJuego(int i, int w)
	{
		Item item = _items[i - 1];
		int valorCorrecto;
		if (item.Peso <= w)
		{
			int incluir = ((w - item.Peso >= 0) ? (_dp[i - 1, w - item.Peso] + item.Valor) : 0);
			int excluir = _dp[i - 1, w];
			valorCorrecto = Math.Max(excluir, incluir);
		}
		else
		{
			valorCorrecto = _dp[i - 1, w];
		}
		CeldaMochila celdaPeso = _tabla.First((CeldaMochila c) => c.Fila == i && c.Columna == 0);
		CeldaMochila celdaValor = _tabla.First((CeldaMochila c) => c.Fila == i && c.Columna == 1);
		CeldaMochila celdaCapacidad = _tabla.First((CeldaMochila c) => c.Fila == 0 && c.Columna == w + 2);
		CeldaMochila celdaDP = _tabla.First((CeldaMochila c) => c.Fila == i && c.Columna == w + 2);
		celdaPeso.ColorFondo = "LightBlue";
		celdaValor.ColorFondo = "LightGreen";
		celdaCapacidad.ColorFondo = "LightYellow";
		celdaDP.ColorFondo = "Cyan";
		await Task.Delay(800);
		if (int.TryParse(await InputBox($"Item {i} (Peso={item.Peso}, Valor={item.Valor}), Capacidad {w}.\n¿Cuál es el valor óptimo?"), out var guess) && guess == valorCorrecto)
		{
			_score += 10;
			celdaDP.Valor = valorCorrecto.ToString();
			celdaDP.ColorFondo = "LightGreen";
			InfoLabel.Text = $"✔ Correcto: {valorCorrecto}";
		}
		else
		{
			_score -= 5;
			celdaDP.Valor = valorCorrecto.ToString();
			celdaDP.ColorFondo = "LightPink";
			InfoLabel.Text = $"✘ Incorrecto. Correcto={valorCorrecto}";
		}
        _ctsResolverTodo.Token.ThrowIfCancellationRequested(); // Si se activo el token de cancelación

		UpdateScore();
		await Task.Delay(800);
		celdaDP.ColorFondo = "White";
		celdaCapacidad.ColorFondo = "LightYellow";
		celdaPeso.ColorFondo = "LightBlue";
		celdaValor.ColorFondo = "LightGreen";
		_dp[i, w] = valorCorrecto;
	}

	private async Task ResaltarLineaPseudo(TextBlock linea)
	{
		CleanLineHighlight();
		linea.Background = Brushes.Yellow;
		await Task.Delay(100);
	}

	private void CleanLineHighlight()
	{
		LineInicio.Background = Brushes.Transparent;
		LineLoopExterno.Background = Brushes.Transparent;
		LineLoopInterno.Background = Brushes.Transparent;
		LineCondicion.Background = Brushes.Transparent;
		LineIncluirExcluir.Background = Brushes.Transparent;
		LineElse.Background = Brushes.Transparent;
		LineFin.Background = Brushes.Transparent;
	}

	private async Task SolveAll()
	{
        //Console.WriteLine($"ResolviendoTodo:{ResolviendoTodo}");
		if (ResolviendoTodo)
		{
			return;
		}
		ResolviendoTodo = true;
        DisableMainButtons();
        try
        {
		    while (_pasoI <= _items.Count && !StopResolverTodo)
		    {
			    while (_pasoW <= _capacidad && !StopResolverTodo)
			    {
				    if (_modoJuego)
				    {
					    await PasoJuego(_pasoI, _pasoW);
				    }
				    else
				    {
					    await PasoEstudio(_pasoI, _pasoW);
				    }
				    _pasoW++;
			    }
			    _pasoW = 0;
			    _pasoI++;
		    }
		    ShowSolution();
            EnableMainButtons();
        }
        catch(OperationCanceledException )
        {
            //Console.WriteLine("Excepcion atrapada");
            InitKnapsack();
        }
        finally
        {
            StopResolverTodo = false;
		    ResolviendoTodo = false;
        }
	}


    /// <summary>
    /// Deshabilita los botobes principlaes
    /// </summary>
    private void DisableMainButtons()
    {
		PasoBtn.IsEnabled = false;
		ResolverBtn.IsEnabled = false;
		ModoBtn.IsEnabled = false;
		RandomBtn.IsEnabled = false;
    }

    /// <summary>
    /// Habilita los botobes principlaes
    /// </summary>
    private void EnableMainButtons()
    {
        PasoBtn.IsEnabled = true;
        ResolverBtn.IsEnabled = true;
        ModoBtn.IsEnabled = true;
        RandomBtn.IsEnabled = true;
    }

	private async Task<string> InputBox(string mensaje)
	{
		Window dlg = new Window
		{
			Title = "Adivina",
			Width = 300.0,
			Height = 150.0
		};
		StackPanel panel = new StackPanel
		{
			Margin = new Thickness(10.0)
		};
		TextBox txt = new TextBox
		{
			Watermark = "Ingresa número"
		};
		Button btnOk = new Button
		{
			Content = "Aceptar",
			Margin = new Thickness(0.0, 10.0, 0.0, 0.0)
		};
        var btnCancelGame = new Button
        {
            Content = "Cancelar Juego",
            Margin = new Avalonia.Thickness(10,0,10,0)
        };
		string result = "";

        // Manejador del evento Click del botón
		btnOk.Click += delegate
		{
			result = txt.Text ?? "";
			dlg.Close();
		};

        // Manejador del evento Click del botón
        btnCancelGame.Click += (_,__) =>
        {
            result = "Cancel";

            // Cancelar inmediatamente cualquier operación en curso
            StopResolverTodo = true;
            _ctsResolverTodo.Cancel();
            dlg.Close();
        };

        // Manejador para el evento KeyDown del TextBox
        txt.KeyDown += (sender, e) =>
        {
            // Se valida si la tecla presionada es "ENTER"
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                // Si pulsó enter. se asigna el valor i se cierra el formulario
                result = txt.Text ?? "";
                dlg.Close();
            }
        };

        // Establecer el foco en el TextBox cuando el diálogo se abra
        dlg.Opened += (sender, e) =>
        {
            txt.Focus();
        };

		panel.Children.Add(new TextBlock
		{
			Text = mensaje,
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
		});
		panel.Children.Add(txt);
		panel.Children.Add(btnOk);
		panel.Children.Add(btnCancelGame);
		dlg.Content = panel;
		await dlg.ShowDialog(this);
		return result;
	}

	private void ShowSolution()
	{
		int num = _items.Count;
		int num2 = _capacidad;
		List<int> list = new List<int>();
		while (num > 0 && num2 > 0)
		{
			if (_dp[num, num2] != _dp[num - 1, num2])
			{
				list.Add(num - 1);
				num2 -= _items[num - 1].Peso;
			}
			num--;
		}
		list.Reverse();
		int value = list.Sum((int idx) => _items[idx].Peso);
		int value2 = list.Sum((int idx) => _items[idx].Valor);
		string value3 = string.Join(", ", list.Select((int idx) => $"Item{idx + 1}"));
		ResultadoLabel.Text = $"Resultado: Valor={value2}, Peso={value}, Usados= {value3}";
	}

	private async Task StepAlgorithm()
	{
		switch (_estado)
		{
		case EstadoMochila.Inicio:
			InfoLabel.Text = "Inicializando tabla dp en ceros...";
			InitKnapsack();
			await ResaltarLineaPseudo(LineInicio);
			_estado = EstadoMochila.LoopExterno;
			break;
		case EstadoMochila.LoopExterno:
			await ResaltarLineaPseudo(LineLoopExterno);
			InfoLabel.Text = $"Procesando ítem {_pasoI}";
			_estado = EstadoMochila.LoopInterno;
			break;
		case EstadoMochila.LoopInterno:
			InfoLabel.Text = $"Capacidad actual w={_pasoW}";
			await ResaltarLineaPseudo(LineLoopInterno);
			_estado = EstadoMochila.Condicion;
			break;
		case EstadoMochila.Condicion:
		{
			Item item = _items[_pasoI - 1];
			await ResaltarLineaPseudo(LineCondicion);
			if (item.Peso <= _pasoW)
			{
				InfoLabel.Text = $"El peso {item.Peso} cabe en capacidad {_pasoW}";
				_estado = EstadoMochila.IncluirExcluir;
			}
			else
			{
				InfoLabel.Text = $"El peso {item.Peso} no cabe, copiar valor de arriba.";
				_estado = EstadoMochila.Asignacion;
			}
			break;
		}
		case EstadoMochila.IncluirExcluir:
			await ResaltarLineaPseudo(LineIncluirExcluir);
			InfoLabel.Text = "Comparando incluir vs excluir...";
			_estado = EstadoMochila.Asignacion;
			break;
		case EstadoMochila.Asignacion:
			InfoLabel.Text = $"Asignando dp[{_pasoI},{_pasoW}]...";
			if (!_modoJuego)
			{
				await PasoEstudio(_pasoI, _pasoW);
			}
			else
			{
				await PasoJuego(_pasoI, _pasoW);
			}
			_estado = EstadoMochila.Avanzar;
			break;
		case EstadoMochila.Avanzar:
			await ResaltarLineaPseudo(LineLoopInterno);
			_pasoW++;
			if (_pasoW > _capacidad)
			{
				_pasoW = 0;
				_pasoI++;
				if (_pasoI > _items.Count)
				{
					_estado = EstadoMochila.Fin;
					break;
				}
			}
			_estado = EstadoMochila.LoopInterno;
			break;
		case EstadoMochila.Fin:
			await ResaltarLineaPseudo(LineFin);
			InfoLabel.Text = $"Algoritmo terminado. Mejor valor = {_dp[_items.Count, _capacidad]}";
			break;
		}
	}

	private void UpdateScore()
	{
		ScoreLabel.Text = $"Puntaje: {_score}";
		int num = Math.Clamp(_score, 0, 100);
		ScoreProgress.Value = num;
		string value = ((num >= 90) ? "\ud83e\udd47 Oro" : ((num >= 70) ? "\ud83e\udd48 Plata" : ((num < 50) ? "❌ Sin medalla" : "\ud83e\udd49 Bronce")));
		MedalLabel.Text = $"Medalla: {value} ({num}%)";
	}

}


