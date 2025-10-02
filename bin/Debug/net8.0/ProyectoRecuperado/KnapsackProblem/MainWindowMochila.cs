using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using CompiledAvaloniaXaml;

namespace KnapsackProblem;

public class MainWindowMochila : Window
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

	private class XamlClosure_1
	{
		public static object Build_1(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila> context = CreateContext(P_0);
			context.IntermediateRoot = new TextBlock();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			TextBlock textBlock = (TextBlock)intermediateRoot;
			context.PushParent(textBlock);
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			ReflectionBindingExtension reflectionBindingExtension = new ReflectionBindingExtension();
			context.ProvideTargetProperty = TextBlock.TextProperty;
			Binding binding = reflectionBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			AvaloniaObjectExtensions.Bind(textBlock, textProperty, binding);
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}

		public static CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila> CreateContext(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila> context = new CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FMainWindowMochila_002Eaxaml.Singleton }, "avares://KnapsackProblem/MainWindowMochila.axaml");
			if (P_0 != null)
			{
				object service = P_0.GetService(typeof(IRootObjectProvider));
				if (service != null)
				{
					service = ((IRootObjectProvider)service).RootObject;
					context.RootObject = (MainWindowMochila)service;
				}
			}
			return context;
		}

		public static object Build_2(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila> context = CreateContext(P_0);
			context.IntermediateRoot = new UniformGrid();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			UniformGrid uniformGrid = (UniformGrid)intermediateRoot;
			context.PushParent(uniformGrid);
			StyledProperty<int> rowsProperty = UniformGrid.RowsProperty;
			ReflectionBindingExtension reflectionBindingExtension = new ReflectionBindingExtension("NumFilas");
			context.ProvideTargetProperty = UniformGrid.RowsProperty;
			Binding binding = reflectionBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			AvaloniaObjectExtensions.Bind(uniformGrid, rowsProperty, binding);
			StyledProperty<int> columnsProperty = UniformGrid.ColumnsProperty;
			ReflectionBindingExtension reflectionBindingExtension2 = new ReflectionBindingExtension("NumColumnas");
			context.ProvideTargetProperty = UniformGrid.ColumnsProperty;
			Binding binding2 = reflectionBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			AvaloniaObjectExtensions.Bind(uniformGrid, columnsProperty, binding2);
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}

		public static object Build_3(IServiceProvider P_0)
		{
			CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila> context = CreateContext(P_0);
			context.IntermediateRoot = new Border();
			object intermediateRoot = context.IntermediateRoot;
			((ISupportInitialize)intermediateRoot).BeginInit();
			Border border = (Border)intermediateRoot;
			context.PushParent(border);
			border.BorderBrush = new ImmutableSolidColorBrush(4278190080u);
			border.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
			StyledProperty<IBrush?> backgroundProperty = Border.BackgroundProperty;
			ReflectionBindingExtension reflectionBindingExtension = new ReflectionBindingExtension("ColorFondo");
			context.ProvideTargetProperty = Border.BackgroundProperty;
			Binding binding = reflectionBindingExtension.ProvideValue(context);
			context.ProvideTargetProperty = null;
			AvaloniaObjectExtensions.Bind(border, backgroundProperty, binding);
			TextBlock textBlock2;
			TextBlock textBlock = (textBlock2 = new TextBlock());
			((ISupportInitialize)textBlock).BeginInit();
			border.Child = textBlock;
			TextBlock textBlock4;
			TextBlock textBlock3 = (textBlock4 = textBlock2);
			context.PushParent(textBlock4);
			StyledProperty<string?> textProperty = TextBlock.TextProperty;
			ReflectionBindingExtension reflectionBindingExtension2 = new ReflectionBindingExtension("Valor");
			context.ProvideTargetProperty = TextBlock.TextProperty;
			Binding binding2 = reflectionBindingExtension2.ProvideValue(context);
			context.ProvideTargetProperty = null;
			AvaloniaObjectExtensions.Bind(textBlock4, textProperty, binding2);
			textBlock4.HorizontalAlignment = HorizontalAlignment.Center;
			textBlock4.VerticalAlignment = VerticalAlignment.Center;
			context.PopParent();
			((ISupportInitialize)textBlock3).EndInit();
			context.PopParent();
			((ISupportInitialize)intermediateRoot).EndInit();
			return intermediateRoot;
		}
	}

	private List<Item> _items = new List<Item>();

	private int _capacidad = 7;

	private int[,] _dp = new int[0, 0];

	private ObservableCollection<CeldaMochila> _tabla = new ObservableCollection<CeldaMochila>();

	private bool _modoJuego = false;

	private int _score = 0;

	private int _pasoI = 1;

	private int _pasoW = 0;

	private object _objStopResolverTodo = new object();

	private bool _stopResolverTodo = false;

	private object _lockResolviendoTodo = new object();

	private bool _resolviendoTodo = false;

	private EstadoMochila _estado = EstadoMochila.Inicio;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal Button PasoBtn;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal Button ResolverBtn;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal Button ReiniciarBtn;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal Button RandomBtn;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal Button ModoBtn;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineInicio;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineLoopExterno;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineLoopInterno;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineCondicion;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineIncluirExcluir;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineElse;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock LineFin;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock InfoLabel;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal ItemsControl ItemsView;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock ResultadoLabel;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal DataGrid TablaDP;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal ProgressBar ScoreProgress;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock ScoreLabel;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal TextBlock MedalLabel;

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	internal ItemsControl TablaView;

	private static Action<object> _0021XamlIlPopulateOverride;

	public int NumFilas => _items.Count + 2;

	public int NumColumnas => _capacidad + 3;

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
		InicializarMochila();
		HookEventos();
	}

	private void HookEventos()
	{
		PasoBtn.Click += async delegate
		{
			await PasoGeneralPasoAlgoritmo();
		};
		ResolverBtn.Click += async delegate
		{
			await ResolverTodo();
		};
		ModoBtn.Click += delegate
		{
			_modoJuego = !_modoJuego;
			_score = 0;
			UpdateScore();
			InicializarMochila();
			ModoBtn.Content = (_modoJuego ? "Modo Juego" : "Modo Estudio");
		};
		ReiniciarBtn.Click += async delegate
		{
			int counter = 0;
			StopResolverTodo = true;
			while (ResolviendoTodo && counter <= 100)
			{
				await Task.Delay(1000);
				int num = counter + 1;
				counter = num;
			}
			InicializarMochila();
			StopResolverTodo = false;
		};
		RandomBtn.Click += async delegate
		{
			await Task.Delay(1000);
			GenerarRandom();
		};
	}

	private void GenerarRandom()
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
		InicializarMochila();
		base.DataContext = null;
		base.DataContext = this;
		ItemsView.ItemsSource = _items.Select((Item it, int idx) => $"Item {idx + 1}: Peso={it.Peso}, Valor={it.Valor}");
		TablaView.ItemsSource = _tabla;
		InfoLabel.Text = $"Generado random: {_items.Count} ítems, capacidad = {_capacidad}";
	}

	private void InicializarMochila()
	{
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
	}

	private async Task PasoGeneral()
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
		await PasoAlgoritmo();
		_pasoW++;
		EnableDisabledButtosAfectedByPass(isEnabled: true);
	}

	private async Task PasoGeneralPasoAlgoritmo()
	{
		if (_pasoI <= _items.Count)
		{
			EnableDisabledButtosAfectedByPass(isEnabled: false);
			if (_pasoW <= _capacidad || _pasoI <= _items.Count)
			{
				await PasoAlgoritmo();
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

	private async Task ResolverTodo()
	{
		if (ResolviendoTodo)
		{
			return;
		}
		ResolviendoTodo = true;
		PasoBtn.IsEnabled = false;
		ResolverBtn.IsEnabled = false;
		ModoBtn.IsEnabled = false;
		RandomBtn.IsEnabled = false;
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
		MostrarSolucion();
		PasoBtn.IsEnabled = true;
		ResolverBtn.IsEnabled = true;
		ModoBtn.IsEnabled = true;
		RandomBtn.IsEnabled = true;
		ResolviendoTodo = false;
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
		Button btn = new Button
		{
			Content = "Aceptar",
			Margin = new Thickness(0.0, 10.0, 0.0, 0.0)
		};
		string result = "";
		btn.Click += delegate
		{
			result = txt.Text ?? "";
			dlg.Close();
		};
		panel.Children.Add(new TextBlock
		{
			Text = mensaje,
			Margin = new Thickness(0.0, 0.0, 0.0, 10.0)
		});
		panel.Children.Add(txt);
		panel.Children.Add(btn);
		dlg.Content = panel;
		await dlg.ShowDialog(this);
		return result;
	}

	private void MostrarSolucion()
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

	private async Task PasoAlgoritmo()
	{
		switch (_estado)
		{
		case EstadoMochila.Inicio:
			InfoLabel.Text = "Inicializando tabla dp en ceros...";
			InicializarMochila();
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

	[GeneratedCode("Avalonia.Generators.NameGenerator.InitializeComponentCodeGenerator", "11.1.0.0")]
	[ExcludeFromCodeCoverage]
	public void InitializeComponent(bool loadXaml = true)
	{
		if (loadXaml)
		{
			_0021XamlIlPopulateTrampoline(this);
		}
		INameScope nameScope = this.FindNameScope();
		PasoBtn = nameScope?.Find<Button>("PasoBtn");
		ResolverBtn = nameScope?.Find<Button>("ResolverBtn");
		ReiniciarBtn = nameScope?.Find<Button>("ReiniciarBtn");
		RandomBtn = nameScope?.Find<Button>("RandomBtn");
		ModoBtn = nameScope?.Find<Button>("ModoBtn");
		LineInicio = nameScope?.Find<TextBlock>("LineInicio");
		LineLoopExterno = nameScope?.Find<TextBlock>("LineLoopExterno");
		LineLoopInterno = nameScope?.Find<TextBlock>("LineLoopInterno");
		LineCondicion = nameScope?.Find<TextBlock>("LineCondicion");
		LineIncluirExcluir = nameScope?.Find<TextBlock>("LineIncluirExcluir");
		LineElse = nameScope?.Find<TextBlock>("LineElse");
		LineFin = nameScope?.Find<TextBlock>("LineFin");
		InfoLabel = nameScope?.Find<TextBlock>("InfoLabel");
		ItemsView = nameScope?.Find<ItemsControl>("ItemsView");
		ResultadoLabel = nameScope?.Find<TextBlock>("ResultadoLabel");
		TablaDP = nameScope?.Find<DataGrid>("TablaDP");
		ScoreProgress = nameScope?.Find<ProgressBar>("ScoreProgress");
		ScoreLabel = nameScope?.Find<TextBlock>("ScoreLabel");
		MedalLabel = nameScope?.Find<TextBlock>("MedalLabel");
		TablaView = nameScope?.Find<ItemsControl>("TablaView");
	}

	private unsafe static void _0021XamlIlPopulate(IServiceProvider P_0, MainWindowMochila P_1)
	{
		CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila> context = new CompiledAvaloniaXaml.XamlIlContext.Context<MainWindowMochila>(P_0, new object[1] { _0021AvaloniaResources.NamespaceInfo_003A_002FMainWindowMochila_002Eaxaml.Singleton }, "avares://KnapsackProblem/MainWindowMochila.axaml");
		context.RootObject = P_1;
		context.IntermediateRoot = P_1;
		((ISupportInitialize)P_1).BeginInit();
		context.PushParent(P_1);
		P_1.Title = "Problema de la Mochila";
		P_1.Width = 800.0;
		P_1.Height = 600.0;
		StackPanel stackPanel2;
		StackPanel stackPanel = (stackPanel2 = new StackPanel());
		((ISupportInitialize)stackPanel).BeginInit();
		P_1.Content = stackPanel;
		StackPanel stackPanel4;
		StackPanel stackPanel3 = (stackPanel4 = stackPanel2);
		context.PushParent(stackPanel4);
		StackPanel stackPanel5 = stackPanel4;
		stackPanel5.Margin = new Thickness(10.0, 10.0, 10.0, 10.0);
		Controls children = stackPanel5.Children;
		TextBlock textBlock2;
		TextBlock textBlock = (textBlock2 = new TextBlock());
		((ISupportInitialize)textBlock).BeginInit();
		children.Add(textBlock);
		textBlock2.Text = "Problema de la Mochila 0/1";
		textBlock2.FontSize = 20.0;
		textBlock2.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
		((ISupportInitialize)textBlock2).EndInit();
		Controls children2 = stackPanel5.Children;
		WrapPanel wrapPanel2;
		WrapPanel wrapPanel = (wrapPanel2 = new WrapPanel());
		((ISupportInitialize)wrapPanel).BeginInit();
		children2.Add(wrapPanel);
		wrapPanel2.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
		Controls children3 = wrapPanel2.Children;
		Button button2;
		Button button = (button2 = new Button());
		((ISupportInitialize)button).BeginInit();
		children3.Add(button);
		button2.Name = "PasoBtn";
		object element = button2;
		context.AvaloniaNameScope.Register("PasoBtn", element);
		button2.Content = "Paso";
		button2.Margin = new Thickness(5.0, 5.0, 5.0, 5.0);
		((ISupportInitialize)button2).EndInit();
		Controls children4 = wrapPanel2.Children;
		Button button4;
		Button button3 = (button4 = new Button());
		((ISupportInitialize)button3).BeginInit();
		children4.Add(button3);
		button4.Name = "ResolverBtn";
		element = button4;
		context.AvaloniaNameScope.Register("ResolverBtn", element);
		button4.Content = "Resolver Todo";
		button4.Margin = new Thickness(5.0, 5.0, 5.0, 5.0);
		((ISupportInitialize)button4).EndInit();
		Controls children5 = wrapPanel2.Children;
		Button button6;
		Button button5 = (button6 = new Button());
		((ISupportInitialize)button5).BeginInit();
		children5.Add(button5);
		button6.Name = "ReiniciarBtn";
		element = button6;
		context.AvaloniaNameScope.Register("ReiniciarBtn", element);
		button6.Content = "Reiniciar";
		button6.Margin = new Thickness(5.0, 5.0, 5.0, 5.0);
		((ISupportInitialize)button6).EndInit();
		Controls children6 = wrapPanel2.Children;
		Button button8;
		Button button7 = (button8 = new Button());
		((ISupportInitialize)button7).BeginInit();
		children6.Add(button7);
		button8.Name = "RandomBtn";
		element = button8;
		context.AvaloniaNameScope.Register("RandomBtn", element);
		button8.Content = "Generar Aleatorio";
		button8.Margin = new Thickness(5.0, 5.0, 5.0, 5.0);
		((ISupportInitialize)button8).EndInit();
		Controls children7 = wrapPanel2.Children;
		Button button10;
		Button button9 = (button10 = new Button());
		((ISupportInitialize)button9).BeginInit();
		children7.Add(button9);
		button10.Name = "ModoBtn";
		element = button10;
		context.AvaloniaNameScope.Register("ModoBtn", element);
		button10.Content = "Modo Estudio";
		button10.Margin = new Thickness(5.0, 5.0, 5.0, 5.0);
		((ISupportInitialize)button10).EndInit();
		((ISupportInitialize)wrapPanel2).EndInit();
		Controls children8 = stackPanel5.Children;
		Border border2;
		Border border = (border2 = new Border());
		((ISupportInitialize)border).BeginInit();
		children8.Add(border);
		border2.BorderBrush = new ImmutableSolidColorBrush(4286611584u);
		border2.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
		border2.Margin = new Thickness(0.0, 10.0, 0.0, 10.0);
		border2.Padding = new Thickness(5.0, 5.0, 5.0, 5.0);
		StackPanel stackPanel7;
		StackPanel stackPanel6 = (stackPanel7 = new StackPanel());
		((ISupportInitialize)stackPanel6).BeginInit();
		border2.Child = stackPanel6;
		Controls children9 = stackPanel7.Children;
		TextBlock textBlock4;
		TextBlock textBlock3 = (textBlock4 = new TextBlock());
		((ISupportInitialize)textBlock3).BeginInit();
		children9.Add(textBlock3);
		textBlock4.Text = "Algoritmo 0/1 Knapsack";
		textBlock4.FontWeight = FontWeight.Bold;
		textBlock4.Margin = new Thickness(0.0, 0.0, 0.0, 5.0);
		((ISupportInitialize)textBlock4).EndInit();
		Controls children10 = stackPanel7.Children;
		TextBlock textBlock6;
		TextBlock textBlock5 = (textBlock6 = new TextBlock());
		((ISupportInitialize)textBlock5).BeginInit();
		children10.Add(textBlock5);
		textBlock6.Name = "LineInicio";
		element = textBlock6;
		context.AvaloniaNameScope.Register("LineInicio", element);
		textBlock6.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock6.FontSize = 14.0;
		textBlock6.Inlines.Add("1. Crear tabla dp[0..n,0..W] = 0");
		((ISupportInitialize)textBlock6).EndInit();
		Controls children11 = stackPanel7.Children;
		TextBlock textBlock8;
		TextBlock textBlock7 = (textBlock8 = new TextBlock());
		((ISupportInitialize)textBlock7).BeginInit();
		children11.Add(textBlock7);
		textBlock8.Name = "LineLoopExterno";
		element = textBlock8;
		context.AvaloniaNameScope.Register("LineLoopExterno", element);
		textBlock8.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock8.FontSize = 14.0;
		textBlock8.Inlines.Add("2. Para i = 1 hasta n:");
		((ISupportInitialize)textBlock8).EndInit();
		Controls children12 = stackPanel7.Children;
		TextBlock textBlock10;
		TextBlock textBlock9 = (textBlock10 = new TextBlock());
		((ISupportInitialize)textBlock9).BeginInit();
		children12.Add(textBlock9);
		textBlock10.Name = "LineLoopInterno";
		element = textBlock10;
		context.AvaloniaNameScope.Register("LineLoopInterno", element);
		textBlock10.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock10.FontSize = 14.0;
		textBlock10.Inlines.Add("3. Para w = 0 hasta W:");
		((ISupportInitialize)textBlock10).EndInit();
		Controls children13 = stackPanel7.Children;
		TextBlock textBlock12;
		TextBlock textBlock11 = (textBlock12 = new TextBlock());
		((ISupportInitialize)textBlock11).BeginInit();
		children13.Add(textBlock11);
		textBlock12.Name = "LineCondicion";
		element = textBlock12;
		context.AvaloniaNameScope.Register("LineCondicion", element);
		textBlock12.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock12.FontSize = 14.0;
		textBlock12.Inlines.Add("4. Si peso[i] <= w:");
		((ISupportInitialize)textBlock12).EndInit();
		Controls children14 = stackPanel7.Children;
		TextBlock textBlock14;
		TextBlock textBlock13 = (textBlock14 = new TextBlock());
		((ISupportInitialize)textBlock13).BeginInit();
		children14.Add(textBlock13);
		textBlock14.Name = "LineIncluirExcluir";
		element = textBlock14;
		context.AvaloniaNameScope.Register("LineIncluirExcluir", element);
		textBlock14.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock14.FontSize = 14.0;
		textBlock14.Inlines.Add("5. dp[i,w] = max(dp[i-1,w], valor[i] + dp[i-1,w-peso[i]])");
		((ISupportInitialize)textBlock14).EndInit();
		Controls children15 = stackPanel7.Children;
		TextBlock textBlock16;
		TextBlock textBlock15 = (textBlock16 = new TextBlock());
		((ISupportInitialize)textBlock15).BeginInit();
		children15.Add(textBlock15);
		textBlock16.Name = "LineElse";
		element = textBlock16;
		context.AvaloniaNameScope.Register("LineElse", element);
		textBlock16.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock16.FontSize = 14.0;
		textBlock16.Inlines.Add("6. En otro caso: dp[i,w] = dp[i-1,w]");
		((ISupportInitialize)textBlock16).EndInit();
		Controls children16 = stackPanel7.Children;
		TextBlock textBlock18;
		TextBlock textBlock17 = (textBlock18 = new TextBlock());
		((ISupportInitialize)textBlock17).BeginInit();
		children16.Add(textBlock17);
		textBlock18.Name = "LineFin";
		element = textBlock18;
		context.AvaloniaNameScope.Register("LineFin", element);
		textBlock18.FontFamily = new FontFamily(((IUriContext)context).BaseUri, "Consolas");
		textBlock18.FontSize = 14.0;
		textBlock18.Inlines.Add("7. Resultado = dp[n,W]");
		((ISupportInitialize)textBlock18).EndInit();
		((ISupportInitialize)stackPanel7).EndInit();
		((ISupportInitialize)border2).EndInit();
		Controls children17 = stackPanel5.Children;
		TextBlock textBlock20;
		TextBlock textBlock19 = (textBlock20 = new TextBlock());
		((ISupportInitialize)textBlock19).BeginInit();
		children17.Add(textBlock19);
		textBlock20.Name = "InfoLabel";
		element = textBlock20;
		context.AvaloniaNameScope.Register("InfoLabel", element);
		textBlock20.Text = "Info:";
		textBlock20.FontSize = 16.0;
		textBlock20.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
		((ISupportInitialize)textBlock20).EndInit();
		Controls children18 = stackPanel5.Children;
		StackPanel stackPanel9;
		StackPanel stackPanel8 = (stackPanel9 = new StackPanel());
		((ISupportInitialize)stackPanel8).BeginInit();
		children18.Add(stackPanel8);
		StackPanel stackPanel10 = (stackPanel4 = stackPanel9);
		context.PushParent(stackPanel4);
		StackPanel stackPanel11 = stackPanel4;
		stackPanel11.Margin = new Thickness(0.0, 10.0, 0.0, 10.0);
		Controls children19 = stackPanel11.Children;
		TextBlock textBlock22;
		TextBlock textBlock21 = (textBlock22 = new TextBlock());
		((ISupportInitialize)textBlock21).BeginInit();
		children19.Add(textBlock21);
		textBlock22.Text = "Ítems disponibles:";
		textBlock22.FontWeight = FontWeight.Bold;
		((ISupportInitialize)textBlock22).EndInit();
		Controls children20 = stackPanel11.Children;
		ItemsControl itemsControl2;
		ItemsControl itemsControl = (itemsControl2 = new ItemsControl());
		((ISupportInitialize)itemsControl).BeginInit();
		children20.Add(itemsControl);
		ItemsControl itemsControl4;
		ItemsControl itemsControl3 = (itemsControl4 = itemsControl2);
		context.PushParent(itemsControl4);
		ItemsControl itemsControl5 = itemsControl4;
		itemsControl5.Name = "ItemsView";
		element = itemsControl5;
		context.AvaloniaNameScope.Register("ItemsView", element);
		DataTemplate dataTemplate;
		DataTemplate itemTemplate = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV3<Control>((nint)(delegate*<IServiceProvider, object>)(&XamlClosure_1.Build_1), context);
		context.PopParent();
		itemsControl5.ItemTemplate = itemTemplate;
		context.PopParent();
		((ISupportInitialize)itemsControl3).EndInit();
		context.PopParent();
		((ISupportInitialize)stackPanel10).EndInit();
		Controls children21 = stackPanel5.Children;
		TextBlock textBlock24;
		TextBlock textBlock23 = (textBlock24 = new TextBlock());
		((ISupportInitialize)textBlock23).BeginInit();
		children21.Add(textBlock23);
		textBlock24.Name = "ResultadoLabel";
		element = textBlock24;
		context.AvaloniaNameScope.Register("ResultadoLabel", element);
		textBlock24.FontSize = 16.0;
		textBlock24.Margin = new Thickness(0.0, 10.0, 0.0, 10.0);
		textBlock24.Text = "Resultado: -";
		((ISupportInitialize)textBlock24).EndInit();
		Controls children22 = stackPanel5.Children;
		DataGrid dataGrid2;
		DataGrid dataGrid = (dataGrid2 = new DataGrid());
		((ISupportInitialize)dataGrid).BeginInit();
		children22.Add(dataGrid);
		DataGrid dataGrid4;
		DataGrid dataGrid3 = (dataGrid4 = dataGrid2);
		context.PushParent(dataGrid4);
		dataGrid4.Name = "TablaDP";
		element = dataGrid4;
		context.AvaloniaNameScope.Register("TablaDP", element);
		dataGrid4.AutoGenerateColumns = false;
		dataGrid4.HeadersVisibility = DataGridHeadersVisibility.All;
		dataGrid4.GridLinesVisibility = DataGridGridLinesVisibility.All;
		dataGrid4.IsReadOnly = true;
		dataGrid4.CanUserResizeColumns = false;
		ObservableCollection<DataGridColumn> columns = dataGrid4.Columns;
		DataGridTextColumn dataGridTextColumn;
		DataGridTextColumn item = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn2 = dataGridTextColumn;
		dataGridTextColumn2.Header = "Fila";
		ReflectionBindingExtension reflectionBindingExtension = new ReflectionBindingExtension("Fila");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		Binding binding = reflectionBindingExtension.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn2.Binding = binding;
		context.PopParent();
		columns.Add(item);
		ObservableCollection<DataGridColumn> columns2 = dataGrid4.Columns;
		DataGridTextColumn item2 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn3 = dataGridTextColumn;
		dataGridTextColumn3.Header = "Columna";
		ReflectionBindingExtension reflectionBindingExtension2 = new ReflectionBindingExtension("Columna");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		Binding binding2 = reflectionBindingExtension2.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn3.Binding = binding2;
		context.PopParent();
		columns2.Add(item2);
		ObservableCollection<DataGridColumn> columns3 = dataGrid4.Columns;
		DataGridTextColumn item3 = (dataGridTextColumn = new DataGridTextColumn());
		context.PushParent(dataGridTextColumn);
		DataGridTextColumn dataGridTextColumn4 = dataGridTextColumn;
		dataGridTextColumn4.Header = "Valor";
		ReflectionBindingExtension reflectionBindingExtension3 = new ReflectionBindingExtension("Valor");
		context.ProvideTargetProperty = CompiledAvaloniaXaml.XamlIlHelpers.Avalonia_002EControls_002EDataGridBoundColumn_002CAvalonia_002EControls_002EDataGrid_002EBinding_0021Property();
		Binding binding3 = reflectionBindingExtension3.ProvideValue(context);
		context.ProvideTargetProperty = null;
		dataGridTextColumn4.Binding = binding3;
		context.PopParent();
		columns3.Add(item3);
		context.PopParent();
		((ISupportInitialize)dataGrid3).EndInit();
		Controls children23 = stackPanel5.Children;
		StackPanel stackPanel13;
		StackPanel stackPanel12 = (stackPanel13 = new StackPanel());
		((ISupportInitialize)stackPanel12).BeginInit();
		children23.Add(stackPanel12);
		stackPanel13.Orientation = Orientation.Vertical;
		stackPanel13.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
		Controls children24 = stackPanel13.Children;
		TextBlock textBlock26;
		TextBlock textBlock25 = (textBlock26 = new TextBlock());
		((ISupportInitialize)textBlock25).BeginInit();
		children24.Add(textBlock25);
		textBlock26.Text = "Progreso del Juego:";
		textBlock26.FontWeight = FontWeight.Bold;
		((ISupportInitialize)textBlock26).EndInit();
		Controls children25 = stackPanel13.Children;
		ProgressBar progressBar2;
		ProgressBar progressBar = (progressBar2 = new ProgressBar());
		((ISupportInitialize)progressBar).BeginInit();
		children25.Add(progressBar);
		progressBar2.Name = "ScoreProgress";
		element = progressBar2;
		context.AvaloniaNameScope.Register("ScoreProgress", element);
		progressBar2.Minimum = 0.0;
		progressBar2.Maximum = 100.0;
		progressBar2.Height = 20.0;
		progressBar2.Width = 200.0;
		((ISupportInitialize)progressBar2).EndInit();
		Controls children26 = stackPanel13.Children;
		TextBlock textBlock28;
		TextBlock textBlock27 = (textBlock28 = new TextBlock());
		((ISupportInitialize)textBlock27).BeginInit();
		children26.Add(textBlock27);
		textBlock28.Name = "ScoreLabel";
		element = textBlock28;
		context.AvaloniaNameScope.Register("ScoreLabel", element);
		textBlock28.Text = "Puntaje: 0";
		textBlock28.FontSize = 14.0;
		textBlock28.Margin = new Thickness(10.0, 0.0, 10.0, 0.0);
		((ISupportInitialize)textBlock28).EndInit();
		Controls children27 = stackPanel13.Children;
		TextBlock textBlock30;
		TextBlock textBlock29 = (textBlock30 = new TextBlock());
		((ISupportInitialize)textBlock29).BeginInit();
		children27.Add(textBlock29);
		textBlock30.Name = "MedalLabel";
		element = textBlock30;
		context.AvaloniaNameScope.Register("MedalLabel", element);
		textBlock30.Text = "-";
		textBlock30.FontSize = 16.0;
		textBlock30.FontWeight = FontWeight.Bold;
		textBlock30.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		((ISupportInitialize)textBlock30).EndInit();
		((ISupportInitialize)stackPanel13).EndInit();
		Controls children28 = stackPanel5.Children;
		ScrollViewer scrollViewer2;
		ScrollViewer scrollViewer = (scrollViewer2 = new ScrollViewer());
		((ISupportInitialize)scrollViewer).BeginInit();
		children28.Add(scrollViewer);
		ScrollViewer scrollViewer4;
		ScrollViewer scrollViewer3 = (scrollViewer4 = scrollViewer2);
		context.PushParent(scrollViewer4);
		scrollViewer4.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
		scrollViewer4.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		ItemsControl itemsControl7;
		ItemsControl itemsControl6 = (itemsControl7 = new ItemsControl());
		((ISupportInitialize)itemsControl6).BeginInit();
		scrollViewer4.Content = itemsControl6;
		ItemsControl itemsControl8 = (itemsControl4 = itemsControl7);
		context.PushParent(itemsControl4);
		ItemsControl itemsControl9 = itemsControl4;
		itemsControl9.Name = "TablaView";
		element = itemsControl9;
		context.AvaloniaNameScope.Register("TablaView", element);
		ItemsPanelTemplate itemsPanelTemplate;
		ItemsPanelTemplate itemsPanel = (itemsPanelTemplate = new ItemsPanelTemplate());
		context.PushParent(itemsPanelTemplate);
		itemsPanelTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV3<Control>((nint)(delegate*<IServiceProvider, object>)(&XamlClosure_1.Build_2), context);
		context.PopParent();
		itemsControl9.ItemsPanel = itemsPanel;
		DataTemplate itemTemplate2 = (dataTemplate = new DataTemplate());
		context.PushParent(dataTemplate);
		dataTemplate.Content = XamlIlRuntimeHelpers.DeferredTransformationFactoryV3<Control>((nint)(delegate*<IServiceProvider, object>)(&XamlClosure_1.Build_3), context);
		context.PopParent();
		itemsControl9.ItemTemplate = itemTemplate2;
		context.PopParent();
		((ISupportInitialize)itemsControl8).EndInit();
		context.PopParent();
		((ISupportInitialize)scrollViewer3).EndInit();
		context.PopParent();
		((ISupportInitialize)stackPanel3).EndInit();
		context.PopParent();
		((ISupportInitialize)P_1).EndInit();
		if (P_1 is StyledElement styled)
		{
			NameScope.SetNameScope(styled, context.AvaloniaNameScope);
		}
		context.AvaloniaNameScope.Complete();
	}

	private static void _0021XamlIlPopulateTrampoline(MainWindowMochila P_0)
	{
		if (_0021XamlIlPopulateOverride != null)
		{
			_0021XamlIlPopulateOverride(P_0);
		}
		else
		{
			_0021XamlIlPopulate(XamlIlRuntimeHelpers.CreateRootServiceProviderV3(null), P_0);
		}
	}
}
