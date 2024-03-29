﻿using System.Windows;

namespace ImageHandlerEXIFWPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.Message, "Message from your sponsor", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            e.Handled = true;
        }
    }
}
