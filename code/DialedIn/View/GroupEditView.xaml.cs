using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using DialedIn.Model;
using DialedIn.ViewModel;

// TASKS EDIT UI DEFINITION

namespace DialedIn.View
{
    public partial class GroupEditView : PhoneApplicationPage
    {
        private Group selectedGroup;

        // use system chooser components to access the address book from the phone
        private EmailAddressChooserTask emailAddressChooserTask;

        public GroupEditView()
        {
            InitializeComponent();
            this.selectedGroup = null;

            // initialize email chooser and add an event handler
            emailAddressChooserTask = new EmailAddressChooserTask();
            emailAddressChooserTask.Completed += new EventHandler<EmailResult>(emailAddressChooserTask_Completed);
        }

        // event handler set-up initial screen with data
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string GroupTitle;

            // get group to edit from uri provided by the NavigationContext
            if (NavigationContext.QueryString.TryGetValue("msg", out GroupTitle))
            {
                App.vm.GetGroupWithTitle(GroupTitle, out this.selectedGroup);

                textBox1.Text = this.selectedGroup.Title;
                textBox2.Text = this.selectedGroup.GroupMembers;
            }
        }

        // The Completed event handler for the email chooser
        // set email address in text box
        void emailAddressChooserTask_Completed(object sender, EmailResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                textBox2.Text += e.Email + ";";
            }
        }

        // click event handler for button to save the new group
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (this.selectedGroup != null)
            {
                this.selectedGroup.Title = textBox1.Text;
                this.selectedGroup.GroupMembers = textBox2.Text;
            }
            else
            {
                App.vm.AddGroup(new Group(AppInstance.CurrentUser, textBox1.Text,textBox2.Text, null));
            }

            NavigationService.Navigate(new Uri("/View/GroupsView.xaml", UriKind.RelativeOrAbsolute));
        }

        // click event handler for button to launch the phone's address book using the email chooser
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                emailAddressChooserTask.Show();
            }
            catch (System.InvalidOperationException)
            {
                // catch but dont handle the exception
            }
        }
    }
}