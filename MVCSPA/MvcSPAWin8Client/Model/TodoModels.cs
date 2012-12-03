using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcSPAWin8Client.Mvvm;
using Windows.Data.Json;

namespace MvcSPAWin8Client.Model
{

    public class TodoItem : ViewModelBase
    {
        public int TodoItemId { get; set; }
        public int TodoListId { get; set; }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value != _Title)
                {
                    _Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _IsDone;
        public bool IsDone
        {
            get { return _IsDone; }
            set
            {
                if (value != _IsDone)
                {
                    _IsDone = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SerializeToJsonObject()
        {
            JsonObject obj = new JsonObject();
            obj.Add("TodoItemId", JsonValue.CreateNumberValue(TodoItemId));
            obj.Add("IsDone", JsonValue.CreateBooleanValue(IsDone));
            obj.Add("Title", JsonValue.CreateStringValue(Title));
            obj.Add("TodoListId", JsonValue.CreateNumberValue(TodoListId));
            return obj.Stringify();
        }
        public string SerializeToJsonObjectWithoutId()
        {
            JsonObject obj = new JsonObject();
            obj.Add("IsDone", JsonValue.CreateBooleanValue(IsDone));
            obj.Add("Title", JsonValue.CreateStringValue(Title));
            obj.Add("TodoListId", JsonValue.CreateNumberValue(TodoListId));
            return obj.Stringify();
        }
    }

    public class TodoList : ViewModelBase
    {
        public int TodoListId { get; set; }
        public string UserId { get; set; }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value != _Title)
                {
                    _Title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<TodoItem> _Todos;
        public ObservableCollection<TodoItem> Todos
        {
            get { return _Todos; }
            set
            {
                if (value != _Todos)
                {
                    _Todos = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SerializeToJsonObject()
        {
            return String.Format("{{\"TodoListId\":\"{0}\",\"UserId\":\"{1}\",\"Title\":\"{2}\",\"Todos\":[]}}", this.TodoListId, this.UserId, this.Title);
        }
        public string SerializeToJsonObjectWithoutId()
        {
            return String.Format("{{\"UserId\":\"{0}\",\"Title\":\"{1}\",\"Todos\":[]}}", this.UserId, this.Title);
        }
    }
}
