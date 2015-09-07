using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Chlk;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Web.Models.AnnouncementsViewData
{

    public class StandardViewData
    {
        public int StandardId { get; set; }
        public int? ParentStandardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StandardSubjectId { get; set; }
        public string CCStandardCode { get; set; }
        public Guid? AcademicBenchmarkId { get; set; }
        public bool IsActive { get; set; }

        protected StandardViewData(){}

        protected StandardViewData(Standard standard)
        {
            StandardId = standard.Id;
            Description = standard.Description;
            Name = standard.Name;
            ParentStandardId = standard.ParentStandardRef;
            StandardSubjectId = standard.StandardSubjectRef;
            AcademicBenchmarkId = standard.AcademicBenchmarkId;
            CCStandardCode = standard.CCStandardCode;
            IsActive = standard.IsActive;
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
        public static StandardsTableViewData Create(StandardTreePath standardTreePath)
        {
            var res = new StandardsTableViewData {StandardsColumns = new List<IList<StandardTableItemViewData>>()};
            var column = 0;
            if (standardTreePath.AllStandards.Count > 0 && standardTreePath.Path.Count > 0)
            {
                for (var i = 0; i < standardTreePath.Path.Count + 1; i++)
                {
                    var row = 0;
                    var standardsColumn = i == 0
                                        ? standardTreePath.AllStandards.Where(s => !s.ParentStandardRef.HasValue).ToList() 
                                        : standardTreePath.AllStandards.Where(s => s.ParentStandardRef == standardTreePath.Path[i - 1].Id).ToList();
                    res.StandardsColumns.Add(standardsColumn.Select(s => StandardTableItemViewData.Create(s, i < standardTreePath.Path.Count && s.Id == standardTreePath.Path[i].Id, ++row, column)).ToList());
                    column++;         
                }

            }
            return res;
        }
    }
}