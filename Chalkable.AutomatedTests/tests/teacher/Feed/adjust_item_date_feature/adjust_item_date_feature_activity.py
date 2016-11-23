from base_auth_test import *
import unittest

class TestFeedAdjustItemDatesFeature(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.teacher_id

        # reset settings
        self.teacher.post_json('/Feed/SetSettings.json?', data={})

    def internal_(self, item_type):
        for one_class in self.teacher.list_of_classes():
            def list_items_json_unicode(activity_id):
                list_items_json_unicode = self.teacher.post_json(
                    '/Announcement/Read.json?' + "announcementId=" + str(activity_id) + "&announcementType=" + str(item_type))
                dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']
                return dictionary_verify_annoucementviewdatas_all

            def get_item_date(one_item):
                if one_item['type'] == 3:
                    return one_item['lessonplandata']['startdate']
                if one_item['type'] == 1:
                    return one_item['classannouncementdata']['expiresdate']
                if one_item['type'] == 4:
                    return one_item['supplementalannouncementdata']['expiresdate']

            list_activity_dates = []
            list_for_items_id = []
            list_for_activity_id_ = []
            list_for_dates = []
            get_dates = self.teacher.get_json('/Class/Days.json?' + 'classId=' + str(one_class))

            for one_date in get_dates['data']:
                one_date = one_date.split()
                list_for_dates.append(datetime.strptime(one_date[0], "%m/%d/%Y").strftime("%m-%d-%Y"))

            start_date_one_class = list_for_dates[0]
            end_date_one_class = list_for_dates[-1]

            date_object = datetime.strptime(str(end_date_one_class), '%m-%d-%Y')
            date_substract_10 = date_object - timedelta(days=10)
            date_substract_10_correct_format = date_substract_10.strftime('%m-%d-%Y')

            date_substract_20 = date_object - timedelta(days=20)
            date_substract_20_correct_format = date_substract_20.strftime('%m-%d-%Y')

            date_substract_30 = date_object - timedelta(days=30)
            date_substract_30_correct_format = date_substract_30.strftime('%m-%d-%Y')

            date_object2 = datetime.strptime(str(date_substract_30_correct_format), '%m-%d-%Y')
            add_15_days = date_object2 + timedelta(days=15)
            add_15_days = add_15_days.strftime('%m-%d-%Y')

            def post_one_activity(date_in_correct_format):
                random_name = ''.join([random.choice(string.ascii_letters + string.digits) for n in xrange(30)])

                self.teacher.get_json(
                    '/Announcement/Create.json?')

                get_create_activity = self.teacher.get_json(
                    '/ClassAnnouncement/CreateClassAnnouncement.aspx?' + 'classId=' + str(one_class))
                get_create_activity_data = get_create_activity['data']

                activity_id = get_create_activity_data['announcement']['classannouncementdata']['id']  # id of activity
                announcement_type_id = get_create_activity_data['announcement']['classannouncementdata'][
                    'announcementtypeid']# 9781
                activity_type = get_create_activity_data['announcement']['type']

                list_for_items_id.append({'announcementId': activity_id, 'announcementType': activity_type})

                # date plus 15
                date_object_inner = datetime.strptime(str(date_in_correct_format), '%m-%d-%Y')
                date_add_15 = date_object_inner + timedelta(days=15)
                date_add_15_correct_format = date_add_15.strftime('%m-%d-%Y')

                if date_add_15_correct_format < end_date_one_class:
                    if date_add_15_correct_format in list_for_dates:
                        list_activity_dates.append({'announcementId': activity_id, 'expiresdate/startdate': str(date_add_15_correct_format)})
                    else:
                        for i in list_for_dates:
                            if i > date_add_15_correct_format:
                                list_activity_dates.append(
                                    {'announcementId': activity_id, 'expiresdate/startdate': str(i)})
                                break
                else:
                    list_activity_dates.append(
                        {'announcementId': activity_id, 'expiresdate/startdate': str(end_date_one_class)})

                get_exist_title = self.teacher.get_json(
                    '/ClassAnnouncement/Exists.json?' + 'title=' + str(random_name) + '&classId=' + str(
                        one_class) + '&expiresDate=' + str(date_in_correct_format) + '&excludeAnnouncementId=' + str(
                        activity_id))

                self.teacher.post_json('/ClassAnnouncement/SubmitAnnouncement.json', data={
                        'announcementid': activity_id,

                        "attributes": [],

                        "candropstudentscore": False,

                        "classId": one_class,

                        "classannouncementtypeid": announcement_type_id,

                        "content": "",

                        "discussionEnabled": True,

                        "expiresDate": str(date_in_correct_format),  # "10-27-2016"

                        "gradable": False,

                        'hidefromstudents': False,

                        "maxscore": 100,

                        "previewCommentsEnabled": True,

                        "requireCommentsEnabled": True,

                        "title": random_name,

                        "weightaddition": 0,

                        "weightmultiplier": 1,
                    })
                #print one_class, activity_id
                list_for_activity_id_.append(activity_id)

            post_one_activity(date_substract_10_correct_format)
            post_one_activity(date_substract_20_correct_format)
            post_one_activity(date_substract_30_correct_format)

            #print one_class, list_for_items_id, str(add_15_days)
            self.teacher.post_json('/Announcement/AdjustDates.json',
                                   json={"classId": one_class, "announcements": list_for_items_id,
                                         "startDate": str(add_15_days)})

            for item in list_for_activity_id_:
                date = get_item_date(list_items_json_unicode(item))
                date = str(datetime.strptime(date, "%Y-%m-%d").strftime("%m-%d-%Y"))
                for i in list_activity_dates:
                    if item == i['announcementId']:
                        if i['expiresdate/startdate'] > end_date_one_class:
                            self.assertEqual(date, end_date_one_class, 'date are equal')
                        else:
                            #print one_class, item, i['expiresdate/startdate'], date
                            self.assertEqual(i['expiresdate/startdate'], date, 'dates are not equal')


            for one_activity in list_for_activity_id_:
                #print one_activity
                self.teacher.post_json('/Announcement/Delete.json',
                                       data={"announcementId": one_activity, "announcementType": item_type})

            #print list_for_items_id

    def test_activities(self):
        self.internal_(1)

if __name__ == '__main__':
    unittest.main()