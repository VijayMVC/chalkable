using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;

namespace Chalkable.StiConnector.Mapping
{
    public class InowErrors
    {
        public const string ACTIVITY_DATE_INVALID_RANGE_FOR_SECION_ERROR = "Activity_Date_InvalidRangeForSection";
        public const string ACTIVITY_DELETEFAILED_ACTIVITYSCORESFOUND_ERROR = "Activity_DeleteFailed_ActivityScoresFound";
        public const string ACTIVITY_MAXIMUM_SCORE_REQUIRED_IF_SCORED_ERROR = "Activity_Maximum_Score_RequiredIfScored";
        public const string ACTIVITY_MAXIMUM_SCORE_INVALID_RANGE_0_TO_9999_99_ERROR = "Activity_Maximum_Score_InvalidRange_0to9999_99";
        public const string ACTIVITY_MAYBEDROPPED_REQUIRED_IF_SCORED_ERROR = "Activity_MayBeDropped_RequiredIfScored";
        public const string ACTIVITY_NAME_MUST_BE_ALPHANUMERIC_ONLY_ERROR = "Activity_Name_MustBeAlphaNumericOnly";
        public const string ACTIVITY_NAME_REQUEIRED_ERROR = "Activity_Name_Required";
        public const string ACTIVITY_NAME_TOO_LONG_ERROR = "Activity_Name_TooLong";
        public const string ACTIVITY_UNIT_MUST_BE_ALPHA_NUMERIC_ONLY_ERROR = "Activity_Unit_MustBeAlphaNumericOnly";
        public const string ACTIVITY_UNIT_TOO_LONG_ERROR = "Activity_Unit_TooLong";
        public const string ACTIVITY_SECTION_NAME_DATE_MUSTBEUNIQUE_ERROR = "Activity_Section_Name_Date_MustBeUnique";
        public const string ACTIVITY_WEIGHTADDTION_INVALID_RANGE_ERROR = "Activity_WeightAddition_InvalidRange";
        public const string ACTIVITY_WEIGHTMULTIPLIER_INVALIDRANGE_0TO999_99999_ERROR = "Activity_WeightMultiplier_InvalidRange_0to999_99999";

        public const string ACTIVITYASSIGNEDATTRIBUTE_TEXT_REQUIRED_ERROR = "ActivityAssignedAttribute_Text_Required";

        public const string ACTIVITYCATEGORY_CANNOTDELETE_GRADEBOOKCATEGORIES_ERROR =
            "ActivityCategory_CannotDelete_GradeBookCategories";

        public const string ACTIVITYCATEGORY_DELETE_INUSEFORACTIVITY_ERROR = "ActivityCategory_Delete_InUseForActivity";
        public const string ACTIVITYCATEGORY_DESCRIPTION_INVALIDFORMAT_ERROR = "ActivityCategory_Description_InvalidFormat";

        public const string ACTIVITYCATEGORY_DESCRIPTION_TOOLONG_ERROR = "ActivityCategory_Description_TooLong";
        public const string ACTIVITYCATEGORY_PERCENTAGE_TOOLOW_ERROR = "ActivityCategory_Percentage_TooLow";

        public const string ACTIVITYCATEGORY_HIGHSCORESTODROP_TOOLOW_ERROR = "ActivityCategory_HighScoresToDrop_TooLow";
        public const string ACTIVITYCATEGORY_LOWSCORESTODROP_TOOLOW_ERROR = "ActivityCategory_LowScoresToDrop_TooLow";
        public const string ACTIVITYCATEGORY_HIGHSCORESTODROP_TOOHIGH_ERROR = "ActivityCategory_HighScoresToDrop_TooHigh";
        public const string ACTIVITYCATEGORY_LOWSCORESTODROP_TOOHIGH_ERROR = "ActivityCategory_LowScoresToDrop_TooHigh";

        public const string ACTIVITYCATEGORY_NAME_INVALIDFORMAT_ERROR = "ActivityCategory_Name_InvalidFormat";
        public const string ACTIVITYCATEGORY_NAME_REQUIRED_ERROR = "ActivityCategory_Name_Required";
        public const string ACTIVITYCATEGORY_NAME_TOOLONG_ERROR = "ActivityCategory_Name_TooLong";
        public const string ACTIVITYCATEGORY_NAME_MUSTBEUNIQUE_ERROR = "ActivityCategory_Name_MustBeUnique";
        public const string ACTIVITYCATEGORY_PERCENTAGE_TOOHIGH_ERROR = "ActivityCategory_Percentage_TooHigh";

