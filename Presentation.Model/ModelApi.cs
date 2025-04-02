﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPUM.Logic;

namespace TPUM.Presentation.Model
{
    public abstract class ModelApiBase : IDisposable
    {
        public abstract ReadOnlyObservableCollection<IRoom> Rooms { get; }
        public abstract IRoom AddRoom(string name, float width, float height);
        public abstract void RemoveRoom(long id);
        public abstract ICommand CreateCommand(Action<object?> execute, Predicate<object?> canExecute);
        public abstract ICommand CreateCommand(Action<object?> execute);
        public abstract void Dispose();

        public static ModelApiBase GetApi(LogicApiBase? logic = null)
        {
            return new ModelApi(logic ?? LogicApiBase.GetApi());
        }
    }

    internal class ModelApi : ModelApiBase
    {
        private readonly LogicApiBase _logic;
        private readonly ObservableCollection<IRoom> _rooms = [];
        public override ReadOnlyObservableCollection<IRoom> Rooms { get; }

        public ModelApi(LogicApiBase logic)
        {
            _logic = logic;
            Rooms = new ReadOnlyObservableCollection<IRoom>(_rooms);
            foreach (var room in _logic.Rooms)
            {
                _rooms.Add(new Room("", room));
            }
        }

        public override IRoom AddRoom(string name, float width, float height)
        {
            var room = new Room(name, _logic.AddRoom(width, height));
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(long id)
        {
            var room = _rooms.First(room => room.Id == id);
            if (room != null) _rooms.Remove(room);
            _logic.RemoveRoom(id);
        }

        public override ICommand CreateCommand(Action<object?> execute, Predicate<object?> canExecute)
        {
            return new CustomCommand(execute, canExecute);
        }

        public override ICommand CreateCommand(Action<object?> execute)
        {
            return new CustomCommand(execute);
        }

        public override void Dispose()
        {
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
