using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManager
{
    public class FirebaseService : MainWindow
    {
        private readonly FirebaseClient firebase;
        private readonly FirebaseAuthProvider authProvider;
        private FirebaseAuthLink auth;

        public FirebaseService(string apiKey)
        {
            authProvider = new FirebaseAuthProvider(new FirebaseConfig(apiKey));
            firebase = new FirebaseClient("https://taskmanager-747a3-default-rtdb.firebaseio.com/");
        }

        public async Task SignIn(string email, string password)
        {
            auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
        }

        public async Task<List<TaskModel>> GetTasks()
        {
            return (await firebase.Child("tasks").OnceAsync<TaskModel>()).Select(item => item.Object).ToList();
        }

        public async Task AddTask(TaskModel task)
        {
            await firebase.Child("tasks").PostAsync(task);
        }

        public async Task UpdateTask(TaskModel task)
        {
            await firebase.Child("tasks").Child(task.Id).PutAsync(task);
        }

        public async Task DeleteTask(string taskId)
        {
            await firebase.Child("tasks").Child(taskId).DeleteAsync();
        }
    }
}