        public const string ACTIVITYSTANDARD_CANNOTDELETEINTEGRATEDSTANDARDORWITHVALUES_ERROR = "ActivityStandard_CannotDeleteIntegratedStandardOrWithValues";
        public const string ACTIVITYSTANDARDSCORE_MAXIMUMVALUETOOLARGE_ERROR = "ActivityStandardScore_MaximumValueTooLarge";

        public const string ACTIVITYSTANDARDSCORE_CORRECTVALUETOOLARGE_ERROR = "ActivityStandardScore_CorrectValueTooLarge";

        public const string ACTIVITYSTANDARDSCORE_MAXIMUMVALUEINVALIDFORMAT_ERROR = "ActivityStandardScore_MaximumValueInvalidFormat";

        public const string ACTIVITYSTANDARDSCORE_CORRECTVALUEINVALIDFORMAT_ERROR =
            "ActivityStandardScore_CorrectValueInvalidFormat";

        public const string ACTIVITYSTANDARDSCORE_CANNOTUPDATEINTEGRATEDSCORE_ERROR =
            "ActivityStandardScore_CannotUpdateIntegratedScore";

        public const string AVERAGE_CANNOTSETMULTIPLEACTIVITIESIFSINGLEACTIVITYAVERAGINGRULEISUSED_ERROR =
            "Average_CannotSetMultipleActivitiesIfSingleActivityAveragingRuleIsUsed";

        public const string AVERAGE_CANNOTDELETESYSTEMAVERAGE_ERROR = "Average_CannotDeleteSystemAverage";
        public const string AVERAGE_CANNOTDELETESCOREDNONSYSTEMAVERAGE_ERROR = "Average_CannotDeleteScoredNonSystemAverage";

        public const string
            AVERAGE_CANNOTMODIFYIFSECTIONAVERAGEMODIFICATIONISDISABLEDANDAVERAGEISCOMPOSEDOFSYSTEMAVERAGES_ERROR =
                "Average_CannotModifyIfSectionAverageModificationIsDisabledAndAverageIsComposedOfSystemAverages";

        public const string AVERAGE_CANNOTSPECIFYCIRCULARAVERAGEREFERENCE_ERROR = "Average_CannotSpecifyCircularAverageReference";

        public const string AVERAGE_AVERAGINGRULE_ONLYAVAILABLEFORSYSTEMAVERAGES_ERROR =
            "Average_AveragingRule_OnlyAvailableForSystemAverages";

        public const string AVERAGE_AVERAGINGRULE_MUSTINCLUDEPERCENTAGESIFAVERAGINGRULEISOTHERAVERAGES_ERROR =
            "Average_AveragingRule_MustIncludePercentagesIfAveragingRuleIsOtherAverages";

        public const string AVERAGE_AVERAGINGRULE_MUSTSUPPLYACTIVITIESIFAVERAGINGRULEISSINGLEORMANUAL_ERROR =
            "Average_AveragingRule_MustSupplyActivitiesIfAveragingRuleIsSingleOrManual";

        public const string AVERAGE_DESCRIPTION_INVALIDFORMAT_ERROR = "Average_Description_InvalidFormat";
        public const string AVERAGE_DESCRIPTION_TOOLONG_ERROR = "Average_Description_TooLong";
        public const string AVERAGE_NAME_INVALIDFORMAT_ERROR = "Average_Name_InvalidFormat";
        public const string AVERAGE_NAME_MUSTBEUNIQUE_ERROR = "Average_Name_MustBeUnique";
        public const string AVERAGE_NAME_REQUIRED_ERROR = "Average_Name_Required";
        public const string AVERAGE_NAME_TOOLONG_ERROR = "Average_Name_TooLong";

        public const string AVERAGE_HIGHSCORESTODROP_CANNOTDEFINEIFAVERAGINGRULEISSINGLEACTIVITY_ERROR =
            "Average_HighScoresToDrop_CannotDefineIfAveragingRuleIsSingleActivity";

        public const string AVERAGE_LOWSCORESTODROP_CANNOTDEFINEIFAVERAGINGRULEISSINGLEACTIVITY_ERROR =
            "Average_LowScoresToDrop_CannotDefineIfAveragingRuleIsSingleActivity";

        public const string AVERAGE_PERCENTAGE_OUTOFRANGE_ERROR = "Average_Percentage_OutOfRange";

        public const string AVERAGE_PERCENTAGE_EXEMPTIONPERCENTAGETOTALMUSTEQUALPERCENTAGETOTAL_ERROR =
            "Average_Percentage_ExemptionPercentageTotalMustEqualPercentageTotal";

