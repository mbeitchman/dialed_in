using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Linq;
using DialedIn.Model;
using DialedIn.ViewModel;

// the viewmodel provides the to the views which are the application screens
// the viewmodel does the data processing for the application
namespace DialedIn.ViewModel
{
    public class TaskViewModel
    {
        // list of groups
        public ObservableCollection<Group> Groups { get; set; }
        // list of notifications
        public ObservableCollection<Notification> Notifications { get; set; }
        public Group selectedGroup { get; set; }
        public Task selectedTask { get; set; }
        private bool InitializedGroups;

        public TaskViewModel()
        {
            this.InitializedGroups = false;
            this.selectedTask = null;
            this.Groups = new ObservableCollection<Group>();
            this.Notifications = new ObservableCollection<Notification>();
        }

        // initializes groups from storage or creates the default group settings
        public void GetGroups()
        {
            if (!InitializedGroups)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Count > 0)
                {
                    // TODO: get last selected group from storage and set that as default
                    GetSavedGroups();
                }
                else
                {
                    // no groups exist so create the default group and set it as selected
                    CreateDefaultGroup();
                }

                InitializedGroups = true;
            }
        }

        private void CreateDefaultGroup()
        {

            // create default group and add a default task assigned to the current user
            DateTime now = DateTime.Today;
            now = now.AddDays(3);
            string format = "M/d/yyyy";
            string nowStr = now.ToString(format);
            
            String DefaultGroupTitle = AppInstance.CurrentUser + "'s Default Group";
            Group g = new Group(AppInstance.CurrentUser, DefaultGroupTitle, AppInstance.CurrentUser, new Task("Default Task", AppInstance.CurrentUser, "Initial Application Task 1", null, nowStr));

            // set the new group as the selected group and add the new group to the collection
            Groups.Add(g);
            this.selectedGroup = g;
        }

        private void GetSavedGroups()
        {
            throw new NotImplementedException();
        }

        public void SetSelectedGroup(string Title = null)
        {
            foreach (Group g in this.Groups)
            {
                if (g.Title == Title)
                {
                    this.selectedGroup = g;
                    break;
                }
            }
        }

        public void GetGroupWithTitle(string Title, out Group targetGroup)
        {
            targetGroup = null;

            foreach (Group g in this.Groups)
            {
                if (g.Title == Title)
                {
                    targetGroup = g;
                    break;
                }
            }
        }

        public void SetSelectedTask(string Title, string AssignedTo = null, string Description = null, BitmapImage TaskImage = null)
        {
           foreach (Task task in selectedGroup.Tasks)
           {
               if (task.Title == Title)
               {
                   this.selectedTask = task;
                   break;
               }
           }
        }

        public void AddGroup(Group group)
        {
            if (group.Title != null || group.Title != "")
            {
                this.Groups.Add(group);
            }
        }

        public void AddTask(Task task)
        {
            if ((task.AssignedTo != null) && (task.Title != null || task.Title != ""))
            {
                this.selectedGroup.Tasks.Add(task);
            }
        }

        public void AddNotifcation(Notification notification)
        {
            this.Notifications.Add(notification);
        }
    }
}
