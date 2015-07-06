using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace GranBlueHelper.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainWindow Current { get; private set; }
		public MainWindow()
		{
			InitializeComponent();
			Current = this;
		}
	}
}
