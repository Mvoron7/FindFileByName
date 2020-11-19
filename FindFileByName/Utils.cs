using System.Windows;

namespace FindFileByName
{
    public static class Utils
    {
        /// <summary>
        /// Маркер отображения событий, возможность переключения не реализована.
        /// </summary>
        public static bool ShowError = true;

        /// <summary>
        /// Отображает переданное сообщени
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="title">Заголовок</param>
        public static void ShowErrorWindow(string message, string title = "")
        {
            if (ShowError)
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
