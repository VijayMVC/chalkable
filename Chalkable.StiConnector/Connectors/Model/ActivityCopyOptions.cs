using System;
using System.Collections.Generic;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityCopyOptions
    {

        /// <summary>
        /// The list of activity ids to copy
        /// </summary>
        public IEnumerable<int> ActivityIds { get; set; }

        /// <summary>
        /// A list of section ids to copy the activities to
        /// </summary>
        public IEnumerable<int> CopyToSectionIds { get; set; }

        /// <summary>
        /// The start date the first activity should be copied to.  If null, we use day number logic from beginning of grading period. 
        /// Example: Copy From 
        ///             Grading Period Start Date 9/19/2005 
        ///             Activity1 Date: 9/20/2005 
        ///             Activity2 Date 9/22/2005 
        ///         Copy To Grading Period starts 9/18/2006.  
        ///             If start date is blank, 
        ///                 Activity 1 Date would be 9/19/2006 
        ///                 Activity 2 Date would be 9/21/2006  
        ///             If start date of 9/26/2006 is provided
        ///                 Activity 1 Date would be 9/26/2006 
        ///                 Activity 2 Date would be 9/28/2006
        /// </summary>
        public DateTime? StartDate { get; set; }
    }
}
