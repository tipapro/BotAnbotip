using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BotAnbotip.Bot.Data.CustomClasses
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
        private List<RatingListObject> _listOfObjectsOfRatingList;
        public int Count => _listOfObjectsOfRatingList.Count;

        public ObjectsOfRatingList()
        {
            _listOfObjectsOfRatingList = new List<RatingListObject>();
        }

        public RatingListObject this[string name] => FindByName(name);

        public RatingListObject this[int position] => _listOfObjectsOfRatingList[position];


        private RatingListObject FindByName(string name)
        {
            foreach (var objectOfRatingList in _listOfObjectsOfRatingList)
            {
                if (name == objectOfRatingList.Name) return objectOfRatingList;
            }
            return null;
        }

        public void Sort(RatingListObject obj)
        {
            int endValue, eval = (int)obj.LastEvaluation;

            if (eval == (int)Evaluation.Dislike) endValue = _listOfObjectsOfRatingList.Count-1;
            else endValue = 0;

            for (int i = obj.Position - eval;; i-= eval)
            {
                if ((i == endValue - eval))
                {
                    _listOfObjectsOfRatingList[endValue] = obj;
                    break;
                }
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
        }

        public void ReverseMessageIds()
        {
            int count = _listOfObjectsOfRatingList.Count;
            for (int i = 0; i < count; i++)
            {
                if (i >= count - 1 - i) break;
                _listOfObjectsOfRatingList[i].SwapWith(_listOfObjectsOfRatingList[count - 1 - i]);
            }
        }

        public void Add(string name, ulong messageId, string url = "", string thumbnailUrl = "")
        {
            var newObject = new RatingListObject(name, messageId, url, thumbnailUrl)
            {
                Position = _listOfObjectsOfRatingList.Count
            };

            _listOfObjectsOfRatingList.Add(newObject);
        }

        public void Remove(string name)
        {
            var foundedObj = FindByName(name);

            for (int i = foundedObj.Position + 1; i < _listOfObjectsOfRatingList.Count; i++)
            {
                _listOfObjectsOfRatingList[i].Position--;
            }
            _listOfObjectsOfRatingList.Remove(foundedObj);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_listOfObjectsOfRatingList).GetEnumerator();
        }
    }

    [JsonObject]
    public class RatingListObject : IComparable<RatingListObject>
    {
        private Evaluation _lastEvaluation;
        public string Name { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public List<ulong> LikedUsers { get; set; }
        public long NumberOfLikes { get; set; }
        public ulong MessageId { get; set; }
        public int Position { get; set; }
        public Dictionary<ulong, Evaluation> UserEvaluation { get; set; }
        public Evaluation LastEvaluation {
            get
            {
                if (DataManager.ReverseSign) return (Evaluation)((int)_lastEvaluation * (-1));
                return _lastEvaluation;
            }
            set { _lastEvaluation = value; } }

        private RatingListObject() { }

        public RatingListObject(string name, ulong messageId, string url = "", string thumbnailUrl = "")
        {
            Name = name;
            MessageId = messageId;
            Url = url;
            ThumbnailUrl = thumbnailUrl;
            UserEvaluation = new Dictionary<ulong, Evaluation>();
            LikedUsers = new List<ulong>();
            LastEvaluation = Evaluation.None;
        }

        public int CompareTo(RatingListObject obj)
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

        public void SwapWith(RatingListObject obj)
        {
            var bufPosition = this.Position;
            this.Position = obj.Position;
            obj.Position = bufPosition;

            var bufMessageId = this.MessageId;
            this.MessageId = obj.MessageId;
            obj.MessageId = bufMessageId;
        }
    }

    public enum Evaluation { Like = 1, Dislike = -1, None = 1 }

    public enum RatingListType { Game, Music, Other}
}
