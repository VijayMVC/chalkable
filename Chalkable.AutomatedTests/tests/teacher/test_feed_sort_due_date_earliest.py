from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        #data that needed for filtering by lesson plans for grading period 1
        self.settings_data_lesson_plan = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan)
        
        #get 9 'done' lesson plans of the grading period 1
        done_lesson_plans = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_lesson_plan_id_1 = done_lesson_plans['data']['annoucementviewdatas']

        self.lesson_plan_and_activities_id_for_grading_period_1 = []
        self.lesson_plan_and_activities_id_for_grading_period_2 = []
        
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id_1:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_1.append(id)  #id for lesson plans and activities - grading period 1
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #data that needed for filtering by lesson plans for grading period 2
        self.settings_data_lesson_plan_2 = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan_2)
        
        #get 9 'done' lesson plans of the grading period 2
        done_lesson_plans_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_lesson_plan_id_2 = done_lesson_plans_2['data']['annoucementviewdatas']       
        
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id_2:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_2.append(id)
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})           

        #data that needed for filtering by activities for grading period 1
        self.settings_data_activity = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity)
        
        #get 9 'done' activities of the grading period 1
        done_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_item_id_1 = done_items['data']['annoucementviewdatas']
        
        #making all of these activities 'unmark'
        for item in for_item_id_1:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_1.append(id) #id for lesson plans and activities - grading period 1
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #data that needed for filtering by activities for grading period 2
        self.settings_data_activity_2 = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity_2)
        
        #get 9 'done' activities of the grading period 2
        done_items_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_item_id_2 = done_items_2['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id_2:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_2.append(id)
            self.post('/Announcement/Complete', {'announcementId': id, 'announcementType':type, 'complete':'false'})

        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        ##########################################################################################

        self.settings_data = {'sortType': '0'}

        self.post('/Feed/SetSettings.json?', self.settings_data)
    
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=9999):
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        #@dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed']
        #@self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        #@self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        #@self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        
        #@announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        #@sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        #@gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])
    
        #self.assertFalse(set(self.lesson_plan_and_activities_id_for_grading_period_2) & set(self.lesson_plan_and_activities_id_for_grading_period_1), 'Items exits in different grading perios')
        
        list_for_date = []
        last_item = None
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']
        if len(dictionary_verify_annoucementviewdatas_all) > 0:
            for item in dictionary_verify_annoucementviewdatas_all:
                if last_item != None:
                    def get_item_date(one_item):
                        if one_item['type'] == 3:
                            return one_item['lessonplandata']['startdate']
                        if one_item['type'] == 1:
                            return one_item['classannouncementdata']['expiresdate']
                        if one_item['type'] == 4:
                            return one_item['supplementalannouncementdata']['expiresdate']

                    def get_item_id(one_item):
                        return one_item['id']

                    def get_item_type(one_item):
                        return one_item['type']

                    self.assertTrue(get_item_date(last_item) <= get_item_date(item),
                                    "Items are sorted not in earliest order" + ": " +
                                    str(get_item_date(last_item)) + " " +
                                    str(get_item_date(item)) + " " + "item_type: " + str(get_item_type(last_item)) +
                                    " " + "item_id: " + str(get_item_id(last_item)))


                last_item = item
            #@decoded_list = [x.encode('utf-8') for x in list_for_date]
            #@decoded_list_to_str = ', '.join(decoded_list)

            

            #sorted_list_dates = sorted(decoded_list, key = lambda d: map(int, d.split('-')))
            #reverse_list_to_str = ', '.join(sorted_list_dates)

            #size1 = len(decoded_list)
           # i = 0
            #while i < size1 - 1:
            #    self.assertTrue(decoded_list[i] <= decoded_list[i + 1], 'Items are sorted not in earliest order' +
               #                 ": " + decoded_list[i] + " " + decoded_list[i + 1])
              #  i += 1

                #self.assertTrue(decoded_list[i] >= decoded_list[i+1]-1, 'Items are sorted not in earliest order')
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')
    
    def tearDown(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        #marking all items as 'done'
        #get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(2000)) 
        self.settings_data = {'option':'3'}
        self.post('/Announcement/Done.json?', self.settings_data) 
            
if __name__ == '__main__':
    unittest.main()