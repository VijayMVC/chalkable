from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        list_of_classes = self.list_of_classes

        one_random_class = random.choice(list_of_classes)
        random_value_of_marking_period = random.choice(self.marking_periods())

        get_class_explorer = self.get('/Class/Explorer.json?' + 'classId=' + str(one_random_class))
        get_class_explorer_data_standards = get_class_explorer['data']['standards']
        if len(get_class_explorer_data_standards) > 0:
            get_class_explorer_suggested_apps = self.get('/AppMarket/SuggestedApps.json?' + 'classId=' + str(one_random_class) + '&markingPeriodId=' + str(random_value_of_marking_period) + '&start =' + str(0) + '&count=' + str(9999) + '&myAppsOnly=' + str(True))

if __name__ == '__main__':
    unittest.main()