using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace Patcharp.Performance
{
    public class PerformanceBenchmarks
    {
        public static void Run(IPatcharp patcharp)
        {
            var times = new List<TimeSpan>(10000);
            var watch = new Stopwatch();
            var watch2 = new Stopwatch();

            var items = GenerateEntities().ToArray();
            var itemsToPatch = GenerateEntitiesJson().ToArray();

            watch2.Restart();
            for (int i = 0; i < 10000; i++)
            {
                var item = items[i];
                var itemToPatch = itemsToPatch[i];
                watch.Restart();
                patcharp.ApplyPatchOperation(item, itemToPatch);
                times.Add(watch.Elapsed);
                watch.Reset();
            }
            Console.WriteLine(watch2.Elapsed.TotalMilliseconds);

            Console.WriteLine($"Average: {times.Average(ts => ts.TotalMilliseconds)}");
        }

        private static IEnumerable<Entity> GenerateEntities()
        {
            var rand = new Random();
            return Enumerable.Range(0, 10000).Select(i =>
            {
                var nxt = rand.Next(0, 100);
                var entity = new Entity
                {
                    Description = nxt.ToString(),
                    ValueThis = null
                };

                switch (nxt)
                {
                    case var _ when nxt < 33:
                        entity.ValueInt = nxt;
                        break;

                    case var _ when nxt < 66:
                        entity.ValueInt = nxt * 11 % 2 >> 2;
                        entity.ValueThis = new Entity
                        {
                            ValueThis = new Entity
                            {
                                Description = null,
                                ValueInt = 200
                            }
                        };
                        break;

                    default:
                        entity.ValueThis = new Entity
                        {
                            ValueInt = entity.ValueInt = nxt,
                            ValueThis = new Entity
                            {
                                Description = nxt.ToString(),
                                ValueInt = nxt,
                                ValueThis = new Entity
                                {
                                    ValueThis = new Entity
                                    {
                                        ValueInt = entity.ValueInt = nxt,
                                        ValueThis = new Entity
                                        {
                                            Description = nxt.ToString(),
                                            ValueInt = nxt,
                                            ValueThis = new Entity
                                            {
                                                ValueThis = new Entity
                                                {
                                                    ValueInt = entity.ValueInt = nxt,
                                                    ValueThis = new Entity
                                                    {
                                                        Description = nxt.ToString(),
                                                        ValueInt = nxt,
                                                        ValueThis = new Entity
                                                        {
                                                            ValueThis = new Entity()
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                        break;
                }


                return entity;
            });
        }

        private static IEnumerable<string> GenerateEntitiesJson()
        {
            var rand = new Random();
            return Enumerable.Range(0, 10000).Select(i =>
            {
                var nxt = rand.Next(0, 100);
                var entity = new Entity
                {
                    Description = nxt.ToString(),
                    ValueThis = null
                };

                switch (nxt)
                {
                    case var _ when nxt < 33:
                        return $"{{\"ValueInt\":\"{nxt}\", \"ValueThis\": \"null\"}}";

                    case var _ when nxt < 66:
                        entity.ValueInt = nxt * 11 % 2 >> 2;
                        entity.ValueThis = new Entity
                        {
                            ValueThis = new Entity
                            {
                                Description = null,
                                ValueInt = 200
                            }
                        };
                        break;

                    default:
                        entity.ValueThis = new Entity
                        {
                            ValueInt = entity.ValueInt = nxt,
                            ValueThis = new Entity
                            {
                                Description = nxt.ToString(),
                                ValueInt = nxt,
                                ValueThis = new Entity
                                {
                                    ValueThis = new Entity
                                    {
                                        ValueInt = entity.ValueInt = nxt,
                                        ValueThis = new Entity
                                        {
                                            Description = nxt.ToString(),
                                            ValueInt = nxt,
                                            ValueThis = new Entity
                                            {
                                                ValueThis = new Entity
                                                {
                                                    ValueInt = entity.ValueInt = nxt,
                                                    ValueThis = new Entity
                                                    {
                                                        Description = nxt.ToString(),
                                                        ValueInt = nxt,
                                                        ValueThis = new Entity
                                                        {
                                                            ValueThis = new Entity()
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                        break;
                }


                return JsonConvert.SerializeObject(entity, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            });
        }
    }

    public class Entity
    {
        public string Description { get; set; }

        public int ValueInt { get; set; }

        public Entity ValueThis { get; set; }
    }
}