        public const string AVERAGE_WEIGHTADDITION_CANNOTDEFINEIFSECTIONAVERAGEMODIFICATIONISDISABLED_ERROR =
            "Average_WeightAddition_CannotDefineIfSectionAverageModificationIsDisabled";

        public const string AVERAGE_WEIGHTADDITION_OUTOFRANGE_ERROR = "Average_WeightAddition_OutOfRange";
        public const string AVERAGECOMMENT_CANNOTDELETESTUDENTDATA_ERROR = "AverageComment_CannotDeleteStudentData";
        public const string AVERAGESCORE_CALCULATEDAVERAGE_OUTOFRANGE_ERROR = "AverageScore_CalculatedAverage_OutOfRange";
        public const string AVERAGESCORE_ENTEREDAVERAGE_OUTOFRANGE_ERROR = "AverageScore_EnteredAverage_OutOfRange";

        public const string AVERAGESCORE_ALPHAGRADENAME_VALUEISREQUIRED_ERROR = "AverageScore_AlphaGradeName_ValueIsRequired";
        public const string AVERAGESCORE_AVERAGINGEQUIVALENT_NOTFOUND_ERROR = "AverageScore_AveragingEquivalent_NotFound";

        public const string ALPHAGRADE_CANNOTDETERMINEALPHAGRADE_ERROR = "AlphaGrade_CannotDetermineAlphaGrade";
        
        public const string CLASSROOM_CANNOTPOST_SECTIONISNOTSCHEDULEDONTHISDAY_ERROR = "Classroom_CannotPost_SectionIsNotScheduledOnThisDay";
        public const string CLASSROOMOPTION_SEATINGCHARTROWS_TOOHIGH_ERROR = "ClassroomOption_SeatingChartRows_TooHigh";

        public const string CLASSROOMSTUDENT_COMMENT_INVALIDFORMAT_ERROR = "ClassroomStudent_Comment_InvalidFormat";
        public const string CLASSROOMSTUDENT_GRADINGPERIOD_ISREQUIRED_ERROR = "ClassroomStudent_GradingPeriod_IsRequired";
        public const string CLASSROOMSTUDENT_SECTION_ISREQUIRED_ERROR = "ClassroomStudent_Section_IsRequired";
        public const string CLASSROOMSTUDENT_STUDENT_ISREQUIRED_ERROR = "ClassroomStudent_Student_IsRequired";
        public const string DOCUMENT_DATA_REQUIRED_ERROR = "Document_Data_Required";

        public const string DOCUMENT_EXTENSION_REQUIRED_ERROR = "Document_Extension_Required";
        public const string DOCUMENT_MIMETYPE_REQUIRED_ERROR = "Document_MIMEType_Required";
        public const string DOCUMENT_AGGREGATEFILESIZE_EXCEEDED_ERROR = "Document_AggregateFileSize_Exceeded";
        public const string DOCUMENT_INDIVIDUALFILESIZE_EXCEEDED_ERROR = "Document_IndividualFileSize_Exceeded";
        public const string SCORE_ACTIVITY_ISREQUIRED_ERROR = "Score_Activity_IsRequired";
        public const string SCORE_ACTIVITYCANNOTBEDROPPED_ERROR = "Score_ActivityCannotBeDropped";
        public const string SCORE_ACTIVITYDOESNOTALLOWEXEMPTIONS_ERROR = "Score_ActivityDoesNotAllowExemptions";
        public const string SCORE_CANNOTHAVEALPHAANDALTERNATESCORES_ERROR = "Score_CannotHaveAlphaAndAlternateScores";

        public const string SCORE_CANNOTHAVEASCOREVALUE_FORANACTIVITYMARKEDASEXEMPT_ERROR =
            "Score_CannotHaveAScoreValue_ForAnActivityMarkedAsExempt";

        public const string SCORE_CANNOTMODIFYUNEXCUSEDABSENCESCORE_ERROR = "Score_CannotModifyUnexcusedAbsenceScore";
        public const string SCORE_INVALIDACTIVITYDROPSTATUS_ERROR = "Score_InvalidActivityDropStatus";
        public const string SCORE_INVALIDALPHAGRADE_ERROR = "Score_InvalidAlphaGrade";
        public const string SCORE_INVALIDALPHAGRADE_OR_ALTERNATESCORE_ERROR = "Score_InvalidAlphaGrade_Or_AlternateScore";
        public const string SCORE_INVALIDALPHAGRADERANGE_ERROR = "Score_InvalidAlphaGradeRange";
        public const string SCORE_INVALIDALTERNATESCORE_ERROR = "Score_InvalidAlternateScore";
        public const string SCORE_INVALIDALTERNATESCORERANGE_ERROR = "Score_InvalidAlternateScoreRange";
        public const string SCORE_INVALIDRANGE_NEG9999_99TO9999_99_ERROR = "Score_InvalidRange_neg9999_99to9999_99";
        public const string SCORE_STUDENT_ISREQUIRED_ERROR = "Score_Student_IsRequired";
        public const string SCORE_SCHOOL_ISREQUIRED_ERROR = "Score_School_IsRequired";
        public const string SCORECOPY_STUDENT_ISREQUIRED_ERROR = "ScoreCopy_Student_IsRequired";
        public const string SCORECOPY_TOACTIVITY_ISREQUIRED_ERROR = "ScoreCopy_ToActivity_IsRequired";
        public const string SCORECOPY_FROMACTIVITY_ISREQUIRED_ERROR = "ScoreCopy_FromActivity_IsRequired";
        public const string SCORECOPY_TOTALPOINTSPOSSIBLE_MUSTBEEQUAL_ERROR = "ScoreCopy_TotalPointsPossible_MustBeEqual";

