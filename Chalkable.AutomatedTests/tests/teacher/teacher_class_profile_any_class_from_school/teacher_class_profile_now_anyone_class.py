from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        school_year = self.school_year()

        get_class_summary = self.get('/Class/ClassesStats.json?' + 'schoolYearId=' + str(school_year) + '&start=' + str(0) + '&count=' +str(250) + '&sortType=' + str())

        get_class_summary_data = get_class_summary['data']

        list_for_class_id = []

        for i in get_class_summary_data:
            list_for_class_id.append(i['id'])

        random_class_id = random.choice(list_for_class_id)

        get_now_page_of_one_class = self.get('/Class/Summary.json?' + 'classId=' + str(random_class_id))

if __name__ == '__main__':
    unittest.main()