namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class SimpleAttendanceSummary
    {
        public decimal? Absences { get; set; }
        public decimal? Presents { get; set; }
        public int? Tardies { get; set; }
        public virtual decimal PosibleAttendanceCount
        {
            get
            {
                decimal sum = 0;
                if (Absences.HasValue)
                    sum += Absences.Value;
                if (Tardies.HasValue)
                    sum += Tardies.Value;
                if (Presents.HasValue)
                    sum += Presents.Value;
                return sum;
            }
        }
    }
}
