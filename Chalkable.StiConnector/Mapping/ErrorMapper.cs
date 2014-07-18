using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                {InowErrors.ACTIVITYASSIGNEDATTRIBUTE_TEXT_REQUIRED_ERROR, "Looks like you forgot to include any text with your assignment. "}
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