        public const string SEATINGCHART_SEATROW_SEATCOLUMN_SHOULDBEUNIQUE_ERROR =
            "SeatingChart_SeatRow_SeatColumn_ShouldBeUnique";

        public const string SECTIONCHART_SEATCOLUMN_TOOHIGH_ERROR = "SectionChart_SeatColumn_TooHigh";
        public const string SECTIONCHART_SEATROW_TOOHIGH_ERROR = "SectionChart_SeatRow_TooHigh";
        public const string STANDARDSCORE_CANNOTDELETESTUDENTDATA_ERROR = "StandardScore_CannotDeleteStudentData";
        public const string STANDARDSCORE_NOTETOOLONG_ERROR = "StandardScore_NoteTooLong";

        public const string STANDARDSCORECOMMENT_CANNOTDELETESTUDENTDATA_ERROR =
            "StandardScoreComment_CannotDeleteStudentData";

        public const string STUDENTABSENCE_ABSENCELEVEL_REQUIRED_ERROR = "StudentAbsence_AbsenceLevel_Required";

        public const string STUDENTABSENCE_DATE_DOESNOTFALLWITHINACADEMICSESSION_ERROR =
            "StudentAbsence_Date_DoesNotFallWithinAcademicSession";

        public const string STUDENTABSENCE_DATE_ISNOTASCHOOLDAY_ERROR = "StudentAbsence_Date_IsNotASchoolDay";
        public const string STUDENTABSENCE_DATE_STUDENTNOTENROLLED_ERROR = "StudentAbsence_Date_StudentNotEnrolled";
        public const string STUDENTABSENCE_DATE_NOSAMEDAYENROLLMENT_ERROR = "StudentAbsence_Date_NoSameDayEnrollment";
        public const string STUDENTABSENCE_NOTE_TOOLONG_ERROR = "StudentAbsence_Note_TooLong";
        public const string STUDENTABSENCE_NOTE_INVALIDFORMAT_ERROR = "StudentAbsence_Note_InvalidFormat";
        public const string STUDENTDAILYABSENCE_DUPLICATEABSENCE_ERROR = "StudentDailyAbsence_DuplicateAbsence";
        public const string STUDENTGRADE_GRADEPOSTINGNOTALLOWED_ERROR = "StudentGrade_GradePostingNotAllowed";
        public const string STUDENTGRADINGCOMMENT_GRADINGPERIODID_REQUIRED_ERROR = "StudentGradingComment_GradingPeriodID_Required";

        public const string STUDENTGRADINGCOMMENT_MUSTBEUNIQUE_BYSTUDENTSECTIONANDGRADINGCOMMENTHEADERIDS_ERROR =
            "StudentGradingComment_MustBeUnique_ByStudentSectionAndGradingCommentHeaderIDs";

        public const string STUDENTGRADINGCOMMENT_SECTIONID_REQUIRED_ERROR = "StudentGradingComment_SectionID_Required";
        public const string STUDENTGRADINGCOMMENT_STUDENTID_REQUIRED_ERROR = "StudentGradingComment_StudentID_Required";

        public const string STUDENTGRADEDITEM_MUSTBEUNIQUE_BYSTUDENTSECTIONANDGRADEDITEMIDS_ERROR =
            "StudentGradedItem_MustBeUnique_ByStudentSectionAndGradedItemIDs";

        public const string STUDENTGRADEDITEM_NUMERICGRADE_ISREQUIREDIFALPHAGRADENOTSUPPLIEDANDGRADEDITEMNOTALPHAONLY_ERROR =
            "StudentGradedItem_NumericGrade_IsRequiredIfAlphaGradeNotSuppliedAndGradedItemNotAlphaOnly";

