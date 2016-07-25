from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        list_of_classes = self.list_of_classes
        school_year = self.school_year()
        random_value_of_marking_period = random.choice(self.marking_periods())



        get_class_summary = self.get('/Class/ClassesStats.json?' + 'schoolYearId=' + str(school_year) + '&start=' + str(0) + '&count=' +str(250) + '&sortType=' + str())

        get_class_summary_data = get_class_summary['data']

        list_for_class_id = []

        for i in get_class_summary_data:
            list_for_class_id.append(i['id'])

        random_class_id = random.choice(list_for_class_id)


        get_class_explorer = self.get('/Class/Explorer.json?' + 'classId=' + str(random_class_id))
        get_class_explorer_data_standards = get_class_explorer['data']['standards']
        if len(get_class_explorer_data_standards) > 0:
            get_class_explorer_suggested_apps = self.get('/AppMarket/SuggestedApps.json?' + 'classId=' + str(random_class_id) + '&markingPeriodId=' + str(random_value_of_marking_period) + '&start =' + str(0) + '&count=' + str(9999) + '&myAppsOnly=' + str(True))

if __name__ == '__main__':
    unittest.main()