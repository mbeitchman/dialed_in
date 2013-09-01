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

// GROUPS OVERVIEW UI DEFINITION

namespace DialedIn.View
{
    public partial class GroupsView : PhoneApplicationPage
    {
        public GroupsView()
        {
            InitializeComponent();
        }

        // set up Group List box with Groups data context
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // LINQ Query
            // display all groups
            GroupsListBox.DataContext = from Group in App.vm.Groups select Group;
        }

        // click event handler for navigation buttons
        //
        // navigate to edit screen
        private void appbar_button1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/GroupEditView.xaml", UriKind.RelativeOrAbsolute));
        }

        // navigate to group details view
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock senderTB = (TextBlock)sender;

            NavigationService.Navigate(new Uri("/View/GroupDetailsView.xaml?msg=" + senderTB.Text, UriKind.RelativeOrAbsolute));
        }
    }
}