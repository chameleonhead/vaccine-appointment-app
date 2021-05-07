using Newtonsoft.Json;
using System;

namespace VaccineAppointment.Web.Infrastructure.Models
{
    public class Configuration
    {
        public Configuration()
        {
            Id = Guid.Empty.ToString();
            Name = "NOTSET";
            Json = "{}";
        }

        private Configuration(string name, string json)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Json = json;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }

        public static Configuration From<T>(T config)
        {
            return new Configuration(typeof(T).FullName!, JsonConvert.SerializeObject(config));
        }

        public void Set<T>(T config)
        {
            Name = typeof(T).FullName!;
            Json = JsonConvert.SerializeObject(config);
        }

        public T Get<T>()
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }
    }
}
