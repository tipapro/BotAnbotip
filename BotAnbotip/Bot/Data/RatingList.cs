﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BotAnbotip.Bot.Data
{
    [JsonObject]
    public class RatingList
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public RatingListType Type { get; set; }
        public ObjectsOfRatingList ListObjects { get; set; }


        private RatingList() { }

        public RatingList(ulong id, string name, RatingListType type)
        {
            Id = id;
            Name = name;
            ListObjects = new ObjectsOfRatingList();
            Type = Type;
        }
    }

    [JsonObject]
    public class ObjectsOfRatingList:IEnumerable
    {
        [JsonProperty]
        private List<ObjectOfRatingList> _listOfObjectsOfRatingList;
        public int Count => _listOfObjectsOfRatingList.Count;

        public ObjectsOfRatingList()
        {
            _listOfObjectsOfRatingList = new List<ObjectOfRatingList>();
        }

        public ObjectOfRatingList this[string name] => FindByName(name);

        public ObjectOfRatingList this[int position] => _listOfObjectsOfRatingList[position];


        private ObjectOfRatingList FindByName(string name)
        {
            foreach (var objectOfRatingList in _listOfObjectsOfRatingList)
            {
                if (name == objectOfRatingList.Name)
                {
                    return objectOfRatingList;
                }
            }
            return null;
        }

        public void Sort(ObjectOfRatingList obj)
        {
            int i, endValue, eval, previousPosition;

            previousPosition = obj.CurrentPosition;
            eval = (int)obj.LastEvaluation;

            if (obj.LastEvaluation == Evaluation.Dislike) endValue = _listOfObjectsOfRatingList.Count-1;
            else endValue = 0;

            for (i = obj.CurrentPosition - eval; i != endValue - eval; i-= eval)//eval
            {
                if (obj.CompareTo(_listOfObjectsOfRatingList[i]) == eval)
                {
                    obj.SwapWith(_listOfObjectsOfRatingList[i]);
                    _listOfObjectsOfRatingList[i + eval] = _listOfObjectsOfRatingList[i];
                }
                else
                {
                    _listOfObjectsOfRatingList[i + eval] = obj;
                    break;
                }
                
            }

            if ((i == endValue - eval))
            {
                _listOfObjectsOfRatingList[endValue] = obj;
            }

            obj.PreviousPosition = previousPosition;
        }

        public void Add(string name, ulong messageId)
        {
            var newObject =
                new ObjectOfRatingList(name, messageId)
                {
                    PreviousPosition = _listOfObjectsOfRatingList.Count,
                    CurrentPosition = _listOfObjectsOfRatingList.Count
                };


            _listOfObjectsOfRatingList.Add(newObject);
        }

        public void Remove(string name)
        {
            var foundedObj = FindByName(name);

            for (int i = foundedObj.CurrentPosition + 1; i < _listOfObjectsOfRatingList.Count; i++)
            {
                _listOfObjectsOfRatingList[i].CurrentPosition--;
                _listOfObjectsOfRatingList[i].PreviousPosition--;
            }
            _listOfObjectsOfRatingList.Remove(foundedObj);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_listOfObjectsOfRatingList).GetEnumerator();
        }
    }

    [JsonObject]
    public class ObjectOfRatingList : IComparable<ObjectOfRatingList>
    {
        public string Name { get; set; }
        public List<ulong> LikedUsers { get; set; }
        public long NumberOfLikes { get; set; }
        public ulong MessageId { get; set; }
        public int PreviousPosition { get; set; }
        public int CurrentPosition { get; set; }
        public Dictionary<ulong, Evaluation> UserEvaluation { get; set; }
        public Evaluation LastEvaluation { get; set; }

        private ObjectOfRatingList() { }

        public ObjectOfRatingList(string name, ulong messageId)
        {
            Name = name;
            MessageId = messageId;
            UserEvaluation = new Dictionary<ulong, Evaluation>();
            LikedUsers = new List<ulong>();
            LastEvaluation = Evaluation.None;
        }

        public int CompareTo(ObjectOfRatingList obj)
        {
            var result = this.NumberOfLikes.CompareTo(obj.NumberOfLikes);
            if (result == 0) result = obj.Name.CompareTo(this.Name);
            return result;
        }

        public void ChangeEvaluation(ulong userId, Evaluation eval)
        {
            if (eval == Evaluation.Like) LikedUsers.Add(userId);
            else LikedUsers.Remove(userId);
            NumberOfLikes = LikedUsers.Count;
            UserEvaluation[userId] = eval;
        }

        public void SwapWith(ObjectOfRatingList obj)
        {
            this.PreviousPosition = this.CurrentPosition;
            obj.PreviousPosition = obj.CurrentPosition;

            this.CurrentPosition = obj.PreviousPosition;
            obj.CurrentPosition = this.PreviousPosition;

            var bufMessageId = this.MessageId;
            this.MessageId = obj.MessageId;
            obj.MessageId = bufMessageId;
        }
    }

    public enum Evaluation { Like = 1, Dislike = -1, None = 1 }

    public enum RatingListType { Game, Music, Other}
}