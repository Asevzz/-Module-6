using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace TaskManager
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TaskModel> tasks = new ObservableCollection<TaskModel>();
        private FirebaseService firebaseService;
        private TaskModel selectedTask;  // Для отслеживания выбранной задачи
        public MainWindow()
        {
            InitializeComponent();
            TasksListBox.ItemsSource = tasks;
            firebaseService = new FirebaseService("38f7a379cc3f4197a8748ec5f702942e870f0293");
            LoadTasks();
        }

        // Загрузка задач из Firebase
        private async void LoadTasks()
        {
            var loadedTasks = await firebaseService.GetTasks();
            foreach (var task in loadedTasks)
            {
                tasks.Add(task);
            }
        }

        // Обработка ввода в текстовом поле для скрытия/показа Placeholder
        private void TaskTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TaskTitlePlaceholder.Visibility = string.IsNullOrWhiteSpace(TaskTitleTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        // Добавление новой задачи
        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new TaskModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = TaskTitleTextBox.Text,
                SubTasks = new ObservableCollection<SubTaskModel>() // Изменено на ObservableCollection
            };
            await firebaseService.AddTask(task);
            tasks.Add(task);
            TaskTitleTextBox.Clear();
        }

        // Удаление выбранной задачи
        private async void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTask != null)
            {
                await firebaseService.DeleteTask(selectedTask.Id);
                tasks.Remove(selectedTask);
                selectedTask = null;
                DeleteTaskButton.IsEnabled = false;
            }
        }

        // Выбор задачи для последующего удаления
        private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTask = (TaskModel)TasksListBox.SelectedItem;
            DeleteTaskButton.IsEnabled = selectedTask != null;
        }

        // Добавление подзадачи
        private async void AddSubTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var task = button.Tag as TaskModel;

            // Создаем новое TextBox для ввода названия подзадачи
            var parentPanel = (StackPanel)button.Parent;
            var subTaskTitleTextBox = parentPanel.Children[0] as TextBox;

            var subTask = new SubTaskModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = subTaskTitleTextBox.Text
            };

            task.SubTasks.Add(subTask);
            await firebaseService.UpdateTask(task);  // Обновляем задачу с новыми подзадачами

            subTaskTitleTextBox.Clear();  // Очищаем поле после добавления подзадачи
        }

        // Удаление подзадачи
        private async void DeleteSubTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var subTask = button.Tag as SubTaskModel;
            var task = (TaskModel)TasksListBox.SelectedItem;

            task.SubTasks.Remove(subTask);
            await firebaseService.UpdateTask(task);  // Обновляем задачу после удаления подзадачи
        }
    }
}