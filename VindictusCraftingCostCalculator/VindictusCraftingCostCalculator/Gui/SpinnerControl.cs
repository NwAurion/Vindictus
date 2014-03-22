// Copyright 2012 lapthorn.net.
//
// This software is provided "as is" without a warranty of any kind. All
// express or implied conditions, representations and warranties, including
// any implied warranty of merchantability, fitness for a particular purpose
// or non-infringement, are hereby excluded. lapthorn.net and its licensors
// shall not be liable for any damages suffered by licensee as a result of
// using the software. In no event will lapthorn.net be liable for any
// lost revenue, profit or data, or for direct, indirect, special,
// consequential, incidental or punitive damages, however caused and regardless
// of the theory of liability, arising out of the use of or inability to use
// software, even if lapthorn.net has been advised of the possibility of
// such damages.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VindictusCraftingCostCalculator.Gui
{
	/// <summary>
	/// Interaction logic for SpinnerControl.xaml
	/// </summary>
	public class SpinnerControl : UserControl
	{
		public SpinnerControl()
		{
			MouseWheel += SpinnerControl_MouseWheel;
		}

		void SpinnerControl_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
				OnIncrease();
			else if (e.Delta < 0)
				OnDecrease();
		}

		static SpinnerControl()
		{
			InitializeCommands();
		}


		/// <summary>
		/// This is the Control property that we expose to the user.
		/// </summary>
		[Category("SpinnerControl")]
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(SpinnerControl), new UIPropertyMetadata(DefaultValue));


		private static double LimitValueByBounds(double newValue, SpinnerControl control)
		{
			newValue = Math.Max(control.Minimum, Math.Min(control.Maximum, newValue));
			return newValue;
		}




		/// <summary>
		/// This is the Control property that we expose to the user.
		/// </summary>
		[Category("SpinnerControl")]
		public double Minimum
		{
			get { return (double)GetValue(MinimumValueProperty); }
			set { SetValue(MinimumValueProperty, value); }
		}

		private static readonly DependencyProperty MinimumValueProperty =
			DependencyProperty.Register("Minimum", typeof(double), typeof(SpinnerControl),
			new PropertyMetadata(DefaultMinimumValue));


		/// <summary>
		/// This is the Control property that we expose to the user.
		/// </summary>
		[Category("SpinnerControl")]
		public double Maximum
		{
			get { return (double)GetValue(MaximumValueProperty); }
			set { SetValue(MaximumValueProperty, value); }
		}

		private static readonly DependencyProperty MaximumValueProperty =
			DependencyProperty.Register("Maximum", typeof(double), typeof(SpinnerControl),
			new PropertyMetadata(DefaultMaximumValue));



		/// <summary>
		/// This is the Control property that we expose to the user.
		/// </summary>
		[Category("SpinnerControl")]
		public double Change
		{
			get { return (double)GetValue(ChangeProperty); }
			set { SetValue(ChangeProperty, value); }
		}

		private static readonly DependencyProperty ChangeProperty =
			DependencyProperty.Register("Change", typeof(double), typeof(SpinnerControl),
			new PropertyMetadata(DefaultChange));




		/// <summary>
		/// Define the min, max and starting value, which we then expose 
		/// as dependency properties.
		/// </summary>
		private const double DefaultMinimumValue = 0,
			DefaultMaximumValue = double.MaxValue,
			DefaultValue = DefaultMinimumValue,
			DefaultChange = 1;


		public static RoutedCommand IncreaseCommand { get; set; }

		protected static void OnIncreaseCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			SpinnerControl control = sender as SpinnerControl;

			if (control != null)
			{
				control.OnIncrease();
			}
		}

		protected void OnIncrease()
		{
			//  see https://connect.microsoft.com/VisualStudio/feedback/details/489775/
			//  for why we do this.
			Value = LimitValueByBounds(Value + Change, this);
		}

		public static RoutedCommand DecreaseCommand { get; set; }

		protected static void OnDecreaseCommand(Object sender, ExecutedRoutedEventArgs e)
		{
			SpinnerControl control = sender as SpinnerControl;

			if (control != null)
			{
				control.OnDecrease();
			}
		}

		protected void OnDecrease()
		{
			//  see https://connect.microsoft.com/VisualStudio/feedback/details/489775/
			//  for why we do this.
			Value = LimitValueByBounds(Value - Change, this);
		}

		/// <summary>
		/// Since we're using RoutedCommands for the up/down buttons, we need to
		/// register them with the command manager so we can tie the events
		/// to callbacks in the control.
		/// </summary>
		private static void InitializeCommands()
		{
			//  create instances
			IncreaseCommand = new RoutedCommand("IncreaseCommand", typeof(SpinnerControl));
			DecreaseCommand = new RoutedCommand("DecreaseCommand", typeof(SpinnerControl));

			//  register the command bindings - if the buttons get clicked, call these methods.
			CommandManager.RegisterClassCommandBinding(typeof(SpinnerControl), new CommandBinding(IncreaseCommand, OnIncreaseCommand));
			CommandManager.RegisterClassCommandBinding(typeof(SpinnerControl), new CommandBinding(DecreaseCommand, OnDecreaseCommand));

			//  lastly bind some inputs:  i.e. if the user presses up/down arrow 
			//  keys, call the appropriate commands.
			CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
			CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(IncreaseCommand, new KeyGesture(Key.Right)));
			CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));
			CommandManager.RegisterClassInputBinding(typeof(SpinnerControl), new InputBinding(DecreaseCommand, new KeyGesture(Key.Left)));
		}
	}
}
