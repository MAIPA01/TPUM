﻿using TPUM.Client.Data;

namespace TPUM.Client.Logic
{
    public abstract class LogicApiBase : IDisposable
    {
        public abstract IReadOnlyCollection<IRoom> Rooms { get; }

        public abstract IRoom AddRoom(float width, float height);

        public abstract void RemoveRoom(long id);

        public abstract void ClearRooms();

        public static LogicApiBase GetApi(DataApiBase? data = null)
        {
            return new LogicApi(data ?? DataApiBase.GetApi());
        }

        public abstract void Dispose();
    }

    internal class LogicApi : LogicApiBase
    {
        private readonly DataApiBase _data;
        private readonly List<IRoom> _rooms = [];
        public override IReadOnlyCollection<IRoom> Rooms => _rooms;

        public LogicApi(DataApiBase data)
        {
            _data = data;
            foreach (var room in _data.Rooms)
            {
                _rooms.Add(new Room(room));
            }
        }

        public override IRoom AddRoom(float width, float height)
        {
            var room = new Room(_data.AddRoom(width, height));
            room.StartSimulation();
            _rooms.Add(room);
            return room;
        }

        public override void RemoveRoom(long id)
        {
            var room = _rooms.Find(room => room.Id == id);
            if (room != null)
            {
                room.EndSimulation();
                _rooms.Remove(room);
            }
            _data.RemoveRoom(id);
        }

        public override void ClearRooms()
        {
            _rooms.Clear();
            _data.ClearRooms();
        }

        public override void Dispose()
        {
            foreach (var room in _rooms)
            {
                room.EndSimulation();
            }
            _data.Dispose();
            foreach (var room in _rooms)
            {
                room.Dispose();
            }
            _rooms.Clear();
            GC.SuppressFinalize(this);
        }
    }
}