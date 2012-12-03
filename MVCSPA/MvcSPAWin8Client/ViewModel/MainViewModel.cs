using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using MvcSPAWin8Client.Model;
using MvcSPAWin8Client.Mvvm;

namespace MvcSPAWin8Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                if (value != _UserName)
                {
                    _UserName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                if (value != _Password)
                {
                    _Password = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _LoginShow;
        public bool LoginShow
        {
            get { return _LoginShow; }
            set
            {
                if (value != _LoginShow)
                {
                    _LoginShow = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private String _Messages;
        public String Messages
        {
            get { return _Messages; }
            set
            {
                if (value != _Messages)
                {
                    _Messages = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<TodoList> _MyLists;
        public ObservableCollection<TodoList> MyLists
        {
            get { return _MyLists; }
            set
            {
                if (value != _MyLists)
                {
                    _MyLists = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            if (IsDesignMode)
            {
                Load();
            }
        }
                
        public async void Load()
        {
            if (IsDesignMode)
            {
                MyLists = new ObservableCollection<TodoList>
                             {
                                 new TodoList {Title = "design TodoList1"},
                                 new TodoList {Title= "design TodoList2"},
                             };
            }
            else
            {
                if (WebAPIHelper.IsAuthorized())
                {
                    await Refresh();
                }
            }
        }

        public async Task Refresh()
        {
            List<TodoList> myTodoLists = await WebAPIHelper.WebAPIGetTodoList();
            if (myTodoLists == null)
            {
                Messages = "Could not get any todo lists";
            }
            else
            {
                MyLists = new ObservableCollection<TodoList>(myTodoLists);
            }
        }

        //public async void SubmitLogin()
        //{
        //    bool isSuccess = await WebAPIHelper.Login(UserName, Password);
        //    if (isSuccess)
        //    {
        //        Messages = "Login Success, loading data...";

        //        await Refresh();
        //    }
        //    else
        //    {
        //        Messages = "Form login Failure";
        //    }
        //}
    }
}