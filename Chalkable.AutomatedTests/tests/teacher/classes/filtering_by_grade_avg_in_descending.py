from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        list_for_permissions = []
        person_me = self.get('/Person/me')
        person_me_list_of_dictionaries = person_me['data']['claims']

        for item in person_me_list_of_dictionaries:
            list_for_permissions.append(item['values'])

        final_list = [item for sublist in list_for_permissions for item in sublist]
        decoded_list_for_permissions = [x.encode('utf-8') for x in final_list]


        school_year = self.school_year()
        id_of_current_teacher = self.id_of_current_teacher()

        list_of_classes = self.list_of_classes

        if ('View Classroom' or 'View Classroom (Admin)' or 'Maintain Classroom' or 'Maintain Classroom (Admin)') in decoded_list_for_permissions:
            get_list_of_classes = self.get('/Class/ClassesStats.json?' + 'schoolYearId=' + str(school_year) + '&start=' + str(0) + '&count=' + str(100) + '&sortType=' + str(11) + '&teacherId=' + str(id_of_current_teacher))
            get_list_of_classes_data = get_list_of_classes['data']
            list_for_avg = []
            for key in get_list_of_classes_data:
                for key2, value2 in key.iteritems():
                    if key2 == 'average':
                        list_for_avg.append(average)
            print list_for_avg
            self.assertTrue(list_for_avg == sorted(list_for_avg, reverse=True), 'Avg sorted in descending order')

        else:
            data = {"schoolYearId": str(school_year), "start": str(0), "count": str(100), "sortType": str(0), "teacherId":str(id_of_current_teacher)}
            get_list_of_classes = self.post_instead_of_get('/Class/ClassesStats.json?', data, success=False)

if __name__ == '__main__':
    unittest.main()