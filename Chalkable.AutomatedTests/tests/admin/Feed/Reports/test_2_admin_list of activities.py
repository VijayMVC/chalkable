from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

        self.settings_data_for_mark_undone = {'option': '3'}

        # making admin announcements as 'undone'
        self.post_admin('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)

        feed_report_settings = self.get_admin('/Reporting/FeedReportSettings.json?' + 'classId=')
        startdate = feed_report_settings['data']['startdate']
        enddate = feed_report_settings['data']['enddate']
        start_date_correct_format = datetime.strptime(startdate,"%Y-%m-%d").strftime("%m-%d-%Y")
        end_date_correct_format = datetime.strptime(enddate, "%Y-%m-%d").strftime("%m-%d-%Y")

        self.get_file_admin('/Reporting/FeedReport.json?' + 'startDate=' + str(start_date_correct_format) + '&endDate=' + str(end_date_correct_format) + '&lessonPlanOnly=' + str(False) + '&includeAttachments=' + str(False) + '&includeDetails=' + str(False) + '&includeHiddenAttributes=' + str(False) + '&includeHiddenActivities=' + str(False) + '&classId=' + '&complete=' + str(False) + '&announcementType=')

    def tearDown(self):
        # reset all filters on the feed
        self.dict = {}
        self.post_admin('/Feed/SetSettings.json?', self.dict)

if __name__ == '__main__':
    unittest.main()