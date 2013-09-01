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
using DialedIn.ViewModel;
using DialedIn.Model;

// TASKS DETAILS UI DEFINITION

namespace DialedIn.View
{
    public partial class GroupDetailsView : PhoneApplicationPage
    {

        private Group selectedGroup;

        public GroupDetailsView()
        {
            InitializeComponent();
        }

        // set-up the initial group screen with requred data
        // this screen parses the group to display's title from the NavigationContext object
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string GroupTitle;

            if (NavigationContext.QueryString.TryGetValue("msg", out GroupTitle))
            {
                var group = from Group in App.vm.Groups where Group.Title == GroupTitle select Group;

                foreach (Group g in group)
                {
                    this.selectedGroup = g;
                    break;
                }

                GroupTitleBlock.Text = this.selectedGroup.Title;
                OwnerTextBlock.Text = this.selectedGroup.Owner;
                GroupMembersTextBox.Text = this.selectedGroup.GroupMembers;
            }
            else
            {
                GroupTitleBlock.Text = "Error!";
                OwnerTextBlock.Text = "Error!";
                GroupMembersTextBox.Text = "Error!";
            }
        }

        // click button event handlers
        //
        // check mark icon to set the group as the current group
        private void SetButton_Click(object sender, EventArgs e)
        {
            // set the global selected group on save
            App.vm.selectedGroup = this.selectedGroup;
            NavigationService.Navigate(new Uri("/View/TasksView.xaml", UriKind.RelativeOrAbsolute));
        }

        // edit the current group
        private void EditButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/GroupEditView.xaml?msg=" + this.selectedGroup.Title, UriKind.RelativeOrAbsolute));
        }
    }
}