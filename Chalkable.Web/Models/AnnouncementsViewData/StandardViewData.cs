using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{

    public class StandardViewData
    {
        public int StandardId { get; set; }
        public int? ParentStandardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StandardSubjectId { get; set; }
        public Guid? AcademicBenchmarkId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeepest { get; set; }
        protected StandardViewData(){}

        protected StandardViewData(Standard standard)
        {
            StandardId = standard.Id;
            Description = standard.Description;
            Name = standard.Name;
            ParentStandardId = standard.ParentStandardRef;
            StandardSubjectId = standard.StandardSubjectRef;
            AcademicBenchmarkId = standard.AcademicBenchmarkId;
            IsActive = standard.IsActive;
            IsDeepest = standard.IsDeepest;
        }
        
        public static StandardViewData Create(Standard standard)
        {
            return new StandardViewData(standard);
        }

        public static IList<StandardViewData> Create(IList<Standard> standards)
        {
            return standards.Select(Create).ToList();
        }
        public static IList<StandardViewData> Create(IList<AnnouncementStandardDetails> standardDetailses)
        {
            return standardDetailses.Select(x => Create(x.Standard)).ToList();
        }
    }


    public class StandardTableItemViewData : StandardViewData
    {
        public bool IsSelected { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        protected StandardTableItemViewData(Standard standard, bool isSelected, int row, int column)
            : base(standard)
        {
            IsSelected = isSelected;
            Row = row;
            Column = column;
        }

        public static StandardTableItemViewData Create(Standard standard, bool isSelected, int row, int column)
        {
            return new StandardTableItemViewData(standard, isSelected, row, column);
        }
    }

    public class StandardsTableViewData
    {
        public IList<IList<StandardTableItemViewData>> StandardsColumns { get; set; } 

        public static StandardsTableViewData Create(IList<StandardTreeItem> standardsTree)
        {
            var res = new StandardsTableViewData { StandardsColumns = new List<IList<StandardTableItemViewData>>() };
            if (standardsTree.Count > 0)
                for (var i = 0; i <= standardsTree.Max(s => s.Column); i++)
                    res.StandardsColumns.Add(standardsTree.Select(s => StandardTableItemViewData.Create(s.Standard, s.IsSelected, s.Row, s.Column)).Where(s => s.Column == i).ToList());
            return res;
        }
    }
}