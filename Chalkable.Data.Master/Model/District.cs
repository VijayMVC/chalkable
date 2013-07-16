using System;

namespace Chalkable.Data.Master.Model
{
    public class District
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DbName { get; set; }
        public int SisSystemType { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
    }
}