        public const string STUDENTGRADEDITEM_NUMERICGRADE_MUSTBENUMERIC_ERROR =
            "StudentGradedItem_NumericGrade_MustBeNumeric";

        public const string STUDENTGRADEDSTANDARD_CANNOTDELETESTUDENTDATA_ERROR =
            "StudentGradedStandard_CannotDeleteStudentData";

        public const string STUDENTPERIODABSENCE_DUPLICATEABSENCE_ERROR = "StudentPeriodAbsence_DuplicateAbsence";
        public const string STUDENTPERIODABSENCE_TIMESLOT_REQUIRED_ERROR = "StudentPeriodAbsence_TimeSlot_Required";

        public const string STUDENTPERIODABSENCE_TIMESLOT_NOTINSTUDENTSCHEDULE_ERROR =
            "StudentPeriodAbsence_TimeSlot_NotInStudentSchedule";

        public const string STUDENTSTANDARDCOMMENT_CANNOTDELETESTUDENTDATA_ERROR =
            "StudentStandardComment_CannotDeleteStudentData";
    }

    
    public class ErrorMapper
    {
        private static IDictionary<string, string> mapper = new Dictionary<string, string>
            {
                {InowErrors.ACTIVITY_DATE_INVALID_RANGE_FOR_SECION_ERROR, "Looks like you set the due date when class isn't scheduled. Try a different date."},
                {InowErrors.ACTIVITY_DELETEFAILED_ACTIVITYSCORESFOUND_ERROR, "You can't delete an item if there are already grades on it. Delete those grades to delete the item."},
                {InowErrors.ACTIVITY_MAXIMUM_SCORE_REQUIRED_IF_SCORED_ERROR, "Looks like you forgot to add a Max Score for your assignment. You need to add this if the item is gradable."},
                {InowErrors.ACTIVITY_MAXIMUM_SCORE_INVALID_RANGE_0_TO_9999_99_ERROR, "The max score has to be a number between 0 and 9,999."},
                {InowErrors.ACTIVITY_MAYBEDROPPED_REQUIRED_IF_SCORED_ERROR, "If your item is gradable, it needs to be droppable. "},
                {InowErrors.ACTIVITY_NAME_MUST_BE_ALPHANUMERIC_ONLY_ERROR, "Looks like you have invalid charachters in your assignment name. Try sticking to letters and numbers."},
                {InowErrors.ACTIVITY_NAME_REQUEIRED_ERROR, "Looks like you forgot to add a name to your assignment."},
                {InowErrors.ACTIVITY_NAME_TOO_LONG_ERROR, "Looks like your assignment name is a bit too long."},
                {InowErrors.ACTIVITY_WEIGHTADDTION_INVALID_RANGE_ERROR, "The weight addition needs to be a number between 0 and 9,999."},
                {InowErrors.ACTIVITY_WEIGHTMULTIPLIER_INVALIDRANGE_0TO999_99999_ERROR, "The weight multiplier needs to be a number between 0 and 9,999."},
                {InowErrors.ACTIVITYASSIGNEDATTRIBUTE_TEXT_REQUIRED_ERROR, "Looks like you forgot to include any text with your assignment. "},

                {InowErrors.ACTIVITYCATEGORY_DESCRIPTION_INVALIDFORMAT_ERROR, String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Description")},
                {InowErrors.ACTIVITYCATEGORY_DESCRIPTION_TOOLONG_ERROR, "Description is too long. Please re-enter Description."},
                {InowErrors.ACTIVITYCATEGORY_PERCENTAGE_TOOLOW_ERROR, "Percentage's value is too low."},
                {InowErrors.ACTIVITYCATEGORY_HIGHSCORESTODROP_TOOLOW_ERROR,  "HighScores To Drop's value is too low."},

                {InowErrors.ACTIVITYCATEGORY_LOWSCORESTODROP_TOOLOW_ERROR, "LowScores To Drop's value is too low."},
                {InowErrors.ACTIVITYCATEGORY_HIGHSCORESTODROP_TOOHIGH_ERROR, "HighScores To Drop's value is too high."},
                {InowErrors.ACTIVITYCATEGORY_LOWSCORESTODROP_TOOHIGH_ERROR, "LowScores To Drop's value is too high."},
                {InowErrors.ACTIVITYCATEGORY_NAME_INVALIDFORMAT_ERROR,  String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Name")},
                {InowErrors.ACTIVITYCATEGORY_NAME_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Name")}, 
                {InowErrors.ACTIVITYCATEGORY_NAME_TOOLONG_ERROR,  "Name is too long. Please re-enter Name."},

                {InowErrors.AVERAGE_CANNOTDELETESYSTEMAVERAGE_ERROR,  "Cannot delete System Average."},
                {InowErrors.AVERAGE_CANNOTDELETESCOREDNONSYSTEMAVERAGE_ERROR,  "Cannot delete average because scores have been directly entered for students."}, 
                {InowErrors.AVERAGE_AVERAGINGRULE_MUSTINCLUDEPERCENTAGESIFAVERAGINGRULEISOTHERAVERAGES_ERROR,  "Must include percentages if averaging rule is other averages."},

                {InowErrors.AVERAGE_DESCRIPTION_INVALIDFORMAT_ERROR,  String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Description")},
                {InowErrors.AVERAGE_DESCRIPTION_TOOLONG_ERROR,  "Description is too long. Please re-enter Description."}, 
                {InowErrors.AVERAGE_NAME_INVALIDFORMAT_ERROR,  String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Name")},

                {InowErrors.AVERAGE_NAME_MUSTBEUNIQUE_ERROR,   String.Format(ChlkResources.ERR_DUPLICATE_ERROR_MSG_FORMAT, "Name")}, 
                {InowErrors.AVERAGE_NAME_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Name")},

                {InowErrors.AVERAGE_NAME_TOOLONG_ERROR,  "Name is too long. Please re-enter Name."},
                {InowErrors.AVERAGE_HIGHSCORESTODROP_CANNOTDEFINEIFAVERAGINGRULEISSINGLEACTIVITY_ERROR,  "Cannot define High Scores To Drop if averaging rule is Single Activity."}, 
                {InowErrors.AVERAGE_LOWSCORESTODROP_CANNOTDEFINEIFAVERAGINGRULEISSINGLEACTIVITY_ERROR,  "Cannot define Low Scores To Drop if averaging rule is Single Activity."},


                {InowErrors.AVERAGE_PERCENTAGE_OUTOFRANGE_ERROR,  "Percentage is out of range."},
                {InowErrors.AVERAGE_WEIGHTADDITION_OUTOFRANGE_ERROR,  "Weight Addition is out of range."}, 
                {InowErrors.AVERAGECOMMENT_CANNOTDELETESTUDENTDATA_ERROR,  ChlkResources.ERR_DELETE_IN_USE_MSG_FORMAT},

                {InowErrors.AVERAGESCORE_CALCULATEDAVERAGE_OUTOFRANGE_ERROR,  "Calculated Average is out of range."},
                {InowErrors.AVERAGESCORE_ENTEREDAVERAGE_OUTOFRANGE_ERROR,  "Entered Average is out of range."}, 
                {InowErrors.CLASSROOMOPTION_SEATINGCHARTROWS_TOOHIGH_ERROR,  "Seating Chart Rows' value is too high."},
                {InowErrors.SEATINGCHART_SEATROW_SEATCOLUMN_SHOULDBEUNIQUE_ERROR,   String.Format(ChlkResources.ERR_DUPLICATE_ERROR_MSG_FORMAT, "Seat row, Seat column")},
                {InowErrors.SECTIONCHART_SEATCOLUMN_TOOHIGH_ERROR,  "Seat column 's value is too high."},
                {InowErrors.SECTIONCHART_SEATROW_TOOHIGH_ERROR,  "Seat row 's value is too high."},
                {InowErrors.AVERAGE_CANNOTMODIFYIFSECTIONAVERAGEMODIFICATIONISDISABLEDANDAVERAGEISCOMPOSEDOFSYSTEMAVERAGES_ERROR,  "Average cannot be modified if section average modification is disabled and average is composed of system averages."}, 
                {InowErrors.AVERAGE_WEIGHTADDITION_CANNOTDEFINEIFSECTIONAVERAGEMODIFICATIONISDISABLED_ERROR,  "Weight Addition cannot be defined if section average modification is disabled."},
                
                {InowErrors.CLASSROOMSTUDENT_COMMENT_INVALIDFORMAT_ERROR,  String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Comment")},
                {InowErrors.CLASSROOMSTUDENT_SECTION_ISREQUIRED_ERROR,   String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Section")},
                {InowErrors.CLASSROOMSTUDENT_STUDENT_ISREQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Student")},
                {InowErrors.CLASSROOMSTUDENT_GRADINGPERIOD_ISREQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Grading Period")},
         

                {InowErrors.ALPHAGRADE_CANNOTDETERMINEALPHAGRADE_ERROR,  "Alpha Grade cannot be determined."},
                {InowErrors.AVERAGE_CANNOTSETMULTIPLEACTIVITIESIFSINGLEACTIVITYAVERAGINGRULEISUSED_ERROR,   "Multiple Activities cannot be set if Single Activity's Averageing Rule is using."},
                {InowErrors.AVERAGE_CANNOTSPECIFYCIRCULARAVERAGEREFERENCE_ERROR,  "Circular Average Reference cannot be specified."},
                {InowErrors.SCORE_CANNOTMODIFYUNEXCUSEDABSENCESCORE_ERROR,  "The student has an unexcused absence for this day, only a 0 score can be entered for the activity."},
         
                {InowErrors.SCORE_INVALIDRANGE_NEG9999_99TO9999_99_ERROR,  "The value of Score is out of range. Please re-enter Score."},
                {InowErrors.SCORE_INVALIDALPHAGRADERANGE_ERROR,   "Alpha Grade is not in range. Please re-enter Alpha Grade."},
                {InowErrors.SCORE_ACTIVITY_ISREQUIRED_ERROR,   String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Activity")},
                {InowErrors.SCORE_STUDENT_ISREQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Student")},
         
                {InowErrors.STUDENTDAILYABSENCE_DUPLICATEABSENCE_ERROR,  ChlkResources.DUPLICATE_ENTRY_MSG},
                {InowErrors.SCORE_ACTIVITYCANNOTBEDROPPED_ERROR,   "Activity cannot be dropped."},
                {InowErrors.SCORE_INVALIDACTIVITYDROPSTATUS_ERROR,   "Activity Drop Status is invalid."},
                {InowErrors.STUDENTPERIODABSENCE_DUPLICATEABSENCE_ERROR, ChlkResources.DUPLICATE_ENTRY_MSG},
    
                {InowErrors.AVERAGESCORE_ALPHAGRADENAME_VALUEISREQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Alpha Grade Name")},
                {InowErrors.AVERAGESCORE_AVERAGINGEQUIVALENT_NOTFOUND_ERROR,   "Averaging Equivalent is not found."},
             
                {InowErrors.ACTIVITYSTANDARDSCORE_MAXIMUMVALUETOOLARGE_ERROR,   "Maximum value is too large for activity."},
                {InowErrors.ACTIVITYSTANDARDSCORE_CORRECTVALUETOOLARGE_ERROR, "Correct value is too large for activity."},
             
                {InowErrors.ACTIVITYSTANDARDSCORE_MAXIMUMVALUEINVALIDFORMAT_ERROR,   String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Maximum Value")},
                {InowErrors.ACTIVITYSTANDARDSCORE_CORRECTVALUEINVALIDFORMAT_ERROR, String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Correct Value")},
                {InowErrors.ACTIVITYSTANDARDSCORE_CANNOTUPDATEINTEGRATEDSCORE_ERROR,   "You cannot update the Integrated Score."},
            
                {InowErrors.STANDARDSCORE_NOTETOOLONG_ERROR, String.Format(ChlkResources.ERR_TOO_LONG_MSG_FORMAT, "Note", "255")},
         
                {InowErrors.ACTIVITYSTANDARD_CANNOTDELETEINTEGRATEDSTANDARDORWITHVALUES_ERROR,   "Cannot delete because it is an integrated standard or student has entered values for it."},
                {InowErrors.STANDARDSCORE_CANNOTDELETESTUDENTDATA_ERROR,  String.Format(ChlkResources.ERR_DELETE_IN_USE_MSG_FORMAT, "Student")},
                {InowErrors.STANDARDSCORECOMMENT_CANNOTDELETESTUDENTDATA_ERROR,  String.Format(ChlkResources.ERR_DELETE_IN_USE_MSG_FORMAT, "Student")},
         
                {InowErrors.STUDENTGRADEDSTANDARD_CANNOTDELETESTUDENTDATA_ERROR,   String.Format(ChlkResources.ERR_DELETE_IN_USE_MSG_FORMAT, "Student")},
                {InowErrors.STUDENTSTANDARDCOMMENT_CANNOTDELETESTUDENTDATA_ERROR,  String.Format(ChlkResources.ERR_DELETE_IN_USE_MSG_FORMAT, "Student")},
                {InowErrors.CLASSROOM_CANNOTPOST_SECTIONISNOTSCHEDULEDONTHISDAY_ERROR,  "Cannot post attendance because the Section is not scheduled on this day."},

                {InowErrors.ACTIVITY_UNIT_MUST_BE_ALPHA_NUMERIC_ONLY_ERROR,  "Unit must be AlphaNumeric.Please re-enter Unit."},
                {InowErrors.ACTIVITY_UNIT_TOO_LONG_ERROR,  String.Format(ChlkResources.ERR_TOO_LONG_MSG_FORMAT, "Unit", "50")},
                {InowErrors.SCORE_ACTIVITYDOESNOTALLOWEXEMPTIONS_ERROR,  "This Activity is not matched with a Graded Item that allows exemptions."},
                {InowErrors.SCORE_CANNOTHAVEASCOREVALUE_FORANACTIVITYMARKEDASEXEMPT_ERROR,  "The Activity is marked as exempt. No score can be entered for exempt activities."},
                {InowErrors.SCORE_INVALIDALPHAGRADE_OR_ALTERNATESCORE_ERROR,  "The New Score is in an invalid format.  Please re-enter the New Score."},
           
           
                {InowErrors.DOCUMENT_DATA_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Document Data")},
                {InowErrors.DOCUMENT_EXTENSION_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Document Extension")},
                {InowErrors.DOCUMENT_MIMETYPE_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "MIMEType")},
                {InowErrors.DOCUMENT_AGGREGATEFILESIZE_EXCEEDED_ERROR,  "District-defined storage capacity for attachments is full. Please contact your administrator."},
                {InowErrors.DOCUMENT_INDIVIDUALFILESIZE_EXCEEDED_ERROR,  "File exceeds the {0} MB file limit."},
                
                {InowErrors.STUDENTABSENCE_ABSENCELEVEL_REQUIRED_ERROR, String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Absence Level")},
                {InowErrors.STUDENTABSENCE_DATE_DOESNOTFALLWITHINACADEMICSESSION_ERROR,  "Absence Date is invalid. Absence Date must fall within academic session."},
                {InowErrors.STUDENTABSENCE_DATE_ISNOTASCHOOLDAY_ERROR,  "Absence Date is not a school day. Please re-enter with a school day."},
                {InowErrors.STUDENTABSENCE_DATE_STUDENTNOTENROLLED_ERROR,  "Student has not been enrolled. Please enroll student before adding daily absence."},
                {InowErrors.STUDENTABSENCE_DATE_NOSAMEDAYENROLLMENT_ERROR,  "Student has an enrollment or withdrawal on this date. Daily attendance records should not be created."},
           
                {InowErrors.STUDENTABSENCE_NOTE_TOOLONG_ERROR,  String.Format(ChlkResources.ERR_TOO_LONG_MSG_FORMAT, "Note", "255")},
                {InowErrors.STUDENTABSENCE_NOTE_INVALIDFORMAT_ERROR,  String.Format(ChlkResources.ERR_INVALID_FORMAT_MSG_FORMAT, "Note")},
                {InowErrors.STUDENTGRADE_GRADEPOSTINGNOTALLOWED_ERROR,  "Current grading period is closed for grade posting."},
                {InowErrors.STUDENTGRADINGCOMMENT_GRADINGPERIODID_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "GradingPeriod")},
                {InowErrors.STUDENTGRADINGCOMMENT_MUSTBEUNIQUE_BYSTUDENTSECTIONANDGRADINGCOMMENTHEADERIDS_ERROR,  "Student Grading Comment must be unique in Student Sections and Grading Comment Headers."},
                
                {InowErrors.STUDENTGRADINGCOMMENT_SECTIONID_REQUIRED_ERROR, String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Section")},
                {InowErrors.STUDENTGRADINGCOMMENT_STUDENTID_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Student")},
                {InowErrors.STUDENTGRADEDITEM_MUSTBEUNIQUE_BYSTUDENTSECTIONANDGRADEDITEMIDS_ERROR,  "Student Graded Item must be unique in Student Sections and Graded Items."},
                {InowErrors.STUDENTGRADEDITEM_NUMERICGRADE_ISREQUIREDIFALPHAGRADENOTSUPPLIEDANDGRADEDITEMNOTALPHAONLY_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Numeric Grade")},
                {InowErrors.STUDENTGRADEDITEM_NUMERICGRADE_MUSTBENUMERIC_ERROR,  "Numeric Grade must be numeric."},
            
                {InowErrors.STUDENTPERIODABSENCE_TIMESLOT_REQUIRED_ERROR,  String.Format(ChlkResources.ERR_REQUIRED_FIELD_MSG_FORMAT, "Period")},
                {InowErrors.STUDENTPERIODABSENCE_TIMESLOT_NOTINSTUDENTSCHEDULE_ERROR,  "Period is not in Student Schedule."},

            }; 

           

        public string Map(string inowError)
        {
            return mapper.ContainsKey(inowError) ? mapper[inowError] : null;
        }

        public string BackMap(string chalkableError)
        {
            if (mapper.Any(x => x.Value == chalkableError))
                return mapper.First(x => x.Value == chalkableError).Key;
            return null;
        }
    }
}
