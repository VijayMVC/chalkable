using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Model.PanoramaStuff
{
    public class ShortStudentInfractionsInfo
    {
        public int StudentId { get; set; }
        public int NumberOfInfractions { get; set; }

        public static ShortStudentInfractionsInfo Create(StudentInfraction model)
        {
            return new ShortStudentInfractionsInfo
            {
                StudentId = model.StudentId,
                NumberOfInfractions = model.NumberOfInfractions
            };
        }      
    }
}
