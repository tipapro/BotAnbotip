using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BotAnbotip.Bot.Data.CustomClasses
{
    [JsonObject]
    public class RatingList
    {
        public ulong Id { get; set; }
        public RatingListType Type { get; set; }
        public RLObjectCollection ListOfObjects { get; set; }
        public MessageIdCollection ListOfMessageIds { get; set; }

        private RatingList() { }

        public RatingList(ulong id, RatingListType type)
        {
            Id = id;
            Type = Type;
            ListOfObjects = new RLObjectCollection();
            ListOfMessageIds = new MessageIdCollection(id);            
        }
    }

    [JsonObject]
    public class MessageIdCollection : IEnumerable
    {
        [JsonProperty]
        private List<ulong> _listOfRLMessageIds;

        public bool IsReversed { get; set; }
        public int Count => _listOfRLMessageIds.Count;

        public MessageIdCollection(ulong groupId)
        {
            IsReversed = false;
            _listOfRLMessageIds = new List<ulong>();
        }

        public ulong this[int position] => _listOfRLMessageIds[ConvertPosition(position)];

        public void Add(ulong messageId) => _listOfRLMessageIds.Add(messageId);

        public void Remove(ulong messageId) => _listOfRLMessageIds.Remove(messageId);

        public int ConvertPosition(int position)
        {
            if (IsReversed)
                return _listOfRLMessageIds.Count - 1 - position;
            else
                return position;
        }

        public IEnumerator GetEnumerator() => _listOfRLMessageIds.GetEnumerator();   
    }

    [JsonObject]
    public class RLObjectCollection : IEnumerable
    {
        [JsonProperty]
        private List<RLObject> _listOfRLObjects;

        public int Count => _listOfRLObjects.Count;

        public RLObjectCollection()
        {
            _listOfRLObjects = new List<RLObject>();
        }

        public RLObject this[int position] => _listOfRLObjects[position];


        public (int, RLObject) FindByName(string name)      // Неэффективно!!!
        {
            for (int i = 0; i < _listOfRLObjects.Count; i++)
            {
                if (name == _listOfRLObjects[i].Name) return (i, _listOfRLObjects[i]);
            }
            return (0, null);
        }

        public int Sort(RLObject obj, int currentPosition, Evaluation eval)
        {
            try
            {
                int endValue = eval == Evaluation.Dislike ? _listOfRLObjects.Count - 1 : 0;
                int newPosition = 0;

                for (int i = currentPosition - (int)eval; ; i -= (int)eval)
                {
                    if ((i == endValue - (int)eval) || (obj.CompareTo(_listOfRLObjects[i]) != (int)eval))
                    {
                        newPosition = i + (int)eval;
                        _listOfRLObjects[i + (int)eval] = obj;
                        break;
                    }
                    _listOfRLObjects[i + (int)eval] = _listOfRLObjects[i];
                }
                return newPosition;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при сортировке рейтингового листа", ex);
            }
        }

        public void Add(RLObject obj) => _listOfRLObjects.Add(obj);

        public void Remove(RLObject obj) => _listOfRLObjects.Remove(obj);

        public IEnumerator GetEnumerator() => _listOfRLObjects.GetEnumerator();
    }

    [JsonObject]
    public class RLObject : IComparable<RLObject>
    {
        public string Name { get; }
        public string Url { get; }
        public string ThumbnailUrl { get; }
        public List<ulong> LikedUsers { get; set; }

        private RLObject() { }

        public RLObject(string name, int position, string url = "", string thumbnailUrl = "")
        {
            Name = name;
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            LikedUsers = new List<ulong>();
        }

        public int CompareTo(RLObject obj)
        {
            var result = this.LikedUsers.Count.CompareTo(obj.LikedUsers.Count);
            if (result == 0) result = obj.Name.CompareTo(this.Name);
            return result;
        }
    }

    public enum Evaluation
    {
        Like = 1,
        None = 1,
        Dislike = -1
    }

    public enum RatingListType { Gaming, Musical, Other}
}
