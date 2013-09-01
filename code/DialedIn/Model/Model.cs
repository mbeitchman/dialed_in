using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
// The model defines the core data structures for the application.
// CoreData Structions are:
//   1) AppInststance
//   2) Task
//   3) Group
//   4) Notification
//

namespace DialedIn.Model
{
    // this class defines static data related to the current instance of the app running
    public class AppInstance
    {
        // this value stores the currentuser for the application
        public static string CurrentUser { get; set; }
    }

    // this class defines the core Task data structure
    // the class has a constructor and a GetCopy function
    public class Task
    {
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public string Description { get; set; }
        public BitmapImage TaskImage { get; set; }
        public bool IsActive { get; set; }
        public string Location { get; set; }
        public string DueDate { get; set; } 

        public Task(string Title, string AssignedTo, string Description, BitmapImage TaskImage, string DueDate, string Location = "Default")
        {
            this.Title = Title;
            this.AssignedTo = AssignedTo;
            this.Description = Description;
            this.TaskImage = TaskImage;
            this.IsActive = true;
            this.Location = Location;
            this.DueDate = DueDate;
        }

        public Task(string Title, string AssignedTo, string Description, string DueDate = "none", string Location = "Default")
        {
            this.Title = Title;
            this.AssignedTo = AssignedTo;
            this.Description = Description;
            this.IsActive = true;
            this.Location = Location;
            this.DueDate = DueDate;
        }

        public Task GetCopy()
        {
            Task copy = (Task)this.MemberwiseClone();
            return copy;
        }
    }

    // this class defines a group
    public class Group
    {
        public string Owner { get; set; }
        public string Title { get; set; }
        public string GroupMembers { get; set; }

        // list of tasks
        public ObservableCollection<Task> Tasks { get; set; }

        public Group(string Owner, string Title, string GroupMembers, Task newTask)
        {
            Tasks = new ObservableCollection<Task>();

            if (newTask != null)
            {
                this.AddTask(newTask);
            }

            this.Owner = Owner;
            this.Title = Title;
            this.GroupMembers = GroupMembers;
        }

        public void AddTask(Task taskToAdd)
        {
            Tasks.Add(taskToAdd);
        }
    }

    // this class defines a Notification
    public class Notification
    {
        public string MessageInfo { get; set; }
        DateTime TimeStamp { get; set; }
        public bool isActive { get; set; }

        public Notification(string Message)
        {
            this.MessageInfo = Message;
            this.TimeStamp = DateTime.Now;
            this.isActive = true;
        }
    }
}
