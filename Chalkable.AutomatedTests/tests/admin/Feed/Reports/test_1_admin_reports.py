from base_auth_test import *
import unittest

class TestFeedReports(BaseTestCase):
    def setUp(self):
        self.admin = DistrictAdminSession(self).login(user_email, user_pwd)

        # reset settings
        self.admin.post_json('/Feed/SetSettings.json?', data={})

        # making all types of admin announcements as 'undone'
        self.admin.post_json('/Announcement/UnDone.json?', data={'option': 3})

        feed_report_settings = self.admin.get_json('/Reporting/FeedReportSettings.json?' + 'classId=')

        start_date = feed_report_settings['data']['startdate']
        end_date = feed_report_settings['data']['enddate']
        self.start_date_correct_format = datetime.strptime(start_date, "%Y-%m-%d").strftime("%m-%d-%Y")
        self.end_date_correct_format = datetime.strptime(end_date, "%Y-%m-%d").strftime("%m-%d-%Y")

    def internal_(self, includeDetails):
        self.admin.get_file_(
            '/Reporting/FeedReport.json?' + 'startDate=' + str(self.start_date_correct_format) + '&endDate=' + str(
                self.end_date_correct_format) + '&lessonPlanOnly=' + str(False) + '&includeAttachments=' + str(includeDetails) + '&includeDetails=' + str(True) + '&includeHiddenAttributes=' + str(
                False) + '&includeHiddenActivities=' + str(False) + '&classId=' + '&complete=' + str(
                False) + '&announcementType=')

    def test_activity_details_report(self):
        self.internal_(True)

    def test_list_of_activities_report(self):
        self.internal_(False)

    def tearDown(self):
        # reset all filters on the feed
        self.admin.post_json('/Feed/SetSettings.json?', data={})

if __name__ == '__main__':
    unittest.main()