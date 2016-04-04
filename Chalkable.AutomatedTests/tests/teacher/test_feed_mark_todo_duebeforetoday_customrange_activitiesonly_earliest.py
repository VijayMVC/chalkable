# Feed > ToDo. ["Items due before today", Custom Range", "Activities Only", "Earliest"]
# http://screencast.com/t/jLpDvTAcpG

from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        #data that will need for filtering by lesson plans for grading period 1
        self.settings_data_lesson_plan = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan)   
        
        #get 'done' lesson plans of the grading period 1
        done_lesson_plans_json_unicode = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(550))
        annoucementviewdatas_json_unicode = done_lesson_plans_json_unicode['data']['annoucementviewdatas']

        self.id_of_lessonplan_and_activities_gradingperiod1 = []
        self.id_of_lessonplan_and_activities_gradingperiod2 = []
        
        #making all of these lesson plans 'unmark'
        for item in annoucementviewdatas_json_unicode:
            id = str(item['id'])
            type = str(item['type'])
            self.id_of_lessonplan_and_activities_gradingperiod1.append(id) 
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
      
        #data that needed for filtering by lesson plans for grading period 2
        self.settings_data_lesson_plan_2 = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan_2)
        
        #get 'done' lesson plans of the grading period 2
        done_lesson_plans_json_unicode_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(550))
        annoucementviewdatas_json_unicode_2 = done_lesson_plans_json_unicode_2['data']['annoucementviewdatas']       
        
        #making all of these lesson plans 'unmark'
        for item in annoucementviewdatas_json_unicode_2:
            id = str(item['id'])
            type = str(item['type'])
            self.id_of_lessonplan_and_activities_gradingperiod2.append(id)
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'}) 
            
#########################################################################################################################
        #data that needed for filtering by activities for grading period 1
        self.settings_data_activity = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity)   
        
        #get 'done' activities of the grading period 1
        done_activities_json_unicode = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(550))
        for_item_id_1 = done_activities_json_unicode['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id_1:
            id = str(item['id'])
            type = str(item['type'])

            self.id_of_lessonplan_and_activities_gradingperiod1.append(id) #id for lesson plans and activities - grading period 1
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})    
        
        #data that needed for filtering by activities for grading period 2
        self.settings_data_activity_2 = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity_2)   
        
        #get 'done' activities of the grading period 2
        done_activities_json_unicode_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(550))
        for_item_id_2 = done_activities_json_unicode_2['data']['annoucementviewdatas']  
        
        #making all of these activities 'unmark'
        for item in for_item_id_2:
            id = str(item['id'])
            type = str(item['type'])
            self.id_of_lessonplan_and_activities_gradingperiod2.append(id)
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})  
           
#########################################################################################################################

        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
       
        self.assertFalse(set(self.id_of_lessonplan_and_activities_gradingperiod2) & set(self.id_of_lessonplan_and_activities_gradingperiod1), 'Items are in different grading periods')
       
        #get items 'Due before today'
        self.settings_data = {'option':'1'}
        self.post('/Announcement/Done.json?', self.settings_data)   
       
        #set filter by activity
        self.type_of_item = {'type':'1'}
        
        #get a needed date
        self.current_time = time.strftime('%Y-%m-%d')     
        Date = datetime.strptime(self.current_time, "%Y-%m-%d")
        StartDate = Date.today() - timedelta(days=10)
        EndDate = Date.today() + timedelta(days=10)
        self.current_date_minus_10 = StartDate.strftime("%Y-%m-%d")
        self.current_date_plus_10 = EndDate.strftime("%Y-%m-%d")
        self.settings_data = {'sortType':'0', 'announcementType':'1', 'fromDate': self.current_date_minus_10, 'toDate':self.current_date_plus_10}
        self.post('/Feed/SetSettings.json?', self.settings_data) 
        
        #get items 'Due before today'
        self.settings_data_2 = {'option':'1'}
        self.post('/Announcement/Done.json?', self.settings_data_2)   
        
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=2500):
        #get a current time
        self.current_time = time.strftime('%Y-%m-%d')     
    
        list_items_json_unicode = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        dictionary_verify_settingsforfeed = list_items_json_unicode['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')

        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])
  
  
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']
        
        #getting grading periods
        list_for_start_date = []
        list_for_end_date = []
        get_grading_periods = self.get('/GradingPeriod/List.json?')
        get_first_period = get_grading_periods['data']
        if len(get_first_period) > 0:
            for item in get_first_period:
                self.startdate_2 = item['startdate']
                list_for_start_date.append(self.startdate_2)
                self.enddate_2 = item['enddate']
                list_for_end_date.append(self.enddate_2)
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')    
        decoded_list_1 = [x.encode('utf-8') for x in list_for_start_date]
        decoded_list_2 = [x.encode('utf-8') for x in list_for_end_date]

        list_for_date = []
        

        if len(dictionary_verify_annoucementviewdatas_all) > 0:
            for item in dictionary_verify_annoucementviewdatas_all:
                type = str(item['type'])
                complete = item['complete']
                self.assertTrue(complete == False, "Verify item's state on false")
                classannouncementdata = item ['classannouncementdata']
                expiresdate = classannouncementdata['expiresdate']
                list_for_date.append(expiresdate)
                self.assertDictContainsSubset(self.type_of_item, {'type':type}, 'Only activities are shown')
                self.assertLessEqual(self.current_time, expiresdate, 'Current date <= expiresdate of an activity ' + str(item["id"]))
                self.assertLessEqual(self.current_date_minus_10, expiresdate, 'Verify exprisesdate of activity ' + str(item["id"]))
                self.assertGreaterEqual(self.current_date_plus_10, expiresdate, 'Verify exprisesdate of activity ' + str(item["id"]))
            decoded_list = [x.encode('utf-8') for x in list_for_date]
            sorted_list_dates = sorted(decoded_list, key = lambda d: map(int, d.split('-')))
            self.assertTrue(decoded_list == sorted_list_dates, 'Items are sorted in earliest order')            
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')
            
    def tearDown(self):
        #marking all items as 'done'
        get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(1000)) 
        for_item_id = get_all_unmarket_items['data']['annoucementviewdatas'] 
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'true'})
   
if __name__ == '__main__':
    unittest.main()