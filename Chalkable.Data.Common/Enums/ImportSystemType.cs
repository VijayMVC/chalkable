namespace Chalkable.Data.Common.Enums
{
    public class ImportSystemType
    {
        public ImportSystemTypeEnum Type { get; set; }
        public string Name { get; set; }
    }

    public enum ImportSystemTypeEnum
    {
        None = -1,
        Lighthouse = 0,
        InfiniteCampus = 1,
        Esd = 2,
        Sti = 3
    }
}
