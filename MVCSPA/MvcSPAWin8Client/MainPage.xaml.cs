using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MvcSPAWin8Client.Model;
using MvcSPAWin8Client.ViewModel;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MvcSPAWin8Client
{
    public sealed partial class MainPage : Page
    {
        readonly MainViewModel _viewModel = new MainViewModel();

        public MainPage()
        {
            DataContext = _viewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.Load();
        }

        public async void LoginSubmitted(object sender, SubmitLoginEventArgs args)
        {
            bool isSuccess = await WebAPIHelper.Login(_viewModel.UserName, _viewModel.Password);
            if (isSuccess)
            {
                await ShowMessage("Form login success");

                await _viewModel.Refresh();
            }
            else
            {
                await ShowMessage("Form login failed");
            }
        }

        private void formBasedLogin_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoginShow = true;
        }

        private async void facebookLogin_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = await WebAPIHelper.LoginToFacebook();
            if (isSuccess)
            {
                await ShowMessage("Facebook login success");
                await _viewModel.Refresh();
            }
            else
            {
                await ShowMessage("Facebook login failed");
            }
        }

        private async Task ShowMessage(string msg)
        {
            //await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            //   () => { messageBlock.Text = msg + "\r\n" + messageBlock.Text; });
            _viewModel.Messages = msg;

            //var messageDialog = new MessageDialog(msg);
            //await messageDialog.ShowAsync();
        }
        
        private async void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.Refresh();
        }

        private async void DeleteTodoList_Click(object sender, RoutedEventArgs e)
        {
            TodoList myTodoList = (TodoList)((Button)sender).DataContext;
            bool success = await WebAPIHelper.WebAPIDeleteTodoList(myTodoList.TodoListId);
            if (success)
            {
                _viewModel.MyLists.Remove(myTodoList);
                await ShowMessage("Delete success");
            }
            else
            {
                await ShowMessage("Delete failure");
            }
        }

        private async void addTodoList_Click(object sender, RoutedEventArgs e)
        {
            TodoList newList = new TodoList()
            {
                TodoListId = 0,
                Title = "New Todo List",
                UserId = "to be replaced by server",
                Todos = new ObservableCollection<TodoItem>(new List<TodoItem>())
            };

            bool success = await WebAPIHelper.WebAPIAddTodoList(newList);
            if (success)
            {
                _viewModel.MyLists.Insert(0, newList);
                await ShowMessage("Add success");
            }
            else
            {
                await ShowMessage("Add failure");
            }
        }

        private async void UpdateTodoList_Click(object sender, RoutedEventArgs e)
        {
            TodoList myTodoList = (TodoList)((TextBox)sender).DataContext;
            myTodoList.Title = ((TextBox)sender).Text;
            bool success = await WebAPIHelper.WebAPIUpdateTodoList(myTodoList);
            if (!success)
            {
                await ShowMessage("update failure");
            }
            else
            {
                await ShowMessage("update success");
            }
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            TodoItem myTodoItem = (TodoItem)((CheckBox)sender).DataContext;
            myTodoItem.IsDone = (bool)((CheckBox)sender).IsChecked;
            bool success = await WebAPIHelper.WebAPIUpdateTodoItem(myTodoItem);
            if (!success)
            {
                await ShowMessage("update IsDone failure");
            }
            else
            {
                await ShowMessage("update IsDone success");
            }
        }

        private async void UpdateTodoItem_Click(object sender, RoutedEventArgs e)
        {
            TodoItem myTodoItem = (TodoItem)((TextBox)sender).DataContext;
            myTodoItem.Title = ((TextBox)sender).Text;
            bool success = await WebAPIHelper.WebAPIUpdateTodoItem(myTodoItem);
            if (!success)
            {
                await ShowMessage("update todo item text failure");
            }
            else
            {
                await ShowMessage("update todo item text success");
            }
        }

        private async void DeleteTodoItem_Click(object sender, RoutedEventArgs e)
        {
            TodoItem myTodoItem = (TodoItem)((Button)sender).DataContext;
            bool success = await WebAPIHelper.WebAPIDeleteTodoItem(myTodoItem.TodoItemId);
            if (success)
            {
                foreach (TodoList list in _viewModel.MyLists)
                {
                    if (list.TodoListId == myTodoItem.TodoListId)
                    {
                        list.Todos.Remove(myTodoItem);
                    }
                }

                await ShowMessage("Delete success");
            }
            else
            {
                await ShowMessage("Delete failure");
            }
        }

        private async void CreateTodoItem_Click(object sender, RoutedEventArgs e)
        {
            TodoList myTodoList = (TodoList)((TextBox)sender).DataContext;
            var newText = ((TextBox)sender).Text;

            if (newText == "Type here to Add")
            {
                return;
            }

            ((TextBox)sender).Text = "Type here to Add"; //reset

            TodoItem newItem = new TodoItem()
            {
                TodoItemId = 0,
                Title = newText,
                TodoListId = myTodoList.TodoListId,
                IsDone = false
            };

            bool success = await WebAPIHelper.WebAPIAddTodoItem(newItem);
            if (success)
            {
                myTodoList.Todos.Add(newItem);
                await ShowMessage("Add success");
            }
            else
            {
                await ShowMessage("Add failure");
            }
        }
    }

}
