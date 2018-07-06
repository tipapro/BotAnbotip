using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotAnbotip.Bot.Commands
{
    public abstract class CommandsBase
    {
        public (Func<IMessage, string, Task> CommandMethod, string[] CommandNames)[] Commands { get; }
        public static Dictionary<string, (ulong, List<ulong>)> MethodQueueDictionary;     // varName -- (lastId, listOfIds)

        protected CommandsBase(params (Func<IMessage, string, Task>, string[])[] commands)
        {
            Commands = commands;
            MethodQueueDictionary = new Dictionary<string, (ulong, List<ulong>)>();
        }

        public Func<IMessage, string, Task> this[string commandName]
        {
            get
            {
                foreach (var (CommandMethod, CommandNames) in Commands)
                    foreach (var name in CommandNames)
                        if (name == commandName) return CommandMethod;
                return null;
            }
        }

        protected static IEnumerable<(string, ulong)> AddToQueue(params string[] varNames)
        {
            try
            {
                var list = new List<(string, ulong)>();
                foreach (var name in varNames)
                {
                    if (!MethodQueueDictionary.ContainsKey(name)) MethodQueueDictionary.Add(name, (0, new List<ulong>()));
                    var (lastId, methodQueue) = MethodQueueDictionary[name];
                    MethodQueueDictionary[name] = (++lastId, methodQueue);
                    methodQueue.Add(lastId);
                    while ((methodQueue.Count != 0) && (methodQueue[0] != lastId)) { }
                    list.Add((name, lastId));
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении метода в очередь", ex);
            }

        }

        protected static void RemoveFromQueue(IEnumerable<(string, ulong)> ids)
        {
            try
            {
                foreach (var (name, id) in ids)
                {
                    if (!MethodQueueDictionary.ContainsKey(name)) continue;
                    MethodQueueDictionary[name].Item2.Remove(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при удаления метода из очереди", ex);
            }
        }
    }
}
