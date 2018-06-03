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
        public RLObjectList ListOfObjects { get; set; }
        public RLMessageIdList ListOfMessageIds { get; set; }

        private RatingList() { }

        public RatingList(ulong id, RatingListType type)
        {
            Id = id;
            Type = Type;
            ListOfObjects = new RLObjectList();
            ListOfMessageIds = new RLMessageIdList(id);            
        }
    }

    [JsonObject]
    public class RLMessageIdList : IEnumerable
    {
        [JsonProperty]
        private List<ulong> _listOfRLMessageIds;

        public bool IsReversed { get; set; }
        public int Count => _listOfRLMessageIds.Count;

        public RLMessageIdList(ulong groupId)
        {
            IsReversed = false;
            _listOfRLMessageIds = new List<ulong>();
        }

        public ulong this[int position] => _listOfRLMessageIds[ConvertPosition(position)];        

        public void Add(ulong messageId) => _listOfRLMessageIds.Add(messageId);

        public void Remove(ulong messageId) => _listOfRLMessageIds.Remove(messageId);
        
        public ulong Last() => this[_listOfRLMessageIds.Count - 1];

        private int ConvertPosition(int position)
        {
            if (IsReversed)
                return _listOfRLMessageIds.Count - 1 - position;
            else
                return position;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_listOfRLMessageIds).GetEnumerator();
        }
    }

    [JsonObject]
    public class RLObjectList : IEnumerable
    {
        [JsonProperty]
        private List<RLObject> _listOfRLObjects;

        public int Count => _listOfRLObjects.Count;

        public RLObjectList()
        {
            _listOfRLObjects = new List<RLObject>();
        }

        public RLObject this[int position] => _listOfRLObjects[position];


        public (int, RLObject) FindByName(string name)
        {
            for (int i = 0; i < _listOfRLObjects.Count; i++)
            {
                if (name == _listOfRLObjects[i].Name) return (i, _listOfRLObjects[i]);
            }
            return (0, null);
        }

        public int Sort(RLObject obj, int position, Evaluation eval)
        {
            int endValue = eval == Evaluation.Dislike ? _listOfRLObjects.Count - 1 : 0;
            int newPosition;

            for (int i = position - (int)eval; ; i -= (int)eval)
            {
                if ((i == endValue - (int)eval))
                {
                    newPosition = endValue;
                    _listOfRLObjects[endValue] = obj;
                    break;
                }
                if (obj.CompareTo(_listOfRLObjects[i]) == (int)eval)
                {
                    _listOfRLObjects[i + (int)eval] = _listOfRLObjects[i];
                }
                else
                {
                    newPosition = i + (int)eval;
                    _listOfRLObjects[i + (int)eval] = obj;
                    break;
                }
            }
            return newPosition;
        }

        public void Add(RLObject obj)
        {
            _listOfRLObjects.Add(obj);
        }

        public void Remove(string name)
        {
            _listOfRLObjects.Remove(FindByName(name).Item2);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_listOfRLObjects).GetEnumerator();
        }
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


    public enum RatingListType { Game, Music, Other}
}
