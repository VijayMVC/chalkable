from base_auth_test import *
import unittest

class TestFeedAdjustItemDatesFeature(BaseTestCase):
    def setUp(self):
        self.teacher = TeacherSession(self).login(user_email, user_pwd)
        self.teacher_id = self.teacher.teacher_id

    def internal_(self, item_type):
        for one_class in self.teacher.list_of_classes():
            print one_class
            list_supplemental_dates = []
            self.list_for_items_id_and_types = []
            list_of_item_id = []
            list_of_scheduled_dates = []

            st_gr_period_for_list = self.teacher.start_date_gr_period_date_time_format
            end_gr_period_for_list = self.teacher.end_date_gr_period_date_time_format
            list_for_gr_period_dates = []

            while st_gr_period_for_list <= end_gr_period_for_list:
                list_for_gr_period_dates.append(st_gr_period_for_list)
                st_gr_period_for_list += timedelta(days=1)

            # getting gr. periods for a class
            get_the_last_gr_pr_of_class = self.teacher.get_json(
                '/GradingPeriod/ListByClassId.json?' + 'classId=' + str(one_class))
            get_the_last_gr_pr_of_class_data = get_the_last_gr_pr_of_class['data']
            list_gr_periods_for_class = []
            for i in get_the_last_gr_pr_of_class_data:
                list_gr_periods_for_class.append(i) and (
                datetime.strptime(i['startdate'], "%Y-%m-%d") <= self.teacher.start_date_gr_period_date_time_format)

            # get the last gr. period dates
            last_gr_period_for_class = list_gr_periods_for_class[-1]
            list_dates_last_gr_period = []
            list_dates_last_gr_period.append(last_gr_period_for_class['startdate'])
            list_dates_last_gr_period.append(last_gr_period_for_class['enddate'])

            st_date_of_the_last_gr_period = datetime.strptime(list_dates_last_gr_period[0], "%Y-%m-%d")
            end_date_of_the_last_gr_period = datetime.strptime(list_dates_last_gr_period[-1], "%Y-%m-%d")
            dates_of_the_last_gr_period = []
            while st_date_of_the_last_gr_period <= end_date_of_the_last_gr_period:
                dates_of_the_last_gr_period.append(st_date_of_the_last_gr_period)
                st_date_of_the_last_gr_period += timedelta(days=1)

            # get the first gr. period dates
            first_gr_period_for_class = list_gr_periods_for_class[0]
            list_dates_first_gr_period = []
            list_dates_first_gr_period.append(first_gr_period_for_class['startdate'])
            list_dates_first_gr_period.append(first_gr_period_for_class['enddate'])

            st_date_of_the_first_gr_period = datetime.strptime(list_dates_first_gr_period[0], "%Y-%m-%d")
            end_date_of_the_first_gr_period = datetime.strptime(list_dates_first_gr_period[-1], "%Y-%m-%d")
            dates_of_the_first_gr_period = []
            while st_date_of_the_first_gr_period <= end_date_of_the_first_gr_period:
                dates_of_the_first_gr_period.append(st_date_of_the_first_gr_period)
                st_date_of_the_first_gr_period += timedelta(days=1)

            def list_items_json_unicode(activity_id):
                list_items_json_unicode = self.teacher.post_json(
                    '/Announcement/Read.json?' + "announcementId=" + str(activity_id) + "&announcementType=" + str(item_type))
                dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']

                return dictionary_verify_annoucementviewdatas_all

            def get_item_date(one_item):
                if one_item['type'] == 4:
                    return one_item['supplementalannouncementdata']['expiresdate']

            def dates_for_adjust_feature(item_date, **kwargs):
                date = item_date + timedelta(**kwargs)

                if date < start_date_one_class_date_time:
                    return start_date_one_class_date_time

                if date > end_date_one_class_date_time:
                    return end_date_one_class_date_time

                return date

            def pre_closest_scheduled_date(item_date, **kwargs):
                index_date = list_of_scheduled_dates.index(item_date)
                index_new_date = index_date + kwargs.values()[0]

                if index_new_date <= 0:
                    return start_scheduled_day_one_class

                if index_new_date >= len(list_of_scheduled_dates):
                    return end_scheduled_day_one_class

                return list_of_scheduled_dates[index_new_date]

            def scheduled_class_days():
                get_dates = self.teacher.get_json('/Class/Days.json?' + 'classId=' + str(one_class))

                dates = []
                for one_date in get_dates['data']:
                    one_date = one_date.split()
                    dates.append(datetime.strptime(one_date[0], "%m/%d/%Y").strftime("%m-%d-%Y"))

                return [datetime.strptime(str(date), '%m-%d-%Y') for date in dates]

            list_of_scheduled_dates = scheduled_class_days()

            def closest_scheduled_date(list_for_dates, item_date):
                cur_date = list_for_dates[0]
                distance = abs((cur_date - item_date).days)

                for i in list_for_dates[1:]:
                    new_distance = abs((i - item_date).days)
                    if new_distance >= distance:
                        break

                    cur_date = i
                    distance = new_distance

                return cur_date

            def item_date_in_gr_period(item_date, **kwargs):
                date = item_date + timedelta(**kwargs)

                if (item_date >= dates_of_the_first_gr_period[0]) and (item_date <= dates_of_the_first_gr_period[-1]):
                    return closest_gr_period_date(dates_of_the_first_gr_period, date)

                if (item_date >= dates_of_the_last_gr_period[0]) and (item_date <= dates_of_the_last_gr_period[-1]):
                    return closest_gr_period_date(dates_of_the_last_gr_period, date)

            def closest_gr_period_date(list_for_gr_period_dates, item_date):
                cur_date = list_for_gr_period_dates[0]
                distance = abs((cur_date - item_date).days)
                for i in list_for_gr_period_dates:
                    new_distance = abs((i - item_date).days)
                    if new_distance > distance:
                        break

                    cur_date = i
                    distance = new_distance

                return cur_date

            def artificial_days_of_items(item_date, number_):
                index_item_date = list_of_scheduled_dates.index(item_date)
                resulted_number = index_item_date + number_
                resulted_date = list_of_scheduled_dates[resulted_number]

                return resulted_date

            def returned_index_(subtracted_date, item_date):
                index_subtracted_date = list_of_scheduled_dates.index(subtracted_date)
                index_item_date = list_of_scheduled_dates.index(item_date)

                result_of_index = index_subtracted_date - index_item_date

                return result_of_index

            def dict_item_date(date_correct_format, activity_id):
                if date_correct_format > list_of_scheduled_dates[-1]:
                    list_supplemental_dates.append(
                        {'announcementId': activity_id, 'expiresdate/startdate': list_of_scheduled_dates[-1]})
                if date_correct_format < list_of_scheduled_dates[0]:
                    list_supplemental_dates.append(
                        {'announcementId': activity_id, 'expiresdate/startdate': list_of_scheduled_dates[0]})
                if (date_correct_format >= list_of_scheduled_dates[0]) and (
                            date_correct_format <= list_of_scheduled_dates[-1]):
                    for i in list_of_scheduled_dates:
                        if i >= date_correct_format:
                            list_supplemental_dates.append(
                                {'announcementId': activity_id, 'expiresdate/startdate': i})
                            break

            def verify_current_vs_expected_dates():
                for item in list_of_item_id:
                    date = get_item_date(list_items_json_unicode(item))
                    date = datetime.strptime(date,
                                             "%Y-%m-%d")
                    for i in list_supplemental_dates[:]:
                        if item == i['announcementId']:
                            self.assertEqual(i['expiresdate/startdate'], date, 'dates are not equal: ' + 'expiresdate/startdate- ' + str(i['expiresdate/startdate']) + " " + 'date- '+ str(date) + ' item- ' + str(item))
                            del list_supplemental_dates[list_supplemental_dates.index(i)]

            def deleting_items():
                for one_activity in list_of_item_id[:]:
                    self.teacher.post_json('/Announcement/Delete.json',
                                           data={"announcementId": one_activity, "announcementType": item_type})
                    del list_of_item_id[list_of_item_id.index(one_activity)]

            start_date_one_class_date_time = scheduled_class_days()[0]
            start_scheduled_day_one_class = list_of_scheduled_dates[0]

            end_date_one_class_date_time = scheduled_class_days()[-1]
            end_scheduled_day_one_class = list_of_scheduled_dates[-1]

            # start_date_one_class_date_time = max(start_date_one_class_date_time, self.teacher.start_date_gr_period_date_time_format) if (scheduled_class_days()[(len(scheduled_class_days())/2)] in list_for_gr_period_dates) else min(start_date_one_class_date_time, self.teacher.start_date_gr_period_date_time_format)
            start_date_one_class_date_time = min(start_date_one_class_date_time, dates_of_the_first_gr_period[0])
            # end_date_one_class_date_time = dates_of_the_last_gr_period[-1] if end_date_one_class_date_time > dates_of_the_last_gr_period[-1] else end_date_one_class_date_time
            end_date_one_class_date_time = max(end_date_one_class_date_time, dates_of_the_last_gr_period[-1])

            def first_unscheduled_date_from_the_start():
                global_date_for_item = list_of_scheduled_dates[0]
                for i in list_of_scheduled_dates:
                    i += timedelta(days=1)
                    if i not in list_of_scheduled_dates:
                        global_date_for_item = i

                        break

                return global_date_for_item

            def first__second_unscheduled_date_from_the_start():
                global_date_for_item = list_of_scheduled_dates[0]
                global_date_for_item2 = list_of_scheduled_dates[0]
                for i in list_of_scheduled_dates:
                    i += timedelta(days=1)
                    if i not in list_of_scheduled_dates and ((i + timedelta(days=1)) not in list_of_scheduled_dates and ((i + timedelta(days=2)) not in list_of_scheduled_dates)):
                        global_date_for_item = i
                        global_date_for_item2 = i + timedelta(days=1)

                        break

                return global_date_for_item, global_date_for_item2

            def first_second_unscheduled_date_from_the_end():
                reversed_ = reversed(list_of_scheduled_dates)
                global_date_for_item = list_of_scheduled_dates[0]
                global_date_for_item2 = list_of_scheduled_dates[0]
                for i in reversed_:
                    i -= timedelta(days=1)
                    if i not in list_of_scheduled_dates and (
                            (i - timedelta(days=1)) not in list_of_scheduled_dates and (
                        (i - timedelta(days=2)) not in list_of_scheduled_dates)):
                        global_date_for_item = i
                        global_date_for_item2 = i - timedelta(days=1)

                        break

                return global_date_for_item, global_date_for_item2

            def first_scheduled_second_unscheduled_date_from_the_end():
                reversed_ = reversed(list_of_scheduled_dates)
                global_date_for_item = list_of_scheduled_dates[0]
                global_date_for_item2 = list_of_scheduled_dates[0]
                for i in reversed_:
                    i -= timedelta(days=1)
                    if i in list_of_scheduled_dates and (
                                    (i - timedelta(days=1)) not in list_of_scheduled_dates and (
                                        (i - timedelta(days=2)) not in list_of_scheduled_dates)):
                        global_date_for_item = i
                        global_date_for_item2 = i - timedelta(days=1)

                        break

                return global_date_for_item, global_date_for_item2

            def first_unscheduled_date_from_the_end():
                global_date_for_item = list_of_scheduled_dates[0]
                reversed_ = reversed(list_of_scheduled_dates)
                for i in reversed_:
                    i -= timedelta(days=1)
                    if i not in list_of_scheduled_dates:
                        global_date_for_item = i

                        break

                return global_date_for_item

            def middle_unscheduled_date_from_the_end():
                has_mediana = False
                mediana_ = list_of_scheduled_dates[0]
                for index_, day_ in enumerate(list_of_scheduled_dates):
                    if index_ < len(list_of_scheduled_dates) - 1:
                        next_day = list_of_scheduled_dates[index_ + 1]
                        diff_ = abs((next_day - day_).days)
                        if diff_ > 3 and diff_%2 == 0: # might be without the second part of the expression
                            has_mediana = True
                            mediana_ = day_ + timedelta(days=diff_ / 2)
                            break
                if has_mediana:
                    return mediana_
                else:
                    return False

            subtr_item_date_1_in_gr_period = item_date_in_gr_period(end_date_one_class_date_time, days=-10)  #create items on any dates
            subtr_item_date_2_in_gr_period = item_date_in_gr_period(end_date_one_class_date_time, days=-20)
            subtr_item_date_3_in_gr_period = item_date_in_gr_period(end_date_one_class_date_time, days=-30)

            add_item_date_1_in_gr_period = item_date_in_gr_period(start_date_one_class_date_time, days=10)
            add_item_date_2_in_gr_period = item_date_in_gr_period(start_date_one_class_date_time, days=20)
            add_item_date_3_in_gr_period = item_date_in_gr_period(start_date_one_class_date_time, days=30)

            def post_one_supplemental(date_in_correct_format):
                random_name = ''.join([random.choice(string.ascii_letters + string.digits) for n in xrange(30)])

                self.teacher.get_json(
                    '/Announcement/Create.json?')

                get_create_supplemental = self.teacher.get_json(
                    '/SupplementalAnnouncement/CreateSupplemental.json?' + 'classId=' + str(one_class))
                get_create_supplemental_data = get_create_supplemental['data']

                supplemental_id = get_create_supplemental_data['announcement']['supplementalannouncementdata']['id']
                announcement_type_id = get_create_supplemental_data['announcement']['supplementalannouncementdata'][
                    'announcementtypeid']
                announcement_type = get_create_supplemental_data['announcement']['type']

                get_students_supplemental = self.teacher.get_json(
                    '/Student/GetStudents.json?' + 'classId=' + str(
                        one_class) + '&myStudentsOnly=' + str(True) +
                    '&byLastName=' + str(True) + '&start=' + str(0) + '&count=' + str(999) + '&enrolledOnly=' +
                    str(True))

                list_for_students_id = []
                for i in get_students_supplemental['data']:
                    list_for_students_id.append(str(i['id']))

                if len(list_for_students_id) != 0:
                    self.list_for_items_id_and_types.append({'announcementId': supplemental_id, 'announcementType': announcement_type})

                    dict_item_date(date_in_correct_format, supplemental_id)

                    data_post_supplemental = {
                        "attributes": [],

                        "classAnnouncementTypeId": announcement_type_id,

                        "classId": one_class,

                        "content": "Description in Supplemental",

                        "discussionEnabled": True,

                        'expiresDate': str(date_in_correct_format),

                        'hidefromstudents': False,

                        'previewCommentsEnabled': True,

                        'recipientsIds': list_for_students_id,

                        "requireCommentsEnabled": True,

                        "supplementalAnnouncementPlanId": supplemental_id,

                        'title': random_name
                    }

                    self.teacher.post_json('/SupplementalAnnouncement/Submit.json', data=data_post_supplemental)

                    list_of_item_id.append(supplemental_id)

            def posting_items(*arglist_dates, **kwargs):
                for i in arglist_dates:
                    post_one_supplemental(i)

                if len(list_of_item_id) != 0:
                    closest_scheduled_dates = []
                    for one_date in arglist_dates:
                        closest_scheduled_dates.append(closest_scheduled_date(list_of_scheduled_dates, one_date))

                    list_of_subtr_dates_inner = []
                    for one_day in closest_scheduled_dates:
                        list_of_subtr_dates_inner.append(pre_closest_scheduled_date(one_day, **kwargs))

                    date_for_adjust_method = list_of_subtr_dates_inner[0]

                    k_for_dict = 0
                    for i in list_supplemental_dates:
                        i['expiresdate/startdate'] = list_of_subtr_dates_inner[k_for_dict]
                        k_for_dict += 1

                    self.teacher.post_json('/Announcement/AdjustDates.json',
                                           json={"classId": one_class, "announcements": self.list_for_items_id_and_types,
                                                 "shift": kwargs.values()[0]})

                    verify_current_vs_expected_dates()

                    deleting_items()

                    self.list_for_items_id_and_types = []

            posting_items(dates_of_the_first_gr_period[0], days=0)

            posting_items(list_of_scheduled_dates[0], days=0)

            posting_items(dates_of_the_first_gr_period[-2], days=0)

            posting_items(dates_of_the_first_gr_period[-3], days=0)

            posting_items(list_of_scheduled_dates[-1], days=0)

            posting_items(first_unscheduled_date_from_the_start(), days=15)

            posting_items(first_unscheduled_date_from_the_end(), days=15)

            posting_items(first_unscheduled_date_from_the_start(), days=-15)

            posting_items(first_unscheduled_date_from_the_end(), days=-15)

            posting_items(first_unscheduled_date_from_the_start(), first_unscheduled_date_from_the_end(), days=15)

            posting_items(first_unscheduled_date_from_the_start(), first_unscheduled_date_from_the_end(), days=-15)

            posting_items(first_unscheduled_date_from_the_start(), first_unscheduled_date_from_the_end(), days=15)

            posting_items(first_unscheduled_date_from_the_start(), first_unscheduled_date_from_the_end(), days=-15)

            posting_items(subtr_item_date_1_in_gr_period, subtr_item_date_2_in_gr_period,
                          subtr_item_date_3_in_gr_period, days=15)

            posting_items(subtr_item_date_1_in_gr_period, subtr_item_date_2_in_gr_period,
                          subtr_item_date_3_in_gr_period, days=7)

            posting_items(add_item_date_1_in_gr_period, add_item_date_2_in_gr_period, add_item_date_3_in_gr_period,
                          days=-15)

            posting_items(add_item_date_1_in_gr_period, add_item_date_2_in_gr_period, add_item_date_3_in_gr_period,
                          days=-7)

    def test_suppplementals(self):
        self.internal_(4)

if __name__ == '__main__':
    unittest.main()