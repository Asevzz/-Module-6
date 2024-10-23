using System.Collections.ObjectModel;

namespace TaskManager
{
    public class TaskModel : MainWindow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ObservableCollection<SubTaskModel> SubTasks { get; set; }  // Изменено на ObservableCollection
        public bool IsCompleted { get; set; }
    }

    public class SubTaskModel : MainWindow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}