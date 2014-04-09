/**
 * Created with JetBrains WebStorm.
 * User: Ostap
 * Date: 24.05.13
 * Time: 12:54
 * To change this template use File | Settings | File Templates.
 */

String.format = function() {
  var s = arguments[0];
  for (var i = 0; i < arguments.length - 1; i++) {
    var reg = new RegExp("\\{" + i + "\\}", "gm");
    s = s.replace(reg, arguments[i + 1]);
  }

  return s;
}

function OneOrManyInst(txt, needE){
    var text = txt || '';

    this.getText = function(many){
        this.toString = function(){
            return text;
        };

        return text + (many ? (needE ? 'es' : 's') : '');
    };

    this.setText = function(txt){
        text = txt;
    };

}

function oneOrMany(text, needE){
    var inst = new OneOrManyInst(text, needE);
    var res = inst.getText;
    res.toString = function(){
        return this.getText(false);
    }.bind(inst);
    return res;
}

var Msg = {
    //Simple words-----------------------------------------------------

    Accept: 'Accept',
    Accepted: 'Accepted',
    Add: 'Add',
    Add_File: 'Add File',
    Added: 'Added',
    Admin: oneOrMany('Admin'),
    Adjusted: 'Adjusted',
    All: 'All',
    Amount: 'Amount',
    App: oneOrMany('App'),
    Approve: 'Approve',
    Arrival: 'Arrival',
    Ask: 'Ask',
    Announcement: oneOrMany('Announcement'),
    Answer: 'Answer',
    Average: 'Average',
    Avg: 'Avg',
    Attachment: oneOrMany('Attachment'),
    Attendance: 'Attendance',
    Balance: 'Balance',
    Banner: 'Banner',
    Birthday: 'Birthday',
    Box: oneOrMany('Box', true),
    Browse: 'Browse',
    Cancel: 'Cancel',
    Card: 'Card',
    Cell: 'Cell',
    Check: 'Check',
    CheckIn: 'Check-in',
    Class: 'Class',
    Classes: 'Classes',
    Click: 'Click',
    Code: 'Code',
    Comment: 'Comment',
    Commented: 'Commented',
    Complete: 'Complete',
    Completed: 'Completed',
    Confirm: 'Confirm',
    Course: 'Course',
    Created: 'Created',
    Created_by: 'Created by',
    Created_on: 'Created on',
    Credits: 'Credits',
    Daily: 'Daily',
    Day: oneOrMany('Day'),
    Decline: 'Decline',
    Delete: 'Delete',
    Department: 'Department',
    Description: 'Description',
    Destination: 'Destination',
    Details: 'Details',
    Discard: 'Discard',
    Discipline: 'Discipline',
    Done: 'Done',
    Drop: 'Drop',
    Dropped: 'Dropped',
    Due: 'Due',
    Edit: 'Edit',
    Error: 'Error',
    Exempt: 'Exempt',
    Failed: 'Failed',
    Format: 'Format',
    Frequency: 'Frequency',
    Free: 'Free',
    File: 'File',
    Fill: 'Fill',
    First: 'First',
    Gender: 'Gender',
    Grade: oneOrMany('Grade'),
    Graded: 'Graded',
    Grading: 'Grading',
    Grid: 'Grid',
    Home: 'Home',
    Hour: 'Hour',
    Icon: oneOrMany('Icon'),
    Id: 'Id',
    Import: 'Import',
    Incomplete: 'Incomplete',
    Item: oneOrMany('Item'),
    Keywords: 'Keywords',
    Last: 'Last',
    Less: 'Less',
    Live: 'Live',
    List: 'List',
    Message: 'Message',
    Messaging: 'Messaging',
    Method: oneOrMany('Method'),
    Mistake: 'Mistake',
    Month: oneOrMany('Month'),
    Monthly: 'Monthly',
    Multiple: 'Multiple',
    Name: 'Name',
    New: 'New',
    Newest: 'Newest',
    Next: 'Next',
    Now: 'Now',
    Other: oneOrMany('Other'),
    OK: 'OK',
    Out_of: 'out of',
    Overview: 'Overview',
    Paid: 'Paid',
    Parameter: oneOrMany('Parameter'),
    Parent: oneOrMany('Parent'),
    Percentile: 'Percentile',
    Participation: 'Participation',
    Period: 'Period',
    Person: oneOrMany('Person'),
    Personal: 'Personal',
    PO_header: 'PO#',
    Popular: 'Popular',
    Pricing: 'Pricing',
    Profile: 'Profile',
    Question: 'Question',
    Ready: 'Ready',
    Reason: 'Reason',
    Recent: 'Recent',
    Reject: 'Reject',
    Rejected: 'Rejected',
    Remove: 'Remove',
    Reminder: oneOrMany('Reminder'),
    Required: 'Required',
    Room: 'Room',
    Save: 'Save',
    Saved: 'Saved',
    Saving: 'Saving',
    Schedule: 'Schedule',
    Scheduled: 'Scheduled',
    School: 'School',
    Score: 'Score',
    Screenshot: oneOrMany('Screenshot'),
    Settings: 'Settings',
    Simple: 'Simple',
    Source: 'Source',
    Staff: 'Staff',
    Standard: oneOrMany('Standard'),
    Start: 'Start',
    Status: 'Status',
    Summary: 'Summary',
    Subject: oneOrMany('Subject'),
    Submit: 'Submit',
    Submited: 'Submited',
    Success: 'Success',
    Terms: 'Terms',
    Test: 'Test',
    Title: 'Title',
    To: 'To',
    Today: 'Today',
    Tomorrow: 'Tomorrow',
    Total: 'Total',
    Type: 'Type',
    Yesterday: 'Yesterday',
    Your_Vs_Your_Class: 'Your Vs Your Class',
    Usually: 'Usually',
    Value: 'Value',
    Week: oneOrMany('Week'),
    Weekly: 'Weekly',
    Weight: 'Weight',
    Whoa: 'whoa.',

    Sunday: 'Sunday',
    Monday: 'Monday',
    Tuesday: 'Tuesday',
    Wednesday: 'Wednesday',
    Thursday: 'Thursday',
    Friday: 'Friday',
    Saturday: 'Saturday',

    January: 'January',
    February: 'February',
    March: 'March',
    April: 'April',
    May: 'May',
    June: 'June',
    July: 'July',
    August: 'August',
    September: 'September',
    October: 'October',
    November: 'November',
    December: 'December',

    Absent: 'Absent',
    Late: 'Late',
    Excused: 'Excused',
    Present: 'Present',
    NA: 'N/A',

    //END Simple words-----------------------------------------------------


    Absent_Average_MP: function(mp_name){
        return String.format('Absent Average {0}', mp_name)
    },
    Absent_from_school: 'Absent from school',
    Absent_from_a_class: 'Absent from a class',
    Absent_students: 'Absent students',
    Absent_Today: 'Absent Today',
    Add_comment: 'Add comment',
    Add_a_comment: 'Add a comment',
    Add_discipline: 'Add discipline',
    Add_reminder: 'Add Reminder',
    Add_remove_students: 'Add/Remove students',
    Add_student: 'Add student',
    Add_your_students: 'Add your students',
    Advanced_options: 'Advanced options...',
    All_day: 'All day',
    All_categories: 'All categories',
    All_standards: 'All standards',
    All_students: 'All students',
    All_subjects: 'All subjects',
    All_teachers: 'All teachers',
    Answer_the_question: 'Answer the question',
    API_Access: 'API Access',
    Application: oneOrMany('Application'),
    App_budget: 'App budget',
    App_Name: 'App Name',
    App_Name_empty_text: 'Your app\'s name',
    App_Paginator_text: 'Applications {0} - {1} of {2}',
    App_URL: 'App URL',
    App_URL_empty_text: 'The URL where your app lives',
    Ask_a_question: 'Ask a question',
    Attach_an_App: 'Attach an App',
    Attach_file: 'Attach file',
    Attached: 'Attached',
    Attendance_by_Period: 'Attendance by Period',
    Attendance_MP: 'Attendance MP',
    Attendance_count: 'Attendance count',
    Attendance_Score: 'Attendance Score',
    Attendance_today: 'Attendance today',
    Auto_grade: 'Auto-grade',
    Auto_Emails: 'Auto Emails',
    Can_Teachers_launch_app: function(app){
        return 'Can Teachers launch <span style="max-width: 123px;" class="application-name">' + (app || 'application') + ' in My Apps?';
    },
    Can_Teachers_attach_app: function(app){
        return 'Can Teachers attach <span style="max-width: 123px;" class="application-name">' + (app || 'application') + ' in New Items?';
    },
    Can_Students_launch_app: function(app){
        return 'Can Students launch <span style="max-width: 123px;" class="application-name">' + (app || 'application') + ' in My Apps?';
    },
    Can_Admins_launch_app: function(app){
        return 'Can Admins launch <span class="application-name">' + (app || 'application') + ' in My Apps?';
    },
    Can_Parents_launch_app: function(app){
        return 'Can Parents launch <span class="application-name">' + (app || 'application') + ' in My Apps?';
    },
    Base_Info: 'Base Info',
    Basic_info: 'Basic info',
    Best_Class: 'Best Class',
    Biggest_improvement: 'Biggest improvement',
    Can_drop_student_score: 'Can drop student score',
    Choose_date: 'Choose date',
    Choose_picture: 'Choose picture',
    Class_Avg: 'Class Avg',
    Class_average: 'Class average',
    Class_cost: 'Class cost',
    Class_size: 'Class size',
    Class_flat_rate: 'Class flat rate',
    Class_flat_rate_check: 'Is there a flat rate for whole class purchases?',
    Class_ID: 'Class ID',
    Class_Of: 'Class of',
    Class_Ranking: 'Class Ranking',
    Classmates: 'Classmates',
    Click_to_add_standard: 'CLICK TO ADD STANDARDS',
    Click_to_complete_transaction: 'Click the button to complete the transaction',
    Code_examples: 'Code examples',
    Coming_soon: 'Coming soon',
    Comments_for: 'Comments for',
    Cost_per_user: 'Cost per user',
    Create_AutoEmail: 'Create AutoEmail',
    Create_grid: 'Create grid',
    Day_Of_Month: 'Day Of Month',
    Day_Of_Week: 'Day Of Week',
    Days_Ago: function(n){
        return String.format("{0} day{1} ago", n, (n == 1 ? '' : 's'));
    },
    Developer_Email: 'Developer Email',
    Developer_Name: 'Developer Name',
    Developer_Website: 'Developer Website',
    Destination_Name: 'Destination Name',
    Discipline_count: 'Discipline count',
    Discipline_on_date: function(date){
        return String.format('Discipline on {0}',date.format('mm/dd/yy'));
    },
    Discipline_today: 'Discipline today',
    Discipline_MP: 'Discipline MP',
    Document_Attached: 'Document Attached',
    Dont_attach: 'Don\'t attach',
    Download_original: 'Download original',
    Download_marked_up: 'Download marked-up PDF',
    Download_Template: 'Download Template',
    Due_days_ago: function(days){
        return String.format("Due {0} days ago", days);
    },
    Due_in_days: function(days){
        return String.format("Due in {0} days", days);
    },
    Drop_lowest: 'Drop lowest',
    Due_date: 'Due date',
    Due_today: 'Due today',
    Due_tomorrow: 'Due tomorrow',
    Due_yesterday: 'Due yesterday',
    Edit_grid: 'Edit grid',
    Edit_the_student_grid: 'Edit the student grid',
    Email: 'Email',
    Empty_new_item_message_text: 'Message',
    Enter_app_name: 'Enter app name',
    Enter_when_done: 'Enter when done',
    Error_message: 'Error message',
    Excused_from_a_class: 'Excused from a class',
    Final_Grade: oneOrMany('Final Grade'),
    First_name: 'First name',
    General_Information: 'General Information',
    Go_Back: 'Go Back',
    GOT_IT: 'Got It',
    GPA: 'GPA',
    GradeLevel: 'GradeLevel',
    Grade_Level: 'Grade Level',
    Grade_manually: 'Grade manually',
    Grade_type: oneOrMany('Grade type'),
    Grading_Period: 'Grading Period',
    Grading_view: 'Grading view',
    Grading_style: 'Grading style',
    Hide_from_Students: 'Hide from Students',
    Highest_rated: 'Highest rated',
    hours_ago: function(hrs){
        if(hrs == 1)
            return '1 hour ago';
        return String.format("{0} hours ago", hrs);
    },
    Home_Address: 'Home Address',
    Home_Phone: 'Home Phone',
    How_do_you_grade_students_in: function(courseName){
        return 'How do you grade students in ' + courseName + '?'
    },
    Import_Types: 'Import Types',
    Info_this_app_uses: 'Info this app uses:',
    Install_App: 'Install App',
    Invite_users: 'Invite users',
    IsInternal: 'IsInternal',
    Issues_right_now: 'Issues right now',
    Item_Ready_to_be_Graded: 'Item Ready to be Graded',
    Just_me: 'Just me',
    Last_name: 'Last name',
    Late_students: 'Late students',
    Last_week: 'Last week',
    Late_to_a_class: 'Late to a class',
    Lets_do_it: 'Lets do it',
    Loading_text: 'Loading...',
    Long_Description: 'Long Description',
    Long_description_empty_text : 'Between 500-1500 characters works well',
    Lose_Changes: 'Lose Changes',
    Max_score: 'Max score',
    Mark_all_Present: 'Mark all Present',
    Marking_Period: oneOrMany('Marking Period'),
    Marking_Period_shortcut: 'Mp',
    min: oneOrMany('min'),
    Mins_Late: 'Mins Late',
    minutes_ago: function(mins){
        if(mins == 1)
            return '1 minute ago';
        return String.format("{0} mins ago", mins);
    },
    More_students: 'More students',
    My_Apps: 'My Apps',
    My_Students: 'My Students',
    My_Teachers: 'My Teachers',
    Name_already_taken: 'Name already taken',
    Needs_improvement: 'Needs improvement',
    Next_Class: 'Next Class',
    New_Item: 'New Item',
    New_SIS_info_CSV: "New SIS info / CSV",
    No_applications: 'No applications to display',
    No_class_scheduled: function(date){
        return String.format('No class scheduled on {0}', date);
    },
    No_check_in : 'No check-in',
    No_data: 'No data',
    No_grades_yet: 'No grades yet',
    No_items_marked_important: 'No items marked important.',
    No_new_notifications: 'No new notifications',
    No_notifications_to_display: 'No notifications to display',
    No_reason: 'No reason',
    Not_In_Class_Now: 'Not In Class Now',
    Not_in_class_right_now: 'Not in class right now',
    Nothing_to_display: 'Nothing to display',
    Notification_paginator_text: 'Days {0} - {1} of {2}',
    Not_Graded: 'Not Graded',
    Number_of_columns: 'Number of columns :',
    Number_of_students: 'Number of students',
    Number_of_rows: 'Number of rows :',
    Password_reset: 'Password reset',
    Parent_info: 'Parent Info',
    Old_Password: 'Old Password',
    New_Password: 'New Password',
    New_Password_Confirm: 'Confirm New Password',
    Press_enter: 'Press enter',
    Recent_Downloads: 'Recent Downloads',
    Recently_Graded_Items: 'Recently Graded Items',
    Right_now: 'Right now',
    Room_number: 'Room number',
    Room_shortcut: 'RM',
    Search_api: 'Search for an api method or parameter to view its call tree...',
    Scores_exceeds: 'Score exceeds maximum points allowed',
    School_Avg: 'School Avg.',
    School_cost: 'School cost',
    School_flat_rate: 'School flat rate',
    School_flat_rate_check: 'Is there a flat rate for whole school purchases?',
    School_setup: 'School setup',
    See_more_items: 'See more items',
    Select_icon_to_upload: 'Select icon to upload...',
    Select_file: 'Select a file',
    Select_file_to_upload: 'Select file to upload...',
    Searching_text: 'Searching...',
    Seating_Chart: 'Seating Chart',
    Send_message: 'Send message',
    Sent_today: 'Sent today',
    Sent_in_date: function(date){
        return String.format('Sent {0}', date)
    },
    Show_grades_to_students: 'Show grades to students?',
    Show_less: 'Show less',
    Sorted_by: 'Sorted by',
    Source_Name: 'Source Name',
    Showing_items_due_before: function(endDateWithYear, endDate){
        return String.format('Showing items due before {0}. Check out the Calendar to see items due after {1}.', endDateWithYear, endDate)
    },
    Step_of: function(a,b){
        return 'Step ' + a + ' of ' + b;
    },
    Start_by_creating_a_grid: 'Start by creating a grid',
    Student: oneOrMany('Student'),
    Student_marked_absent: 'Student marked as absent on the date of that item',
    Students_are_in_trouble: function(count){
        return count > 1 ? 'Students are in trouble' : 'Student is in trouble';
    },
    Student_grades_are_out_of: function(coursename){
        return 'Student grades in ' + coursename + ' are out of';
    },
    Students_doing_well: function(count){
        return count > 1 ? 'Students doing well' : 'Student doing well';
    },
    Show_all: 'Show all',
    Short_Description: 'Short Description',
    Short_description_empty_text: 'Up to 150 characters',
    Stats_and_facts: 'Stats and facts',
    Strongest_class: 'Strongest class',
    Students_graded: function(count){
        return String.format("Student{0} graded", (count == 1 ? '' : 's'));
    },
    Students_with_attendance_issues: 'Students with attendance issues',
    Suggested_grade: 'Suggested grade',
    Sum_of_percents: 'Sum of percents is not 100!',
    Swipe_or_Search: 'Swipe or Search',
    System_Type: 'System Type',
    Task_State: 'Task State',
    Task_Type: 'Task Type',
    Teacher: oneOrMany('Teacher'),
    Tell_Chalkable: 'Tell Chalkable',
    This_app_is_for: 'This app is for:',
    This_info_will_appear_in_the_App_Store: '*This info will appear in the App Store',
    Todays_work: 'Today\'s work',
    Total_days_missed: 'Total days missed',
    Total_percent_market_share: 'Total percent market share',
    Try_it_out: 'Try it out',
    Type_Name: 'Type Name',
    Type_Number: 'Type Number',
    Video_Demo: 'Video Demo',
    Video_Demo_empty_text: 'Video Url',
    Undrop: 'Undrop',
    Unsaved_changes: 'Unsaved changes',
    Upcoming_assignments: 'Upcoming assignments',
    Update: 'Update',
    Update_Draft: 'Update Draft',
    User_Ratings: 'User Ratings',
    Weakest_class: 'Weakest class',
    Weeks_ago: function(n){
        String.format('{0} week{1} ago', n, (n==1 ? '' : 's'));
    },
    Weight_Addition: 'Weight Addition',
    Weight_Multiplier :'Weight Multiplier',
    Whole_day: 'Whole day',
    Whole_School: 'Whole School',
    Will_teachers_be_viewing_student_output: 'Will teachers be viewing student output?',
    Write_a_review: 'Write a review',
    You_vs_Peers: 'You vs. Peers',
    Your_grade: 'Your grade',

    Max_Score_Tooltip_Message: 'Enter a maximum score for this assignment',
    Weight_Multiplier_Tooltip_Message: 'For this item to be part of the marking period average, a number greater than 0 must be entered.',
    Weight_Addition_Tooltip_Message: 'Enter a number here if points are to be added to each student’s grade for this item.',
    Hide_From_Student_Tooltip_Message: 'This is will hide the item from students',
    Can_Drop_Student_Score_Tooltip_Message: 'Check this box if a score may be dropped for this item',

    Add_Credit_Transaction_Success: function(balance){
        return String.format("Success! Your new balance is ${0}", balance)
    },
    Add_Credit_Transaction_Fail: "The transaction was unsuccessful",

    //Window titles----------------------------------------------------------------------------------------------------------\

    Add_App_title: 'Chalkable App',
    Add_Auto_Email: 'Add Auto Email',
    Add_Course: 'Add Course',
    Add_Discipline_Type: 'Add Discipline Type',
    Add_System_Type: 'Add System Type',
    Application_Info: 'Application Info',
    Attendance_Export: 'Attendance Export',
    Department_Edit_Window: 'Department Edit Window',
    Department_Add_Window: 'Department Add Window',
    Import_Students: 'Import Students',

    //---------------------Messages------------------------------------------------------------------------------------------

    Chalkable_REST_API: "Chalkable REST API",
    Chalkable_REST_API_description: "The API allows you to access real-time data from schools. " +
        "All methods return JSON. There are user roles on the bar above. Choosing a role, for example teacher, " +
        "displays all of the methods available to your app if a teacher had installed it.",
    Close_without_attaching_the_app: 'Close without attaching the app?',
    Go_Lose_Changes: 'Go - Lose Changes / Cancel',
    If_you_navigate_away: 'If you navigate away from this page, all entered data will be lost.',
    Grade_values_cant_overlap: 'Grade values can&rsquo;t overlap between letter grades.',
    Just_checking: 'just checking',
    Keep_the_change: 'Keep the change?',
    Not_number_grade: 'Data is not valid!',
    Oops_Cancel: 'Oops, Cancel',
    Ok_Clear_Grades: 'OK, CLEAR GRADES',
    Please_enter_a_grade: 'Please enter a grade.',
    OAuth_description: " OAuth 2.0 is used for authentication. The authentication token you receive, " +
        "after making your request, includes a user role. The user role defines the level of access.",
    You_already_entered_some_grades: 'You already entered some grades. These grades</br>will be cleared if you continue with the change.',
    You_ll_be_grading_by_hand: 'You\'ll be grading each student by hand',
    Tab_is_missing_a_grade: function(name){
        return String.format('{0} tab is missing a grade.', name.toLowerCase())
    },
    You_set_a_0_value: function(name){
        return String.format('You set a 0% value for {0} in Settings> Grades.', name.toLowerCase())
    },
    Installed: 'Installed'
};
